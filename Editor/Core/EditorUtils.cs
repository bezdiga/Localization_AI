using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace HatchStudios.Editor.Utils
{
    public static class EditorUtils
    {
        
        public static class Styles
        {
            public static GUIStyle IconButton => GUI.skin.FindStyle("IconButton");
            public static readonly GUIContent PlusIcon = EditorGUIUtility.TrIconContent("Toolbar Plus", "Add Item");
            public static readonly GUIContent MinusIcon = EditorGUIUtility.TrIconContent("Toolbar Minus", "Remove Item");
            public static readonly GUIContent TrashIcon = EditorGUIUtility.TrIconContent("TreeEditor.Trash", "Remove Item");
            public static readonly GUIContent RefreshIcon = EditorGUIUtility.TrIconContent("Refresh", "Refresh");
            public static readonly GUIContent Linked = EditorGUIUtility.TrIconContent("Linked");
            public static readonly GUIContent UnLinked = EditorGUIUtility.TrIconContent("Unlinked");
            public static readonly GUIContent Database = EditorGUIUtility.TrIconContent("Package Manager");
            public static readonly GUIContent GreenLight = EditorGUIUtility.TrIconContent("greenLight");
            public static readonly GUIContent OrangeLight = EditorGUIUtility.TrIconContent("orangeLight");
            public static readonly GUIContent RedLight = EditorGUIUtility.TrIconContent("redLight");

            public static GUIStyle RichLabel => new GUIStyle(EditorStyles.label)
            {
                richText = true
            };
        }
        public static IEnumerable<SerializedProperty> GetVisibleChildrens(this SerializedProperty serializedProperty)
        {
            SerializedProperty currentProperty = serializedProperty.Copy();
            SerializedProperty nextSiblingProperty = serializedProperty.Copy();
            {
                nextSiblingProperty.NextVisible(false);
            }

            if (currentProperty.NextVisible(true))
            {
                do
                {
                    if (SerializedProperty.EqualContents(currentProperty, nextSiblingProperty))
                        break;

                    yield return currentProperty;
                }
                while (currentProperty.NextVisible(false));
            }
        }
    }
}