using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ProjectLinky.Tests
{
    [TestClass, 
        DeploymentItem("Data\\test-linky.xml"),
        DeploymentItem("Data\\BattlePillars.Droid.csproj"),
        DeploymentItem("Data\\BattlePillars.iOS.csproj")]
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

        [TestMethod]
        public void ParseConfig()
        {
            _options.InputFile = "test-linky.xml";

            bool callback = false;
            Linky.Run(_options, c =>
            {
                callback = true;
            });

            Assert.IsTrue(callback, "Didn't fire callback!");
        }
    }
}
