using System;
using System.Collections.Generic;

namespace Game.SaveSystem
{
    public interface ISaveable
    {
        #region Public Properties
        public IEnumerable<Type> SaveDataTypes { get; }
        #endregion

        #region Public Methods
        public bool TryLoadData(SaveDataBase saveData);
        public SaveDataBase SaveData();
        #endregion
    }
}
