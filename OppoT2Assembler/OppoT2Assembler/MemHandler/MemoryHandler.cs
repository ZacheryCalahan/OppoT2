using System.Collections;

namespace OppoT2Assembler.MemHandler {
    public struct MemoryHandler : IEnumerable<Memory> {
        List<Memory> program;

        public MemoryHandler() {
            program = new List<Memory>();
        }

        public void SortMemory() {
            program.OrderBy(x => x.memoryAddress);
        }

        public void AddMemoryEntry(Memory data) {
            uint dataAddress = data.memoryAddress;

            if (ContainsAddress(dataAddress)) {
                Console.Error.WriteLine("Memory overwrite error.");
                return;
            }

            program.Add(data);
        }

        public bool ContainsAddress(uint dataAddress) {
            return program.Any(x => x.memoryAddress == dataAddress);
        }

        public IEnumerator<Memory> GetEnumerator() {
            foreach (Memory memEntry in program) {
                yield return memEntry;
            }
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
    }
}
