using System.Text.RegularExpressions;

namespace OppoT2Assembler.ISADefinitions
{
    public static class ISA
    {

        // Tokens
        public static Dictionary<string, uint> Opcodes { get; private set; }
        public static Dictionary<string, uint> Registers { get; private set; }
        public static Dictionary<string, uint> Conditionals { get; private set; }

        // Instruction Tokens
        public static Dictionary<string, uint> OpcodeTokens { get; private set; }
        public static Dictionary<string, uint> DirectiveTokens { get; private set; }

        // Assembly code definitions
        public static readonly string labelMarker = ".";
        public static readonly string directiveMarker = "@";

        /// <summary>
        /// All token names, and their decimal values are created in the InitDictionaries method.
        /// </summary>
        public static void InitDictionaries()
        {
            Opcodes = new Dictionary<string, uint>();
            Registers = new Dictionary<string, uint>();
            Conditionals = new Dictionary<string, uint>();
            OpcodeTokens = new Dictionary<string, uint>();
            DirectiveTokens = new Dictionary<string, uint>();

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
            Opcodes.Add("sira", 15);
            Opcodes.Add("ori", 17);
            Opcodes.Add("csrw", 18);
            Opcodes.Add("csrr", 19);
            // Psuedo
            Opcodes.Add("movi", 300);

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

            // Directive Tokens
            DirectiveTokens.Add("fillto", 2);
            DirectiveTokens.Add("ascii", 2);
            DirectiveTokens.Add("include", 2);

            // Opcode Tokens
            OpcodeTokens.Add("add", 4);
            OpcodeTokens.Add("addi", 4);
            OpcodeTokens.Add("or", 4);
            OpcodeTokens.Add("xor", 4);
            OpcodeTokens.Add("shll", 4);
            OpcodeTokens.Add("shlr", 4);
            OpcodeTokens.Add("neg", 3);
            OpcodeTokens.Add("and", 4);
            OpcodeTokens.Add("lw", 4);
            OpcodeTokens.Add("sw", 4);
            OpcodeTokens.Add("brc", 5);
            OpcodeTokens.Add("jalr", 3);
            OpcodeTokens.Add("push", 2);
            OpcodeTokens.Add("pop", 2);
            OpcodeTokens.Add("lui", 3);
            OpcodeTokens.Add("sira", 2);
            OpcodeTokens.Add("ori", 4);
            OpcodeTokens.Add("csrw", 2);
            OpcodeTokens.Add("csrr", 2);
            // Psuedos
            OpcodeTokens.Add("movi", 3);
        }

        /// <summary>
        /// Definitions of all used instruction formats.
        /// </summary>
        public enum InstructionFormat
        {
            RRR,
            RRI,
            RR,
            R,
            RRCI,
            OP,
            RI,
            INVALID
        }

        /// <summary>
        /// Get an <see cref="InstructionFormat"/> based off it's mneumonic.
        /// </summary>
        /// <param name="opcode">The mnuemonic representing an opcode</param>
        /// <returns>The <see cref="InstructionFormat"/> of the opcode.</returns>
        /// <exception cref="InvalidDataException">Thrown if the opcode isn't defined.</exception>
        public static InstructionFormat GetInstructionType(string opcode)
        {
            if (!TryGetBinFromType(opcode, out uint value))
            {
                throw new InvalidDataException();
            }

            switch (value)
            {
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
                    return InstructionFormat.RR;
                case 12:    // push
                    return InstructionFormat.R;
                case 13:    // pop
                    return InstructionFormat.R;
                case 14:    // lui
                    return InstructionFormat.RI;
                case 15:    // sira
                    return InstructionFormat.R;
                case 17:    // ori
                    return InstructionFormat.RRI;
                case 18:    // csrw
                    return InstructionFormat.R;
                case 19:    // csrr
                    return InstructionFormat.R;
                case 300:
                    return InstructionFormat.RI;
                default:
                    return InstructionFormat.INVALID;
            }
        }

        public static uint GetTokenCountFromOpcode(string opcode)
        {
            if (!Opcodes.TryGetValue(opcode, out uint result))
            {
                throw new InvalidDataException();
            }

            return OpcodeTokens[opcode];
        }

        public static bool TryGetBinFromType(string token, out uint decodedValue)
        {
            token = token.ToLower();

            if (Opcodes.TryGetValue(token, out uint opcode))
            {
                decodedValue = opcode;
                return true;
            }
            else if (Registers.TryGetValue(token, out uint register))
            {
                decodedValue = register;
                return true;
            }
            else if (Conditionals.TryGetValue(token, out uint conditional))
            {
                decodedValue = conditional;
                return true;
            }
            else
            {
                // Verify that this value is parsable
                if (!Regex.IsMatch(token, @"\A\b[0-9a-fA-Fx]+\b\Z"))
                {
                    decodedValue = 0;
                    return false;
                }

                // Value must be of an integer, a binary value, or a hex value.
                if (token.StartsWith("0x"))
                {
                    // Token is a hex value
                    token = token[2..];
                    decodedValue = Convert.ToUInt32(token, 16);
                    return true;
                }
                else if (token.StartsWith("0b"))
                {
                    // Token is a binary value
                    token = token[2..];
                    decodedValue = Convert.ToUInt32(token, 2);
                    return true;
                }
                else if (uint.TryParse(token, out uint immVal))
                {
                    // Token is a integer
                    decodedValue = immVal;
                    return true;
                }
            }

            decodedValue = 0;
            return false;
        }

        public static bool IsDirective(string token)
        {
            return DirectiveTokens.ContainsKey(token);
        }

        public static string GetAsciiFromDirective(string asciiString)
        {
            return Regex.Unescape(asciiString);
        }




    }
}
