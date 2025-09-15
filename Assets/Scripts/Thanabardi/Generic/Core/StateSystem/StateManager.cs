using System.Collections.Generic;
using Thanabardi.Generic.Utility;
using UnityEngine;

namespace Thanabardi.Generic.Core.StateSystem
{
    public abstract class StateManager<T> : MonoSingleton<T> where T : MonoBehaviour
    {
        public StateModel CurrentState { get; private set; }
        public StateModel PreviousState { get; private set; }

        private Dictionary<int, StateModel> _stateModels;

        protected void Initialize(int starterStateID, StateModel[] gameStateModels)
        {
            InitStates(gameStateModels);
            GoToState(starterStateID);
        }

        public void GoToState(int stateID)
        {
            PreviousState = CurrentState;
            CurrentState = _stateModels[stateID];
            ChangeState(PreviousState, CurrentState);
            Debug.Log($"[{typeof(T).Name}] Change game state from {PreviousState?.StateName} to {CurrentState.StateName}");
        }

        private void InitStates(StateModel[] gameStateModels)
        {
            _stateModels ??= new Dictionary<int, StateModel>();
            _stateModels.Clear();

            foreach (StateModel state in gameStateModels)
            {
                _stateModels.Add(state.StateID, state);
            }
        }

        private void ChangeState(
            StateModel oldStateModel,
            StateModel newStateModel)
        {
            oldStateModel?.OnStateOut();
            newStateModel?.OnStateIn();
        }
    }
}