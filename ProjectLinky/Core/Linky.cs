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

        public static Func<ProjectFile> GetProjectFile = () => new ProjectFile();

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
            if (config.Projects == null)
                throw new Exception("No projects found!");

            if (configCallback != null)
                configCallback(config);

            string inputDirectory = Path.GetDirectoryName(Path.GetFullPath(inputPath));

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

                var projectFile = GetProjectFile();
                projectFile.Load(projectPath);

                bool needsSave = false;
                var existing = new Dictionary<Project, List<string>>();
                var removeNodes = new List<Action>();

                //Remove files from project that do not exist
                foreach (ItemGroup itemGroup in projectFile)
                {
                    foreach (var rule in project.Rules)
                    {
                        string pattern = RegexFormat(rule.OutputPattern);

                        if (Regex.IsMatch(itemGroup.Link, "^" + pattern, RegexOptions.IgnoreCase))
                        {
                            string fullPath = Path.Combine(Path.GetDirectoryName(projectPath), itemGroup.Include);
                            if (File.Exists(fullPath) && (string.IsNullOrEmpty(rule.ExcludePattern) || !Regex.IsMatch(itemGroup.Include, rule.ExcludePattern, RegexOptions.IgnoreCase)))
                            {
                                List<string> list;
                                if (!existing.TryGetValue(project, out list))
                                {
                                    existing[project] =
                                        list = new List<string>();
                                }

                                //Use ToUpper() to account for weird casing issues
                                list.Add(itemGroup.Include.ToUpperInvariant());
                            }
                            else
                            {
                                if (removeCallback != null)
                                    removeCallback(project, Path.GetFileName(itemGroup.Include));

                                if (!options.DryRun)
                                {
                                    //We have to add a lambda to the list so it can be removed after the for-each loop
                                    var temp = itemGroup;
                                    removeNodes.Add(() => projectFile.Remove(temp));
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
                    foreach (var fullPath in Directory.EnumerateFiles(inputDirectory, rule.InputPattern, SearchOption.AllDirectories))
                    {
                        List<string> list;
                        string relativePath = ToRelativePath(projectPath, fullPath);
                        if (existing.TryGetValue(project, out list) && !list.Contains(relativePath.ToUpperInvariant()))
                        {
                            //Check for exclude
                            if (!string.IsNullOrEmpty(rule.ExcludePattern) && Regex.IsMatch(relativePath, rule.ExcludePattern, RegexOptions.IgnoreCase))
                            {
                                continue;
                            }

                            if (addCallback != null)
                                addCallback(project, Path.GetFileName(fullPath));

                            if (!options.DryRun)
                            {
                                projectFile.Append(new ItemGroup
                                {
                                    Include = relativePath,
                                    Link = Path.Combine(rule.OutputPattern, Path.GetFileName(fullPath)),
                                    BuildAction = rule.BuildAction,
                                });
                            }
                        }
                    }
                }

                if (!options.DryRun && needsSave)
                {
                    projectFile.Save(projectPath);
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

            return Uri.UnescapeDataString(baseUri.MakeRelativeUri(uri).ToString()).Replace("/", "\\");
        }
    }
}
