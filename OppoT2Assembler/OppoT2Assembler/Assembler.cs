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
                lines[i] = lines[i].Trim();
                // Search for labels

                if (lines[i][0] == '.') {
                    string label = lines[i].Substring(1);
                    try {
                        labelAddresses.Add(label, currentAddr);
                        lines.RemoveAt(i);
                        i--;
                    } catch (ArgumentException e) {
                        Console.WriteLine(e);
                        Console.WriteLine("Duplicate label found at line: " + i);
                    }

                } else if (lines[i][0] == '@') {
                    if (lines[i].StartsWith("@ascii")) {
                        currentAddr += (uint) lines[i].Length - 7;
                    }
                } else {
                    // Special case of movi needs to be handled.
                    if (lines[i].StartsWith("movi")) {
                        currentAddr += 2;
                    } else {
                        currentAddr++;
                    }
                }
            }

            foreach (KeyValuePair<string, uint> label in labelAddresses) {
                Console.WriteLine("Label: {0}, Address: {1}", label.Key, label.Value);
            }

            Console.WriteLine("Linted file: ");
            foreach (string line in lines) {
                Console.WriteLine(line);
            }


            // Pass 2 (Convert to machine code ( string >:( ), and perform directives)
            string[] tokens;
            uint memLocation = 0;
            for (int i = 0; i < lines.Count; i++) {
                
                tokens = lines[i].Trim().Split(' ');

                if (tokens[0][0] == '@') { // Check if line starts with '@' symbol
                    // Directive
                    output += HandleDirectives(lines[i], memLocation, out uint memOffset);
                    memLocation += memOffset;
                } else {
                    // Check for pseudo ops
                    if (tokens[0] == "movi") {
                        // Prepare the double instruction
                        string instr1 = "lui ";
                        string instr2 = "ori ";

                        // Insert the register to write to
                        instr1 += tokens[1] + " ";
                        instr2 += tokens[1] + " ";

                        // Insert rB (same as rA)
                        instr2 += tokens[1] + " ";

                        // Split the imm value (and check if label!!)
                        uint imm = 0;
                        if (labelAddresses.ContainsKey(tokens[2])) {
                            imm = labelAddresses[tokens[2]]; // Set imm value to label address
                        } else {
                            imm = Helper.GetBinFromType(tokens[2]);
                        }
                        
                        uint luiImm = imm >> 17;
                        uint oriImm = imm & 0b00000000000000011111111111111111;

                        // Insert the imm values
                        instr1 += luiImm.ToString();
                        instr2 += oriImm.ToString();

                        // Generate the code
                        output += GenerateCode(instr1.Split(" "), memLocation) + " ";
                        output += GenerateCode(instr2.Split(" "), memLocation) + " ";
                        memLocation += 2;
                    } else {
                        // Normal OPCODE Instruction
                        output += GenerateCode(tokens, memLocation) + " ";
                        memLocation++;
                    }
                }

            }

            return output;
        }

        /// <summary>
        /// Given a tokenized string representing an instruction, convert to it's binary value as a uint.
        /// </summary>
        /// <param name="tokens"></param>
        /// <returns></returns>
        /// <exception cref="InvalidDataException"></exception>
        public static uint GenerateCode(string[] tokens, uint memLocation) {
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
                uint imm = Helper.GetBinFromType(tokens[4]);

                // Handle issue where the token MAY be a label.

                if (labelAddresses.ContainsKey(tokens[4])) {
                    // Label found! Do special math to generate imm value.
                    uint jmpAddr = labelAddresses[tokens[4]] - memLocation - 1;
                    jmpAddr &= 0b00000000000000000001111111111111; // Remove non-imm bits
                    return code | jmpAddr;
                } else if (imm > 0x1FFF) {
                    throw new InvalidDataException();
                }

                code |= cond << Helper.ConditionalShift;
                return code | imm;
            }

            // Shouldn't get here. If we get here, throw exception
            throw new InvalidDataException();
            
        }

        public static string HandleDirectives(string line, uint memLocation, out uint memLocations) {
            string[] tokens = line.Split(' ');
            string data = "";
            uint memoryOffset = 0;
            string directive = tokens[0];

            if (directive == "@fillto") {
                if (tokens.Count() != 2) { // Invalidate wrong token count
                    throw new InvalidDataException();
                }

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

            } else if (directive == "@ascii") {
                string asciiString = line[7..];
                // Fill this portion of memory with an ASCII string, terminated by a null character
                foreach (char c in asciiString) {
                    data += (uint)c + " ";
                    memoryOffset++;
                }

                data += "00000000 ";
                memoryOffset++;
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
