using System;
using System.Collections.Generic;
using Framework.Singleton;
using Game.SaveSystem;

namespace Game.Stats
{
    public class GameStatsManager : GlobalSingleton<GameStatsManager>, ISaveable
    {
        #region Private Fields
        private GameTime _gameTime;
        #endregion

        #region Public Properties
        public TimeSpan TotalGameTime => _gameTime.TotalGameTime;
        #endregion

        #region MonoBehaviour Methods
        private void OnApplicationPause(bool pause)
        {
            if (pause)
                _gameTime.PauseTime();
            else
                _gameTime.ResumeTime();
        }
        #endregion

        #region ISaveable Implementation
        public IEnumerable<Type> SaveDataTypes => new[] { typeof(StatsSaveData) };

        public SaveDataBase SaveData()
        {
            StatsSaveData saveData = new()
            {
                TotalPlayTimeTicks = _gameTime.TotalGameTime.Ticks
            };
            return saveData;
        }

        public bool TryLoadData(SaveDataBase saveData)
        {
            if (saveData is not StatsSaveData statsSaveData)
                return false;

            InitializeFromSaveData(statsSaveData);
            return true;
        }
        #endregion

        #region Public Methods
        public void InitializeAsNew()
        {
            _gameTime = new GameTime();
        }
        #endregion

        #region Private Methods
        private void InitializeFromSaveData(StatsSaveData statsSaveData)
        {
            _gameTime = new GameTime(statsSaveData.TotalPlayTimeTicks);
        }
        #endregion
    }
}
