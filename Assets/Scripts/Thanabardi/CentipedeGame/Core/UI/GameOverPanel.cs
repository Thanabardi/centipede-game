using TMPro;
using UnityEngine;
using Thanabardi.CentipedeGame.Utility.UI;
using Thanabardi.CentipedeGame.Core.GameState;
using Thanabardi.CentipedeGame.Core.GameSystem;
using Thanabardi.CentipedeGame.Core.Interface;

namespace Thanabardi.CentipedeGame.Core.UI
{
    public class GameOverPanel : MonoBehaviour, IUIPanel
    {
        [SerializeField]
        private TextMeshProUGUI _scoreText;

        [SerializeField]
        private ButtonUtility _titleButton;

        public void SetActive(bool isActive)
        {
            gameObject.SetActive(isActive);

            _titleButton.Button.Select();
        }

        private void OnEnable()
        {
            _titleButton.OnPointerClickButton += OnReturnTitleHandler;

            _scoreText.SetText(GameManager.Instance.Score.ToString());
        }

        private void OnDisable()
        {
            _titleButton.OnPointerClickButton -= OnReturnTitleHandler;
        }

        private void OnReturnTitleHandler()
        {
            // go to title state
            GameStateManager.Instance.GoToState((int)GameStates.State.Title);
        }
    }
}