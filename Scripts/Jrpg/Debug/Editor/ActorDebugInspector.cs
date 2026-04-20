using Framework.Identifiers;
using Game.RpgSystem.Data;
using Game.RpgSystem.Models;
using UnityEditor;
using UnityEngine;

namespace Jrpg.Debug.Editor
{
    [CustomEditor(typeof(ActorDebugMenu))]
    public class ActorDebugInspector : AbstractDebugInspector
    {
        #region Private fields
        private ActorDebugMenu _actorDebugMenu;
        #endregion

        #region AbstractDebugInspector Implementation
        public override void SetDebugMenu()
        {
            _actorDebugMenu = (ActorDebugMenu)target;
        }

        public override void DisplayDebugFoldout()
        {
            EditorGUI.indentLevel++;
            GUI.enabled = false;

            foreach (RpgActor actor in _actorDebugMenu.ActorEnumerator())
                DisplayActor(actor);

            GUI.enabled = true;
            EditorGUI.indentLevel--;
        }

        public override void DisplayCheats()
        {
            GUI.enabled = _actorDebugMenu.CheatValue > 0 && Identifier.IsValid(_actorDebugMenu.SelectedActor);
            if (GUILayout.Button($"Give {_actorDebugMenu.CheatValue} Experience to {_actorDebugMenu.SelectedActor}"))
                _actorDebugMenu.CheatGiveExperienceToActor();
            GUI.enabled = Identifier.IsValid(_actorDebugMenu.SelectedActor);
            if (GUILayout.Button($"Give {_actorDebugMenu.CheatValue} HP to {_actorDebugMenu.SelectedActor}"))
                _actorDebugMenu.CheatChangeHp();
            GUI.enabled = true;
        }
        #endregion

        #region Private Methods
        private void DisplayActor(RpgActor actor)
        {
            EditorGUILayout.LabelField(actor.Name.GetLocalizedString());
            EditorGUI.indentLevel++;
            EditorGUILayout.IntField("Level", actor.Level);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField($"Current XP: {actor.LevelInfo.TotalExperience}");
            EditorGUILayout.LabelField($"Next: {actor.LevelInfo.RemainingExperienceToNextLevel()}");
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.LabelField($"Next (Total): {actor.LevelInfo.TotalExperienceToNextLevel()}");

            EditorGUILayout.IntField("HP", actor.GetStatValue(RpgStats.MaxHp));
            EditorGUILayout.IntField("MP", actor.GetStatValue(RpgStats.MaxMp));
            EditorGUILayout.IntField("P.ATK", actor.GetStatValue(RpgStats.PhysicalAttack));
            EditorGUILayout.IntField("P.DEF", actor.GetStatValue(RpgStats.PhysicalDefense));
            EditorGUILayout.IntField("M.ATK", actor.GetStatValue(RpgStats.MagicAttack));
            EditorGUILayout.IntField("M.DEF", actor.GetStatValue(RpgStats.MagicDefense));
            EditorGUILayout.IntField("AGI", actor.GetStatValue(RpgStats.Agility));
            EditorGUI.indentLevel--;
        }
        #endregion
    }
}
