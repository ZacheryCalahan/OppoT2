using OppoT2Assembler.ISADefinitions;
using OppoT2Assembler.MemHandler;

using OppoT2Assembler.ErrorHandler;

namespace OppoT2Assembler
{
    public static class Assembler
    {
        private static Dictionary<String, uint> labelAddresses = new Dictionary<String, uint>();
        

        // Methods
        public static uint[] AssembleFile(string filepath)
        {
            MemoryHandler memMap = GenerateMemoryEntries(filepath);

            // If empty, syntax errors exist, or no file. Either way, what would you write?
            if (memMap.IsEmpty())
            {
                return new uint[0];
            }

            return GenerateCode(memMap);
        }

        public static uint[] GenerateCode(MemoryHandler memMap)
        {
            List<uint> code = new List<uint>();

            uint memMapSize = memMap.program.Last().MemoryAddress;

            uint address = 0;
            foreach (Memory entry in memMap)
            {
                // Instructions need to be converted.
                if (entry.IsInstruction)
                {
                    throw new NotImplementedException();
                }

                while (entry.MemoryAddress != address)
                {
                    code.Add(0);
                    address++;
                }

                code.Add((uint) entry.Data);
                address++;
            }
            

            return code.ToArray();
        }

        public static MemoryHandler GenerateMemoryEntries(string filePath)
        {
            // Vars
            MemoryHandler memMap = new MemoryHandler(); // Memory Map
            string rootFile = GetFileContent(filePath); // String of all source lines
            SyntaxErrors syntaxErrors = new SyntaxErrors(); // List of any syntax errors

            List<string> fileLines = GetFileTree(rootFile);

            // Pass 1: Generate Labels, and decode lines to Instruction.
            //List<Instruction> instructions = new List<Instruction>(); // Instructions list 
            uint address = 0; // Memory Address
            foreach (string line in fileLines)
            {
                // Labels
                if (line.StartsWith(ISA.labelMarker))
                {
                    labelAddresses.Add(line.Substring(1), address);
                }
                // Directives
                else if (line.StartsWith(ISA.directiveMarker))
                {
                    // Ascii Directive
                    if (line.StartsWith("@ascii"))
                    {

                        // Get the ascii string that will be placed in the executable
                        string token = line.Replace("@ascii", "").Trim();
                        token = token.Substring(1, token.Length - 2);
                        string decodedAscii = ISA.GetAsciiFromDirective(token);
                        decodedAscii += "\0";

                        // Create a memory entry for each char.
                        foreach (char c in decodedAscii)
                        {
                            memMap.AddMemoryEntry(new Memory(address, c));
                            address++;
                        }                        
                    }
                    // Org Directive
                    else if (line.StartsWith("@org"))
                    {
                        if (!ISA.TryGetBinFromType(line.Split(" ")[1], out uint orgAddress)) {
                            syntaxErrors.AddError(line, SyntaxErrors.Error.InvalidAddress);
                        }

                        address = orgAddress;
                    } 
                    else
                    {
                        syntaxErrors.AddError(line, SyntaxErrors.Error.InvalidDirective);
                    }
                }
                // Instructions
                else
                {
                    if (!TryHandleInstruction(line, out List<Instruction> listOfInstructions, out uint instructionCount, out SyntaxErrors.Error? error))
                    {
                        if (error == null)
                        {
                            throw new Exception();
                        }

                        syntaxErrors.AddError(line, (SyntaxErrors.Error) error);
                    }
                    
                    // Verify list has no errors.
                    if (listOfInstructions == null)
                    {
                        syntaxErrors.AddError(line, SyntaxErrors.Error.InvalidInstruction);
                    } 
                    else
                    {
                        foreach (Instruction instruction in listOfInstructions)
                        {
                            memMap.AddMemoryEntry(new Memory(address, instruction));
                            address++;
                        }
                    }
                    
                }
            }

            // If syntax errors are present, do not run pass 2 with invalid data.
            if (!syntaxErrors.IsEmpty())
            {
                foreach (SyntaxErrors.SyntaxInfo lineInfo in syntaxErrors)
                {
                    Console.WriteLine("Error: {0} contains {1} error.", lineInfo.line, Enum.GetName(lineInfo.error));
                }
                return new MemoryHandler();
            }

            // Pass 2: Fill all unmarked labels, and convert Instruction objects to uints

            for (int i = 0; i < memMap.Count(); i++)
            {
                Memory memEntry = memMap.program[i];
                // Focus on Memory Entries that are instructions, as non-instructions are finished.
                if (memEntry.IsInstruction)
                {

                    // Get required data
                    Instruction instruction = (Instruction) memEntry.Data;
                    uint memoryLocation = memEntry.MemoryAddress;
                    uint opcode = instruction.GetOpcode();

                    // Only handle instructions with incomplete data.
                    if (instruction.HasSplitLabel)
                    {
                        uint labelAddress = labelAddresses[instruction.Label];
                        // Need to split the label address into its components based on the opcode.
                        if (opcode == ISA.Opcodes["lui"])
                        {
                            uint luiImm = labelAddress >> 17;
                            instruction.SetOperand(Instruction.Operand.Simm17, luiImm);
                        }
                        else if (opcode == ISA.Opcodes["ori"])
                        {
                            uint oriImm = labelAddress & 0b00000000000000011111111111111111;
                            instruction.SetOperand(Instruction.Operand.Simm17, oriImm);
                        }
                        else
                        {
                            // Split label, but not movi?!?
                            throw new Exception();
                        }

                        // Label is mapped, rewrite memEntry to be completed.
                        if (!instruction.TryGetInstruction(out uint data))
                        {
                            throw new Exception();
                        }
                        memMap.program[i] = new Memory(memoryLocation, data);
                    }
                    else if (instruction.HasLabel)
                    {
                        uint labelAddress = labelAddresses[instruction.Label];
                        // Branch Conditional uses an offset, not the direct address. Handle this first.
                        if (opcode == ISA.Opcodes["brc"])
                        {
                            instruction.SetOperand(Instruction.Operand.Simm13, labelAddress - memoryLocation - 1);
                            if (!instruction.TryGetInstruction(out uint data))
                            {
                                throw new Exception(); // This means that the label wasn't properly taken care of.
                            }

                            memMap.program[i] = new Memory(memoryLocation, data);
                        }
                        else
                        {
                            throw new Exception(); // This means something else has a label, and I didn't think of it.
                        }
                    }
                    else
                    // Convert memEntries with Instruction to uints
                    {
                        if (!instruction.TryGetInstruction(out uint data))
                        {
                            throw new Exception(); // This means that the label wasn't properly taken care of.
                        }

                        memMap.program[i] = new Memory(memoryLocation, data);
                    }
                    
                }
            }

            // Sort addresses to be in ascending order for conversion.
            memMap.SortMemory();
            return memMap;
        }

