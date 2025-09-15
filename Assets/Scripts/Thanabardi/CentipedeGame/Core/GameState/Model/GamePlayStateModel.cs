using UnityEngine.InputSystem;
using Thanabardi.Generic.Core.StateSystem;
using Thanabardi.CentipedeGame.Core.GameSystem;
using Thanabardi.CentipedeGame.Core.GameSystem.GameInputSystem;
using Thanabardi.CentipedeGame.Core.UI;

namespace Thanabardi.CentipedeGame.Core.GameState.Model
{
    public class GamePlayStateModel : StateModel
    {
        private HUDPanel _hudPanel;

        public GamePlayStateModel() : base((int)GameStates.State.GamePlay, nameof(GamePlayStateModel)) { }

        public override void OnStateIn()
        {
            base.OnStateIn();

            _hudPanel = (HUDPanel)UIManager.Instance.SetPanelActive(UIManager.UIKey.HUDPanel, true);

            InputManager.Instance.InputSystem.Player.Enable();
            InputManager.Instance.InputSystem.UI.Cancel.Enable();

            InputManager.Instance.InputSystem.UI.Cancel.performed += OnCancelHandler;

            GameManager.Instance.OnScoreUpdate += _hudPanel.SetScore;
            GameManager.Instance.OnLifeUpdate += _hudPanel.SetLife;

            // set active endless mode indicator
            _hudPanel.SetEndlessActive(GameManager.Instance.IsEndless);

            GameManager.Instance.OnGameStateInHandler();
        }

        public override void OnStateOut()
        {
            base.OnStateOut();

            UIManager.Instance.SetPanelActive(UIManager.UIKey.HUDPanel, false);

            InputManager.Instance.InputSystem.Player.Disable();
            InputManager.Instance.InputSystem.UI.Cancel.Disable();

            InputManager.Instance.InputSystem.UI.Cancel.performed -= OnCancelHandler;

            GameManager.Instance.OnScoreUpdate -= _hudPanel.SetScore;
            GameManager.Instance.OnLifeUpdate -= _hudPanel.SetLife;

            GameManager.Instance.OnGameStateOutInHandler();
        }

        private void OnCancelHandler(InputAction.CallbackContext context)
        {
            GameStateManager.Instance.GoToState((int)GameStates.State.GamePause);
        }
    }
}

