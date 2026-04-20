using System.Collections.Generic;
using System.Text;
using Game.QuestSystem.Data;
using Game.QuestSystem.Models;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

namespace Jrpg.Menus.Views
{
    public class QuestInfoView : MonoBehaviour
    {
        #region Statics
        private static readonly StringBuilder s_summaryBuilder = new();
        #endregion

        #region Enums
        private enum DisplayState { Objectives, Summary }
        #endregion

        #region Serialized Fields
        [SerializeField] private LocalizeStringEvent _questNameText;
        [SerializeField] private LocalizeStringEvent _questDescriptionText;
        [SerializeField] private GameObject _questGiverLabel;
        [SerializeField] private LocalizeStringEvent _questGiverText;
        [SerializeField] private GameObject _locationLabel;
        [SerializeField] private LocalizeStringEvent _locationText;
        [SerializeField] private QuestObjectiveEntry _questObjectiveEntryPrefab;
        [SerializeField] private GameObject _objectiveLabel;
        [SerializeField] private RectTransform _objectiveContent;
        [SerializeField] private RectTransform _summaryContent;
        [SerializeField] private TextMeshProUGUI _summaryText;
        [SerializeField] private ScrollRect _scrollRect;
        [SerializeField] private float _summaryScrollSpeed;
        #endregion

        #region Private Fields
        private readonly List<QuestObjectiveEntry> _questObjectiveEntries = new();
        private Quest _selectedQuest;
        private DisplayState _displayState;
        private float _currentScrollDelta;
        #endregion

        #region Public Properties
        public bool IsDisplayingObjectives => _displayState == DisplayState.Objectives;
        public bool IsDisplayingSummary => _displayState == DisplayState.Summary;
        public bool IsScrollable => _summaryText.preferredHeight > _scrollRect.viewport.rect.height;
        #endregion

        #region MonoBehaviour Methods
        private void Update()
        {
            if (_displayState != DisplayState.Summary || _currentScrollDelta == 0)
                return;

            ScrollSummary();
        }
        #endregion

        #region Public Methods
        public void Refresh(Quest selectedQuest)
        {
            _selectedQuest = selectedQuest;
            DisplayQuestName();
            DisplaySideQuestInformation();
            DisplayQuestDescription();
            RefreshObjectives();
            BuildStorySummary();
            _scrollRect.verticalNormalizedPosition = 1;
        }

        public void ToggleDisplayState()
        {
            if (_displayState == DisplayState.Objectives)
                DisplaySummary();
            else
                DisplayObjectives();
        }

        public void DisplayObjectives()
        {
            _displayState = DisplayState.Objectives;
            _objectiveContent.gameObject.SetActive(true);
            _summaryContent.gameObject.SetActive(false);
            _scrollRect.content = _objectiveContent;
        }

        public void DisplaySummary()
        {
            _displayState = DisplayState.Summary;
            _objectiveContent.gameObject.SetActive(false);
            _summaryContent.gameObject.SetActive(true);
            _scrollRect.content = _summaryContent;
            _scrollRect.verticalNormalizedPosition = 1;
        }

        public void StartScrolling(float scrollSpeed)
        {
            float scrollableHeight = _scrollRect.content.rect.height - _scrollRect.viewport.rect.height;
            if (scrollableHeight <= 0f)
                return;

            _currentScrollDelta = scrollSpeed * _summaryScrollSpeed / scrollableHeight;
        }

        public void StopScrolling()
        {
            _currentScrollDelta = 0;
        }
        #endregion

        #region Private Methods
        private void ClearObjectives()
        {
            //TODO: Pool elements
            foreach (QuestObjectiveEntry item in _questObjectiveEntries)
                Destroy(item.gameObject);

            _questObjectiveEntries.Clear();
        }

        private void DisplayQuestName()
        {
            if(_selectedQuest == null)
            {
                _questNameText.gameObject.SetActive(false);
                return;
            }
            _questNameText.gameObject.SetActive(true);
            _questNameText.StringReference = _selectedQuest.Name;
        }

        private void DisplayQuestDescription()
        {
            if (_selectedQuest == null)
            {
                _questDescriptionText.gameObject.SetActive(false);
                return;
            }
            _questDescriptionText.gameObject.SetActive(true);
            _questDescriptionText.StringReference = _selectedQuest.Description;
        }

        private void DisplaySideQuestInformation()
        {
            if(_selectedQuest == null || _selectedQuest.Type == QuestType.Main)
            {
                _questGiverLabel.gameObject.SetActive(false);
                _locationLabel.gameObject.SetActive(false);
                return;
            }
            _questGiverLabel.gameObject.SetActive(true);
            _locationLabel.gameObject.SetActive(true);
            _questGiverText.StringReference = _selectedQuest.QuestGiver;
            _locationText.StringReference = _selectedQuest.Location;
        }

        private void RefreshObjectives()
        {
            ClearObjectives();
            if(_selectedQuest == null)
            {
                _objectiveLabel.SetActive(false);
                return;
            }

            _objectiveLabel.SetActive(true);

            if (_selectedQuest.IsInProgress)
                CreateCurrentObjective();

            for (int i = _selectedQuest.CurrentStepIndex - 1; i >= 0; i--)
                CreateCompletedObjective(i);
        }

        private void CreateCurrentObjective()
        {
            QuestStepData questStep = _selectedQuest.GetNextObjective();
            if (questStep == null)
                return;
            
            CreateObjective(questStep, false);
        }

        private void CreateCompletedObjective(int stepIndex)
        {
            QuestStepData questStep = _selectedQuest.GetStepData(stepIndex);
            if (!questStep.IsObjective)
                return;

            CreateObjective(questStep, true);
        }

        private void CreateObjective(QuestStepData questStep, bool isCompleted)
        {
            QuestObjectiveEntry objective = Instantiate(_questObjectiveEntryPrefab, _objectiveContent);
            objective.Initialize(questStep, isCompleted);
            _questObjectiveEntries.Add(objective);
        }

        private void BuildStorySummary()
        {
            s_summaryBuilder.Clear();
            if(_selectedQuest == null)
            {
                _summaryText.text = string.Empty;
                return;
            }

            for (int i = 0; i < _selectedQuest.CurrentStepIndex; i++)
            {
                QuestStepData step = _selectedQuest.GetStepData(i);
                if (step.StorySummary.IsEmpty)
                    continue;

                s_summaryBuilder.AppendLine(step.StorySummary.GetLocalizedString());
                s_summaryBuilder.AppendLine();
            }

            _summaryText.text = s_summaryBuilder.ToString();
        }

        private void ScrollSummary()
        {            
            _scrollRect.verticalNormalizedPosition += _currentScrollDelta * Time.deltaTime;
        }
        #endregion
    }
}
