using UnityEngine;
using Thanabardi.CentipedeGame.Utility.UI;
using Thanabardi.CentipedeGame.Core.GameState;
using Thanabardi.CentipedeGame.Core.GameSystem;
using Thanabardi.CentipedeGame.Core.Interface;

namespace Thanabardi.CentipedeGame.Core.UI
{
    public class PausePanel : MonoBehaviour, IUIPanel
    {
        [SerializeField]
        private ButtonUtility _resumeButton;
        [SerializeField]
        private ButtonUtility _restartButton;
        [SerializeField]
        private ButtonUtility _titleButton;

        public void SetActive(bool isActive)
        {
            gameObject.SetActive(isActive);

            _resumeButton.Button.Select();
        }

        private void OnEnable()
        {
            _resumeButton.OnPointerClickButton += OnResumeHandler;
            _restartButton.OnPointerClickButton += OnRestartHandler;
            _titleButton.OnPointerClickButton += OnReturnTitleHandler;
        }

        private void OnDisable()
        {
            _resumeButton.OnPointerClickButton -= OnResumeHandler;
            _restartButton.OnPointerClickButton -= OnRestartHandler;
            _titleButton.OnPointerClickButton -= OnReturnTitleHandler;
        }

        private void OnRestartHandler()
        {
            GameManager.Instance.StartGame(GameManager.Instance.IsEndless);
            GameStateManager.Instance.GoToState((int)GameStates.State.GamePlay);
        }

        private void OnResumeHandler()
        {
            GameStateManager.Instance.GoToState((int)GameStates.State.GamePlay);
        }

        private void OnReturnTitleHandler()
        {
            GameStateManager.Instance.GoToState((int)GameStates.State.Title);
        }
    }
}