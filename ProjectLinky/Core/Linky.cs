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

        public static void Run(Options options, Action<Config> configCallback = null, Action<Project, string> removeCallback = null, Action<Project, string> addCallback = null)
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

            string inputDirectory = Path.GetDirectoryName(Path.GetFullPath(options.InputFile));

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

                bool needsSave = false;

                List<string> existing = new List<string>();
                List<Action> removeNodes = new List<Action>();

                //Remove files from project that do not exist
                foreach (XmlNode itemGroup in doc.DocumentElement.GetElementsByTagName("ItemGroup"))
                {
                    foreach (XmlNode node in itemGroup)
                    {
                        var linkNode = node.FirstChild;

                        if (linkNode == null || linkNode.Name != "Link")
                            continue;

                        var includeNode = node.Attributes["Include"];
                        if (includeNode == null)
                            continue;

                        foreach (var rule in project.Rules)
                        {
                            string pattern = RegexFormat(rule.OutputPattern);

                            if (Regex.IsMatch(linkNode.InnerText, pattern))
                            {
                                if (File.Exists(includeNode.Value))
                                {
                                    existing.Add(includeNode.Value);
                                }
                                else
                                {
                                    if (removeCallback != null)
                                        removeCallback(project, includeNode.Value);

                                    if (!options.DryRun)
                                    {
                                        //We have to add a lambda to the list so it can be removed after the for-each loop
                                        var temp = node;
                                        var tempGroup = itemGroup;
                                        removeNodes.Add(() =>
                                        {
                                            temp.ParentNode.RemoveChild(temp);
                                            if (tempGroup.ChildNodes.Count == 0)
                                                tempGroup.ParentNode.RemoveChild(tempGroup);
                                        });
                                    }
                                }
                            }
                        }
                    }
                }

                //Peform actions to remove nodes
                foreach (var action in removeNodes)
                {
                    action();

                    needsSave = true;
                }

                //Add new files to project
                foreach (var rule in project.Rules)
                {
                    foreach (var file in Directory.EnumerateFiles(inputDirectory, rule.InputPattern))
                    {
                        string relative = ToRelativePath(project.Path, file);

                        if (!existing.Contains(relative))
                        {
                            if (addCallback != null)
                                addCallback(project, relative);

                            if (!options.DryRun)
                            {
                                var itemGroup = doc.CreateElement("ItemGroup", doc.DocumentElement.NamespaceURI);

                                var content = doc.CreateElement(rule.BuildAction, doc.DocumentElement.NamespaceURI);
                                content.SetAttribute("Include", relative);
                                itemGroup.AppendChild(content);

                                var link = doc.CreateElement("Link", doc.DocumentElement.NamespaceURI);
                                link.InnerText = Path.Combine(rule.OutputPattern, Path.GetFileName(file));
                                content.AppendChild(link);

                                doc.DocumentElement.AppendChild(itemGroup);

                                needsSave = true;
                            }
                        }
                    }
                }

                if (!options.DryRun && needsSave)
                {
                    doc.Save(projectPath);
                }
            }
        }

        private static string RegexFormat(string pattern)
        {
            pattern = pattern.Replace("\\", "\\\\");

            return pattern;
        }

        private static string ToRelativePath(string basePath, string absolutePath)
        {
            var uri = new Uri(absolutePath, UriKind.RelativeOrAbsolute);
            if (!uri.IsAbsoluteUri)
            {
                uri = new Uri(Path.GetFullPath(absolutePath));
            }

            var baseUri = new Uri(basePath, UriKind.RelativeOrAbsolute);
            if (!baseUri.IsAbsoluteUri)
            {
                baseUri = new Uri(Path.GetFullPath(basePath));
            }

            return baseUri.MakeRelativeUri(uri).ToString().Replace("/", "\\");
        }
    }
}
