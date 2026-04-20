using System.Collections.Generic;
using Game.QuestSystem.Data;
using Game.QuestSystem.Models;
using UnityEditor;
using UnityEngine;

namespace Jrpg.Debug.Editor
{
    [CustomEditor(typeof(QuestDebugMenu))]
    public class QuestDebugInspector : AbstractDebugInspector
    {
        #region Private fields
        private QuestDebugMenu _questDebugMenu;
        #endregion

        #region AbstractDebugInspector Implementation
        public override void SetDebugMenu()
        {
            _questDebugMenu = (QuestDebugMenu)target;
        }

        public override void DisplayCheats()
        {
            GUI.enabled = _questDebugMenu.IsSetQuestCheatValid();
            if (GUILayout.Button("Set Quest to State"))
                _questDebugMenu.CheatSetQuestToState();
            GUI.enabled = true;
        }

        public override void DisplayDebugFoldout()
        {
            EditorGUI.indentLevel++;
            GUI.enabled = false;
            DisplayQuestList("Active Quests", _questDebugMenu.ActiveQuests);
            DisplayQuestList("Inactive Quests", _questDebugMenu.InactiveQuests);
            DisplayQuestList("Completed Quests", _questDebugMenu.CompletedQuests);
            GUI.enabled = true;
            EditorGUI.indentLevel--;
        }
        #endregion

        #region Private Methods
        private void DisplayQuestList(string header, IReadOnlyCollection<Quest> questList)
        {
            EditorGUILayout.LabelField(header);
            EditorGUI.indentLevel++;
            foreach (Quest quest in questList)
                DisplayQuest(quest);
            EditorGUI.indentLevel--;
        }

        private void DisplayQuest(Quest quest)
        {
            EditorGUILayout.LabelField(quest.Id.Name);
            EditorGUI.indentLevel++;
            EditorGUILayout.EnumPopup("Status", quest.State);
            QuestStep currentStep = quest.CurrentStep;
            if (currentStep != null)
                EditorGUILayout.ObjectField("Current Step: ", quest.CurrentStep.Data, typeof(QuestStepData), false);

            EditorGUI.indentLevel--;
        }
        #endregion
    }
}
