using Game.QuestSystem.Data;
using UnityEditor;
using UnityEngine;

namespace Game.QuestSystem.Editor
{
    [CustomPropertyDrawer(typeof(QuestPrecondition))]
    public class QuestPreconditionPropertyDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float paddedHeight = EditorGUIUtility.singleLineHeight + 2;
            return IsQuestInProgress(property.FindPropertyRelative("<RequiredQuestState>k__BackingField")) ? paddedHeight * 4 : paddedHeight * 2;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            SerializedProperty requiredQuestProperty = property.FindPropertyRelative("<RequiredQuest>k__BackingField");
            SerializedProperty requiredQuestStateProperty = property.FindPropertyRelative("<RequiredQuestState>k__BackingField");
            SerializedProperty stepComparisonProperty = property.FindPropertyRelative("<StepComparison>k__BackingField");
            SerializedProperty requiredStepProperty = property.FindPropertyRelative("<RequiredStep>k__BackingField");

            float paddedHeight = EditorGUIUtility.singleLineHeight + 2;
            Rect rect = new(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            EditorGUI.PropertyField(rect, requiredQuestProperty);
            rect.y += paddedHeight;
            EditorGUI.PropertyField(rect, requiredQuestStateProperty);

            if (IsQuestInProgress(requiredQuestStateProperty))
            {
                rect.y += paddedHeight;
                EditorGUI.PropertyField(rect, stepComparisonProperty);
                rect.y += paddedHeight;
                EditorGUI.PropertyField(rect, requiredStepProperty);
            }

            EditorGUI.EndProperty();
        }

        private static bool IsQuestInProgress(SerializedProperty questStateProperty)
        {
            QuestState questState = (QuestState)questStateProperty.intValue;
            return questState == QuestState.InProgress;
        }
    }
}
