using OppoT2Assembler;

class Program {
    public static void Main(String[] args) {
        Helper.InitDictionaries();

        if (args.Length != 2) {
            Console.WriteLine("Arguments expected are (.asm) (.o)");
            return;
        }

        string fileRead = args[0];
        string fileWrite = args[1];

        string asmFile = "";

        try {
            StreamReader sr = new StreamReader(fileRead);
            String line = sr.ReadLine();
            while (line != null) {
                asmFile += line + "\n";
                line = sr.ReadLine();
            }

            sr.Close();
            
        } catch (Exception ex) {
            Console.WriteLine("Exception: " + ex.Message);
        }

        // Do parsing here!
        string intCode = Assembler.Assemble(asmFile);
        //Console.WriteLine(Assembler.GetHexCode(intCode));

        try {
            StreamWriter sw = new StreamWriter(fileWrite);
            sw.Write(Assembler.GetHexCode(intCode));
            sw.Close();
        } catch (Exception ex) {
            Console.WriteLine("Unable to write here.");
        }
    }
}