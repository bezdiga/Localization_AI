using System;
using System.Collections;
using System.Text;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

namespace HatchStudio.Editor.Localization.AI
{
    public class GptWindow : EditorWindow
    {
        private string apiKey = "key";
        private string inputText = "";
        [MenuItem("Tools/Localization/Gpt")]
        public static void OpneGptWindow()
        {
            GptWindow window = EditorWindow.GetWindow<GptWindow>();
            window.titleContent = new GUIContent("Gpt Window");
            Vector2 windowSize = new(1000, 500);
            window.minSize = windowSize;
            window.Show();
        }

        private void OnGUI()
        {
            Rect toolbarRect = new(0, 0, position.width, 20f);
            float width = position.width - 100;
            Rect requestRect = new(50, (position.height - 40 )/2, width, 40f);
            GUIContent finderIcon = EditorGUIUtility.TrIconContent("Search Icon", "Localization Key Selector");
            GUI.Box(requestRect, GUIContent.none, EditorStyles.label);
            float textFieldWidth = requestRect.width * .7f;
            Rect textFieldRect = new Rect(requestRect.x + 10, requestRect.y + 5, textFieldWidth, 30f);
            Rect buttonRect = new Rect((textFieldRect.width + textFieldRect.x) + 5, requestRect.y + 5, 50, 30);
            inputText = GUI.TextField(textFieldRect, inputText, EditorStyles.textField);
            if (GUI.Button(buttonRect, new GUIContent("Send")))
            {
                Debug.LogError("Send " + inputText);
                EditorCoroutineUtility.StartCoroutineOwnerless(SendRequest());
            }
            float buttonWidth = 100f;
            float spacing = 5f;
            
        }
        
        IEnumerator SendRequest()
        {
            string apiKey = "your_api_key_here"; // Replace with your actual API key
            if (string.IsNullOrEmpty(apiKey))
            {
                Debug.LogError("API Key not set!");
                yield break;
            }

            string prompt = "Translate 'Play' to Romanian.";

            string jsonPayload = "{\"model\": \"gpt-3.5-turbo\", \"messages\": [{\"role\": \"user\", \"content\": \"" + prompt + "\"}], \"temperature\": 0.7}";

            using (UnityWebRequest request = new UnityWebRequest("https://api.openai.com/v1/chat/completions", "POST"))
            {
                byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonPayload);
                request.uploadHandler = new UploadHandlerRaw(bodyRaw);
                request.downloadHandler = new DownloadHandlerBuffer();
                request.SetRequestHeader("Content-Type", "application/json");
                request.SetRequestHeader("Authorization", "Bearer " + apiKey);

                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    Debug.Log("Response: " + request.downloadHandler.text);
                }
                else
                {
                    Debug.LogError("Error: " + request.error);
                }
            }
        }
    }
}