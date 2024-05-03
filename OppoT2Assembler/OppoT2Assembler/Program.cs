using System;
using System.IO;
using System.Text;

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

        GenerateBinFile(args);
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
        uint[] dwords = Assembler.GetBinCode(Assembler.MapAssemblySource(args[0]));

        using (var stream = File.Open(args[1], FileMode.OpenOrCreate)) {
            using (var writer = new BinaryWriter(stream)) {
                foreach (uint word in dwords) {
                    writer.Write(word);
                }
            }
        }
    }
}