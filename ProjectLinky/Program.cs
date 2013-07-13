using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommandLine;
using System.IO;

namespace ProjectLinky
{
    class Program
    {
        static void Main(string[] args)
        {
            var options = new Options();
            if (Parser.Default.ParseArguments(args, options))
            {
                Linky.Run(options,
                    removeCallback: (p, f) => Console.WriteLine("Removed: " + f + ", " + Path.GetFileNameWithoutExtension(p.Path)),
                    addCallback: (p, f) => Console.WriteLine("Added: " + f + ", " + Path.GetFileNameWithoutExtension(p.Path)));
            }

#if DEBUG
            Console.WriteLine("Press enter to quit...");
            Console.ReadLine();
#endif
        }
    }
}
