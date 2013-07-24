using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace ProjectLinky
{
    public class ProjectFile : IEnumerable<ItemGroup>
    {
        private readonly XmlDocument _document = new XmlDocument();

        public virtual void Load(string path)
        {
            _document.Load(path);
        }

        public virtual IEnumerator<ItemGroup> GetEnumerator()
        {
            foreach (XmlNode itemGroup in _document.DocumentElement.GetElementsByTagName("ItemGroup"))
            {
                foreach (XmlNode node in itemGroup)
                {
                    var linkNode = node.FirstChild;

                    if (linkNode == null || linkNode.Name != "Link")
                        continue;

                    var includeNode = node.Attributes["Include"];
                    if (includeNode == null)
                        continue;

                    yield return new ItemGroup
                    {
                        Link = linkNode.InnerText,
                        Include = includeNode.Value,
                        BuildAction = node.Name,
                        Node = node,
                    };
                }
            }
        }

        public virtual void Remove(ItemGroup itemGroup)
        {
            if (itemGroup.Node != null)
            {
                var parent = itemGroup.Node.ParentNode;
                parent.RemoveChild(itemGroup.Node);
                if (parent.ChildNodes.Count == 0)
                    parent.ParentNode.RemoveChild(parent);
            }
        }

        public virtual void Append(ItemGroup itemGroup)
        {
            var groupNode = _document.CreateElement("ItemGroup", _document.DocumentElement.NamespaceURI);

            var content = _document.CreateElement(itemGroup.BuildAction, _document.DocumentElement.NamespaceURI);
            content.SetAttribute("Include", itemGroup.Include);
            groupNode.AppendChild(content);

            var link = _document.CreateElement("Link", _document.DocumentElement.NamespaceURI);
            link.InnerText = itemGroup.Link;
            content.AppendChild(link);

            _document.DocumentElement.AppendChild(groupNode);
        }

        public virtual void Save(string projectPath)
        {
            _document.Save(projectPath);
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
