using OppoT2Assembler.ISADefinitions;

namespace OppoT2Assembler.MemHandler
{
    public struct Memory
    {
        public uint MemoryAddress { get; private set; }
        public object Data { get; private set; }
        public bool IsInstruction { get; private set; }

        public Memory(uint memoryAddress, uint data)
        {
            MemoryAddress = memoryAddress;
            Data = data;
            IsInstruction = false;
        }

        public Memory(uint memoryAddress, Instruction instruction)
        {
            MemoryAddress = memoryAddress;
            Data = instruction;
            IsInstruction = true;
        }

        public void SetData(uint data)
        {
            Data = data;
            IsInstruction = false;
        }
    }
}
