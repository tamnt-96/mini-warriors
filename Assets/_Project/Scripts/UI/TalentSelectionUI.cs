using TMPro;
using UnityEngine;
using UnityEngine.UI;
using TWR.Battle;
using CoreKit.UI;

namespace TWR.UI
{
    public class TalentSelectionUI : BasePanel
    {
        [SerializeField] TalentSystem _talentSystem;
        [SerializeField] Button[]     _talentButtons;
        [SerializeField] TMP_Text[]   _talentLabels;

        protected override void OnShow()
        {
            RefreshCards();
        }

        void RefreshCards()
        {
            var options = _talentSystem.CurrentOptions;
            for (int i = 0; i < _talentButtons.Length; i++)
            {
                bool hasOption = i < options.Count;
                _talentButtons[i].gameObject.SetActive(hasOption);
                if (!hasOption) continue;

                _talentLabels[i].text = options[i].isWarriorSelection
                    ? options[i].warrior.displayName
                    : options[i].talent.displayName;

                int captured = i;
                _talentButtons[i].onClick.RemoveAllListeners();
                _talentButtons[i].onClick.AddListener(() => OnCardSelected(captured));
            }
        }

        void OnCardSelected(int index)
        {
            _talentSystem.PickOption(index);
        }
    }
}
