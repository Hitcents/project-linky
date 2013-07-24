using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ProjectLinky.Tests.Mocking
{
    public class MockProjectFile : ProjectFile
    {
        private string _basePath;
        private string _path;
        public Dictionary<string, List<ItemGroup>> ItemGroups = new Dictionary<string, List<ItemGroup>>();

        public MockProjectFile()
        {
            _basePath = Path.GetDirectoryName(GetType().Assembly.Location);
        }

        private List<ItemGroup> GetGroups()
        {
            return ItemGroups[_path.Replace(_basePath, string.Empty).TrimStart('\\')];
        }

        public override void Load(string path)
        {
            _path = path;
        }

        public override IEnumerator<ItemGroup> GetEnumerator()
        {
            var list = GetGroups();
            foreach (var item in list)
            {
                yield return item;
            }
        }

        public override void Append(ItemGroup itemGroup)
        {
            var list = GetGroups();
            list.Add(itemGroup);
        }

        public override void Remove(ItemGroup itemGroup)
        {
            var list = GetGroups();
            list.Remove(itemGroup);
        }

        public override void Save(string projectPath)
        {
            
        }
    }
}
