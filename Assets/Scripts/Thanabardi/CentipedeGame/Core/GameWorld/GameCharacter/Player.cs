using UnityEngine;
using Thanabardi.CentipedeGame.Core.GameSystem;
using Thanabardi.CentipedeGame.Core.Interface;

namespace Thanabardi.CentipedeGame.Core.GameWorld.GameCharacter
{
    public class Player : Character, IMoveable
    {
        #region move method

        public Vector2Int CalculateMoveTarget()
        {
            return GridPosition;
        }

        public void Move(Vector2Int moveTarget)
        {
            if (GridPosition == moveTarget) return;
            GameManager.Instance.GridManager.MoveWorldObject(this, moveTarget);
        }

        #endregion
        #region hit method

        public override void OnHit(WorldObject other)
        {
            switch (other)
            {
                case Spider:
                case Centipede:
                    SetHealth(Health - 1);
                    break;
            }
        }

        #endregion
    }
}