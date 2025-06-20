using HatchStudios.Input;

namespace HatchStudio.Localization
{
    public class InputGlyphProvider : ILocalizationGlyphProvider
    {
        Func<string,string> GlyphGetter;
        
        public InputGlyphProvider(ActionGlyphGetter glyphGetter)
        {
            GlyphGetter = glyphGetter ?? throw new ArgumentNullException(nameof(glyphGetter), "Glyph getter function cannot be null.");
        }
        public string GetGlyphForAction(string actionName)
        {
            if (string.IsNullOrWhiteSpace(actionName))
                throw new ArgumentException("Action name cannot be null or empty.", nameof(actionName));
            //var bindingPath = InputManager.GetBindingPath(actionName);
            return GlyphGetter.Invoke(actionName); //bindingPath?.inputGlyph.GlyphPath;
        }
    }
    
    public delegate string ActionGlyphGetter(string actionName);
}