using UnityEngine;
using Thanabardi.CentipedeGame.Core.GameSystem;
using Thanabardi.CentipedeGame.Core.GameWorld.GameCharacter;
using Thanabardi.CentipedeGame.Core.Interface;


namespace Thanabardi.CentipedeGame.Core.GameWorld
{
    public class Bullet : WorldObject, IMoveable
    {
        #region move method

        public Vector2Int CalculateMoveTarget()
        {
            return GridPosition + Vector2Int.up;
        }

        public void Move(Vector2Int moveTarget)
        {
            // check bounds
            if (GameManager.Instance.GridManager.IsWithinGridBounds(moveTarget))
                GameManager.Instance.GridManager.MoveWorldObject(this, moveTarget);
            else
                DestroyWObject();
        }

        #endregion
        #region hit method

        public override void OnHit(WorldObject other)
        {
            if (other is not Player)
                DestroyWObject();
        }

        #endregion
    }
}