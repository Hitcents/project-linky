using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace ProjectLinky
{
    public class ItemGroup
    {
        public string Link { get; set; }

        public string Include { get; set; }

        public string BuildAction { get; set; }

        public XmlNode Node { get; set; }

        public override string ToString()
        {
            return string.Format("Link: {0} Include: {1} BuildAction: {2}", Link, Include, BuildAction);
        }
    }
}
