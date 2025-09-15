using UnityEngine;
using Thanabardi.CentipedeGame.Core.GameSystem;
using Thanabardi.CentipedeGame.Core.Interface;

namespace Thanabardi.CentipedeGame.Core.GameWorld.GameCharacter
{
    public class Centipede : Character, IMoveable
    {
        #region field
        [SerializeField]
        protected Sprite _headSprite;

        private Centipede PreviousNode;
        private Centipede NextNode;
        public bool IsHead => NextNode == null;
        private Vector2Int _direction = Vector2Int.right + Vector2Int.down;

        protected Sprite _defaultSprite;

        #endregion
        #region lifecycle method

        private void Awake()
        {
            _defaultSprite = ObjectImage.sprite;
        }

        #endregion
        #region move method

        public Vector2Int CalculateMoveTarget()
        {
            if (!IsHead) return GridPosition;

            Vector2Int moveTarget = GridPosition + new Vector2Int(_direction.x, 0);

            // check horizontal collision or boundary
            if (moveTarget.x < 0 || moveTarget.x >= GameManager.Instance.GridSize.x ||
                GameManager.Instance.GridManager.IsContainType(moveTarget, typeof(Mushroom), typeof(Centipede)))
            {
                // convert horizontal direction
                _direction.x *= -1;

                // try moving vertically
                moveTarget = GridPosition + new Vector2Int(0, _direction.y);

                // check vertical boundary
                if (moveTarget.y < 0 || moveTarget.y >= GameManager.Instance.GridSize.y)
                {
                    // invert vertical direction
                    _direction.y *= -1;
                    moveTarget = GridPosition + new Vector2Int(0, _direction.y);
                }
            }
            return moveTarget;
        }

        public void Move(Vector2Int moveTarget)
        {
            if (!IsHead) return;
            UpdateGridPosition(moveTarget);
        }

        #endregion
        #region hit method

        public override void OnHit(WorldObject other)
        {
            switch (other)
            {
                case Bullet:
                    SplitBody();
                    SetHealth(Health - 1);
                    // spawn mushroom on destroy
                    GameManager.Instance.GridManager.SpawnMushroom(GridPosition);
                    break;
            }
        }

        #endregion
        #region public method

        public void SetDirection(Vector2Int direction)
        {
            _direction = direction;
        }

        public void SetNextNode(Centipede node)
        {
            NextNode = node;
            ObjectImage.sprite = IsHead ? _headSprite : _defaultSprite;
        }

        public void SetPreviousNode(Centipede node)
        {
            PreviousNode = node;
            ObjectImage.sprite = IsHead ? _headSprite : _defaultSprite;
        }

        #endregion
        #region private method

        private void UpdateGridPosition(Vector2Int moveTarget)
        {
            Vector2Int currentPosition = GridPosition;

            // move this node to the new target
            GameManager.Instance.GridManager.MoveWorldObject(this, moveTarget);

            // recursively update previous node
            if (PreviousNode != null)
            {
                PreviousNode._direction = _direction;
                PreviousNode.UpdateGridPosition(currentPosition);
            }
        }

        private void SplitBody()
        {
            if (NextNode != null) NextNode.SetPreviousNode(null);
            if (PreviousNode != null) PreviousNode.SetNextNode(null);
        }

        #endregion
    }
}