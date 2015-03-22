namespace Console
{
    using System;
    using Core;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Diagnostics;
    using System.Security.AccessControl;

    class Program
    {
        private static uint length;
        private static uint count;
        private static string outputfile;
        private static string dictionaryfile;
        private static bool useSymbols;
        private static bool forceUpper;
        private static bool forceLower;
        private static bool forceNumber;
        private static uint minUpper;
        private static uint minLower;
        private static uint minNumber;
        private static byte minStrength;
        private static char[] dictionary;

        static void Main(string[] args)
        {

            if (args == null || args.Length == 0)
                DisplayArguments();
            else
            {
                ParseArguments(args);    
            }

            bool canGenerate = true;

            if(length < 1)
            {
                canGenerate = false;
                Console.WriteLine("Password length must be greater than 0.");
            }
            if(count < 1)
            {
                canGenerate = false;
                Console.WriteLine("Password count must be greater than 0.");
            }
            if(string.IsNullOrEmpty(outputfile))
            {
                Console.WriteLine("No ouput file specified. Result will be displayed on screen only.");
            }
            if (string.IsNullOrEmpty(dictionaryfile))
            {
                Console.WriteLine("No custom dictionary specified.");
            }
            if(!canGenerate)
            {
                Console.WriteLine("Generation canceled due to missing or invalid arguments.");
                return;
            }

            if (!string.IsNullOrEmpty(dictionaryfile))
                dictionary = ReadDictionaryFileContent(dictionaryfile);
            
            if (useSymbols && dictionary != null)
            {
                useSymbols = false;
                Console.WriteLine("A custom dictionary has been specified. UseSymbols has been set to False.");
            }
            
            StartGeneration();
#if DEBUG
            Console.WriteLine("Press R to restart");
            while(Console.ReadKey(true).KeyChar == 'r' )
                StartGeneration();
#endif   
        }

        static void StartGeneration()
        {
            DisplaySelectedArguments();

            Console.WriteLine("Generation started");

            var st = new Stopwatch();
            st.Start();

            Password[] passwords = null;

            try
            {
                using (var gen = new DefaultPasswordGenerator(new CryptoRandomNumberGenerator()))
                {
                    passwords = gen.Generate(count, length, forceNumber, forceLower, forceUpper, useSymbols, dictionary,
                                 minNumber, minLower, minUpper, (PasswordStrengthEnum)minStrength);
                }
            }
            catch (Exception exp)
            {
                Console.WriteLine("ERROR: {0}", exp.Message);
                return;
            }
            finally { st.Stop(); }

            if (string.IsNullOrEmpty(outputfile))
                foreach (var password in passwords)
                {
                    Console.WriteLine(password);
                }
            else
                WritePasswordsToFile(outputfile, passwords);

            Console.WriteLine("Generation finished. Elapsed time : {0}.", st.Elapsed);
        }

        static void ParseArguments(IList<string> args)
        {
            if(args.Count > 0)
                for( int i=0 ; i< args.Count; i+=2)
                {
                    if(string.Equals("-length",args[i],StringComparison.InvariantCultureIgnoreCase) ||
                       string.Equals("-l", args[i], StringComparison.InvariantCultureIgnoreCase))
                    {
                        if(i+1 > args.Count) continue;
                        length = uint.Parse(args[i + 1]);
                        continue;
                    }
                    if(string.Equals("-count",args[i],StringComparison.InvariantCultureIgnoreCase) ||
                       string.Equals("-c", args[i], StringComparison.InvariantCultureIgnoreCase))
                    {
                        if (i + 1 > args.Count) continue;
                        count = uint.Parse(args[i + 1]);
                        continue;
                    }
                    if (string.Equals("-output", args[i], StringComparison.InvariantCultureIgnoreCase) ||
                        string.Equals("-o", args[i], StringComparison.InvariantCultureIgnoreCase))
                    {
                        if (i + 1 > args.Count) continue;
                        outputfile = args[i + 1];
                        continue;
                    }
                    if (string.Equals("-dictionary", args[i], StringComparison.InvariantCultureIgnoreCase) ||
                        string.Equals("-d", args[i], StringComparison.InvariantCultureIgnoreCase))
                    {
                        if (i + 1 > args.Count) continue;
                        dictionaryfile = args[i + 1];
                        continue;
                    }
                    if (string.Equals("-forceUpper", args[i], StringComparison.InvariantCultureIgnoreCase) ||
                        string.Equals("-fu", args[i], StringComparison.InvariantCultureIgnoreCase))
                    {
                        forceUpper = true;
                        if(string.IsNullOrEmpty(args[i+1]) || args[i+1].StartsWith("-"))
                        {
                            i -= 1;
                        }
                        else
                        {
                            uint.TryParse(args[i + 1], out minUpper);
                        }
                        continue;
                    }
                    if (string.Equals("-forceLower", args[i], StringComparison.InvariantCultureIgnoreCase) ||
                        string.Equals("-fl", args[i], StringComparison.InvariantCultureIgnoreCase))
                    {
                        forceLower = true;
                        if (string.IsNullOrEmpty(args[i + 1]) || args[i + 1].StartsWith("-"))
                        {
                            i -= 1;
                        }
                        else
                        {
                            uint.TryParse(args[i + 1], out minLower);
                        }
                        continue;
                    }
                    if (string.Equals("-forceNumber", args[i], StringComparison.InvariantCultureIgnoreCase) ||
                        string.Equals("-fn", args[i], StringComparison.InvariantCultureIgnoreCase))
                    {
                        forceNumber = true;
                        if (string.IsNullOrEmpty(args[i + 1]) || args[i + 1].StartsWith("-"))
                        {
                            i -= 1;
                        }
                        else
                        {
                            uint.TryParse(args[i + 1], out minNumber);
                        }
                        continue;
                    }
                    if (string.Equals("-useSymbols", args[i], StringComparison.InvariantCultureIgnoreCase) ||
                        string.Equals("-us", args[i], StringComparison.InvariantCultureIgnoreCase))
                    {
                        useSymbols = true;
                        i -= 1;
                        continue;
                    }
                    if (string.Equals("-minStrength", args[i], StringComparison.InvariantCultureIgnoreCase) ||
                        string.Equals("-ms", args[i], StringComparison.InvariantCultureIgnoreCase))
                    {
                        if (string.IsNullOrEmpty(args[i + 1]) || args[i + 1].StartsWith("-"))
                        {
                            i -= 1;
                        }
                        else
                        {
                            byte.TryParse(args[i + 1], out minStrength);
                            if (minStrength > 5) minStrength = 0;
                        }
                        continue;
                    }
                    if(string.Equals("-?",args[i],StringComparison.InvariantCultureIgnoreCase) ||
                       string.Equals("-help", args[i], StringComparison.InvariantCultureIgnoreCase))
                    {
                        DisplayArguments();
                        i -= 1;
                        continue;
                    }
                }
        }

        static void WritePasswordsToFile(string file, IEnumerable<Password> passwords)
        {
            using(var fs = new FileStream(file,FileMode.Append,FileSystemRights.Write,FileShare.Read,4096,FileOptions.None))
            {

                byte[] buffer;
                foreach (var password in passwords)
                {
                    buffer = Encoding.UTF8.GetBytes(String.Format("{0}{1}",password,Environment.NewLine));
                    fs.Write( buffer,0,buffer.Length );
                }
                fs.Flush(true);
            }
        }

        static char[] ReadDictionaryFileContent(string file)
        {
            var content = File.ReadAllLines(file);
            if (content == null || content.Length == 0) return null;
            return string.IsNullOrEmpty(content[0])? null : content[0].ToCharArray();
        }

        static void DisplayArguments()
        {
            Console.WriteLine("Format : [argument]|[short form][space][value]");
            Console.WriteLine("-help|-? : Displays this message.");
            Console.WriteLine("-length|-l [uint] : Defines the password length. Value must be greater than 0.");
            Console.WriteLine("-count|-c [uint]  : Defines the password count. Value must be greater than 0.");
            Console.WriteLine("-output|-o [string] : A file path to write generated passwords to. If the file already exists, content is append to it.");
            Console.WriteLine("-dictionary|-d [string] : A file path to a custom dictionary");
            Console.WriteLine("-forceUpper|-fu [uint] : Defines a minimum upper case letters count. The number must be greater than 0.");
            Console.WriteLine("-forceLower|-fl [uint] : Defines a minimum lower case letters count. The number must be greater than 0.");
            Console.WriteLine("-forceNumber|-fc [uint] : Defines a minimum numbers count. The number must be greater than 0.");
            Console.WriteLine("-minStrength|-ms [byte] : Defines the minimum password strength from 1 to 5. 1=None, 2=Weak, 3=Medium, 4=Strong and 5=Best. Default is 0.");
            Console.WriteLine("-useSymbols|-us : Defines if symbols should be used. If a custom dictionary is specified, this value is ignored.");
        }

        static void DisplaySelectedArguments()
        {
            Console.WriteLine();
            Console.WriteLine("Current parameters :");
            Console.WriteLine("Length : {0}",length);
            Console.WriteLine("Count  : {0}", count);
            Console.WriteLine("ForceUpper : {0}", forceUpper);
            Console.WriteLine("ForceLower : {0}", forceLower);
            Console.WriteLine("ForceNumber : {0}", forceNumber);
            Console.WriteLine("MinimumUpper : {0}", minUpper);
            Console.WriteLine("MinimumLower : {0}", minLower);
            Console.WriteLine("MinimumNumber : {0}", minNumber);
            Console.WriteLine("UseSymbols : {0}", useSymbols);
            Console.WriteLine("MinStrength : {0}", (PasswordStrengthEnum)minStrength);
            Console.WriteLine("Custom Dictionary : {0}", dictionary !=null);
            Console.WriteLine("Custom Dictionary File : {0}", dictionaryfile);
            Console.WriteLine("Output : {0}", string.IsNullOrEmpty(outputfile) ? "not defined" : outputfile);
        }

    }
}
