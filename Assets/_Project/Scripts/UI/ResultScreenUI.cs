using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using CoreKit.UI;

namespace TWR.UI
{
    public class ResultScreenUI : BasePanel
    {
        [SerializeField] TMP_Text _resultLabel;

        public void Setup(string message)
        {
            if (_resultLabel != null) _resultLabel.text = message;
        }

        public void OnBackToLobby()
        {
            SceneManager.LoadScene("Lobby");
        }
    }
}
