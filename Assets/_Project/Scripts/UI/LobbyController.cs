using TMPro;
using UnityEngine;
using TWR.Core;
using TWR.Localization;
using TWR.Save;
using CoreKit.UI;

namespace TWR.UI
{
    public class LobbyController : BasePanel
    {
        [SerializeField] TMP_Text      _goldLabel;
        [SerializeField] TMP_Text      _gemsLabel;
        [SerializeField] DeckBuilderUI _deckBuilderPanel;
        [SerializeField] ShopUI        _shopPanel;

        protected override void OnShow()
        {
            if (!ServiceLocator.TryGet<ProgressService>(out var progress)) return;

            SeedStarterDataIfNeeded(progress);

            if (ServiceLocator.TryGet<LocalizationManager>(out var localization))
            {
                if (_goldLabel != null) _goldLabel.text = localization.Get("ui.lobby.gold", progress.Data.gold);
                if (_gemsLabel != null) _gemsLabel.text = localization.Get("ui.lobby.gems", progress.Data.gems);
            }
            else
            {
                if (_goldLabel != null) _goldLabel.text = $"Gold: {progress.Data.gold}";
                if (_gemsLabel != null) _gemsLabel.text = $"Gems: {progress.Data.gems}";
            }
        }

        static readonly string[] StarterWarriorIds = { "infantry", "cavalry", "ranged", "magic" };

        void SeedStarterDataIfNeeded(ProgressService progress)
        {
            if (progress.Data.activeDeck.warriorIds.Length > 0) return;

            foreach (var warriorId in StarterWarriorIds)
            {
                bool owned = false;
                foreach (var ws in progress.Data.ownedWarriors)
                {
                    if (ws.warriorId == warriorId) { owned = true; break; }
                }

                if (!owned)
                {
                    progress.Data.ownedWarriors.Add(new WarriorSaveData
                    {
                        warriorId = warriorId,
                        isOwned   = true,
                        starLevel = 1,
                        pieces    = 0
                    });
                }
            }

            progress.Data.activeDeck.warriorIds = StarterWarriorIds;
            progress.Save();
        }

        public void OnPlayPressed()
        {
            EventBus<ChangeStateEvent>.Publish(new ChangeStateEvent { stateType = typeof(GameState_Play) });
        }

        public void OnDeckBuilderPressed()
        {
            UIManager.Instance?.ShowPanel<DeckBuilderUI>();
        }

        public void OnShopPressed()
        {
            UIManager.Instance?.ShowPanel<ShopUI>();
        }

        public void OnSettingsPressed()
        {
            UIManager.Instance?.ShowPanel<SettingsView>();
        }

        public void OnTroopUpgradePressed()
        {
            UIManager.Instance?.ShowPanel<TroopUpgradeUI>();
        }
    }
}
