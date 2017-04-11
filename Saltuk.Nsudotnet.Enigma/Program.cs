using System;
using System.IO;
using System.Security.Cryptography;

namespace Saltuk.Nsudotnet.Enigma
{
    class Program
    {
        class CryptSettings
        {
            public SymmetricAlgorithm Algorithm { get; set; }
            public string InputFilename { get; set; }
            public string OutputFilename { get; set; }
            public string KeyFileName { get; set; }
            public bool IsEncrypting { get; set; }
        }


        static void Main(string[] args)
        {
            CryptSettings settings;
            if (!TryParseArguments(args, out settings))
            {
                Console.WriteLine("Incorrect parameters");
                return;
            }

            using (var inFile = new FileStream(settings.InputFilename, FileMode.Open))
            using (var outFile = new FileStream(settings.OutputFilename, FileMode.Create))
            using (var key = settings.IsEncrypting ?
                new FileStream(settings.OutputFilename + ".key", FileMode.Create) : 
                new FileStream(settings.KeyFileName, FileMode.Open) )
            {

                if (settings.IsEncrypting)
                    Encrypt(settings.Algorithm, inFile, outFile, key);
                else
                    Decrypt(settings.Algorithm, inFile, outFile, key);
                    
            }
        }

        static void Encrypt(SymmetricAlgorithm algorithm, Stream input, Stream output, Stream key)
        {

            using (var crypted = new CryptoStream(output, algorithm.CreateEncryptor(), CryptoStreamMode.Write))
            {
                input.CopyTo(crypted);
            }

            using (var keyWriter = new StreamWriter(key))
            {
                keyWriter.WriteLine(Convert.ToBase64String(algorithm.Key));
                keyWriter.WriteLine(Convert.ToBase64String(algorithm.IV));
            }
        }

        static void Decrypt(SymmetricAlgorithm algorithm, Stream input, Stream output, Stream key)
        {
            using (var keyReader = new StreamReader(key))
            {

                var keyByte = Convert.FromBase64String(keyReader.ReadLine());
                var ivByte = Convert.FromBase64String(keyReader.ReadLine());
                algorithm.IV = ivByte;
                algorithm.Key = keyByte;

                using (var encrypted = new CryptoStream(input, algorithm.CreateDecryptor(), CryptoStreamMode.Read))
                {
                    encrypted.CopyTo(output);
                }

            }
        }

        static bool TryParseArguments(string[] args, out CryptSettings settings)
        {
            settings = new CryptSettings();

            if (args.Length < 4)
                return false;
            settings.InputFilename = args[1];
            settings.OutputFilename = args[3];

            if (args[0].Equals("encrypt", StringComparison.OrdinalIgnoreCase))
                settings.IsEncrypting = true;
            else if (args[0].Equals("decrypt", StringComparison.OrdinalIgnoreCase))
                settings.IsEncrypting = false;
            else
                return false;

            if (!settings.IsEncrypting)
            {
                if (args.Length < 5)
                    return false;
                settings.KeyFileName = args[4];
            }

            if (args[2].Equals("AES", StringComparison.OrdinalIgnoreCase))
                settings.Algorithm = Aes.Create();
            else if (args[2].Equals("DES", StringComparison.OrdinalIgnoreCase))
                settings.Algorithm = DES.Create();
            else if (args[2].Equals("RC2", StringComparison.OrdinalIgnoreCase))
                settings.Algorithm = RC2.Create();
            else if (args[2].Equals("Rijndael", StringComparison.OrdinalIgnoreCase))
                settings.Algorithm = Rijndael.Create();
            else
                return false;

            return true;
        }

    }
}
