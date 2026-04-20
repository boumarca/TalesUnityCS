using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Framework.Identifiers.Editor
{
    [CustomEditor(typeof(IdentifierDatabase))]
    public class IdentifierDatabaseInspector : UnityEditor.Editor
    {
        #region Constants
        private const string IdPropertyPath = "<Id>k__BackingField";
        private const string NamePropertyPath = "<Name>k__BackingField";
        #endregion

        #region Private Fields
        private SerializedProperty _allIdentifiersProperty;
        private ReorderableList _reorderableList;
        #endregion

        #region Overrides
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditListSize();
            _reorderableList.DoLayoutList();
            DisplayValidationButton();
            serializedObject.ApplyModifiedProperties();
        }
        #endregion

        #region MonBehaviour Methods
        private void OnEnable()
        {
            _allIdentifiersProperty = serializedObject.FindProperty("_allIdentifiers");
            _reorderableList = new ReorderableList(serializedObject, _allIdentifiersProperty)
            {
                drawHeaderCallback = DrawHeader,
                drawElementCallback = DrawElement,
                elementHeightCallback = ElementHeight,
                onAddCallback = AddToList,
                onRemoveCallback = RemoveFromList
            };
        }
        #endregion

        #region Private Methods
        private void EditListSize()
        {
            EditorGUI.BeginChangeCheck();
            int newSize = EditorGUILayout.DelayedIntField("Count", _allIdentifiersProperty.arraySize);
            if (!EditorGUI.EndChangeCheck() || newSize < 0)
                return;

            while (_allIdentifiersProperty.arraySize < newSize)
                AddToList(_reorderableList);

            while (_allIdentifiersProperty.arraySize > newSize)
                RemoveFromList(_reorderableList);

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawHeader(Rect rect)
        {
            EditorGUI.LabelField(rect, "All Identifiers");
        }

        private void DrawElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            SerializedProperty element = _allIdentifiersProperty.GetArrayElementAtIndex(index);
            SerializedProperty nameProperty = element.FindPropertyRelative(NamePropertyPath);

            float paddedLineHeight = EditorGUIUtility.singleLineHeight + 2;
            Rect position = new(rect.x + 9, rect.y, rect.width - 5, paddedLineHeight);
            EditorGUI.PropertyField(position, nameProperty, GUIContent.none);
        }

        private float ElementHeight(int index)
        {
            return EditorGUIUtility.singleLineHeight + 2;
        }

        private void AddToList(ReorderableList list)
        {
            _allIdentifiersProperty.arraySize++;
            SerializedProperty newElement = _allIdentifiersProperty.GetArrayElementAtIndex(_allIdentifiersProperty.arraySize - 1);

            SerializedProperty idProperty = newElement.FindPropertyRelative(IdPropertyPath);
            SerializedProperty nameProperty = newElement.FindPropertyRelative(NamePropertyPath);

            idProperty.stringValue = Guid.NewGuid().ToString();
            nameProperty.stringValue = string.Empty;

            serializedObject.ApplyModifiedProperties();
        }

        private void RemoveFromList(ReorderableList list)
        {
            _allIdentifiersProperty.DeleteArrayElementAtIndex(_allIdentifiersProperty.arraySize - 1);
            serializedObject.ApplyModifiedProperties();
        }

        private void DisplayValidationButton()
        {
            if (GUILayout.Button("Validate id uniqueness"))
            {
                Dictionary<string, string> allIds = new();
                Dictionary<int, string> allHashCode = new();
                bool hasIdClash = false;
                bool hasHashClash = false;
                for (int i = 0; i < _allIdentifiersProperty.arraySize; i++)
                {
                    SerializedProperty element = _allIdentifiersProperty.GetArrayElementAtIndex(i);
                    SerializedProperty idProperty = element.FindPropertyRelative(IdPropertyPath);
                    SerializedProperty nameProperty = element.FindPropertyRelative(NamePropertyPath);
                    string id = idProperty.stringValue;
                    string name = nameProperty.stringValue;
                    if (allIds.TryGetValue(id, out string identifierName))
                    {
                        Debug.LogError($"Clashing Id {id} between {identifierName} and {name}");
                        hasIdClash = true;
                    }
                    else
                    {
                        allIds.Add(id, name);
                    }

                    int idHashCode = id.GetHashCode(StringComparison.Ordinal);
                    if (allHashCode.TryGetValue(idHashCode, out identifierName))
                    {
                        Debug.LogError($"Clashing Id HashCode {idHashCode} between {identifierName} and {name}");
                        hasHashClash = true;
                    }
                    else
                    {
                        allHashCode.Add(idHashCode, name);
                    }
                }

                if(!hasIdClash)
                    Debug.Log("No Id clash detected");

                if(!hasHashClash)
                    Debug.Log("No hash clash detected.");
            }
        }
        #endregion
    }
}
