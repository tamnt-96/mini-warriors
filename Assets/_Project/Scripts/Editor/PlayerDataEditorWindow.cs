using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using TWR.Core;
using TWR.Data;
using TWR.Meta;
using TWR.Save;

namespace TWR.Core.EditorTools
{
    public class PlayerDataEditorWindow : EditorWindow
    {
        PlayerDataProxy   _proxy;
        SerializedObject  _serializedProxy;
        SerializedProperty _progressProperty;
        bool              _liveModeActive;
        Vector2           _scrollPosition;

        [MenuItem("Tools/Mini Warriors/Player Data Editor")]
        public static void OpenWindow()
        {
            var window = GetWindow<PlayerDataEditorWindow>("Player Data");
            window.minSize = new Vector2(360, 400);
        }

        void OnEnable()
        {
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
            LoadData();
        }

        void OnDisable()
        {
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            if (_proxy != null) DestroyImmediate(_proxy);
        }

        void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.EnteredEditMode ||
                state == PlayModeStateChange.EnteredPlayMode)
            {
                LoadData();
                Repaint();
            }
        }

        void LoadData()
        {
            if (_proxy != null) DestroyImmediate(_proxy);

            _proxy = ScriptableObject.CreateInstance<PlayerDataProxy>();
            _proxy.hideFlags = HideFlags.DontSave;

            _liveModeActive = EditorApplication.isPlaying &&
                              ServiceLocator.TryGet<ProgressService>(out _);

            if (_liveModeActive)
            {
                ServiceLocator.TryGet<ProgressService>(out var progress);
                _proxy.progress = JsonUtility.FromJson<PlayerProgress>(JsonUtility.ToJson(progress.Data));
            }
            else
            {
                _proxy.progress = new SaveSystem().Load();
            }

            _serializedProxy  = new SerializedObject(_proxy);
            _progressProperty = _serializedProxy.FindProperty(nameof(PlayerDataProxy.progress));
        }

        void OnGUI()
        {
            if (_proxy == null) LoadData();

            DrawToolbar();
            GUILayout.Space(6);
            DrawModeBanner();
            GUILayout.Space(6);

            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

            _serializedProxy.Update();
            EditorGUILayout.PropertyField(_progressProperty, true);
            bool changed = _serializedProxy.ApplyModifiedProperties();

            if (changed && _liveModeActive)
                SyncToLiveInstance();

            GUILayout.Space(6);
            DrawDeckCapWarnings();

            EditorGUILayout.EndScrollView();
        }

        void DrawToolbar()
        {
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Reload")) LoadData();
            if (GUILayout.Button("Save")) SaveData();
            if (GUILayout.Button("Reset to Default")) ResetToDefault();
            EditorGUILayout.EndHorizontal();

            if (GUILayout.Button("Grant All Warriors + Setup Deck"))
                GrantAllWarriorsAndSetupDeck();
        }

        void DrawModeBanner()
        {
            bool isPlaying = EditorApplication.isPlaying;

            if (!isPlaying)
            {
                EditorGUILayout.HelpBox("EDIT MODE — editing player_progress.json on disk.", MessageType.Info);
            }
            else if (_liveModeActive)
            {
                EditorGUILayout.HelpBox("PLAY MODE — editing live ProgressService.Data. Save persists to disk.", MessageType.Info);
            }
            else
            {
                EditorGUILayout.HelpBox("PLAY MODE — ProgressService not found yet. Falling back to editing the file on disk.", MessageType.Warning);
            }
        }

        void DrawDeckCapWarnings()
        {
            var deck = _proxy.progress.activeDeck;

            if (deck.warriorIds != null && deck.warriorIds.Length > DeckBuilderSystem.MaxWarriorSlots)
                EditorGUILayout.HelpBox(
                    $"activeDeck.warriorIds has {deck.warriorIds.Length} entries, exceeding the max of {DeckBuilderSystem.MaxWarriorSlots}.",
                    MessageType.Warning);

            if (deck.skillIds != null && deck.skillIds.Length > DeckBuilderSystem.MaxSkillSlots)
                EditorGUILayout.HelpBox(
                    $"activeDeck.skillIds has {deck.skillIds.Length} entries, exceeding the max of {DeckBuilderSystem.MaxSkillSlots}.",
                    MessageType.Warning);
        }

        void SyncToLiveInstance()
        {
            if (!ServiceLocator.TryGet<ProgressService>(out var progress)) return;
            JsonUtility.FromJsonOverwrite(JsonUtility.ToJson(_proxy.progress), progress.Data);
        }

        void SaveData()
        {
            if (_liveModeActive && ServiceLocator.TryGet<ProgressService>(out var progress))
            {
                SyncToLiveInstance();
                progress.Save();
            }
            else
            {
                new SaveSystem().Save(_proxy.progress);
            }
        }

        void ResetToDefault()
        {
            _proxy.progress = PlayerProgress.Default();
            _serializedProxy  = new SerializedObject(_proxy);
            _progressProperty = _serializedProxy.FindProperty(nameof(PlayerDataProxy.progress));
        }

        void GrantAllWarriorsAndSetupDeck()
        {
            var defs     = Resources.LoadAll<WarriorDefinitionSO>("Warriors");
            var progress = _proxy.progress;

            foreach (var def in defs)
            {
                if (def == null || string.IsNullOrEmpty(def.warriorId)) continue;

                var existing = progress.ownedWarriors.Find(w => w.warriorId == def.warriorId);
                if (existing != null)
                {
                    existing.isOwned = true;
                }
                else
                {
                    progress.ownedWarriors.Add(new WarriorSaveData
                    {
                        warriorId = def.warriorId,
                        isOwned   = true,
                        starLevel = 1,
                        pieces    = 0
                    });
                }
            }

            var deckIds = new List<string>();
            foreach (var def in defs)
            {
                if (deckIds.Count >= DeckBuilderSystem.MaxWarriorSlots) break;
                if (def == null || string.IsNullOrEmpty(def.warriorId)) continue;
                deckIds.Add(def.warriorId);
            }
            progress.activeDeck.warriorIds = deckIds.ToArray();

            _serializedProxy  = new SerializedObject(_proxy);
            _progressProperty = _serializedProxy.FindProperty(nameof(PlayerDataProxy.progress));

            if (_liveModeActive) SyncToLiveInstance();
        }

        class PlayerDataProxy : ScriptableObject
        {
            public PlayerProgress progress = new();
        }
    }
}
