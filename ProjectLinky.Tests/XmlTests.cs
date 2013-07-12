using ApprovalTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Xml.Serialization;

namespace ProjectLinky.Tests
{
    [TestClass]
    public class XmlTests
    {
        [TestMethod]
        public void Serialize()
        {
            var serializer = new XmlSerializer(typeof(Config));

            using (var file = File.Create(TestConstants.TempFile))
            {
                serializer.Serialize(file, new Config
                {
                    Projects = new []
                    {
                        new Project
                        {
                            Path = "OurGame.iOS.csproj",
                            Rules = new[]
                            {
                                new Rule
                                {
                                    InputPattern = "Assets\\Images\\*.png",
                                },
                            },
                        },
                        new Project
                        {
                            Path = "OurGame.Droid.csproj",
                            Rules = new[]
                            {
                                new Rule
                                {
                                    InputPattern = "Assets\\Images\\*.png",
                                },
                            },
                        },
                        new Project
                        {
                            Path = "OurGame.WinRt.csproj",
                            Rules = new[]
                            {
                                new Rule
                                {
                                    InputPattern = "Assets\\Images\\*.png",
                                },
                            },
                        },
                    }
                });
            }

            Approvals.VerifyFile(TestConstants.TempFile);
        }
    }
}
