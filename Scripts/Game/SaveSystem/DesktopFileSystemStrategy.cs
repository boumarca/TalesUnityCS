using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Game.SaveSystem
{
    public class DesktopFileSystemStrategy : IFileSystemStrategy
    {
        #region Constants
        private const string FileName = "SaveFile_{0}.txt";
        #endregion

        #region Private Properties
        public static string FilePath => Path.Combine(Application.persistentDataPath, FileName);
        #endregion

        #region IFileSystemStrategy Implementation
        public string LoadFile(string fileName)
        {
            string pathToLoad = GetPath(fileName);
            try
            {
                if (!File.Exists(pathToLoad))
                    return string.Empty;

                return File.ReadAllText(pathToLoad);
            }
            catch (IOException e)
            {
                Debug.LogError(e);
                return string.Empty;
            }
        }

        public void SaveToFile(string fileName, string json)
        {
            Debug.Log(json);
            try
            {
                File.WriteAllText(GetPath(fileName), json);
            }
            catch(IOException e)
            {
                Debug.LogError(e);
            }
        }

        public bool DeleteFile(string fileName)
        {
            string pathToDelete = GetPath(fileName);
            try
            {
                if(File.Exists(pathToDelete))
                    File.Delete(pathToDelete);
            }
            catch (IOException e)
            {
                Debug.LogError(e);
                return false;
            }
            return true;
        }

        public IReadOnlyCollection<string> RetrieveAllSaveFiles()
        {
            string[] files = Directory.GetFiles(Application.persistentDataPath);
            return files.Select(Path.GetFileName).ToList();
        }
        #endregion

        #region Private Methods
        private static string GetPath(string fileName) => Path.Combine(Application.persistentDataPath, fileName);
        #endregion
    }
}
