using OppoT2Assembler.MemHandler;
using System.Net;

namespace OppoT2Assembler {
    public static class Assembler {
        private static Dictionary<String, uint> labelAddresses = new Dictionary<String, uint>();

        public static uint[] DecodeLine(string line, uint memLocation, out uint memOffset) {
            // Pass 2 (Convert to machine code ( string >:( ), and perform directives)
            string[] tokens;
            List<uint> output = new List<uint>();

            tokens = line.Trim().Split(' ');

            if (tokens[0][0] == '@') { // Check if line starts with '@' symbol
                // Directive
                output.AddRange(HandleDirectives(line, memLocation, out uint directiveMemoryOffset));
                memOffset = directiveMemoryOffset;

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
                    output.Add(GenerateCode(instr1.Split(" "), memLocation));
                    output.Add(GenerateCode(instr2.Split(" "), memLocation));
                    memOffset = 2;
                } else {
                    // Normal OPCODE Instruction
                    output.Add(GenerateCode(tokens, memLocation));
                    memOffset = 1;
                }
            }

            return output.ToArray();
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

        public static uint[] HandleDirectives(string line, uint memLocation, out uint memLocations) {
            string[] tokens = line.Split(' ');
            List<uint> data = new List<uint>();
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
                    data.Add(0);
                    memoryOffset++;
                    memLocation++;
                }

            } else if (directive == "@ascii") {
                string asciiString = line[7..];
                // This is dirty, due to the need of escaped ' " ' chars. Fix later please.
                asciiString = asciiString.Replace("\"", "");
                // Fill this portion of memory with an ASCII string, terminated by a null character
                foreach (char c in asciiString) {
                    data.Add(c);
                    memoryOffset++;
                }

                data.Add(0);
                memoryOffset++;
            }

            memLocations = memoryOffset;
            return data.ToArray();
        }

        public static string GetHexCode(MemoryHandler memoryHandler) {
            string finishedCode = "";

            foreach (Memory memEntry in memoryHandler) {
                // Get the code
                uint code = memEntry.instruction;

                // Convert to a Hex String
                string hexString = Convert.ToHexString(BitConverter.GetBytes(IPAddress.HostToNetworkOrder(code)));
                hexString.Trim();
                finishedCode += hexString.Substring(hexString.Length - 8);


                // Finish up this instruction
                finishedCode += "\n";
            }

            return finishedCode;
        }

        public static byte[] GetBinCode(MemoryHandler memoryHandler) {
            List<byte> outputFile = new List<byte>();

            foreach (Memory memEntry in memoryHandler) {
                byte[] bits = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(memEntry.instruction));
                outputFile.AddRange(bits);
            }

            return outputFile.ToArray();
        }

        public static MemoryHandler MapAssemblySource(string rootFilePath) {

            string rootFile = GetFileContent(rootFilePath);
            MemoryHandler memoryHandler = new MemoryHandler();

            // Get entire context with includes
            List<string> cleanedSource = GetFileTree(rootFile);

            foreach (string source in cleanedSource) {
                Console.WriteLine(source);
            }

            // Get labels and addresses
            uint currentAddr = 0;
            for (int i = 0; i < cleanedSource.Count; i++) {
                cleanedSource[i] = cleanedSource[i].Trim();
                // Search for labels

                if (cleanedSource[i][0] == '.') {
                    string label = cleanedSource[i].Substring(1);
                    try {
                        labelAddresses.Add(label, currentAddr);
                        cleanedSource.RemoveAt(i);
                        i--;
                    } catch (ArgumentException e) {
                        Console.WriteLine(e);
                        Console.WriteLine("Duplicate label found at line: " + i);
                    }

                } else if (cleanedSource[i][0] == '@') {
                    if (cleanedSource[i].StartsWith("@ascii")) {
                        currentAddr += (uint)cleanedSource[i].Length - 9;
                    } else if (cleanedSource[i].StartsWith("@fillto")) {
                        string[] tokens = cleanedSource[i].Split(' ');
                        uint addrToFillTo = Helper.GetBinFromType(tokens[1]);
                        currentAddr = addrToFillTo;
                    }
                } else {
                    // Special case of movi needs to be handled.
                    if (cleanedSource[i].StartsWith("movi")) {
                        currentAddr += 2;
                    } else {
                        currentAddr++;
                    }
                }
            }

            foreach (KeyValuePair<string, uint> pair in labelAddresses) {
                Console.WriteLine("Label: {0} Address: {1}", pair.Key, pair.Value);
            }

            // Convert to Memory Handler
            currentAddr = 0;
            for (int i = 0; i < cleanedSource.Count; i++) {
                
                string currentLine = cleanedSource[i];

                // Get the code for this line
                uint[] decodedLine = DecodeLine(currentLine, currentAddr, out uint memOffset);
                
                // Add to Memory Handler
                foreach (uint data in decodedLine) {
                    memoryHandler.AddMemoryEntry(new Memory(currentAddr, data));
                    // Increment the address
                    currentAddr++;
                }
            }

            // Ensure memory is now in correct order.
            memoryHandler.SortMemory();

            return memoryHandler;
        }

        public static string GetFileContent(string filePath) {
            string fileContent = "";
            try {
                StreamReader sr = new StreamReader(filePath);
                String? line = sr.ReadLine();
                while (line != null) {
                    fileContent += line + "\n";
                    line = sr.ReadLine();
                }

                sr.Close();

            } catch {
                Console.Error.WriteLine("Could not find file: " + filePath);
            }

            return fileContent;
        }

        public static List<string> CleanAndSplitFile(string file) {
            // Convert file to a list of lines, and strip all empty lines, whitespace, and comments.
            List<String> lines = file.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList();

            for (int i = 0; i < lines.Count; i++) {
                bool dec = false;
                // Double check empty
                if (lines[i] == "") {
                    lines.RemoveAt(i);
                    dec = true;
                }

                // Remove one line comments
                if (lines[i].StartsWith('#')) {
                    lines.RemoveAt(i);
                    dec = true;
                }

                // Remove trailing comments
                if (lines[i].Contains('#')) {
                    int commentStart = lines[i].IndexOf('#');
                    lines[i] = lines[i].Substring(0, commentStart);
                }

                // Remove commas
                lines[i] = lines[i].Replace(",", "");

                // Remove remaining whitespace
                lines[i] = lines[i].Trim();

                if (dec) {
                    i--;
                }
            }

            return lines;
        }

        public static List<string> GetFileTree(string file) {
            List<string> completedFile = new List<string>();

            // Get clean version of file
            List<string> fileLines = CleanAndSplitFile(file);

            for (int i = 0; i < fileLines.Count; i++) {
                string currentLine = fileLines[i];

                // Check to see if an include exists here
                if (currentLine.StartsWith("@include")) {
                    string[] tokens = currentLine.Split(' ');
                    string includedFile = GetFileContent(tokens[1]);
                    completedFile.AddRange(GetFileTree(includedFile));
                    continue;
                }

                // No include, so just add the line to the source
                completedFile.Add(currentLine);
            }

            return completedFile;
        }
    }
}
