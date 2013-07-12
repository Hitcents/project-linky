using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;

namespace ProjectLinky
{
    public static class Linky
    {
        private static readonly XmlSerializer _serializer = new XmlSerializer(typeof(Config));

        public static void Run(Options options, Action<Config> configCallback = null)
        {
            string inputPath = options.InputFile;

            //Look for linky.xml by convention
            if (string.IsNullOrEmpty(inputPath) || !File.Exists(inputPath))
            {
                inputPath = Path.Combine(Environment.CurrentDirectory, "linky.xml");
            }

            Config config;
            using (var file = File.OpenRead(inputPath))
            {
                config = _serializer.Deserialize(file) as Config;
            }

            if (configCallback != null)
                configCallback(config);

            string inputDirectory = Path.GetDirectoryName(options.InputFile);

            foreach (var project in config.Projects)
            {
                string projectPath = project.Path;
                if (string.IsNullOrEmpty(projectPath))
                {
                    throw new ArgumentNullException("projectPath");
                }
                if (!File.Exists(projectPath))
                {
                    projectPath = Path.Combine(inputDirectory, projectPath);
                }

                var doc = new XmlDocument();
                doc.Load(projectPath);

                var ns = new XmlNamespaceManager(doc.NameTable);
                ns.AddNamespace("x", "http://schemas.microsoft.com/developer/msbuild/2003");

                foreach (XmlNode itemGroup in doc.DocumentElement.GetElementsByTagName("ItemGroup"))
                {
                    foreach (XmlNode node in itemGroup)
                    {
                        var linkNode = node.FirstChild;
                        if (linkNode == null || linkNode.Name != "Link")
                            continue;

                        //Remove files from project that don't exist
                        foreach (var rule in project.Rules)
                        {
                            string pattern = RegexFormat(rule.OutputPattern);

                            if (Regex.IsMatch(linkNode.InnerText, pattern))
                            {

                            }
                        }

                        //Add new files to project
                    }
                }
            }
        }

        private static string RegexFormat(string pattern)
        {
            pattern = pattern.Replace("\\", "\\\\");

            return pattern;
        }
    }
}
