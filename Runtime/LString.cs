using System;
using UnityEngine;

namespace HatchStudio.Localization
{
    [Serializable]
    public sealed class LString : IDisposable
    {
        public string LText;
        public string LocalizationKey;

        private Action unsubscribeAction;
        public string Value
        {
            get
            {
                if(string.IsNullOrEmpty(LText)) 
                    return LocalizationKey;

                return LText;
            }

            set
            {
                LText = value;
            }
        }

        public void SubscribeToLocalization(Action<string> onUpdate = null)
        {
            if (String.IsNullOrEmpty(Value))
                return;
            unsubscribeAction = LocalizationKey.SubscribeLocalization(text =>
            {
                LText = text;
                onUpdate?.Invoke(text);
            });
        }
        public void SubscribeToGlyphLocalization(Action<string> onUpdate = null)
        {
            if (String.IsNullOrEmpty(Value))
                return;
            LocalizationKey.SubscribeLocalizationGlyph(text =>
            {
                LText = text;
                onUpdate?.Invoke(text);
            });
        }

        public static implicit operator string(LString localizationString)
        {
            return localizationString?.Value;
        }
        public void Dispose()
        {
            unsubscribeAction?.Invoke();
            unsubscribeAction = null;
        }
    }
}