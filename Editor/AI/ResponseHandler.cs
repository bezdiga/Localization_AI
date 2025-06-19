using Unity.Plastic.Newtonsoft.Json;

namespace HatchStudio.Editor.Localization.AI
{
    internal class ResponseHandler
    {
        internal static object GetTranslateFunctionDefinition()
        {
            return new
            {
                name = "translate_localization",
                description = "Translate localization value fields while keeping the original JSON structure.",
                parameters = new
                {
                    type = "object",
                    properties = new {
                        sections = new {
                            type = "array",
                            items = new {
                                type = "object",
                                properties = new {
                                    SectionName = new { type = "string" },
                                    strings = new {
                                        type = "array",
                                        items = new {
                                            type = "object",
                                            properties = new {
                                                key = new { type = "string" },
                                                value = new { type = "string" }
                                            },
                                            required = new[] { "key", "value" }
                                        }
                                    }
                                },
                                required = new[] { "SectionName", "strings" }
                            }
                        }
                    },
                    required = new[] { "sections" }
                }
            };
        }
        
        internal static object[] GetMessages(LocalizationRequestData data)
        {
            string jsonData = JsonConvert.SerializeObject(data, Formatting.Indented);
            return new object[]
            {
                new { role = "system", content = "You are a professional game translator." },
                new { role = "user", content = $"Translate these localization entries from {data.sourceLanguage} to {data.targetLanguage}. Here is the data: {jsonData}" }
            };
        }
    }
}