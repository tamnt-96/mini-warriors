using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using TWR.Core;
using TWR.Data;
using TWR.Meta;
using TWR.Save;
using CoreKit.UI;

namespace TWR.UI
{
    public class TroopUpgradeUI : BasePanel
    {
        [Header("Roster")]
        [SerializeField] Transform _rosterContainer;
        [SerializeField] Button    _rosterEntryPrefab;

        [Header("Selected Troop")]
        [SerializeField] TMP_Text _selectedNameLabel;
        [SerializeField] TMP_Text _selectedLevelLabel;
        [SerializeField] TMP_Text _selectedCostLabel;
        [SerializeField] Button   _upgradeButton;

        readonly List<Button> _rosterEntries = new();

        TroopUpgradeSystem _troopUpgrade;
        ProgressService    _progress;
        string             _selectedWarriorId;

        protected override void OnShow()
        {
            ServiceLocator.TryGet(out _troopUpgrade);
            ServiceLocator.TryGet(out _progress);
            _selectedWarriorId = null;
            Refresh();
        }

        void Refresh()
        {
            RefreshRoster();
            RefreshSelection();
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
                    label.text = $"{ws.warriorId}  Lv.{ws.level}";

                string id = ws.warriorId;
                entry.onClick.AddListener(() =>
                {
                    _selectedWarriorId = id;
                    RefreshSelection();
                });

                _rosterEntries.Add(entry);
            }
        }

        void RefreshSelection()
        {
            bool hasSelection = !string.IsNullOrEmpty(_selectedWarriorId) && _troopUpgrade != null;

            if (_selectedNameLabel != null)
                _selectedNameLabel.text = hasSelection ? _selectedWarriorId : "Select a troop";

            if (!hasSelection)
            {
                if (_selectedLevelLabel != null) _selectedLevelLabel.text = string.Empty;
                if (_selectedCostLabel != null) _selectedCostLabel.text = string.Empty;
                if (_upgradeButton != null)
                {
                    _upgradeButton.onClick.RemoveAllListeners();
                    _upgradeButton.interactable = false;
                }
                return;
            }

            int level = _troopUpgrade.GetLevel(_selectedWarriorId);
            if (_selectedLevelLabel != null)
                _selectedLevelLabel.text = $"Level {level} / {TroopUpgradeTableSO.MaxLevel}";

            bool isMax = _troopUpgrade.IsMaxLevel(_selectedWarriorId);
            bool hasCost = _troopUpgrade.TryGetUpgradeCost(_selectedWarriorId, out var cost) && !isMax;

            if (_selectedCostLabel != null)
                _selectedCostLabel.text = isMax
                    ? "MAX LEVEL"
                    : hasCost
                        ? $"Cost: {cost.goldCost} gold, {cost.fragmentCost} fragments"
                        : string.Empty;

            if (_upgradeButton != null)
            {
                _upgradeButton.onClick.RemoveAllListeners();
                _upgradeButton.interactable = !isMax && _troopUpgrade.CanUpgrade(_selectedWarriorId);
                _upgradeButton.onClick.AddListener(OnUpgradePressed);
            }
        }

        void OnUpgradePressed()
        {
            if (string.IsNullOrEmpty(_selectedWarriorId) || _troopUpgrade == null) return;
            if (!_troopUpgrade.TryUpgrade(_selectedWarriorId)) return;
            Refresh();
        }
    }
}
