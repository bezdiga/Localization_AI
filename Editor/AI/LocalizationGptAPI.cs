﻿
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Unity.Plastic.Newtonsoft.Json;
using UnityEngine;

namespace HatchStudio.Editor.Localization.AI
{
    public static class LocalizationGptAPI
    {
        public static async void Translate(TempLanguageData from, TempLanguageData to,Action callback)
        {
            LocalizationRequestData data = LocalizationRequestUtility.CreateLocalizationData(from, to);
            
            string prompt = data.CreateTranslationPrompt();
            string apiKey =
                "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";
            string result = await SendPromptAsync(apiKey, prompt);
            
            Debug.LogError("Prompt "+ prompt);
            RootObject response = JsonConvert.DeserializeObject<RootObject>(result);
            
            if (response != null && response.choices != null && response.choices.Count > 0)
            {
                string content = response.choices[0].message.content;
                Debug.LogError("Context " + content);
                if (IsValidJson(content))
                {
                    LocalizationRequestData translationData = JsonConvert.DeserializeObject<LocalizationRequestData>(content);
                    
                    if (translationData != null)
                    {
                        to.LoadTranslation(translationData);
                        foreach (var section in translationData.sections)
                        {
                            Debug.Log("Sectiune: " + section.context);
                            foreach (var translation in section.strings)
                            {
                                Debug.Log($"Key: {translation.key}, Value: {translation.value}");
                            }
                        }
                    }
                    else
                    {
                        Debug.LogError("Deserializarea răspunsului traducerii a eșuat.");
                    }
                }
                else
                {
                    Debug.LogError("Răspunsul nu este JSON valid. Conținutul primit: " + content);
                }
            }
            else
            {
                Debug.LogError("Nu am primit date valide de la API.");
            }
            callback?.Invoke();
        }
        
        
        
        public static async Task<string> SendPromptAsync(string apiKey, string prompt)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

                var request = new ChatRequest
                {
                    model = "gpt-3.5-turbo",
                    messages = new List<ChatMessage>
                    {
                        new ChatMessage { role = "user", content = prompt }
                    }
                };

                string jsonRequest = JsonUtility.ToJson(request);

                var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
                var response = await client.PostAsync("https://api.openai.com/v1/chat/completions", content);

                if (response.IsSuccessStatusCode)
                {
                    string result = await response.Content.ReadAsStringAsync();
                    return result;
                }
                else
                {
                    Debug.LogError($"Request failed: {response.StatusCode}\n{await response.Content.ReadAsStringAsync()}");
                    return null;
                }
            }
        }
        
        public static bool IsValidJson(string json)
        {
            try
            {
                // Încercăm să deserializăm JSON-ul într-un obiect generic pentru a valida formatul
                var obj = JsonConvert.DeserializeObject<object>(json);
                return obj != null;
            }
            catch (JsonException)
            {
                return false;
            }
        }
        
    }
}