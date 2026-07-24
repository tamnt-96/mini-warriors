using TMPro;
using UnityEngine;
using TWR.Core;
using TWR.Meta;
using TWR.Save;
using CoreKit.UI;

namespace TWR.UI
{
    public class ShopUI : BasePanel
    {
        [SerializeField] TMP_Text _keysLabel;
        [SerializeField] TMP_Text _resultLabel;

        GachaSystem     _gacha;
        ProgressService _progress;

        protected override void OnShow()
        {
            ServiceLocator.TryGet(out _gacha);
            ServiceLocator.TryGet(out _progress);
            RefreshKeys();
            if (_resultLabel != null) _resultLabel.gameObject.SetActive(false);
        }

        public void OnOpenChestPressed()
        {
            if (_gacha == null) return;

            var result = _gacha.OpenChest();
            if (result == null)
            {
                ShowResult("Not enough keys!");
                return;
            }

            RefreshKeys();

            string msg = $"+{result.piecesGained} {result.warriorId} pieces";
            if (result.newlyOwned) msg += "\nNEW WARRIOR UNLOCKED!";
            ShowResult(msg);
        }

        void RefreshKeys()
        {
            if (_keysLabel != null && _progress != null)
                _keysLabel.text = $"Keys: {_progress.Data.keys}";
        }

        void ShowResult(string message)
        {
            if (_resultLabel == null) return;
            _resultLabel.text = message;
            _resultLabel.gameObject.SetActive(true);
        }
    }
}
