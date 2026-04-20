using System.Collections.Generic;
using System.Linq;
using Framework.Identifiers;
using Game.RpgSystem.Models;
using UnityEditor;
using UnityEngine;

namespace Jrpg.Debug.Editor
{
    [CustomEditor(typeof(PartyDebugMenu))]
    public class PartyDebugInspector : AbstractDebugInspector
    {
        #region Private Fields
        private PartyDebugMenu _partyDebugMenu;
        #endregion

        #region AbstractDebugInspector Implementation
        public override void SetDebugMenu()
        {
            _partyDebugMenu = (PartyDebugMenu)target;
        }

        public override void DisplayCheats()
        {
            GUI.enabled = Identifier.IsValid(_partyDebugMenu.SelectedActor);
            if (GUILayout.Button($"Add {_partyDebugMenu.SelectedActor} to party"))
                _partyDebugMenu.CheatAddPartyMember();
            if (GUILayout.Button($"Remove {_partyDebugMenu.SelectedActor} from party"))
                _partyDebugMenu.CheatRemovePartyMember();

            GUI.enabled = _partyDebugMenu.ValueToAdd > 0;
            if (GUILayout.Button($"Give {_partyDebugMenu.ValueToAdd} Experience to party"))
                _partyDebugMenu.CheatGainExperience();

            GUI.enabled = _partyDebugMenu.AreSwapIndexesValid();
            DisplaySwapPartyMembersCheat();

            GUI.enabled = true;
            if (GUILayout.Button($"Give {_partyDebugMenu.ValueToAdd} money to party"))
                _partyDebugMenu.CheatGainMoney();
        }

        public override void DisplayDebugFoldout()
        {
            EditorGUI.indentLevel++;
            GUI.enabled = false;
            EditorGUILayout.LabelField("Current Party");
            RpgParty party = _partyDebugMenu.CurrentParty;
            if (party != null)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.LabelField($"Money: {party.Money}");
                EditorGUILayout.LabelField("Active Members");
                EditorGUI.indentLevel++;
                foreach (RpgActor actor in party.ActiveMembers)
                    DisplayActor(actor);
                EditorGUI.indentLevel--;

                EditorGUILayout.LabelField("Reserve Members");
                EditorGUI.indentLevel++;
                foreach (RpgActor actor in party.ReserveMembers)
                    DisplayActor(actor);
                EditorGUI.indentLevel--;
                EditorGUI.indentLevel--;
            }
            GUI.enabled = true;
            EditorGUI.indentLevel--;
        }
        #endregion

        #region Private Methods
        private void DisplaySwapPartyMembersCheat()
        {
            int firstIndex = _partyDebugMenu.SwapIndexes.x;
            int secondIndex = _partyDebugMenu.SwapIndexes.y;
            IReadOnlyList<RpgActor> members = _partyDebugMenu.CurrentParty.Members;
            string firstMember = members.ElementAtOrDefault(firstIndex)?.Name.GetLocalizedString() ?? "None";
            string secondMember = members.ElementAtOrDefault(secondIndex)?.Name.GetLocalizedString() ?? "None";
            if (GUILayout.Button($"Swap {firstMember} with {secondMember}"))
                _partyDebugMenu.CheatSwapPartyMembers();
        }

        private void DisplayActor(RpgActor actor)
        {
            EditorGUILayout.LabelField($"{actor.Name.GetLocalizedString()}  Level: {actor.Level}");
        }
        #endregion
    }
}