        public static bool TryHandleInstruction(string line, out List<Instruction> listOfInstructions, out uint instructionCount, out SyntaxErrors.Error? error)
        {
            string[] tokens = line.Split(" ");
            List<Instruction> instructions = new List<Instruction>();

            // Get the count of tokens, and the instruction format.

            uint expectedTokenCount = ISA.GetTokenCountFromOpcode(tokens[0]);
            ISA.InstructionFormat format = ISA.GetInstructionType(tokens[0]);

            // Catch invalid instructions
            if (format == ISA.InstructionFormat.INVALID)
            {
                error = SyntaxErrors.Error.InvalidToken;
                listOfInstructions = null;
                instructionCount = 0;
                return false;
            }

            if (expectedTokenCount != tokens.Length)
            {
                error = SyntaxErrors.Error.InvalidTokenCount;
                listOfInstructions = null;
                instructionCount = 0;
                return false;
            }

            // Handle the instruction

            // Handle special cases of psuedo ops
            if (line.StartsWith("movi"))
            {
                // Build Instructions
                Instruction luiInstruction = new Instruction();
                luiInstruction.SetOperand(Instruction.Operand.Opcode, ISA.Opcodes["lui"]);
                Instruction oriInstruction = new Instruction();
                oriInstruction.SetOperand(Instruction.Operand.Opcode, ISA.Opcodes["ori"]);

                // Generate reg A
                if (!ISA.TryGetBinFromType(tokens[1], out uint moviRegA))
                {
                    error = SyntaxErrors.Error.InvalidToken;
                    listOfInstructions = null;
                    instructionCount = 0;
                    return false;
                }

                luiInstruction.SetOperand(Instruction.Operand.RegisterA, moviRegA);
                oriInstruction.SetOperand(Instruction.Operand.RegisterA, moviRegA);
                oriInstruction.SetOperand(Instruction.Operand.RegisterB, moviRegA);

                // Determine if label, or hardcoded imm value.
                if (ISA.TryGetBinFromType(tokens[2], out uint moviImm)) {
                    error = null;
                    // Get imm values
                    uint luiImm = moviImm >> 17;
                    uint oriImm = moviImm & 0b00000000000000011111111111111111;

                    luiInstruction.SetOperand(Instruction.Operand.Imm15, luiImm);
                    oriInstruction.SetOperand(Instruction.Operand.Simm17, oriImm);

                    // Push and return
                    instructions.Add(luiInstruction);
                    instructions.Add(oriInstruction);
                    listOfInstructions = instructions;
                    instructionCount = 2;
                    return true;
                }
                // This is a label.
                else
                {
                    error = null;
                    // Flag instruction as incomplete.
                    luiInstruction.HasSplitLabel = true;
                    oriInstruction.HasSplitLabel = true;
                    luiInstruction.SetLabel(tokens[2]);
                    oriInstruction.SetLabel(tokens[2]);

                    // Push and return.
                    instructions.Add(luiInstruction);
                    instructions.Add(oriInstruction);
                    listOfInstructions = instructions;
                    instructionCount = 2;
                    return true;
                }
            }

            // If we reach this point, we know for sure that the instruction is one dword.
            Instruction instruction = new Instruction();
            instructionCount = 1;

            // Get Opcode
            if (!ISA.TryGetBinFromType(tokens[0], out uint opcode))
            {
                error = SyntaxErrors.Error.InvalidToken;
                instructions.Add(instruction);
                listOfInstructions = instructions;
                instructionCount = 0;
                return false;
            }

            instruction.SetOperand(Instruction.Operand.Opcode, opcode);

            // Check for OP
            if (format == ISA.InstructionFormat.OP)
            {
                error = null;
                instructions.Add(instruction);
                listOfInstructions = instructions;
                return true;
            }

            // Get regA
            if (!ISA.TryGetBinFromType(tokens[1], out uint regA))
            {
                error = SyntaxErrors.Error.InvalidToken;
                instructions.Add(instruction);
                listOfInstructions = instructions;
                instructionCount = 0;
                return false;
            }

            instruction.SetOperand(Instruction.Operand.RegisterA, regA);

            // Check for type R
            if (format == ISA.InstructionFormat.R)
            {
                error = null;
                instructions.Add(instruction);
                listOfInstructions = instructions;
                return true;
            }

            // Check for type RI
            if (format == ISA.InstructionFormat.RI)
            {
                if (!ISA.TryGetBinFromType(tokens[2], out uint imm15))
                {
                    error = SyntaxErrors.Error.InvalidToken;
                    instructions.Add(instruction);
                    listOfInstructions = instructions;
                    instructionCount = 0;
                    return false;
                }

                error = null;

                instruction.SetOperand(Instruction.Operand.Imm15, imm15);
                instructions.Add(instruction);
                listOfInstructions = instructions;
                return true;
            }

            // Get regB
            if (!ISA.TryGetBinFromType(tokens[2], out uint regB))
            {
                error = SyntaxErrors.Error.InvalidToken;
                instructions.Add(instruction);
                listOfInstructions = instructions;
                instructionCount = 0;
                return false;
            }

            instruction.SetOperand(Instruction.Operand.RegisterB, regB);

            // Check for type RR
            if (format == ISA.InstructionFormat.RR)
            {
                error = null;
                instructions.Add(instruction);
                listOfInstructions = instructions;
                return true;
            }

            // All remaining opcodes are not identical beyone already computed operands.

            // Check for RRR
            if (format == ISA.InstructionFormat.RRR)
            {
                if (!ISA.TryGetBinFromType(tokens[3], out uint regC))
                {
                    error = SyntaxErrors.Error.InvalidToken;
                    instructions.Add(instruction);
                    listOfInstructions = instructions;
                    instructionCount = 0;
                    return false;
                }

                error = null;

                instruction.SetOperand(Instruction.Operand.RegisterC, regC);
                instructions.Add(instruction);
                listOfInstructions = instructions;
                return true;

            }

            // Check for RRI
            if (format == ISA.InstructionFormat.RRI)
            {
                if (!ISA.TryGetBinFromType(tokens[3], out uint simm17))
                {
                    error = SyntaxErrors.Error.InvalidToken;

                    instructions.Add(instruction);
                    listOfInstructions = instructions;
                    instructionCount = 0;
                    return false;
                }

                error = null;

                instruction.SetOperand(Instruction.Operand.Simm17, simm17);
                instructions.Add(instruction);
                listOfInstructions = instructions;
                return true;
            }

            // Check for RRCI
            if (format == ISA.InstructionFormat.RRCI)
            {
                // Get Conditional
                if (!ISA.TryGetBinFromType(tokens[3], out uint cond))
                {
                    error = SyntaxErrors.Error.InvalidToken;
                    instructions.Add(instruction);
                    listOfInstructions = instructions;
                    instructionCount = 0;
                    return false;
                }

                instruction.SetOperand(Instruction.Operand.Cond, cond);

                // Check if imm or label
                if (ISA.TryGetBinFromType(tokens[4], out uint simm13))
                {

                    error = null;

                    // Imm value
                    instruction.SetOperand(Instruction.Operand.Simm13, simm13);
                    instructions.Add(instruction);
                    listOfInstructions = instructions;
                    return true;
                }
                else
                {
                    error = null;

                    // Label
                    instruction.SetLabel(tokens[4]);
                    instructions.Add(instruction);
                    listOfInstructions = instructions;
                    return true;
                }

            }

            // We should never get here, but to satisfy C#, ¯\_(ツ)_/¯
            error = SyntaxErrors.Error.InvalidInstruction;
            instructions.Add(instruction);
            listOfInstructions = instructions;
            instructionCount = 0;
            return false;
        }

