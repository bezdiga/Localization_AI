using System;
using System.Collections.Generic;
using HatchStudio.Manager;
using HatchStudios.ToolBox;
using UnityEngine;

namespace HatchStudio.Localization
{
    [CreateAssetMenu(menuName = "Hatch Studio/Managers/Game Localization", fileName = "Localization Manager")]
    public class GameLocalization : Manager<GameLocalization>
    {
        public LocalizationTable LocalizationTable => m_LocalizationTable;
        [SerializeField,NotNull] private LocalizationTable m_LocalizationTable;
        [SerializeField] private int m_DefaultLanguage;

        public Action<int> OnLanguageChange;
        internal Dictionary<string, List<Action<string>>> _subscribers;
        public IDictionary<string, string> LanguageMap => _languageDict;
        private IDictionary<string, string> _languageDict;

        private int selectedLanguage;

        #region Initialzie
        
#if UNITY_EDITOR
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
#else
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
#endif
        private static void Init() => LoadOrCreateInstance();

        protected override void OnInitialized()
        {
            _languageDict = GenerateLanguageDictionary(m_DefaultLanguage);
            _subscribers = new Dictionary<string, List<Action<string>>>();
        }
        #endregion
        private IDictionary<string, string> GenerateLanguageDictionary(int language)
        {
            IDictionary<string, string> _dictionary = new Dictionary<string, string>();
            if (m_LocalizationTable == null || m_LocalizationTable.Languages.Count == 0)
            {
                Debug.LogError($"Game Localization: Localization Table is null or is not seted Language",this);
                return _dictionary;
            }
            LocalizationLanguage currentLanguage = m_LocalizationTable.Languages[language];

            if (currentLanguage == null)
            {
                Debug.LogError("Game Localization: Selected language is null",this);
            }
            selectedLanguage = language;

            foreach (var data in currentLanguage.Strings)
            {
                if(String.IsNullOrEmpty(data.key) || String.IsNullOrEmpty(data.value))
                    continue;
                if (_dictionary.ContainsKey(data.key))
                {
                    Debug.LogError($"The string with key {data.key} alread exist");
                    continue;
                }
                _dictionary.Add(data.key,data.value);
            }
            return _dictionary;
        }

        public void ChangeLanguage(int languageIndex)
        {
            if (languageIndex > _languageDict.Count || languageIndex < 0)
            {
                Debug.LogError("Game Localization: Invalid language index",this);
                return;
            }

            _languageDict = GenerateLanguageDictionary(languageIndex);
            
            foreach (var subscribe in _subscribers)
            {
                var text = GetLocalizedString(subscribe.Key);

                foreach (var callback in subscribe.Value)
                {
                    callback.Invoke(text);
                }
            }
            OnLanguageChange?.Invoke(languageIndex);
        }
        
        private string GetLocalizedString(string key)
        {
            if (_languageDict.TryGetValue(key, out string text))
                return text;

            return $"[Missing {key}]";
        }
        
    }
}