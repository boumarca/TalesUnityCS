using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Jrpg.Dialogues.Editor
{
    [CustomPropertyDrawer(typeof(MessageStep))]
    public class MessageStepDrawer : PropertyDrawer
    {
        private Vector2 _scroll = Vector2.zero;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            SerializedProperty speakerProperty = property.FindPropertyRelative("_speaker");
            SerializedProperty messageProperty = property.FindPropertyRelative("_message");
            SerializedProperty locProperty = property.FindPropertyRelative("_localizedString");

            EditorGUILayout.PropertyField(speakerProperty);            
            EditorGUILayout.PropertyField (locProperty);
            EditorGUILayout.LabelField("Message");
            Rect scrollRect = EditorGUILayout.GetControlRect(GUILayout.Height(EditorGUIUtility.singleLineHeight * 3));
            messageProperty.stringValue = DrawScrollableTextArea(scrollRect, messageProperty.stringValue);

            EditorGUI.EndProperty();
        }

        //https://discussions.unity.com/t/editor-textarea-with-scrollbar-not-working-properly/138983/2
        private string DrawScrollableTextArea(Rect rect, string message)
        {     
            MethodInfo method = typeof(EditorGUI).GetMethod("ScrollableTextAreaInternal", BindingFlags.Static | BindingFlags.NonPublic);            
            object[] parameters = new object[] { rect, message, _scroll, EditorStyles.textArea };
            object methodResult = method.Invoke(null, parameters);
            _scroll = (Vector2)parameters[2];
            return methodResult.ToString();
        }
    }
}
