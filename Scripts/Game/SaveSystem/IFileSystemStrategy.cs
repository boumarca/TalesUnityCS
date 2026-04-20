using System.Collections.Generic;

namespace Game.SaveSystem
{
    public interface IFileSystemStrategy
    {
        public string LoadFile(string fileName);
        public void SaveToFile(string fileName, string json);
        public bool DeleteFile(string fileName);
        public IReadOnlyCollection<string> RetrieveAllSaveFiles();
    }
}
