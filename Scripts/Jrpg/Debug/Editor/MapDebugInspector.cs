using UnityEditor;
using UnityEngine;

namespace Jrpg.Debug.Editor
{
    [CustomEditor(typeof(MapDebugMenu))]
    public class MapDebugInspector : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            if (!Application.isPlaying)
                return;

            MapDebugMenu mapDebugMenu = (MapDebugMenu)target;
            GUI.enabled = mapDebugMenu.IsTeleportValid();
            if (GUILayout.Button("Teleport to map"))
                mapDebugMenu.CheatTeleportToScene();
            GUI.enabled = true;
        }
    }
}
