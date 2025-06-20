using HatchStudios.Input;

namespace HatchStudio.Localization
{
    public class InputGlyphProvider : ILocalizationGlyphProvider
    {
        public string GetGlyphForAction(string actionName)
        {
            var bindingPath = InputManager.GetBindingPath(actionName);
            return bindingPath?.inputGlyph.GlyphPath;
        }
    }
}