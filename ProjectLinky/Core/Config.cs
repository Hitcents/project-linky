using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;

namespace ProjectLinky
{
    [XmlRoot(ElementName = "linky")]
    public class Config
    {
        [XmlElement(ElementName = "projects")]
        public Project[] Projects { get; set; }
    }
}
