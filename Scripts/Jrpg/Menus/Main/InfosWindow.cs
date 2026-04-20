using System;
using System.Collections;
using Game.Maps;
using Game.RpgSystem;
using Game.RpgSystem.Models;
using Game.Stats;
using Jrpg.Maps;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;

namespace Jrpg.Menus.Main
{
    public class InfosWindow : MonoBehaviour
    {
        #region Serialized Fields
        [SerializeField] private LocalizeStringEvent _locationText;
        [SerializeField] private TextMeshProUGUI _playtimeText;
        [SerializeField] private TextMeshProUGUI _moneyText;
        #endregion

        #region Public Methods
        public void SetInfos()
        {
            SetLocation();
            SetPlaytime();
            SetMoney();
            StartCoroutine(TimeWatchCoroutine());
        }
        #endregion

        #region Private Methods
        private void SetLocation()
        {
            MapRoot currentMap = MapStateManager.Instance.CurrentMap;
            _locationText.StringReference = currentMap.MapName;
        }

        private void SetPlaytime()
        {
            TimeSpan playtime = GameStatsManager.Instance.TotalGameTime;
            _playtimeText.text = $"{(int)playtime.TotalHours}:{playtime.Minutes:D2}";
        }

        private void SetMoney()
        {
            RpgParty party = PartyManager.Instance.CurrentParty;
            _moneyText.text = party.Money.ToString();
        }

        private IEnumerator TimeWatchCoroutine()
        {
            TimeSpan playtime = GameStatsManager.Instance.TotalGameTime;
            WaitForSecondsRealtime waitForSeconds = new(60 - playtime.Seconds);
            yield return waitForSeconds;
            waitForSeconds.Reset();
            waitForSeconds.waitTime = 60;
            while(true)
            {
                SetPlaytime();
                yield return waitForSeconds;
            }
        }
        #endregion
    }
}
