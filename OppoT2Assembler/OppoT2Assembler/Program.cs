using OppoT2Assembler;

class Program {
    public static void Main(String[] args) {
        Initialize();

        if (args.Length > 2) {
            // Flags are set
            if (args[2] == "hex") {
                GenerateHexFile(args);
            } else {
                Console.Error.WriteLine("Invalid arguments.");
                foreach (var arg in args) {
                    Console.Write(arg + " ");
                }
            }

            return;
        }

        //GenerateBinFile(args);
    }

    public static void Initialize() {
        Helper.InitDictionaries();
    }


    public static void GenerateHexFile(String[] args) {
        string data = Assembler.GetHexCode(Assembler.MapAssemblySource(args[0]));
        try {
            StreamWriter sw = new StreamWriter(args[1]);
            sw.Write(data);
            sw.Close();
        } catch {
            Console.Error.WriteLine("Invalid destination path.");

        }
    }

    public static void GenerateBinFile(String[] args) {
        byte[] bytes = Assembler.GetBinCode(Assembler.MapAssemblySource(args[0]));

        try {
            StreamWriter sw = new StreamWriter(args[1]);
            foreach (byte data in bytes) {
                sw.Write(data);
            }
            sw.Close();
        } catch {
            Console.Error.WriteLine("Invalid destination path.");
        }
    }
}