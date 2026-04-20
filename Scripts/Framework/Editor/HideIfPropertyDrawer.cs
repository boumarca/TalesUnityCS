/*
 * Source: https://forum.unity.com/threads/draw-a-field-only-if-a-condition-is-met.448855/
 */

using System;
using System.IO;
using Framework.Utils;
using UnityEditor;
using UnityEngine;

namespace Framework.Editor
{
    [CustomPropertyDrawer(typeof(HideIfAttribute))]
    public class HideIfPropertyDrawer : PropertyDrawer
    {
#pragma warning disable CA1062 // Validate arguments of public methods

        #region PropertyDrawer Overrides
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return IsConditionMet(property) ? 0 : base.GetPropertyHeight(property, label) ;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!IsConditionMet(property))
                EditorGUI.PropertyField(position, property);
        }
        #endregion

        #region Private Methods
        private object FindPropertyValue(SerializedProperty property)
        {
            HideIfAttribute hideIfAttribute = attribute as HideIfAttribute;
            string path = property.propertyPath.Contains('.', StringComparison.Ordinal) ? Path.ChangeExtension(property.propertyPath, hideIfAttribute.ComparedPropertyName) : hideIfAttribute.ComparedPropertyName;
            SerializedProperty relativePropriety = property.serializedObject.FindProperty(path);
            return relativePropriety?.boxedValue;
        }

        private bool IsConditionMet(SerializedProperty property)
        {
            HideIfAttribute hideIfAttribute = attribute as HideIfAttribute;
            object comparedFieldValue = FindPropertyValue(property);
            return comparedFieldValue.Equals(hideIfAttribute.ComparedValue);
        }
        #endregion
    }
}
