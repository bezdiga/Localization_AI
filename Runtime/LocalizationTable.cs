using System;
using System.Collections.Generic;
using UnityEngine;

namespace HatchStudio.Localization
{
    [CreateAssetMenu(menuName = "Hatch Studio/Localization/Localization Table", fileName = "Localization Table")]
    public class LocalizationTable : ScriptableObject
    {
        public List<LocalizationLanguage> Languages = new ();

        #region Editor
        #if UNITY_EDITOR
        public List<TableData> TableSheet = new();
        

        #region Internal

        [Serializable]
        public struct SheetItem
        {
            public int Id;
            public string Key;

            public SheetItem(string key, int id)
            {
                Id = id;
                Key = key;
            }
        }

        [Serializable]
        public struct TableData
        {
            public int Id;
            public string SectionName;
            public string SectionContext;
            public List<SheetItem> SectionSheet;

            public TableData(string section,string ctx, int id)
            {
                Id = id;
                SectionName = section;
                SectionContext = ctx;
                SectionSheet = new List<SheetItem>();
            }
        }
        

        #endregion
        #endif
        #endregion
        
    }
}