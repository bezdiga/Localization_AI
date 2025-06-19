using HatchStudio.Localization;
using HatchStudios.Editor.Core;
using HatchStudios.Editor.Utils;
using UnityEditor;
using UnityEngine;

namespace HatchStudio.Editor.Localization
{
    [CustomEditor(typeof(LocalizationLanguage))]
    public class LocalizationLanguageEditor : EditorBase<LocalizationLanguage>
    {
        public override void OnInspectorGUI()
        {
            EditorDrawing.DrawHeader( $"Language Asset ({Target.LanguageName.Or("Unknown")})","You can edit this language in the Game Localization Table Editor window.");

            serializedObject.Update();
            {
                
                EditorDrawing.DrawProperty(Properties["LanguageName"]);
                EditorGUI.indentLevel++;
                EditorGUILayout.BeginHorizontal();
                {
                    Rect headerRect = EditorGUILayout.GetControlRect(false,GUILayout.Height(20));
                    Color headerColor = new Color(0.1f, 0.1f, 0.1f, 0.25f);
                    EditorGUI.DrawRect(headerRect, headerColor);
                    headerRect.x += 4;
                    using (new EditorGUI.DisabledScope(true))
                    {
                        EditorGUI.TextField(headerRect,"Strings ",Target.Strings.Count.ToString());
                    }
                }
                EditorGUILayout.EndHorizontal();
                EditorGUI.indentLevel--;
            }
            serializedObject.ApplyModifiedProperties();
        }
    }
}