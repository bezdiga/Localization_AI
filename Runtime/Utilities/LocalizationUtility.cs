using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace HatchStudio.Localization
{
    public static class LocalizationUtility
    {
        private static ILocalizationGlyphProvider _glyphProvider;
        
        public static void SetGlyphProvider(ILocalizationGlyphProvider provider)
        {
            _glyphProvider = provider;
        }
        public static void SubscribeLocalizationGlyph(this string key, Action<string> onUpdate, bool observeBinding = true)
        {
            if(_glyphProvider == null)
            {
                throw new InvalidOperationException("LocalizationGlyphProvider is not set. Please call SetGlyphProvider before subscribing to localization glyphs.");
            }
            SubscribeLocalization(key, text =>
            {
                if (string.IsNullOrEmpty(text))
                    return;
               
                Regex regex = new Regex(@"\[(.*?)\]");
                MatchCollection matches = regex.Matches(text);
                string[] bindingGlyphs = new string[matches.Count];
                string formatText = text;
                
                if (matches.Count > 0)
                {
                    for (int index = 0; index < matches.Count; index++)
                    {
                        var match = matches[index];
                        string group = match.Groups[0].Value;
                        string action = match.Groups[1].Value;
                        
                        /*var bindingPath = InputManager.GetBindingPath(action);
                        if (bindingPath == null) continue;*/
                        string glyph = _glyphProvider.GetGlyphForAction(action);
                        if (string.IsNullOrEmpty(glyph)) continue;
                        
                        
                        formatText = formatText.Replace(group, "{" + index + "}");
                        
                        bindingGlyphs[index] = glyph;
                    }
                    
                }
                string formattedString = string.Format(formatText, bindingGlyphs);
                onUpdate?.Invoke(formattedString);
            });
        }
        public static Action SubscribeLocalization(this string key, Action<string> callback)
        {
            GameLocalization gameLocalization = GameLocalization.Instance;

            if (!gameLocalization._subscribers.TryGetValue(key, out var list))
            {
                list = new List<Action<string>>();
                gameLocalization._subscribers.Add(key,list);
            }
            list.Add(callback);
            
            if (gameLocalization.LanguageMap.TryGetValue(key, out string text))
                callback.Invoke(text);

            return () => list.Remove(callback);
        }
        
        public static string Or(this string str, string otherwise)
        {
            return !string.IsNullOrEmpty(str) ? str : otherwise;
        }
    }
}