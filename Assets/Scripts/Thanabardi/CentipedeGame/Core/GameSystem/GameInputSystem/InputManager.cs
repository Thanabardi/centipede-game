using Thanabardi.Generic.Utility;

namespace Thanabardi.CentipedeGame.Core.GameSystem.GameInputSystem
{
    public class InputManager : MonoSingleton<InputManager>
    {
        public InputSystem InputSystem { get; private set; }

        public override void Awake()
        {
            base.Awake();
            InputSystem = new();
        }
    }
}
