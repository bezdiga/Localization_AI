using HatchStudio.Localization;
using HatchStudios.Editor.Core;
using HatchStudios.Editor.Utils;
using UnityEditor;
using UnityEngine;

namespace HatchStudio.Editor.Localization
{
    [CustomEditor(typeof(LocalizationTable))]
    public class LocalizationTableEditor : EditorBase<LocalizationTable>
    {
        public override void OnInspectorGUI()
        {
            EditorDrawing.DrawHeader("Localization Table","");
            
            using(new EditorDrawing.BorderBoxScope(new GUIContent("Languages"), roundedBox: false))
            {
                if(Target.Languages.Count > 0)
                {
                    using (new EditorGUI.DisabledGroupScope(true))
                    {
                        foreach (var lang in Target.Languages)
                        {
                            string name = lang.LanguageName.Or("Unknown");
                            EditorGUILayout.ObjectField(new GUIContent(name), lang, typeof(LocalizationLanguage), false);
                        }
                    }
                }
                else
                {
                    EditorGUILayout.HelpBox("There are currently no languages available, open the localization editor and add new languages.", MessageType.Info);
                }
            }
            EditorGUILayout.Space();
            
            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.FlexibleSpace();
                Rect buttonRect = EditorGUILayout.GetControlRect(GUILayout.Width(250), GUILayout.Height(30));
                if (GUI.Button(buttonRect, "Open Localization Window"))
                {
                    OpenLocalizationWindow(Target);
                }
                GUILayout.FlexibleSpace();
            }
            EditorGUILayout.EndHorizontal();
        }

        private void OpenLocalizationWindow(LocalizationTable table)
        {
            var window = EditorWindow.GetWindow<LocalizationManagerWindow>();
            window.titleContent = new GUIContent("Localization Manager");
            Vector2 windowSize = new(1000, 500);
            window.minSize = windowSize;
            (window as LocalizationManagerWindow).Show(table);
        }
    }
}