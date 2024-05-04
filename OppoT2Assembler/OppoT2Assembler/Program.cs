﻿using OppoT2Assembler;
using OppoT2Assembler.ISADefinitions;
using System.Net;
using System.Text.RegularExpressions;

class Program
{
    public static void Main(String[] args)
    {
        Initialize();

        string fileRead = args[0];
        string fileWrite = args[1];

        uint[] assembledFile = Assembler.AssembleFile(fileRead);

        WriteHexFile(assembledFile, fileWrite);
    }

    public static void WriteBinFile(uint[] assembledFile, string path)
    {
        using (var stream = File.Open(path, FileMode.OpenOrCreate))
        {
            using (var writer = new BinaryWriter(stream))
            {
                foreach (uint dword in assembledFile)
                {
                    writer.Write(dword);
                    // Writes in little-endian.
                }
            }
        }
    }

    public static void WriteHexFile(uint[] assembledFile, string path)
    {
        string hexCode = "";

        foreach (uint dword in assembledFile)
        {
            string hexString = Convert.ToHexString(BitConverter.GetBytes(IPAddress.HostToNetworkOrder(dword))).Trim();
            hexCode += hexString.Substring(hexString.Length - 8);

            hexCode += "\n";
        }

        try
        {
            StreamWriter sr = new StreamWriter(path);
            sr.Write(hexCode);
            sr.Close();
        }
        catch (Exception ex)
        {

        }
    }

    public static void Initialize()
    {
        ISA.InitDictionaries();
    }

}