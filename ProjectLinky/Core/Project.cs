using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;

namespace ProjectLinky
{
    [XmlRoot(ElementName = "project")]
    public class Project
    {
        [XmlAttribute(AttributeName = "path")]
        public string Path { get; set; }

        [XmlElement(ElementName = "rules")]
        public Rule[] Rules { get; set; }
    }
}
