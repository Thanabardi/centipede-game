namespace Thanabardi.Generic.Core.StateSystem
{
    public class StateModel
    {
        public readonly int StateID;
        public readonly string StateName;

        public StateModel(int stateID, string stateName)
        {
            StateID = stateID;
            StateName = stateName;
        }

        // event on game state change to this state
        public virtual void OnStateIn() { }

        // event on game state exit to the next state
        public virtual void OnStateOut() { }
    }
}