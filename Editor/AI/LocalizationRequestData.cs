using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HatchStudio.Localization;
using Unity.Plastic.Newtonsoft.Json;
using UnityEngine;

namespace HatchStudio.Editor.Localization.AI
{
    [Serializable]
    internal class LocalizationRequestData
    {
        public string sourceLanguage;
        public string targetLanguage;
        public LocalizationSection[] sections;

        #region Internal

        [Serializable]
        internal sealed class LocalizationSection
        {
            public string SectionName;
            public string context;
            public LocalizationString[] strings;
        }
        
        #endregion
    }

    internal static class LocalizationRequestUtility
    {
        public static LocalizationRequestData CreateLocalizationData(TempLanguageData source, TempLanguageData target)
        {
            int index = 0;
            LocalizationRequestData requestData = new LocalizationRequestData();
            requestData.sourceLanguage = source.Entry.LanguageName;
            requestData.targetLanguage = target.Entry.LanguageName;
            requestData.sections = new LocalizationRequestData.LocalizationSection[source.TableSheet.Count];
            foreach (var section in source.TableSheet)
            {
                IList<LocalizationString> strings = new List<LocalizationString>();
                
                string sectionKey = section.Name.Replace(" ", "");
                foreach (var item in section.Items)
                {
                    string itemKey = item.Key.Replace(" ", "");
                    string key = sectionKey + "." + itemKey;
                    strings.Add(new()
                    {
                        SectionId = section.Id,
                        EntryId = item.Id,
                        key = key,
                        value = item.Value
                    });
                }
                requestData.sections[index] = (new LocalizationRequestData.LocalizationSection()
                {
                    SectionName = section.Name,
                    context = section.Reference.Context,
                    strings = strings.Select(x => new LocalizationString
                    {
                        SectionId = x.SectionId,
                        EntryId = x.EntryId,
                        key = x.key,
                        value = x.value
                    }).ToArray()
                });
                index++;
            }

            return requestData;
        }

        public static void LoadTranslation(this TempLanguageData languageData,LocalizationRequestData requestData)
        {
            int sectionIndex = 0;
            LocalizationLanguage asset = languageData.Entry.Asset;
            
            IList<LocalizationString> strings = new List<LocalizationString>();
            foreach (var section in languageData.TableSheet)
            {
                int itemIndex = 0;
                string sectionKey = section.Name.Replace(" ", "");
                foreach (var item in section.Items)
                {
                    string itemKey = item.Key.Replace(" ", "");
                    string key = sectionKey + "." + itemKey;
                    strings.Add(new()
                    {
                        SectionId = section.Id,
                        EntryId = item.Id,
                        key = key,
                        value = requestData.sections[sectionIndex].strings[itemIndex].value
                    });
                    item.Value = requestData.sections[sectionIndex].strings[itemIndex].value;
                    itemIndex++;
                }
                sectionIndex++;
            }
            asset.Strings = new(strings);
        }
        public static string CreateTranslationPrompt(this LocalizationRequestData data)
        {
            string jsonData = JsonConvert.SerializeObject(data, Formatting.Indented);

            // Construim prompt-ul cu context clar pentru GPT
            StringBuilder prompt = new StringBuilder();
            prompt.AppendLine("You are a professional game translator.");
            prompt.AppendLine("In the following JSON object, each 'section' has a 'context' field that describes the purpose or usage of the strings within that section.");
            prompt.AppendLine("Use the 'context' field to better understand and accurately translate the 'value' fields.");
            prompt.AppendLine($"Translate only the 'value' fields from {data.sourceLanguage} to {data.targetLanguage}, keeping the same JSON structure.");
            prompt.AppendLine("Do not change the 'key' fields.");
            prompt.AppendLine("Do not translate the 'context' fields. In the response, set their value to an empty string (\"\").");
            prompt.AppendLine("**Important:** Do not translate any text enclosed within square brackets (e.g., [Interact], [0], [Cancel]). Leave them unchanged in the translated text, but position them naturally to sound fluent in the target language.");
            prompt.AppendLine("Respond only with the translated JSON object, without explanations.");
            prompt.AppendLine();
            prompt.AppendLine(jsonData); 


            /*var requestBody = new
            {
                model = "gpt-3.5-turbo-0125",
                messages = ResponseHandler.GetMessages(data),
                functions = ResponseHandler.GetTranslateFunctionDefinition(),
                function_call = new { name = "translate_localization" }
                
            };
            return JsonConvert.SerializeObject(requestBody, Formatting.Indented);*/
            return prompt.ToString();
        }
       
    }
    
    
    [Serializable]
    public class Choice
    {
        public int index;
        public ChatMessage message;
        public object logprobs;
        public string finish_reason;
    }

    [Serializable]
    public class Usage
    {
        public int prompt_tokens;
        public int completion_tokens;
        public int total_tokens;
    }
    
    [Serializable]
    public class RootObject
    {
        public string id;
        public string @object;
        public long created;
        public string model;
        public List<Choice> choices;
        public Usage usage;
        public string service_tier;
        public object system_fingerprint;
    }
    
    [Serializable]
    public class ChatMessage
    {
        public string role;
        public string content;
    }

    [Serializable]
    public class ChatRequest
    {
        public string model;
        public List<ChatMessage> messages;
    }
}