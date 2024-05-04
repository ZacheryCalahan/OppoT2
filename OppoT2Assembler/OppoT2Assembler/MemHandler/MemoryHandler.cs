using System.Collections;

namespace OppoT2Assembler.MemHandler
{
    public struct MemoryHandler : IEnumerable<Memory>
    {
        public List<Memory> program {  get; private set; }

        public MemoryHandler()
        {
            program = new List<Memory>();
        }

        public void SortMemory()
        {
            program.OrderBy(x => x.MemoryAddress);
        }

        public void AddMemoryEntry(Memory data)
        {
            uint dataAddress = data.MemoryAddress;

            if (ContainsAddress(dataAddress))
            {
                Console.Error.WriteLine("Memory overwrite error.");
                return;
            }

            program.Add(data);
        }

        public bool ContainsAddress(uint dataAddress)
        {
            return program.Any(x => x.MemoryAddress == dataAddress);
        }

        public uint Length()
        {
            return (uint) program.Count();
        }

        public IEnumerator<Memory> GetEnumerator()
        {
            foreach (Memory memEntry in program)
            {
                yield return memEntry;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
