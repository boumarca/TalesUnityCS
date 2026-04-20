using System.Collections.Generic;
using Game.RpgSystem.Data;
using Game.RpgSystem.Models;
using Jrpg.Menus.Items;
using UnityEngine;

namespace Jrpg.Menus.Equip
{
    public class EquipStatInfoView : MonoBehaviour
    {
        #region Serialized Fields
        [SerializeField] private EquipStatEntry[] _statEntries;
        #endregion

        #region Private Fields
        private RpgActor _actor;
        #endregion

        #region Public Methods
        public void Show()
        {
            RefreshOldStats();
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void ChangeActor(RpgActor actor)
        {
            _actor = actor;
        }

        public void RefreshProjectedStats(IReadOnlyDictionary<RpgStats, int> projectedStats)
        {            
            foreach (EquipStatEntry entry in _statEntries)
                entry.SetProjectedStat(_actor, projectedStats[entry.Stat]);
        }
        #endregion

        #region Private Methods
        private void RefreshOldStats()
        {
            foreach (EquipStatEntry entry in _statEntries)
                entry.SetOldStat(_actor);
        }
        #endregion
    }
}
