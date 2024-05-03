namespace OppoT2Assembler.MemHandler {
    public struct Memory {
        public uint memoryAddress { get; private set; }
        public uint instruction { get; private set; }

        public Memory(uint memoryAddress, uint instruction) {
            this.memoryAddress = memoryAddress;
            this.instruction = instruction;
        }

        public Memory() {
            memoryAddress = 0;
            instruction = 0;
        }
    }
}
