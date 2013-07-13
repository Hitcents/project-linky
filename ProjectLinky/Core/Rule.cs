using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;

namespace ProjectLinky
{
    [XmlRoot(ElementName = "rule")]
    public class Rule
    {
        [XmlAttribute(AttributeName = "inputPattern")]
        public string InputPattern { get; set; }

        [XmlAttribute(AttributeName = "outputPattern")]
        public string OutputPattern { get; set; }

        [XmlAttribute(AttributeName = "buildAction")]
        public string BuildAction { get; set; }

        [XmlAttribute(AttributeName = "excludePattern")]
        public string ExcludePattern { get; set; }
    }
}
