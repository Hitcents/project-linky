using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommandLine;

namespace ProjectLinky
{
    class Program
    {
        static void Main(string[] args)
        {
            var options = new Options();
            if (Parser.Default.ParseArguments(args, options))
            {
            }

#if DEBUG
            Console.WriteLine("Press enter to quit...");
            Console.ReadLine();
#endif
        }
    }
}
