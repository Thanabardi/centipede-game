using Thanabardi.Generic.Core.StateSystem;
using Thanabardi.CentipedeGame.Core.GameState.Model;

namespace Thanabardi.CentipedeGame.Core.GameState
{
    public class GameStates : States
    {
        private StateModel[] _states;

        public override StateModel[] GetGameStateModels()
        {
            _states ??= new StateModel[]
            {
                new TitleStateModel(),
                new GamePlayStateModel(),
                new GamePauseStateModel(),
                new GameOverStateModel(),
            };
            return _states;
        }

        public enum State
        {
            Title,
            GamePlay,
            GamePause,
            GameOver
        }
    }
}