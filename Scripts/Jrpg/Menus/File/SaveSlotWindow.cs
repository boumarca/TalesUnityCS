using System;
using System.Collections.Generic;
using System.Globalization;
using Game.RpgSystem;
using Game.RpgSystem.Data;
using Game.RpgSystem.Models;
using Game.UI;
using Jrpg.Save;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

namespace Jrpg.Menus.File
{
    public class SaveSlotWindow : ListEntry
    {
        #region Serialized Fields
        [Header("Component References")]
        [SerializeField] private LocalizeStringEvent _slotText;
        [SerializeField] private TextMeshProUGUI _lastSaveDateText;
        [SerializeField] private TextMeshProUGUI _playTimeText;
        [SerializeField] private LocalizeStringEvent _locationText;
        [SerializeField] private LocalizeStringEvent _chapterText;
        [SerializeField] private GameObject _clearDataIcon;
        [SerializeField] private PartyMemberSmallView _actorPrefab;
        [SerializeField] private Transform _actorContainer;
        [SerializeField] private RawImage _screenshotImage;

        [Header("Localization strings")]
        [SerializeField] private LocalizedString _suspendSaveKey;
        [SerializeField] private LocalizedString _autoSaveKey;
        #endregion

        #region Private Fields
        private RpgSaveMetadata _metadata;
        #endregion

        #region Public Properties
        public int SlotIndex => _metadata.SlotId;
        public bool IsSuspendSave => SlotIndex == 0;
        public bool IsAutoSave => SlotIndex == -1;
        #endregion

        #region Events
        public event Action<SaveSlotWindow> OnSlotClickedEvent = delegate { };
        #endregion

        #region ListEntry Implementation
        protected override void OnInitialize()
        {
            base.OnInitialize();
            PopulateWindow(Data as RpgSaveMetadata);
        }
        #endregion

        #region Public Methods
        public void PopulateWindow(RpgSaveMetadata metadata)
        {
            _metadata = metadata;
            SetSlotNumber();
            SetLastSaveDate();
            SetPlaytime();
            SetChapterInfos();
            SetLocation();
            SetPartyInfos();
            SetScreenshot();
            RepositionHack();
        }
        #endregion

        #region Unity Event UI Methods
        public void UIOnSlotClicked()
        {
            OnSlotClickedEvent(this);
        }
        #endregion

        #region Private Methods
        private void SetSlotNumber()
        {
            if (IsSuspendSave)
                _slotText.StringReference = _suspendSaveKey;
            else if(IsAutoSave)
                _slotText.StringReference = _autoSaveKey;
            else
                _slotText.StringReference.Arguments = new List<object> { _metadata.SlotId.ToString(CultureInfo.InvariantCulture) }; //TODO: Avoid new
            _slotText.RefreshString();
        }

        private void SetLastSaveDate()
        {
            IFormatProvider formatter = LocalizationSettings.SelectedLocale.Formatter;
            DateTime localDate = _metadata.LastSaveDate.ToLocalTime();
            _lastSaveDateText.text = $"{localDate.ToString("d", formatter)} {localDate:HH:mm}"; //TODO: Separate date and hour for nicer display.
        }

        private void SetPlaytime()
        {
            TimeSpan playtime = new (_metadata.TotalPlayTimeTicks);
            _playTimeText.text = $"{(int)playtime.TotalHours}:{playtime.Minutes:D2}"; //TODO: Extract to static fuction
        }

        private void SetChapterInfos()
        {
            _chapterText.StringReference.Arguments = new List<object> { _metadata.CurrentChapter.ToString(CultureInfo.InvariantCulture) }; //TODO: Avoid new
            _chapterText.RefreshString();
            _clearDataIcon.SetActive(_metadata.IsClearData);
        }

        private void SetLocation()
        {
            _locationText.StringReference = new LocalizedString(_metadata.CurrentLocationTable, _metadata.CurrentLocationKey);
        }

        private void SetPartyInfos()
        {
            //TODO: Pool party members?
            for (int i = _actorContainer.childCount - 1; i >= 0; i--)
                Destroy(_actorContainer.GetChild(i).gameObject);

            foreach (PartyMemberInfo info in _metadata.ActivePartyInfos)
                CreatePartyMemberInfo(info);
        }

        private void SetScreenshot()
        {
            if (_screenshotImage.texture != null)
                Destroy(_screenshotImage.texture);

            byte[] bytes = Convert.FromBase64String(_metadata.Screenshot);
            Texture2D tex = new(82, 46, TextureFormat.ARGB32, false); //TODO: Move to constants
            tex.LoadImage(bytes);
            _screenshotImage.texture = tex;
        }

        private void CreatePartyMemberInfo(PartyMemberInfo info)
        {
            PartyMemberSmallView actorView = Instantiate(_actorPrefab, _actorContainer);
            RpgActor actor = ActorManager.Instance.GetActor(new ActorId(info.ActorId));
            actorView.SetActorInfo(actor.Headshot, info.Level);
        }

        /// <summary>
        /// For some reason, the instantiated async transform is at a weird z value.
        /// </summary>
        private void RepositionHack()
        {
            Vector3 position = transform.localPosition;
            position.z = 0;
            transform.localPosition = position;
        }
        #endregion
    }
}
