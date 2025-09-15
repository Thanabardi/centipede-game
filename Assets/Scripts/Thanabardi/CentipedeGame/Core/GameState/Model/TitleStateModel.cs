using Thanabardi.Generic.Core.StateSystem;
using Thanabardi.CentipedeGame.Core.GameSystem;
using Thanabardi.CentipedeGame.Core.GameSystem.GameInputSystem;
using Thanabardi.CentipedeGame.Core.UI;

namespace Thanabardi.CentipedeGame.Core.GameState.Model
{
    public class TitleStateModel : StateModel
    {
        private TitlePanel _titelPanel;

        public TitleStateModel() : base((int)GameStates.State.Title, nameof(TitleStateModel)) { }

        public override void OnStateIn()
        {
            base.OnStateIn();

            _titelPanel = (TitlePanel)UIManager.Instance.SetPanelActive(UIManager.UIKey.TitlePanel, true);

            InputManager.Instance.InputSystem.UI.Enable();
        }

        public override void OnStateOut()
        {
            base.OnStateOut();

            UIManager.Instance.SetPanelActive(UIManager.UIKey.TitlePanel, false);

            InputManager.Instance.InputSystem.UI.Disable();
        }
    }
}