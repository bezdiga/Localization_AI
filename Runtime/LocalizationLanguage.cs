using System;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using System.Linq;
#endif

namespace HatchStudio.Localization
{
    [CreateAssetMenu(menuName = "Hatch Studio/Localization/Language Asset", fileName = "Language Asset")]
    public class LocalizationLanguage : ScriptableObject
    {
        public string LanguageName;
        public List<LocalizationString> Strings = new();
        

#if UNITY_EDITOR
        /// <summary>ATTENTION: Use only in Editor </summary>
        public float TranslatePercent { get; private set; }

        public void UpdatePercent()
        {
            TranslatePercent = (Strings.Count(x => !String.IsNullOrEmpty(x.value)) * 100f) / Strings.Count;
        }
        private void OnValidate()
        {
            UpdatePercent();
        }
#endif

    }
    
    [Serializable]
    public sealed class LocalizationString
    {
        public int SectionId;
        public int EntryId ;
            
        public string key;
        public string value;
    }
}