using UnityEngine.InputSystem;
using Thanabardi.Generic.Core.StateSystem;
using Thanabardi.CentipedeGame.Core.GameSystem;
using Thanabardi.CentipedeGame.Core.GameSystem.GameInputSystem;
using Thanabardi.CentipedeGame.Core.UI;

namespace Thanabardi.CentipedeGame.Core.GameState.Model
{
    public class GamePauseStateModel : StateModel
    {
        private PausePanel _pausePanel;

        public GamePauseStateModel() : base((int)GameStates.State.GamePause, nameof(GamePauseStateModel)) { }

        public override void OnStateIn()
        {
            base.OnStateIn();

            _pausePanel = (PausePanel)UIManager.Instance.SetPanelActive(UIManager.UIKey.PausePanel, true);

            InputManager.Instance.InputSystem.UI.Enable();

            InputManager.Instance.InputSystem.UI.Cancel.performed += OnCancelHandler;
        }

        public override void OnStateOut()
        {
            base.OnStateOut();

            InputManager.Instance.InputSystem.UI.Disable();

            InputManager.Instance.InputSystem.UI.Cancel.performed -= OnCancelHandler;

            UIManager.Instance.SetPanelActive(UIManager.UIKey.PausePanel, false);
        }

        private void OnCancelHandler(InputAction.CallbackContext context)
        {
            GameStateManager.Instance.GoToState((int)GameStates.State.GamePlay);
        }
    }
}

