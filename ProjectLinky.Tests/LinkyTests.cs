using ApprovalTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ProjectLinky.Tests
{
    [TestClass, DeploymentItem("Data\\test-linky.xml")]
    public class LinkyTests
    {
        private Options _options;

        [TestInitialize]
        public void TestInitialize()
        {
            _options = new Options();
        }

        [TestMethod]
        public void BlankInputShouldBeLinkyXml()
        {
            try
            {
                Linky.Run(_options);

                Assert.Fail("Previous call should fail!");
            }
            catch (FileNotFoundException exc)
            {
                Assert.IsTrue(exc.FileName.EndsWith("linky.xml"), "Should default to linky.xml!");
            }
        }

        [TestMethod]
        public void FileNotFoundInputShouldBeLinkyXml()
        {
            try
            {
                _options.InputFile = "asfdsafdsadfsafdfsdfdsafsd.xml";

                Linky.Run(_options);

                Assert.Fail("Previous call should fail!");
            }
            catch (FileNotFoundException exc)
            {
                Assert.IsTrue(exc.FileName.EndsWith("linky.xml"), "Should default to linky.xml!");
            }
        }

        [TestMethod,        
            DeploymentItem("Data\\Android.csproj"),
            DeploymentItem("Data\\iOS.csproj"),
            DeploymentItem("Data\\Images\\chuck.png", "Images")]
        public void ParseConfig()
        {
            _options.DryRun = true;
            _options.InputFile = "test-linky.xml";

            bool callback = false;
            Linky.Run(_options, c =>
            {
                callback = true;
            });

            Assert.IsTrue(callback, "Didn't fire callback!");
        }

        [TestMethod,
            DeploymentItem("Data\\Android.csproj"),
            DeploymentItem("Data\\iOS.csproj"),
            DeploymentItem("Data\\Images\\chuck.png", "Images")]
        public void RemoveItemGroup()
        {
            _options.DryRun = true;
            _options.InputFile = "test-linky.xml";

            List<string> removed = new List<string>();
            Linky.Run(_options, removeCallback: (p, f) =>
            {
                removed.Add(f);
            });

            Approvals.VerifyAll(removed, "file");
        }

        [TestMethod,
            DeploymentItem("Data\\iOS.csproj"),
            DeploymentItem("Data\\Images\\chuck.png", "Images"),
            DeploymentItem("Data\\Images\\nerd.png", "Images")]
        public void ApproveiOS()
        {
            _options.InputFile = "test-linky.xml";

            Linky.Run(_options,c =>
            {
                c.Projects = new[]
                { 
                    new Project
                    {
                        Path = "iOS.csproj",
                        Rules = new[] { new Rule { InputPattern = @"Images\*.png", OutputPattern = @"Content\Images", BuildAction = "Content" } },
                    },
                };
            });

            Approvals.VerifyFile("iOS.csproj");
        }

        [TestMethod,
            DeploymentItem("Data\\Android.csproj"),
            DeploymentItem("Data\\Images\\chuck.png", "Images"),
            DeploymentItem("Data\\Images\\nerd.png", "Images")]
        public void ApproveAndroid()
        {
            _options.InputFile = "test-linky.xml";

            Linky.Run(_options, c =>
            {
                c.Projects = new[]
                { 
                    new Project
                    {
                        Path = "Android.csproj",
                        Rules = new[] { new Rule { InputPattern = @"Images\*.png", OutputPattern = @"Assets\Images\", BuildAction = "AndroidAsset" } },
                    },
                };
            });

            Approvals.VerifyFile("Android.csproj");
        }

        [TestMethod,
            DeploymentItem("Data\\Code\\iOS.csproj", "Code"),
            DeploymentItem("Data\\Images\\chuck.png", "Images"),
            DeploymentItem("Data\\Images\\nerd.png", "Images")]
        public void ApproveiOSSubdirectory()
        {
            _options.InputFile = "test-linky.xml";

            Linky.Run(_options, c =>
            {
                c.Projects = new[]
                { 
                    new Project
                    {
                        Path = "Code\\iOS.csproj",
                        Rules = new[] { new Rule { InputPattern = @"Images\*.png", OutputPattern = @"Content\Images", BuildAction = "Content" } },
                    },
                };
            });

            Approvals.VerifyFile("Code\\iOS.csproj");
        }

        [TestMethod,
            DeploymentItem("Data\\Code\\Android.csproj", "Code"),
            DeploymentItem("Data\\Images\\chuck.png", "Images"),
            DeploymentItem("Data\\Images\\nerd.png", "Images")]
        public void ApproveAndroidSubDirectory()
        {
            _options.InputFile = "test-linky.xml";

            Linky.Run(_options, c =>
            {
                c.Projects = new[]
                { 
                    new Project
                    {
                        Path = "Code\\Android.csproj",
                        Rules = new[] { new Rule { InputPattern = @"Images\*.png", OutputPattern = @"Assets\Images\", BuildAction = "AndroidAsset" } },
                    },
                };
            });

            Approvals.VerifyFile("Code\\Android.csproj");
        }
    }
}
