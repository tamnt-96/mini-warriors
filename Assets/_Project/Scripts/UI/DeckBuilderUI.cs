using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using TWR.Core;
using TWR.Meta;
using TWR.Save;
using CoreKit.UI;

namespace TWR.UI
{
    public class DeckBuilderUI : BasePanel
    {
        [Header("Deck Slots")]
        [SerializeField] Button[]   _deckSlotButtons;
        [SerializeField] TMP_Text[] _deckSlotLabels;

        [Header("Warrior Roster")]
        [SerializeField] Transform _rosterContainer;
        [SerializeField] Button    _rosterEntryPrefab;

        readonly List<Button> _rosterEntries = new();

        DeckBuilderSystem _deckBuilder;
        ProgressService   _progress;

        protected override void OnShow()
        {
            ServiceLocator.TryGet(out _deckBuilder);
            ServiceLocator.TryGet(out _progress);
            Refresh();
        }

        void Refresh()
        {
            RefreshDeckSlots();
            RefreshRoster();
        }

        void RefreshDeckSlots()
        {
            if (_deckBuilder == null) return;

            var ids = _deckBuilder.WarriorIds;
            for (int i = 0; i < _deckSlotButtons.Length; i++)
            {
                bool filled = i < ids.Count;
                if (_deckSlotLabels != null && i < _deckSlotLabels.Length)
                    _deckSlotLabels[i].text = filled ? ids[i] : "Empty";

                int captured = i;
                _deckSlotButtons[i].onClick.RemoveAllListeners();
                if (filled)
                {
                    string warriorId = ids[captured];
                    _deckSlotButtons[i].onClick.AddListener(() =>
                    {
                        _deckBuilder.UnequipWarrior(warriorId);
                        Refresh();
                    });
                }
            }
        }

        void RefreshRoster()
        {
            if (_rosterContainer == null || _rosterEntryPrefab == null) return;

            foreach (var btn in _rosterEntries)
                Destroy(btn.gameObject);
            _rosterEntries.Clear();

            if (_progress == null) return;

            foreach (var ws in _progress.Data.ownedWarriors)
            {
                if (!ws.isOwned) continue;

                var entry = Instantiate(_rosterEntryPrefab, _rosterContainer);
                var label = entry.GetComponentInChildren<TMP_Text>();
                if (label != null)
                {
                    bool inDeck = IsInDeck(ws.warriorId);
                    label.text = inDeck ? $"{ws.warriorId} [IN DECK]" : ws.warriorId;
                }

                string id = ws.warriorId;
                entry.onClick.AddListener(() =>
                {
                    if (IsInDeck(id))
                        _deckBuilder.UnequipWarrior(id);
                    else
                        _deckBuilder.EquipWarrior(id);
                    Refresh();
                });

                _rosterEntries.Add(entry);
            }
        }

        bool IsInDeck(string warriorId)
        {
            if (_deckBuilder == null) return false;
            foreach (var id in _deckBuilder.WarriorIds)
                if (id == warriorId) return true;
            return false;
        }
    }
}
