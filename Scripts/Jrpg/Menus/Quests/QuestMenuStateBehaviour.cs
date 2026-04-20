using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Framework.Inputs;
using Game.QuestSystem;
using Game.QuestSystem.Data;
using Game.QuestSystem.Models;
using Game.UI;
using Jrpg.Menus.Views;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Pool;

namespace Jrpg.Menus
{
    public class QuestMenuStateBehaviour : MenuStateBehaviour
    {
        #region Serialized Fields 
        [SerializeField] private InputtableWindow _questListWindow;
        [SerializeField] private ListView _questListView;
        [SerializeField] private QuestInfoView _questInfoView;
        [SerializeField] private TabList _questTabList;
        #endregion

        #region Private Fields
        private Quest _selectedQuest;
        private QuestListEntry _currentlyFollowedQuest;
        #endregion

        #region MonoBehaviour Methods
        private void OnEnable()
        {
            _questListView.OnSelectedEntryChangedEvent += HandleSelectedQuestChanged;
        }

        private void OnDisable()
        {
            _questListView.OnSelectedEntryChangedEvent -= HandleSelectedQuestChanged;
        }
        #endregion

        #region GameStateBehaviour Implementation
        public override Task OnEnterState(object payload)
        {
            base.OnEnterState(payload);
            DisableConfirmInput();
            _questTabList.SelectFirstTab();
            return Task.CompletedTask;
        }

        public override void OnExitState()
        {
            EnableConfirmInput();
            base.OnExitState();
        }
        #endregion

        #region Unity Event UI Methods
        public void UIOnShowObjectivesPerformed()
        {
            ShowObjectives();
        }

        public void UIOnShowSummaryPerformed()
        {
            ShowSummary();
        }

        public void UIOnScrollSummaryPerformed(InputAction.CallbackContext context)
        {
            _questInfoView.StartScrolling(context.ReadValue<float>());
        }

        public void UIOnScrollSummaryCanceled()
        {
            _questInfoView.StopScrolling();
        }

        public void UIOnFollowQuestPerformed()
        {
            ToggleFollowQuest();
        }

        public void UIOnChangeTabPerformed(InputAction.CallbackContext context)
        {
            _questTabList.ChangeTab((int)context.ReadValue<float>());
        }

        public void UIOnStoryTabSelected()
        {
            SelectTab(QuestType.Main);
        }

        public void UIOnQuestTabSelected()
        {
            SelectTab(QuestType.Side);
        }
        #endregion

        #region Private Methods
        private void HandleSelectedQuestChanged(ListEntry listEntry)
        {
            QuestListEntry questEntry = listEntry as QuestListEntry;
            _selectedQuest = listEntry != null ? questEntry.Quest : null;
            _questInfoView.Refresh(_selectedQuest);
            RefreshInputs();
        }

        private void SelectTab(QuestType tabType)
        {
            CreateQuestList(tabType);
            ShowObjectives();
        }

        private void CreateQuestList(QuestType questType)
        {
            List<Quest> questList = ListPool<Quest>.Get();
            IReadOnlyList<Quest> activeQuests = QuestManager.Instance.ActiveQuests;
            IReadOnlyList<Quest> completedQuests = QuestManager.Instance.CompletedQuests;

            IOrderedEnumerable<Quest> sortedActiveQuests = activeQuests.Where(quest => quest.Type == questType).OrderBy(quest => quest.Id);
            IOrderedEnumerable<Quest> sortedCompletedQuest = completedQuests.Where(quest => quest.Type == questType).OrderByDescending(quest => quest.Id);

            questList.AddRange(sortedActiveQuests);
            questList.AddRange(sortedCompletedQuest);

            _questListView.PopulateList(questList);
            SetFollowedQuest();
            ListPool<Quest>.Release(questList);
        }

        private void SetFollowedQuest()
        {
            foreach(ListEntry entry in _questListView.Entries)
            {
                QuestListEntry questListEntry = entry as QuestListEntry;
                if (!QuestManager.Instance.IsFollowedQuest(questListEntry.Quest))
                    continue;

                _currentlyFollowedQuest = questListEntry;
                _currentlyFollowedQuest.MarkAsFollowed();
                break;
            }
        }

        private void ShowObjectives()
        {
            _questInfoView.DisplayObjectives();
            RefreshInfoToggleInput();
            RefreshScrollSummaryInput();
        }

        private void ShowSummary()
        {
            _questInfoView.DisplaySummary();
            RefreshInfoToggleInput();
            RefreshScrollSummaryInput();
        }

        private void ToggleFollowQuest()
        {
            if(QuestManager.Instance.IsFollowedQuest(_selectedQuest))
                UnfollowSelectedQuest();
            else
                FollowSelectedQuest();
        }

        private void FollowSelectedQuest()
        {
            QuestManager.Instance.FollowQuest(_selectedQuest);
            if (_currentlyFollowedQuest != null)
                _currentlyFollowedQuest.UnmarkAsFollowed();

            _currentlyFollowedQuest = _questListView.SelectedListEntry as QuestListEntry;
            _currentlyFollowedQuest.MarkAsFollowed();
        }

        private void UnfollowSelectedQuest()
        {
            QuestManager.Instance.UnfollowQuest();
            _currentlyFollowedQuest.UnmarkAsFollowed();
            _currentlyFollowedQuest = null;
        }

        private void RefreshInputs()
        {
            RefreshFollowQuestInput();
            RefreshScrollSummaryInput();
            RefreshInfoToggleInput();
        }

        private void RefreshFollowQuestInput()
        {
            RefreshInput(InputManager.Instance.InputActions.QuestMenu.FollowQuest, _selectedQuest != null && !_selectedQuest.IsCompleted);
        }

        private void RefreshScrollSummaryInput()
        {
            RefreshInput(InputManager.Instance.InputActions.QuestMenu.ScrollSummary, _questInfoView.IsDisplayingSummary && _questInfoView.IsScrollable);
        }

        private void RefreshInfoToggleInput()
        {
            InputManager inputManager = InputManager.Instance;
            GameInputs.QuestMenuActions actions = inputManager.InputActions.QuestMenu;

            if (_selectedQuest == null || _selectedQuest.Type == QuestType.Side)
            {
                inputManager.DisableInput(actions.ShowObjectives);
                inputManager.DisableInput(actions.ShowSummary);
                return;
            }            

            if (_questInfoView.IsDisplayingObjectives)
            {
                inputManager.DisableInput(actions.ShowObjectives);
                inputManager.EnableInput(actions.ShowSummary);
            }
            else
            {
                inputManager.EnableInput(actions.ShowObjectives);
                inputManager.DisableInput(actions.ShowSummary);
            }            
        }
        #endregion
    }
}
