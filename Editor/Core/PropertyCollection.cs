using System.Collections.Generic;
using UnityEditor;

namespace HatchStudios.Editor.Utils
{
    public class PropertyCollection : Dictionary<string,SerializedProperty>
    {
        public void Draw(string propertyName, int indent = 0)
        {
            if (TryGetValue(propertyName, out SerializedProperty property))
            {
                if (indent > 0) EditorGUI.indentLevel += indent;
                EditorGUILayout.PropertyField(property);
                //EditorDrawing.DrawProperty(property);
                if (indent > 0) EditorGUI.indentLevel -= indent;
            }
        }
        
    }
}