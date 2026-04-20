using System.IO;
using UnityEditor;

namespace Game.SaveSystem.Editor
{
    public static class SaveSystemTools
    {
        #region Menu Items
        [MenuItem("Tools/Save System/Delete Save File")]
        public static void DeleteSaveFile()
        {
            File.Delete(DesktopFileSystemStrategy.FilePath);
        }
        #endregion
    }
}
