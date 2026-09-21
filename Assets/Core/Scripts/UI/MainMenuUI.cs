using Platformer.SaveLoad;
using UnityEngine;
using UnityEngine.UI;

namespace Platformer.GameFlow
{
    public class MainMenuUI : MonoBehaviour
    {
        [SerializeField] private Button _newGameButton;
        [SerializeField] private Button _continueButton;
        [SerializeField] private Button _quitButton;

        private void Start()
        {
            AddButtonListeners();

            if (_continueButton != null)
            {
                if (!SaveLoadManager.HasSave()) return;

                SaveData data = SaveLoadManager.Load();
                _continueButton.interactable = !data.isLevelCompleted;
            }
        }

        private void OnDestroy()
        {
            RemoveButtonListeners();
        }

        private void AddButtonListeners()
        {
            _newGameButton.onClick.AddListener(OnNewGame);
            _continueButton.onClick.AddListener(OnContinue);
            _quitButton.onClick.AddListener(OnQuit);
        }

        private void RemoveButtonListeners()
        {
            _newGameButton.onClick.RemoveListener(OnNewGame);
            _continueButton.onClick.RemoveListener(OnContinue);
            _quitButton.onClick.RemoveListener(OnQuit);
        }

        private void OnNewGame()
        {
            GameManager.Instance.StartNewGame();
        }

        private void OnContinue()
        {
            GameManager.Instance.ContinueGame();
        }

        private void OnQuit()
        {
            Application.Quit();
        }
    }
}