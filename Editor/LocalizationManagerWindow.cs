using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using HatchStudio.Editor.Localization.AI;
using HatchStudio.Editors;
using HatchStudio.Localization;
using HatchStudios.Editor.Utils;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace HatchStudio.Editor.Localization
{
    public class LocalizationManagerWindow : EditorWindow
    {
        private const float k_LanguagesWidth = 180f;
        private const float k_TableSheetWidth = 180f;
        
        [SerializeField] private TreeViewState languageTreeViewState;
        private LanguagesTreeView languagesTreeView;
        [SerializeField] private TreeViewState tableTreeViewState;
        private TableSheetTreeView _tableSheetTreeView;
        [SerializeField] private LocalizationTable localizationTable;
        
        private WindowSelection selection = null;

        private LocalizationWindowData windowData;
        
        private bool globalExpanded = false;
        private SearchField searchField;
        private string searchString;
        private Vector2 scrollPosition;

        private Vector2 textAreaScroll;
        private ProgressSimulator _progressSimulator;
        private readonly string[] statusMessages = {
            "Packing prompt data...",
            "Validating prompt content...",
            "Connecting to OpenAI API...",
            "Sending HTTP request...",
            "Waiting for server response...",
            "Processing response from server...",
            "Parsing and deserializing data...",
            "Applying response to the editor..."
        };
        private bool isLoading;
        private GUIStyle miniLabelButton => new GUIStyle(EditorStyles.miniButton)
        {
            font = EditorStyles.miniBoldLabel.font,
            fontSize = EditorStyles.miniBoldLabel.fontSize
        };
        
        private float Spacing => EditorGUIUtility.standardVerticalSpacing * 2;
        
        public void Show(LocalizationTable localizationTable)
        {
            this.localizationTable = localizationTable;
            searchField = new SearchField();
            InitializeTreeView();
        }

        void OnEnable()
        {
            searchField = new SearchField();
            if (localizationTable != null)
            {
                InitializeTreeView();
            }
        }

        
        private void InitializeTreeView()
        {
            LocalizationWindowUtility.BuildWindowData(localizationTable, out windowData);
            languageTreeViewState = new TreeViewState();
            languagesTreeView = new LanguagesTreeView(languageTreeViewState,windowData);
            languagesTreeView.OnLanguageSelect += (select) => selection = select;
            tableTreeViewState = new TreeViewState();
            _tableSheetTreeView = new TableSheetTreeView(tableTreeViewState, windowData)
            {
                OnTableSheetSelect = (s) => selection = s
            };
            
        }

        private void OnGUI()
        {
            DrawTable();
            if (isLoading)
            {
                var statusRect = new Rect(0, 0, 300, 30);
                
                statusRect.x = (position.width - statusRect.width) / 2;
                statusRect.y = (position.height - statusRect.height) / 2;
                EditorGUI.DrawRect(statusRect,new Color(0f, 0f, 0f, 0.2f));
                statusRect.x += 10;
                GUI.Label(statusRect, _progressSimulator.GetCurrentMessage(), EditorDrawing.Styles.LabelCenter);
            }
        }
        
        private void DrawTable()
        {
            GUI.enabled = !isLoading;
            Rect toolbarRect = new(0, 0, position.width, 20f);
            GUI.Box(toolbarRect, GUIContent.none, EditorStyles.toolbar);

            // Define button size and spacing
            float buttonWidth = 100f;
            float spacing = 5f;

            // place buttons starting from the left
            Rect saveBtn = new(toolbarRect.xMax - buttonWidth - spacing, 0, buttonWidth, 20f);
            Rect importBtn = new(saveBtn.xMin - buttonWidth - spacing, 0, buttonWidth, 20f);
            Rect exportBtn = new(importBtn.xMin - buttonWidth - spacing, 0, buttonWidth, 20f);
            
            
            // Export CSV button
            if (GUI.Button(exportBtn, "Export CSV", EditorStyles.toolbarButton))
            {
                string path = EditorUtility.SaveFilePanel("Export CSV", "", "Localization", "csv");
                LocalizationExporter.ExportLocalizationToCSV(windowData, path);
            }

            // Export CSV button
            if (GUI.Button(exportBtn, "Export CSV", EditorStyles.toolbarButton))
            {
                string path = EditorUtility.SaveFilePanel("Export CSV", "", "Localization", "csv");
                LocalizationExporter.ExportLocalizationToCSV(windowData, path);
            }

            // Import CSV button
            if (GUI.Button(importBtn, "Import CSV", EditorStyles.toolbarButton))
            {
                string path = EditorUtility.OpenFilePanel("Export CSV", "", "csv");
                LocalizationExporter.ImportLocalizationFromCSV(windowData, path);
            }

            // Save Asset button
            if (GUI.Button(saveBtn, "Save Asset", EditorStyles.toolbarButton))
            {
                BuildLocalizationTable();
                EditorUtility.SetDirty(localizationTable);
                AssetDatabase.SaveAssets();
            }
            
            Rect languagesRect = new Rect(5f, 25f, k_LanguagesWidth, position.height - 35f);
            languagesTreeView.OnGUI(languagesRect);
            
            float tableSheetStartX = languagesRect.xMax + 5f;
            Rect tableSheetRect = new Rect(tableSheetStartX, 25f, k_TableSheetWidth, position.height - 35f);
            _tableSheetTreeView.OnGUI(tableSheetRect);
            
            if (selection != null)
            {
                float inspectorStartX = tableSheetRect.xMax + 5f;
                Rect inspectorRect = new Rect(inspectorStartX, 25f, position.width - inspectorStartX - 5f, position.height - 30f);

                if (selection is LanguageSelect language)
                {
                    string title = language.Language.Entry.LanguageName;
                    GUIContent inspectorTitle = EditorGUIUtility.TrTextContentWithIcon($" INSPECTOR ({title})", "PrefabVariant On Icon");
                    EditorDrawing.DrawHeaderWithBorder(ref inspectorRect, inspectorTitle, 20f, false);

                    Rect inspectorViewRect = inspectorRect;
                    inspectorViewRect.y += Spacing;
                    inspectorViewRect.yMax -= Spacing;
                    inspectorViewRect.xMin += Spacing;
                    inspectorViewRect.xMax -= Spacing;

                    GUILayout.BeginArea(inspectorViewRect);
                    OnDrawLanguageInspector(language);
                    GUILayout.EndArea();
                }
                else if(selection is SectionSelect section)
                {
                    string title = section.Section.Name;
                    GUIContent inspectorTitle = EditorGUIUtility.TrTextContentWithIcon($" INSPECTOR ({title})", "PrefabVariant On Icon");
                    EditorDrawing.DrawHeaderWithBorder(ref inspectorRect, inspectorTitle, 20f, false);

                    Rect inspectorViewRect = inspectorRect;
                    inspectorViewRect.y += Spacing;
                    inspectorViewRect.yMax -= Spacing;
                    inspectorViewRect.xMin += Spacing;
                    inspectorViewRect.xMax -= Spacing;

                    GUILayout.BeginArea(inspectorViewRect);
                    OnDrawSectionInspector(section);
                    GUILayout.EndArea();
                }
                else if (selection is ItemSelect item)
                {
                    string title = item.Item.Key;
                    GUIContent inspectorTitle = EditorGUIUtility.TrTextContentWithIcon($" INSPECTOR ({title})", "PrefabVariant On Icon");
                    EditorDrawing.DrawHeaderWithBorder(ref inspectorRect, inspectorTitle, 20f, false);

                    Rect inspectorViewRect = inspectorRect;
                    inspectorViewRect.y += Spacing;
                    inspectorViewRect.yMax -= Spacing;
                    inspectorViewRect.xMin += Spacing;
                    inspectorViewRect.xMax -= Spacing;

                    GUILayout.BeginArea(inspectorViewRect);
                    OnDrawSectionItemInspector(item);
                    GUILayout.EndArea();
                }
            }
        }
        
        
        private void OnDrawSectionInspector(SectionSelect section)
        {
            // section name change
            EditorGUI.BeginChangeCheck();
            {
                section.Section.Name = EditorGUILayout.TextField("Name", section.Section.Name);
            }
            if (EditorGUI.EndChangeCheck())
            {
                section.TreeViewItem.displayName = section.Section.Name;
            }

            using (new EditorGUI.DisabledGroupScope(true))
            {
                int childerCount = section.TreeViewItem.children?.Count ?? 0;
                EditorGUILayout.IntField(new GUIContent("Keys"), childerCount);
            }

            EditorGUILayout.Space(2);
            EditorDrawing.Separator();
            EditorGUILayout.Space(1);

            using (new EditorGUI.DisabledGroupScope(true))
            {
                EditorGUILayout.LabelField("Id: " + section.Section.Id, EditorStyles.miniBoldLabel);
            }
            EditorGUILayout.Space(2);
            
            //EditorDrawing.DrawHeader("Context","is used for translation accuracy (optional)");
            EditorGUILayout.HelpBox("Contect used for translation accuracy (optional)", MessageType.Info);
            EditorGUILayout.LabelField("Context: ");
            GUIStyle textAreaStyle = new GUIStyle(EditorStyles.textArea);
            textAreaStyle.wordWrap = true;
            float calculatedHeight = textAreaStyle.CalcHeight(new GUIContent(section.Section.Context), position.width - 20);
            
            float clampedHeight = Mathf.Clamp(calculatedHeight, 50, 100);
            //textAreaScroll = EditorGUILayout.BeginScrollView(textAreaScroll, GUILayout.Height(clampedHeight));
            section.Section.Context = EditorGUILayout.TextArea(section.Section.Context,GUILayout.Height(clampedHeight));
            //EditorGUILayout.EndScrollView();
        }

        private void OnDrawSectionItemInspector(ItemSelect item)
        {
            // item key change
            EditorGUI.BeginChangeCheck();
            {
                item.Item.Key = EditorGUILayout.TextField("Key", item.Item.Key);
            }
            if (EditorGUI.EndChangeCheck())
            {
                item.TreeViewItem.displayName = item.Item.Key;
            }

            EditorGUILayout.Space(2);
            EditorDrawing.Separator();
            EditorGUILayout.Space(1);

            using (new EditorGUI.DisabledGroupScope(true))
            {
                string parentName = item.Item.Parent.Name;
                string parentText = item.Item.Parent.Id + $" ({parentName})";
                EditorGUILayout.LabelField("Parent Id: " + parentText, EditorStyles.miniBoldLabel);
                EditorGUILayout.LabelField("Id: " + item.Item.Id, EditorStyles.miniBoldLabel);
            }
        }
        
        private void OnDrawLanguageInspector(LanguageSelect selection)
        {
            var language = selection.Language;
            var entry = language.Entry;
            var treeView = selection.TreeViewItem;

            using (new EditorDrawing.BorderBoxScope(false))
            {
                // language name change
                Rect nameRect = EditorGUILayout.GetControlRect();
                EditorGUI.BeginChangeCheck();
                {
                    nameRect = EditorGUI.PrefixLabel(nameRect, new GUIContent("Name"));
                    nameRect.xMax -= EditorGUIUtility.singleLineHeight + 2f;
                    entry.LanguageName = EditorGUI.TextField(nameRect, entry.LanguageName);
                }
                if (EditorGUI.EndChangeCheck())
                {
                    treeView.displayName = entry.LanguageName;
                }

                Rect renameAssetRect = nameRect;
                renameAssetRect.xMin = nameRect.xMax + 2f;
                renameAssetRect.width = EditorGUIUtility.singleLineHeight;

                using (new EditorGUI.DisabledGroupScope(entry.Asset == null))
                {
                    GUIContent editIcon = EditorGUIUtility.IconContent("editicon.sml", "Rename");
                    if (GUI.Button(renameAssetRect, editIcon, EditorStyles.iconButton))
                    {
                        string assetPath = AssetDatabase.GetAssetPath(entry.Asset);
                        string newName = "(Language) " + entry.LanguageName;
                        AssetDatabase.RenameAsset(assetPath, newName);
                    }
                }

                // language asset change
                EditorGUI.BeginChangeCheck();
                {
                    entry.Asset = (LocalizationLanguage)EditorGUILayout.ObjectField("Asset", entry.Asset, typeof(LocalizationLanguage), false);
                }
                if (EditorGUI.EndChangeCheck())
                {
                    windowData.AssignLanguage(language, entry.Asset);
                    if(entry.Asset != null)
                        treeView.displayName = entry.Asset.LanguageName;
                }

                // draw create asset button if needed
                if(entry.Asset == null)
                {
                    if (GUILayout.Button("Create Asset"))
                    {
                        string tablePath = AssetDatabase.GetAssetPath(localizationTable);
                        if (!string.IsNullOrEmpty(tablePath))
                        {
                            string directoryPath = Path.GetDirectoryName(tablePath);

                            // Create a new instance of LocalizationLanguage
                            LocalizationLanguage newAsset = ScriptableObject.CreateInstance<LocalizationLanguage>();
                            string uniquePath = Path.Combine(directoryPath, "NewLocalizationLanguage.asset");
                            string assetPath = AssetDatabase.GenerateUniqueAssetPath(uniquePath);

                            // Save the asset
                            AssetDatabase.CreateAsset(newAsset, assetPath);
                            AssetDatabase.SaveAssets();
                            AssetDatabase.Refresh();

                            // Assign the created asset to the entry
                            entry.Asset = newAsset;

                            Debug.Log($"LocalizationLanguage asset created at: {assetPath}");
                        }
                        else
                        {
                            Debug.LogError("Unable to determine the path of the LocalizationTable ScriptableObject.");
                        }
                    }
                }
                else
                {
                    Rect wordCountRect = EditorGUILayout.GetControlRect();
                    using (new EditorGUI.DisabledGroupScope(true))
                    {
                        wordCountRect = EditorGUI.PrefixLabel(wordCountRect, new GUIContent("String Count"));
                        //wordCountRect.xMax -= EditorGUIUtility.singleLineHeight + 2f;
                        EditorGUI.LabelField(wordCountRect,new GUIContent($"{entry.Asset.Strings.Count} ({entry.Asset.TranslatePercent.ToString("F1")}%)"));
                    }

                    Rect buttonTranslateAllRect = wordCountRect;
                    buttonTranslateAllRect.xMin = wordCountRect.xMax - 100;

                    if (windowData.DefaultLanguage != null)
                    {
                        if (windowData.DefaultLanguage.Entry.LanguageName == entry.LanguageName)
                        {
                            buttonTranslateAllRect.xMin -= 150;
                            EditorGUI.HelpBox(buttonTranslateAllRect,"The default language cannot be translated",MessageType.Info);
                        }
                        else
                        {
                            using (new EditorGUI.DisabledGroupScope(false))
                            {
                                if (GUI.Button(buttonTranslateAllRect, new GUIContent("Translate All")))
                                {
                                    Debug.LogError("Translate All");
                                    var defaultEntry = windowData.DefaultLanguage.Entry;
                                    if (defaultEntry.Asset.TranslatePercent < 99.9f)
                                        EditorUtility.DisplayDialog("Attention",
                                            $"The default language [{defaultEntry.LanguageName}] must be 100% completed for a full translation.\n Current progress {defaultEntry.Asset.TranslatePercent.ToString("F1")} %",
                                            "OK");
                                    else
                                    {
                                        isLoading = true;
                                        _progressSimulator = new ProgressSimulator(statusMessages,0.25f);
                                        _progressSimulator.OnStatusChanged += () => Repaint();
                                        _progressSimulator.Start();
                                        LocalizationGptAPI.Translate(windowData.DefaultLanguage, language , () =>
                                        {
                                            Repaint();
                                            _progressSimulator.Stop();
                                            isLoading = false;
                                        });
                                    }
                                }
                            }

                            
                            buttonTranslateAllRect.x = buttonTranslateAllRect.xMin - 105;
                            buttonTranslateAllRect.width -= 5;
                            using (new EditorGUI.DisabledGroupScope(false))
                            {
                                if (GUI.Button(buttonTranslateAllRect, new GUIContent("Refresh")))
                                {
                                    Debug.LogError("Refresh translation");
                                }
                            }
                        }
                    }
                    else
                    {
                        if (GUI.Button(buttonTranslateAllRect, new GUIContent("Set Default")))
                        {
                            windowData.DefaultLanguage = language;
                            EditorPrefs.SetString(LocalizationConstant.DefaultLanguage,entry.LanguageName);
                        }
                    }
                    //EditorGUI.DrawRect(buttonTranslateAllRect,Color.cyan);
                }
            }

            using (new EditorGUI.DisabledGroupScope(entry.Asset == null))
            {
                // Draw search field
                EditorGUILayout.Space();

                GUIContent expandText = new GUIContent("Expand");
                float expandWidth = miniLabelButton.CalcSize(expandText).x;

                var searchRect = EditorGUILayout.GetControlRect();
                searchRect.xMax -= (expandWidth + 2f);
                searchString = searchField.OnGUI(searchRect, searchString);

                Rect expandRect = new Rect(searchRect.xMax + 2f, searchRect.y, expandWidth, searchRect.height);
                expandRect.y -= 1f;

                using (new EditorDrawing.BackgroundColorScope("#F7E987"))
                {
                    if (GUI.Button(expandRect, expandText, miniLabelButton))
                    {
                        globalExpanded = !globalExpanded;
                        foreach (var section in language.TableSheet)
                        {
                            section.Reference.IsExpanded = globalExpanded;
                        }
                    }
                }

                if (entry.Asset != null)
                {
                    // Draw localization data
                    scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
                    {
                        foreach (var section in GetSearchResult(language, searchString))
                        {
                            DrawLocalizationKey(section,language);
                        }
                    }
                    EditorGUILayout.EndScrollView();
                }
                else
                {
                    EditorGUILayout.HelpBox("To begin editing localization data, you must first assign a localization asset.", MessageType.Warning);
                }
            }
        }
        
        float percent = 0;
        private void DrawLocalizationKey(TempSheetSection section,TempLanguageData language)
        {
            if (section.Items == null || section.Items.Count == 0)
                return;

            using (new EditorDrawing.BorderBoxScope(false))
            {
                string sectionName = section.Name.Replace(" ", "");
                EditorGUILayout.BeginHorizontal();
                Rect rect = EditorGUILayout.GetControlRect();
                //section.Reference.IsExpanded = EditorGUILayout.Foldout(section.Reference.IsExpanded, new GUIContent(sectionName), true, EditorDrawing.Styles.miniBoldLabelFoldout);
                section.Reference.IsExpanded = EditorGUI.Foldout(new Rect(rect.x, rect.y, rect.width - 240, rect.height), section.Reference.IsExpanded, new GUIContent(sectionName), true, EditorDrawing.Styles.miniBoldLabelFoldout);

                if (GUI.Button(new Rect(rect.x + rect.width - 155, rect.y, 70, rect.height), "Refresh"))
                {
                    var defaultEntry = language.Entry.Asset.Strings
                        .Where(x => x.SectionId == section.Id).ToList();
                    percent = (defaultEntry.Count(x => !String.IsNullOrEmpty(x.value))) / defaultEntry.Count;
                    Debug.LogError("Refresh " + percent.ToString("P"));
                }

                EditorGUI.LabelField(new Rect(rect.x + rect.width - 240, rect.y, 80, rect.height),
                    new GUIContent(percent.ToString("P")));
                
                if (windowData.DefaultLanguage != language)
                {

                    if (GUI.Button(new Rect(rect.x + rect.width - 80, rect.y, 80, rect.height), "Translate"))
                    {
                        var defaultEntry = windowData.DefaultLanguage.Entry.Asset.Strings
                            .Where(x => x.SectionId == section.Id).ToList();
                        percent = (defaultEntry.Count(x => !String.IsNullOrEmpty(x.value))) / defaultEntry.Count;

                        if (percent < 0.99f)
                        {
                            EditorUtility.DisplayDialog("Attention",
                                $"The Section {sectionName} by default language [{windowData.DefaultLanguage.Entry.LanguageName}] must be 100% completed for a full translation.\n Current Section progress {percent.ToString("P")}",
                                "OK");
                            return;
                        }
                        else
                        {
                            isLoading = true;
                            _progressSimulator = new ProgressSimulator(statusMessages, 0.25f);
                            _progressSimulator.OnStatusChanged += () => Repaint();
                            _progressSimulator.Start();

                            LocalizationGptAPI.TranslateSection(windowData.DefaultLanguage, language, section.Id, () =>
                            {
                                Repaint();
                                _progressSimulator.Stop();
                                isLoading = false;
                            });

                        }
                    }
                }

                EditorGUILayout.EndHorizontal();
                // Show section keys when expanded
                if (section.Reference.IsExpanded)
                {
                    foreach (var item in section.Items)
                    {
                        string keyName = item.Key.Replace(" ", "");
                        string key = sectionName + "." + keyName;

                        if (IsMultiline(item.Value))
                            key += " (Multiline)";

                        using (new EditorGUILayout.VerticalScope(GUI.skin.box))
                        {
                            // Display the expandable toggle
                            item.IsExpanded = EditorGUILayout.Foldout(item.IsExpanded, new GUIContent(key), true, EditorDrawing.Styles.miniBoldLabelFoldout);

                            if (item.IsExpanded)
                            {
                                // Show TextArea when expanded
                                float height = (EditorGUIUtility.standardVerticalSpacing + EditorGUIUtility.singleLineHeight) * 3;
                                height += EditorGUIUtility.standardVerticalSpacing;

                                item.Scroll = EditorGUILayout.BeginScrollView(item.Scroll, GUILayout.Height(height));
                                item.Value = EditorGUILayout.TextArea(item.Value, GUILayout.ExpandHeight(true));
                                EditorGUILayout.EndScrollView();
                            }
                            else
                            {
                                // Show TextField when collapsed
                                item.Value = EditorGUILayout.TextField(item.Value);
                            }
                        }
                    }
                }
            }

            EditorGUILayout.Space(1f);
        }
        
        private bool IsMultiline(string text)
        {
            return text.Contains("\n") || text.Contains("\r");
        }
        private IEnumerable<TempSheetSection> GetSearchResult(TempLanguageData languageData, string search)
        {
            if (!string.IsNullOrEmpty(search))
            {
                List<TempSheetSection> searchResult = new();

                foreach (var section in languageData.TableSheet)
                {
                    List<TempSheetItem> sectionItems = new();
                    string sectionName = section.Name.Replace(" ", "");

                    foreach (var item in section.Items)
                    {
                        string keyName = item.Key.Replace(" ", "");
                        string key = sectionName + "." + keyName;

                        if (key.Contains(search))
                            sectionItems.Add(item);
                    }

                    searchResult.Add(new TempSheetSection()
                    {
                        Items = sectionItems,
                        Reference = section.Reference
                    });
                }

                return searchResult;
            }

            return languageData.TableSheet;
        }
        
        private void BuildLocalizationTable()
        {
            // 1. build table sheet
            localizationTable.TableSheet = new();
            
            foreach (var section in windowData.TableSheet)
            {
                LocalizationTable.TableData tableData = new LocalizationTable.TableData(section.Name,section.Context, section.Id);

                foreach (var item in section.Items)
                {
                    LocalizationTable.SheetItem sheetItem = new LocalizationTable.SheetItem(item.Key, item.Id);
                    tableData.SectionSheet.Add(sheetItem);
                }

                localizationTable.TableSheet.Add(tableData);
            }

            // 2. build table sheet for each language
            IList<LocalizationLanguage> languages = new List<LocalizationLanguage>();
            foreach (var language in windowData.Languages)
            {
                if (language.Entry.Asset == null)
                    continue;

                LocalizationLanguage asset = language.Entry.Asset;
                IList<LocalizationString> strings = new List<LocalizationString>();
                
                foreach (var section in language.TableSheet)
                {
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
                }

                // assign name and localization strings to language asset
                asset.LanguageName = language.Entry.LanguageName;
                asset.Strings = new(strings);

                languages.Add(asset);
                EditorUtility.SetDirty(asset);
            }

            // 3. assign languages to localization table
            localizationTable.Languages = new(languages);
            
            //4. update percent translate string
            foreach (var languadeData in localizationTable.Languages)
            {
                languadeData.UpdatePercent();
            }
        }
    }
}