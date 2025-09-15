using UnityEngine;
using Thanabardi.CentipedeGame.Utility.UI;
using Thanabardi.CentipedeGame.Core.GameState;
using Thanabardi.CentipedeGame.Core.GameSystem;
using Thanabardi.CentipedeGame.Core.Interface;

namespace Thanabardi.CentipedeGame.Core.UI
{
    public class TitlePanel : MonoBehaviour, IUIPanel
    {
        [SerializeField]
        private ButtonUtility _playButton;
        [SerializeField]
        private ButtonUtility _playEndlessButton;
        [SerializeField]
        private ButtonUtility _exitButton;

        public void SetActive(bool isActive)
        {
            gameObject.SetActive(isActive);

            // disable exit button when running on WebGL
            _exitButton.gameObject.SetActive(Application.platform != RuntimePlatform.WebGLPlayer);
            _playButton.Button.Select();
        }

        private void OnEnable()
        {
            _playButton.OnPointerClickButton += OnPlayHandler;
            _playEndlessButton.OnPointerClickButton += OnPlayEndlessHandler;
            _exitButton.OnPointerClickButton += OnExitHandler;
        }

        private void OnDisable()
        {
            _playButton.OnPointerClickButton -= OnPlayHandler;
            _playEndlessButton.OnPointerClickButton -= OnPlayEndlessHandler;
            _exitButton.OnPointerClickButton -= OnExitHandler;
        }

        private void OnPlayHandler()
        {
            GameManager.Instance.StartGame();
            GameStateManager.Instance.GoToState((int)GameStates.State.GamePlay);
        }

        private void OnPlayEndlessHandler()
        {
            // play endless mode
            GameManager.Instance.StartGame(true);
            GameStateManager.Instance.GoToState((int)GameStates.State.GamePlay);
        }

        private void OnExitHandler()
        {
            Application.Quit();
        }
    }
}