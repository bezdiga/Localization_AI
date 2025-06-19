using System.Collections.Generic;
using UnityEngine;

namespace HatchStudio.Localization
{
    public static class SystemLanguageConverter
    {
        private static readonly Dictionary<SystemLanguage, string> LanguageCultureMap = new Dictionary<SystemLanguage, string>
        {
            { SystemLanguage.Afrikaans, "af" },
            { SystemLanguage.Arabic, "ar" },
            { SystemLanguage.Basque, "eu" },
            { SystemLanguage.Belarusian, "be" },
            { SystemLanguage.Bulgarian, "bg" },
            { SystemLanguage.Catalan, "ca" },
            { SystemLanguage.Chinese, "zh-CN" },
            { SystemLanguage.ChineseSimplified, "zh-hans" },
            { SystemLanguage.ChineseTraditional, "zh-hant" },
            { SystemLanguage.SerboCroatian, "hr" },
            { SystemLanguage.Czech, "cs" },
            { SystemLanguage.Danish, "da" },
            { SystemLanguage.Dutch, "nl" },
            { SystemLanguage.English, "en" },
            { SystemLanguage.Estonian, "et" },
            { SystemLanguage.Faroese, "fo" },
            { SystemLanguage.Finnish, "fi" },
            { SystemLanguage.French, "fr" },
            { SystemLanguage.German, "de" },
            { SystemLanguage.Greek, "el" },
            { SystemLanguage.Hebrew, "he" },
            { SystemLanguage.Hungarian, "hu" },
            { SystemLanguage.Icelandic, "is" },
            { SystemLanguage.Indonesian, "id" },
            { SystemLanguage.Italian, "it" },
            { SystemLanguage.Japanese, "ja" },
            { SystemLanguage.Korean, "ko" },
            { SystemLanguage.Latvian, "lv" },
            { SystemLanguage.Lithuanian, "lt" },
            { SystemLanguage.Norwegian, "no" },
            { SystemLanguage.Polish, "pl" },
            { SystemLanguage.Portuguese, "pt" },
            { SystemLanguage.Romanian, "ro" },
            { SystemLanguage.Russian, "ru" },
            { SystemLanguage.Slovak, "sk" },
            { SystemLanguage.Slovenian, "sl" },
            { SystemLanguage.Spanish, "es" },
            { SystemLanguage.Swedish, "sv" },
            { SystemLanguage.Thai, "th" },
            { SystemLanguage.Turkish, "tr" },
            { SystemLanguage.Ukrainian, "uk" },
            { SystemLanguage.Vietnamese, "vi" },
#if UNITY_2022_2_OR_NEWER
            { SystemLanguage.Hindi, "hi" }
#endif
        };

        /// <summary>
        /// Converts a SystemLanguage enum into a CultureInfo Code.
        /// </summary>
        /// <param name="lang">The SystemLanguage enum to convert into a Code.</param>
        /// <returns>The language Code or an empty string if the value could not be converted.</returns>
        internal static string GetSystemLanguageCultureCode(SystemLanguage lang)
        {
            return LanguageCultureMap.TryGetValue(lang, out var cultureCode) ? cultureCode : string.Empty;
        }
    }
}