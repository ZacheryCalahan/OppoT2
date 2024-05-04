namespace OppoT2Assembler.ISADefinitions
{
    public struct Instruction
    {
        public enum Operand
        {
            Opcode,
            RegisterA,
            RegisterB,
            RegisterC,
            Simm17,
            Simm13,
            Imm15,
            Cond
        }

        // Bit positions
        public static readonly int OpcodeShift = 27;
        public static readonly int RegAShift = 22;
        public static readonly int RegBShift = 17;
        public static readonly int ConditionalShift = 14;

        // Bit masks
        public static readonly uint simm13Mask = 0x1fff;
        public static readonly uint simm17Mask = 0x1ffff;
        public static readonly uint imm15Mask = 0x7fff;
        public static readonly uint regCMask = 0b11111;

        // Data
        private uint instruction;
        public bool HasLabel { get; private set; }
        public bool HasSplitLabel { get; set; }
        public string? Label { get; private set; }

        public Instruction()
        {
            instruction = 0;
            HasLabel = false;
            Label = null;
        }

        public void SetLabel(string label)
        {
            Label = label;
            // Set label to be true
            HasLabel = true;
        }

        public void SetOperand(Operand operand, uint value)
        {
            switch (operand)
            {
                case Operand.Opcode:
                    instruction |= (value & 0b11111) << OpcodeShift;
                    return;
                case Operand.RegisterA:
                    instruction |= (value & 0b11111) << RegAShift;
                    return;
                case Operand.RegisterB:
                    instruction |= (value & 0b11111) << RegBShift;
                    return;
                case Operand.RegisterC:
                    instruction |= value & 0b11111 & regCMask;
                    return;
                case Operand.Simm17:
                    instruction |= value & simm17Mask;

                    // If this is set, it no longer can be a label.
                    HasLabel = false;
                    HasSplitLabel = false;
                    Label = null;
                    return;
                case Operand.Simm13:
                    instruction |= value & simm13Mask;
                    HasLabel = false;
                    return;
                case Operand.Imm15:
                    instruction |= value & imm15Mask;
                    return;
                case Operand.Cond:
                    instruction |= (value & 0b111) << ConditionalShift;
                    return;
            }
        }

        public uint GetOpcode()
        {
            return instruction >> OpcodeShift;
        }

        /// <summary>
        /// Get the instruction value, if completed.
        /// </summary>
        /// <param name="instruction">The <see cref="uint"/> value of the Instruction</param>
        /// <returns>Returns true if instruction is complete, with no pending label assignment.</returns>
        public bool TryGetInstruction(out uint instruction)
        {
            if (HasLabel || HasSplitLabel)
            {
                instruction = 0;
                return false;
            }

            instruction = this.instruction;
            return true;
        }

        public static explicit operator uint(Instruction d) => d.instruction;

    }
}