        public static string GetFileContent(string filePath)
        {
            string fileContent = "";
            try
            {
                StreamReader sr = new StreamReader(filePath);
                String? line = sr.ReadLine();
                while (line != null)
                {
                    fileContent += line + "\n";
                    line = sr.ReadLine();
                }

                sr.Close();

            }
            catch
            {
                Console.Error.WriteLine("Could not find file: " + filePath);
            }

            return fileContent;
        }

        public static List<string> CleanAndSplitFile(string file)
        {
            // Convert file to a list of lines, and strip all empty lines, whitespace, and comments.
            List<String> lines = file.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList();

            for (int i = 0; i < lines.Count; i++)
            {
                bool dec = false;
                // Double check empty
                if (lines[i] == "")
                {
                    lines.RemoveAt(i);
                    dec = true;
                }

                // Remove one line comments
                if (lines[i].StartsWith('#'))
                {
                    lines.RemoveAt(i);
                    dec = true;
                }

                // Remove trailing comments
                if (lines[i].Contains('#'))
                {
                    int commentStart = lines[i].IndexOf('#');
                    lines[i] = lines[i].Substring(0, commentStart);
                }

                // Remove commas
                lines[i] = lines[i].Replace(",", "");

                // Remove remaining whitespace
                lines[i] = lines[i].Trim();

                if (dec)
                {
                    i--;
                }
            }

            return lines;
        }

        public static List<string> GetFileTree(string file)
        {
            List<string> completedFile = new List<string>();

            // Get clean version of file
            List<string> fileLines = CleanAndSplitFile(file);

            for (int i = 0; i < fileLines.Count; i++)
            {
                string currentLine = fileLines[i];

                // Check to see if an include exists here
                if (currentLine.StartsWith("@include"))
                {
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
