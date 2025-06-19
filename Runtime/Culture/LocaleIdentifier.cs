using System;
using System.Globalization;
using UnityEngine;

namespace HatchStudio.Localization
{
    [Serializable]
    public struct LocaleIdentifier
    {
        [SerializeField] string m_Code;
        CultureInfo m_CultureInfo;
        
        private LocaleIdentifier(string code)
        {
            m_Code = code;
            m_CultureInfo = null;
        }
        public LocaleIdentifier(SystemLanguage systemLanguage)
            : this(SystemLanguageConverter.GetSystemLanguageCultureCode(systemLanguage))
        {
        }
        
    }
}