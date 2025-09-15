using Thanabardi.Generic.Core.StateSystem;

namespace Thanabardi.CentipedeGame.Core.GameState
{
    public class GameStateManager : StateManager<GameStateManager>
    {
        public void Start()
        {
            GameStates gameStates = new();
            Initialize((int)GameStates.State.Title, gameStates.GetGameStateModels());
        }
    }
}