using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Thanabardi.CentipedeGame.Core.GameWorld;
using Thanabardi.CentipedeGame.Core.Interface;
using System.Linq;
using Thanabardi.CentipedeGame.Core.GameWorld.GameCharacter;

namespace Thanabardi.CentipedeGame.Core.GameSystem.GridSystem
{
    public class GridController
    {
        #region field

        private Coroutine _bulletMoveCoroutine;
        private Coroutine _centipedeMoveCoroutine;
        private Coroutine _spiderMoveCoroutine;

        private GridManager _gridManager;

        #endregion
        #region public method

        public GridController(GridManager gridManager)
        {
            _gridManager = gridManager;
        }

        public void StartMoving()
        {
            // start moving world object by grid position
            SetStartMovement(ref _bulletMoveCoroutine, _gridManager.BulletObjectPool, GameManager.Instance.BulletMoveSpeed);
            SetStartMovement(ref _centipedeMoveCoroutine, _gridManager.CentipedeWObjectPool, GameManager.Instance.CentipedeMoveSpeed);
            SetStartMovement(ref _spiderMoveCoroutine, new WorldObject[] { _gridManager.SpiderWObject }, GameManager.Instance.SpiderMoveSpeed);
        }

        public void StopMoving()
        {
            // stop moving world object by grid position
            SetStopMovement(ref _bulletMoveCoroutine);
            SetStopMovement(ref _centipedeMoveCoroutine);
            SetStopMovement(ref _spiderMoveCoroutine);
        }

        public void UpdatePosition()
        {
            // moving world object position from grid coordinates
            UpdateWorldObjectPosition(_gridManager.PlayerWObject, GameManager.Instance.PlayerMoveSpeed);
            UpdateWorldObjectPosition(_gridManager.BulletObjectPool, GameManager.Instance.BulletMoveSpeed);
            UpdateWorldObjectPosition(_gridManager.CentipedeWObjectPool, GameManager.Instance.CentipedeMoveSpeed);
            UpdateWorldObjectPosition(_gridManager.SpiderWObject, GameManager.Instance.SpiderMoveSpeed);
        }

        #endregion
        #region private method

        private IEnumerator MoveWorldObjectCoroutine(IEnumerable<WorldObject> worldObjects, float moveSpeed)
        {
            // moving world object by grid position
            Queue<(WorldObject worldObject, Vector2Int moveTarget)> moveQueue = new();

            while (true)
            {
                moveQueue.Clear();

                foreach (var worldObject in worldObjects)
                {
                    // enqueue world object's move target
                    if (!worldObject.IsDestroyed && worldObject is IMoveable moveable)
                        moveQueue.Enqueue((worldObject, moveable.CalculateMoveTarget()));
                }

                foreach (var (worldObject, moveTarget) in moveQueue)
                {
                    // move world object
                    if (!worldObject.IsDestroyed && worldObject is IMoveable moveable)
                        moveable.Move(moveTarget);
                }

                foreach (var (worldObject, moveTarget) in moveQueue)
                {
                    // check for collision at target position with the recently added object
                    if (_gridManager.GridDataManager.WObjectsByGridPosition.TryGetValue(moveTarget, out var wObjs))
                    {
                        var targetObj = wObjs.LastOrDefault(wObj => wObj != worldObject);
                        if (targetObj != null)
                        {
                            worldObject.OnHit(targetObj);
                            targetObj.OnHit(worldObject);
                        }
                    }
                }
                // delay movement on the grid (cells per second)
                yield return new WaitForSeconds(1f / Mathf.Max(0.001f, moveSpeed));
            }
        }

        private void UpdateWorldObjectPosition(IEnumerable<WorldObject> worldObjects, float moveSpeed)
        {
            if (worldObjects == null)
                return;

            foreach (var wObject in worldObjects)
                UpdateWorldObjectPosition(wObject, moveSpeed);
        }

        private void UpdateWorldObjectPosition(WorldObject worldObject, float moveSpeed)
        {
            if (worldObject == null)
                return;

            //moving world object position
            if (worldObject is IMoveable)
            {
                Vector3 currentPos = worldObject.transform.localPosition;
                Vector3 targetPos = _gridManager.GetWorldGridPosition(worldObject.GridPosition);

                float cellScale = _gridManager.CellSize.magnitude;
                float speed = cellScale * moveSpeed; // scale move speed

                // update position
                worldObject.transform.localPosition = Vector2.MoveTowards(
                    currentPos,
                    targetPos,
                    speed * Time.deltaTime
                );

                // prevent player and bullet from rotating
                if (worldObject is Player || worldObject is Bullet)
                    return;

                float rotationSpeedFactor = 360f;
                float rotationSpeed = Mathf.Max(rotationSpeedFactor, moveSpeed * rotationSpeedFactor);

                // update rotation
                Vector3 direction = targetPos - currentPos;
                if (direction != Vector3.zero)
                {
                    float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                    Quaternion targetRotation = Quaternion.Euler(0, 0, angle);
                    worldObject.transform.rotation = Quaternion.RotateTowards(
                        worldObject.transform.rotation,
                        targetRotation,
                        rotationSpeed * Time.deltaTime
                    );
                }
            }
        }

        #endregion
        #region helper method

        private void SetStartMovement(ref Coroutine coroutine, IEnumerable<WorldObject> worldObjects, float speed)
        {
            if (coroutine != null) _gridManager.StopCoroutine(coroutine);
            coroutine = _gridManager.StartCoroutine(MoveWorldObjectCoroutine(worldObjects, speed));
        }

        private void SetStopMovement(ref Coroutine coroutine)
        {
            if (coroutine != null)
            {
                _gridManager.StopCoroutine(coroutine);
                coroutine = null;
            }
        }

        #endregion
    }
}