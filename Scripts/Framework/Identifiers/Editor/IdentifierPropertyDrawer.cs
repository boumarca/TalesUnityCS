using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Framework.Identifiers.Editor
{
    //Original by DYLAN ENGELMAN http://jupiterlighthousestudio.com/custom-inspectors-unity/
    //Altered by Brecht Lecluyse https://www.brechtos.com/tagselectorattribute/
    //Further modified by me.

    [CustomPropertyDrawer(typeof(Identifier))]
    public abstract class IdentifierPropertyDrawer : PropertyDrawer
    {
#pragma warning disable CA1062 // Validate arguments of public methods
        #region Public Properties
        public abstract string IdentifierDatabasePath { get; }
        public virtual IReadOnlyCollection<IdentifierData> IdentifierList
        {
            get
            {
                IdentifierDatabase questIdDb = AssetDatabase.LoadAssetAtPath<IdentifierDatabase>(IdentifierDatabasePath);
                return questIdDb == null ? Array.Empty<IdentifierData>() : questIdDb.Identifiers;
            }
        }
        #endregion

        #region Overrides
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            SerializedProperty idProperty = property.FindPropertyRelative("_id");
            SerializedProperty nameProperty = property.FindPropertyRelative("<Name>k__BackingField");

            string id = idProperty.stringValue;
            IdentifierData identifierData = IdentifierList.FirstOrDefault(identifier => identifier.Id == id);

            string propertyString = identifierData?.Name;
            int listIndex = 0;
            List<string> stringList = GenerateEntryList();
            if (!string.IsNullOrEmpty(propertyString))
            {
                //Check if there is an entry that matches the value and get the index
                //We skip index 0 as that is a special custom case
                for (int i = 1; i < stringList.Count; i++)
                {
                    if (stringList[i] != propertyString)
                        continue;

                    listIndex = i;
                    break;
                }
            }

            //Draw the popup box with the current selected index
            listIndex = EditorGUI.Popup(position, label.text, listIndex, stringList.ToArray());

            //Adjust the actual string value of the property based on the selection
            if (listIndex >= 1)
            {
                nameProperty.stringValue = stringList[listIndex];
                IdentifierData newIdentifier = IdentifierList.FirstOrDefault(identifier => identifier.Name == stringList[listIndex]);
                idProperty.stringValue = newIdentifier != null ? newIdentifier.Id : string.Empty;
            }
            else
            {
                idProperty.stringValue = string.Empty;
            }
            EditorGUI.EndProperty();
        }
        #endregion

        private List<string> GenerateEntryList()
        {
            List<string> stringList = new() { "<Empty>" };
            stringList.AddRange(IdentifierList.Select(data => data.Name));
            return stringList;
        }
#pragma warning restore CA1062 // Validate arguments of public methods
    }
}
