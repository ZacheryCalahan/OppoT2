using System.Reflection.Emit;
using static System.Net.Mime.MediaTypeNames;

namespace OppoT2Assembler {

    public static class Helper {
        private static Dictionary<string, uint> Opcodes = new Dictionary<string, uint>();
        private static Dictionary<string, uint> Registers = new Dictionary<string, uint>();
        private static Dictionary<string, uint> Conditionals = new Dictionary<string, uint>();

        public static readonly int OpcodeShift = 27;
        public static readonly int RegAShift = 22;
        public static readonly int RegBShift = 17;
        public static readonly int ConditionalShift = 14;


        public enum InstructionFormat {
            RRR,
            RRI,
            RR,
            R,
            RRCI,
            OP,
            RI,
            INVALID
        }

        public static void InitDictionaries() {
            // OPCODES
            Opcodes.Add("add", 0);
            Opcodes.Add("addi", 1);
            Opcodes.Add("or", 2);
            Opcodes.Add("xor", 3);
            Opcodes.Add("shll", 4);
            Opcodes.Add("shlr", 5);
            Opcodes.Add("neg", 6);
            Opcodes.Add("and", 7);
            Opcodes.Add("lw", 8);
            Opcodes.Add("sw", 9);
            Opcodes.Add("brc", 10);
            Opcodes.Add("jalr", 11);
            Opcodes.Add("push", 12);
            Opcodes.Add("pop", 13);
            Opcodes.Add("lui", 14);
            Opcodes.Add("isrr", 15);
            Opcodes.Add("ori", 17);

            // Registers
            Registers.Add("r0", 0);
            Registers.Add("ra", 1);
            Registers.Add("s0", 2);
            Registers.Add("s1", 3);
            Registers.Add("s2", 4);
            Registers.Add("s3", 5);
            Registers.Add("s4", 6);
            Registers.Add("s5", 7);
            Registers.Add("s6", 8);
            Registers.Add("s7", 9);
            Registers.Add("t0", 10);
            Registers.Add("t1", 11);
            Registers.Add("t2", 12);
            Registers.Add("t3", 13);
            Registers.Add("t4", 14);
            Registers.Add("t5", 15);
            Registers.Add("t6", 16);
            Registers.Add("t7", 17);
            Registers.Add("r18", 18);
            Registers.Add("r19", 19);
            Registers.Add("r20", 20);
            Registers.Add("r21", 21);
            Registers.Add("r22", 22);
            Registers.Add("r23", 23);
            Registers.Add("r24", 24);
            Registers.Add("r25", 25);
            Registers.Add("r26", 26);
            Registers.Add("r27", 27);
            Registers.Add("r28", 28);
            Registers.Add("r29", 29);
            Registers.Add("isr", 30);
            Registers.Add("sp", 31);

            // Conditionals
            Conditionals.Add("eq", 0);
            Conditionals.Add("!eq", 0);
            Conditionals.Add("gt", 0);
            Conditionals.Add("gte", 0);
            Conditionals.Add("lt", 0);
            Conditionals.Add("lte", 0);
            Conditionals.Add("pass", 0);

        }

        /// <summary>
        /// Parses a token, returns MaxValue if unable to parse.
        /// </summary>
        /// <param name="token"></param>
        /// <returns>Int32 representing the binary value. Returns MaxValue if unable to parse.</returns>
        public static uint GetBinFromType(string token) {
            token = token.ToLower();

            if (Opcodes.TryGetValue(token, out uint opcode)) {
                return opcode;
            } else if (Registers.TryGetValue(token, out uint register)) {
                return register;
            } else if (Conditionals.TryGetValue(token, out uint conditional)) { 
                return conditional;
            } else {
                // Verify that this value is parsable
                if (!System.Text.RegularExpressions.Regex.IsMatch(token, @"\A\b[0-9a-fA-Fx]+\b\Z")) {
                    return uint.MaxValue;
                }

                // Value must be of an integer, a binary value, or a hex value.
                if (token.StartsWith("0x")) {
                    // Token is a hex value
                    token = token[2..];
                    return Convert.ToUInt32(token, 16);
                } else if (token.StartsWith("0b")) {
                    // Token is a binary value
                    token = token[2..];
                    return Convert.ToUInt32(token, 2);
                } else if (uint.TryParse(token, out uint immVal)) {
                    // Token is a integer
                    return immVal;
                }
            }

            return uint.MaxValue;
        }

        public static InstructionFormat GetInstructionType(string opcode) {
            uint value = GetBinFromType(opcode);
            if (value == int.MaxValue) {
                throw new FormatException();
            }

            switch (value) {
                case 0:     // add
                    return InstructionFormat.RRR;
                case 1:     // addi
                    return InstructionFormat.RRI;
                case 2:     // or
                    return InstructionFormat.RRR;
                case 3:     // xor
                    return InstructionFormat.RRR;
                case 4:     // shll
                    return InstructionFormat.RRR;
                case 5:     // shlr
                    return InstructionFormat.RRR;
                case 6:     // neg
                    return InstructionFormat.RR;
                case 7:     // and
                    return InstructionFormat.RRR;
                case 8:     // lw
                    return InstructionFormat.RRI;
                case 9:     // sw
                    return InstructionFormat.RRI;
                case 10:    // brc
                    return InstructionFormat.RRCI;
                case 11:    // jalr
                    return InstructionFormat.RRI;
                case 12:    // push
                    return InstructionFormat.R;
                case 13:    // pop
                    return InstructionFormat.R;
                case 14:    // lui
                    return InstructionFormat.RI;
                case 15:    // isrr
                    return InstructionFormat.OP;
                case 17:    // ori
                    return InstructionFormat.RRI;
                default:
                    return InstructionFormat.INVALID;
            }
        }
    }
}
