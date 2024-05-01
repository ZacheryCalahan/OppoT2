using System.Net;

namespace OppoT2Assembler {
    public static class Assembler {
        private static Dictionary<String, uint> labelAddresses = new Dictionary<String, uint>();

        public static string Assemble(String file) {
            string output = "";

            // Convert file to a list of lines, and strip all empty lines, whitespace, and comments.
            List<String> lines = file.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList();
            for (int i = 0; i < lines.Count; i++) {               
                // Remove one line comments
                if (lines[i][0] == '#') {
                    lines.RemoveAt(i);
                }

                // Remove trailing comments
                if (lines[i].Contains('#')) {
                    int commentStart = lines[i].IndexOf('#');
                    lines[i] = lines[i].Substring(0, commentStart);
                }

                lines[i] = lines[i].Replace(",", "");
            }

            // Pass 1 (Handle Labels)

            // Get each label
            uint currentAddr = 0;
            for (int i = 0; i < lines.Count; i++) {
                lines[i].Trim(); // Remove extra whitespace for here and later

                // Search for labels

                if (lines[i][0] == '.') {
                    string label = lines[i].Substring(1);
                    try {
                        labelAddresses.Add(label, currentAddr);
                        lines.RemoveAt(i);
                    } catch (ArgumentException e) {
                        Console.WriteLine(e);
                        Console.WriteLine("Duplicate label found at line: " + i);
                    }

                } else if (lines[i][0] == '@') {
                    // Skip this for now, handle their addresses later.
                } else {
                    currentAddr++;
                }
            }


            // Pass 2 (Convert to machine code ( string >:( ), and perform directives)
            string[] tokens;
            uint memLocation = 0;
            for (int i = 0; i < lines.Count; i++) {
                
                tokens = lines[i].Trim().Split(' ');

                if (tokens[0][0] == '@') { // Check if line starts with '@' symbol
                    // Directive
                    output += HandleDirectives(tokens, memLocation, out uint memOffset);
                    memLocation += memOffset;
                } else {
                    // Instruction
                    output += GenerateCode(tokens) + " ";
                    memLocation++;
                }

            }

            return output;
        }

        public static uint GenerateCode(string[] tokens) {
            uint code = 0; 

            // Set OPCODE bits
            code |= (uint) Helper.GetBinFromType(tokens[0]) << Helper.OpcodeShift;

            // Determine Type
            Helper.InstructionFormat instructionFormat = Helper.GetInstructionType(tokens[0]);

            if (instructionFormat == Helper.InstructionFormat.OP) { // No further work here!
                return code;
            }

            // We know for sure that the remaining opcodes at least have a rA value.
            uint regA = (uint) Helper.GetBinFromType(tokens[1]);
            code |= regA << Helper.RegAShift;

            // Return if instruction is of type R
            if (instructionFormat == Helper.InstructionFormat.R) {
                return code;
            }

            // Eliminate oddball RI format
            if (instructionFormat == Helper.InstructionFormat.RI) {
                // Verify that the imm value is smaller than 
                uint imm = (uint) Helper.GetBinFromType(tokens[2]);

                if (imm > 0x7fff) {
                    throw new InvalidDataException();
                }

                return code | imm;
            }

            // Remaining opcodes have a rB
            uint regB = (uint) Helper.GetBinFromType(tokens[2]);
            code |= regB << Helper.RegBShift;

            if (instructionFormat == Helper.InstructionFormat.RR) { // All done here.
                return code;
            }

            // All the remaining opcodes are functionally different, so handle them individually.
            if (instructionFormat == Helper.InstructionFormat.RRR) {
                uint regC = (uint) Helper.GetBinFromType(tokens[3]);
                return code | regC;
            }

            if (instructionFormat == Helper.InstructionFormat.RRI) {
                uint imm = (uint) Helper.GetBinFromType(tokens[3]);
                if (imm > 131071u) {
                    throw new InvalidDataException();
                }

                return code | imm;
            }

            if (instructionFormat == Helper.InstructionFormat.RRCI) {
                uint cond = (uint)Helper.GetBinFromType(tokens[3]);
                uint imm = (uint)Helper.GetBinFromType(tokens[4]);

                // Handle issue where the token MAY be a label.

                if (labelAddresses.ContainsKey(tokens[4])) {
                    // Label found! 
                    return code | labelAddresses[tokens[4]];
                } else if (imm > 0x1FFF) {
                    throw new InvalidDataException();
                }

                code |= cond << Helper.ConditionalShift;
                return code | imm;
            }

            // Shouldn't get here. If we get here, throw exception
            throw new InvalidDataException();
            
        }

        public static string HandleDirectives(string[] tokens, uint memLocation, out uint memLocations) {
            string data = "";
            uint memoryOffset = 0;

            if (tokens[0] == "@fill") {
                if (tokens.Count() == 2) {
                    // This is a fill until command.
                    uint fillTo = Helper.GetBinFromType(tokens[1]);
                    if (fillTo == uint.MaxValue) {
                        throw new InvalidDataException();
                    }
                    while (memLocation < fillTo) {
                        data += "00000000 ";
                        memoryOffset++;
                        memLocation++;
                    }
                }
            }

            memLocations = memoryOffset;
            return data;
        }

        public static string GetHexCode(string processedCode) {
            string[] codes = processedCode.Split(' ');
            string finishedCode = "";
            uint parsedInt;

            foreach (string code in codes) {
                if (uint.TryParse(code, out parsedInt)) {
                    // This is hacky. Yes. But it's fully functional!
                    // This is to get the hex string in big-endian, which is the endianiness of the OppoT2 CPU. This returns a long, but we don't use the upper
                    // 32 bits, so just cut them off.
                    string hexString = Convert.ToHexString(BitConverter.GetBytes(IPAddress.HostToNetworkOrder(parsedInt)));
                    finishedCode += hexString.Substring(hexString.Length - 8);
                } else {
                    finishedCode += code;
                }
                finishedCode += "\n";
            }
            return finishedCode;
        }

    }
}
