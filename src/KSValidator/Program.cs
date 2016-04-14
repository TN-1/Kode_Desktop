using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.IO;

using kOS.Safe.Compilation.KS;

using NDesk.Options;
/// Thanks to 



namespace KSValidator
{
    public class Program
    {
        
        static void Main(string[] args)
        {

            String code = "";
            bool helpOpt = false;
            var p = new OptionSet()
            {
                {"h|help", "show the help message",v=> helpOpt = (v!=null) },
                {"f|file=", "the kerboscript source {file} to validate", f=>{code = File.ReadAllText(f);} },
                {"r|rawCode=","raw kerboscript to be validated, must be in parenthesizes {\"...\"}, all internal parenthesies must be escaped", v=>code = v},
                
            };

            try
            {
                p.Parse(args);
            }
            catch(FileNotFoundException f)
            {
                Console.Error.WriteLine("File not found: {0}\n exit code 1", f.FileName);
                exit(1);
            }
            

            if (helpOpt) showHelp(p);
            else PrintErrors(code);

            
            // Keep the console window open in debug mode.
            exit(0);
        }

        static void exit(Int32 c)
        {
            #if DEBUG
            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
            #endif
            Environment.Exit(c);
        }
        

        public static List<string> PrintErrors(String code)
        {
            List<string> errors = new List<string>();
            var v = listErrors(code);
            foreach(var e in v)
                errors.Add("Error file " + e.File + "line " + e.Line + ",col " + e.Column + " : " + e.Message);
            return errors;
        }
        
        static List<ParseError> listErrors(String code)
        {

           Parser p = (new Parser(new Scanner()));
           return p.Parse(code).Errors;
                        
        }

        static void showHelp(OptionSet p)
        {
            Console.WriteLine("A simple out of game validator for the KerboScript Language");
            p.WriteOptionDescriptions(Console.Out);
        }
        
   

        

    }
}
