using UnityEngine.InputSystem;
using Thanabardi.Generic.Core.StateSystem;
using Thanabardi.CentipedeGame.Core.GameSystem;
using Thanabardi.CentipedeGame.Core.GameSystem.GameInputSystem;
using Thanabardi.CentipedeGame.Core.UI;

namespace Thanabardi.CentipedeGame.Core.GameState.Model
{
    public class GameOverStateModel : StateModel
    {
        private GameOverPanel _gameOverPanel;

        public GameOverStateModel() : base((int)GameStates.State.GameOver, nameof(GameOverStateModel)) { }

        public override void OnStateIn()
        {
            base.OnStateIn();

            _gameOverPanel = (GameOverPanel)UIManager.Instance.SetPanelActive(UIManager.UIKey.GameOverPanel, true);

            InputManager.Instance.InputSystem.UI.Enable();

            InputManager.Instance.InputSystem.UI.Continue.performed += OnRestartHandler;
        }

        public override void OnStateOut()
        {
            base.OnStateOut();

            UIManager.Instance.SetPanelActive(UIManager.UIKey.GameOverPanel, false);

            InputManager.Instance.InputSystem.UI.Disable();

            InputManager.Instance.InputSystem.UI.Continue.performed -= OnRestartHandler;
        }

        private void OnRestartHandler(InputAction.CallbackContext context)
        {
            GameManager.Instance.StartGame(GameManager.Instance.IsEndless);
            GameStateManager.Instance.GoToState((int)GameStates.State.GamePlay);
        }
    }
}