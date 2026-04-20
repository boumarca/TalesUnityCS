using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using Framework.Singleton;
using Framework.Utils;
using Newtonsoft.Json;
using UnityEngine;

namespace Game.SaveSystem
{
    public class SaveDataManager : GlobalSingleton<SaveDataManager>
    {
        #region Constants
        private const string FileName = "SaveFile_{0}.txt";
        private const string MetaDataFileName = "{0}.meta";
        private const string IndexCaptureGroup = "index";
        #endregion

        #region Serialized Fields
        [SerializeField] private SaveMetadataProvider _metadataProvider;
        [SerializeField] private MonoBehaviour[] _saveableObjects;
        #endregion

        #region Private Fields
        private readonly List<ISaveable> _saveables = new();
        private readonly List<SaveDataBase> _saveBuffer = new();
        private readonly Dictionary<int, string> _allMetaFileNames = new();
        private readonly Dictionary<int, SaveMetadataBase> _allMetadata = new();
        private readonly Regex _saveNamesRegex = new (@"SaveFile_(?<index>-?\d+)\.txt\.meta");

        private JsonSerializerSettings _jsonSerializerSettings;
        private IFileSystemStrategy _fileSystem;
        private int _lastSaveIndex;
        #endregion

        #region Public Methods
        public void Initialize()
        {
            _fileSystem = new DesktopFileSystemStrategy();
            foreach(MonoBehaviour monoBehaviour in _saveableObjects)
            {
                if (monoBehaviour is ISaveable saveable)
                    _saveables.Add(saveable);
                else
                    Debug.LogError($"{monoBehaviour} is not a {nameof(ISaveable)}");
            }

            CreateJsonSerializerSettings();
            RetrieveSaveFiles();
        }

        public bool HasSaveFile()
        {
            return _allMetaFileNames.Count > 0;
        }

        public int CreateNewSaveGame()
        {
            int newIndex = _lastSaveIndex;
            SaveGameAtIndex(newIndex);
            _lastSaveIndex++;
            return newIndex;
        }

        public void LoadGameAtIndex(int saveIndex)
        {
            string fileName = GetFileName(saveIndex);
            LoadGame(fileName);
        }

        public void SaveGameAtIndex(int saveIndex)
        {
            string fileName = GetFileName(saveIndex);
            SaveMetadata(fileName, saveIndex);
            SaveGame(fileName);
        }

        public bool DeleteGameAtIndex(int saveIndex)
        {
            if (!_allMetaFileNames.TryGetValue(saveIndex, out string metaFileName))
                return false;

            bool success =_fileSystem.DeleteFile(metaFileName);
            if (!success)
                return false;

            success = _fileSystem.DeleteFile(GetFileName(saveIndex));
            if (!success)
                return false;

            _allMetaFileNames.Remove(saveIndex);
            _allMetadata.Remove(saveIndex);
            return true;
        }

        /// <summary>
        /// Get all metadata saves, sorted by ascending slot number.
        /// Allocates a new list.
        /// </summary>
        /// <returns>All metadata saves, sorted by ascending slot number</returns>
        public IReadOnlyList<SaveMetadataBase> GetAllMetadataSaveFiles()
        {
            return _allMetadata.Values.OrderBy(metadata => metadata.SlotId).ToList();
        }

        public SaveMetadataBase GetMetadataForIndex(int slotIndex)
        {
            return _allMetadata[slotIndex];
        }
        #endregion

        #region Private Methods
        private void LoadAllMetadataSaveFiles()
        {
            _allMetadata.Clear();
            foreach (string fileName in _allMetaFileNames.Values)
            {
                string json = _fileSystem.LoadFile(fileName);
                if (string.IsNullOrEmpty(json))
                    continue;

                SaveMetadataBase metadata = JsonConvert.DeserializeObject<SaveMetadataBase>(json, _jsonSerializerSettings);
                if (metadata == null)
                    continue;

                _allMetadata[metadata.SlotId] = metadata;
            }
        }

        private void SaveMetadata(string fileName, int slotId)
        {
            Debug.Log("Save metadata");
            SaveMetadataBase metadata = _metadataProvider.CreateMetadata(slotId);
            string json = JsonConvert.SerializeObject(metadata, _jsonSerializerSettings);
            string metaFileName = GetMetaFileName(fileName);
            _fileSystem.SaveToFile(metaFileName, json);
            _allMetaFileNames.TryAdd(slotId, metaFileName);
            _allMetadata[slotId] = metadata;
        }

        private void SaveGame(string fileName)
        {
            Debug.Log($"Save game at {fileName}");
            _saveBuffer.Clear();
            foreach (ISaveable saveable in _saveables)
                _saveBuffer.Add(saveable.SaveData());

            string json = JsonConvert.SerializeObject(_saveBuffer, _jsonSerializerSettings);
            _fileSystem.SaveToFile(fileName, json);
        }

        private void LoadGame(string fileName)
        {
            Debug.Log($"Load game at {fileName}");
            string json = _fileSystem.LoadFile(fileName);
            Debug.Log(json);
            List<SaveDataBase> saveDatas = JsonConvert.DeserializeObject<List<SaveDataBase>>(json, _jsonSerializerSettings);
            foreach(ISaveable saveable in _saveables)
            {
                foreach(SaveDataBase saveData in saveDatas)
                    saveable.TryLoadData(saveData);
            }
        }

        private void CreateJsonSerializerSettings()
        {
            List<Type> types = new(_metadataProvider.MetadataTypes);
            foreach (ISaveable saveable in _saveables)
                types.AddRange(saveable.SaveDataTypes);

            KnownTypesBinder knownTypesBinder = new(types);

            _jsonSerializerSettings = new()
            {
                TypeNameHandling = TypeNameHandling.Objects,
                SerializationBinder = knownTypesBinder,
#if UNITY_EDITOR
                Formatting = Formatting.Indented,
#endif
            };
        }

        private void RetrieveSaveFiles()
        {
            IReadOnlyCollection<string> allFiles = _fileSystem.RetrieveAllSaveFiles();
            foreach (string saveFile in allFiles)
            {
                Match match = _saveNamesRegex.Match(saveFile);
                if (!match.Success)
                    continue;

                if (!int.TryParse(match.Groups[IndexCaptureGroup].Value, out int fileIndex))
                    continue;

                _allMetaFileNames.Add(fileIndex, saveFile);
                if(_lastSaveIndex < fileIndex)
                    _lastSaveIndex = fileIndex;
            }
            _lastSaveIndex++;
            LoadAllMetadataSaveFiles();
        }

        private static string GetFileName(int saveIndex)
        {
            return string.Format(CultureInfo.InvariantCulture, FileName, saveIndex);
        }

        private static string GetMetaFileName(string fileName)
        {
            return string.Format(CultureInfo.InvariantCulture, MetaDataFileName, fileName);
        }
        #endregion
    }
}
