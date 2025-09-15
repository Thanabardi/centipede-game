using UnityEngine;


namespace Thanabardi.CentipedeGame.Core.Interface
{
    public interface IMoveable
    {
        public Vector2Int CalculateMoveTarget();
        public void Move(Vector2Int moveTarget);
    }
}