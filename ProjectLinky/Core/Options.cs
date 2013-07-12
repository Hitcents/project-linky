using CommandLine;
using CommandLine.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectLinky
{
    public class Options
    {
        [Option('i', "in", Required = false, HelpText = "Input XML file.")]
        public string InputFile { get; set; }

        [Option('d', "dry-run", Required = false, HelpText = "Just prints what would happen, doesn't do anything.")]
        public bool DryRun { get; set; }

        [ParserState]
        public IParserState LastParserState { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            return HelpText.AutoBuild(this, c => HelpText.DefaultParsingErrorsHandler(this, c));
        }
    }
}
