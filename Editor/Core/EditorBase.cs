
using HatchStudios.Editor.Utils;
using UnityEngine;

namespace HatchStudios.Editor.Core
{
    using UnityEditor;
    public class EditorBase<T> : Editor where T : Object
    {
        public T Target { get; private set; }
        public PropertyCollection Properties { get; private set; }

        protected virtual void OnEnable()
        {
            Target = target as T;
            Properties = EditorDrawing.GetAllProperties(serializedObject);
        }
    }
}