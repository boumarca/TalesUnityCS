using UnityEditor;
using UnityEngine;

namespace Jrpg.Debug.Editor
{
    public abstract class AbstractDebugInspector : UnityEditor.Editor
    {
        #region Private Fields
        private bool _showCheats = true;
        private bool _showDebug = true;
        #endregion

        #region Override Methods
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            if (!Application.isPlaying)
                return;

            SetDebugMenu();
            _showCheats = EditorGUILayout.BeginFoldoutHeaderGroup(_showCheats, "Cheats");
            if (_showCheats)
                DisplayCheats();
            EditorGUILayout.EndFoldoutHeaderGroup();

            _showDebug = EditorGUILayout.BeginFoldoutHeaderGroup(_showDebug, "Debug");
            if (_showDebug)
                DisplayDebugFoldout();

            EditorGUILayout.EndFoldoutHeaderGroup();
        }
        #endregion

        #region Abstract Methods
        public abstract void SetDebugMenu();
        public abstract void DisplayCheats();
        public abstract void DisplayDebugFoldout();
        #endregion
    }
}
