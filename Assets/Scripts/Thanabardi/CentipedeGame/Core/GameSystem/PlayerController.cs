using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using Thanabardi.CentipedeGame.Core.GameSystem.GameInputSystem;
using Thanabardi.CentipedeGame.Core.GameWorld;
using System.Linq;

namespace Thanabardi.CentipedeGame.Core.GameSystem
{
    public class PlayerController : MonoBehaviour
    {
        #region field

        private Vector2Int _moveDirection;
        private Coroutine _moveCoroutine;
        private Coroutine _shootCoroutine;

        #endregion
        #region life cycle method

        void OnEnable()
        {
            GameManager.Instance.OnGameStateIn += OnGameStateInHandler;
            GameManager.Instance.OnGameStateOut += OnGameStateOutHandler;
        }

        void OnDisable()
        {
            GameManager.Instance.OnGameStateIn -= OnGameStateInHandler;
            GameManager.Instance.OnGameStateOut -= OnGameStateOutHandler;
        }

        #endregion
        #region private method

        private void OnGameStateInHandler()
        {
            InputManager.Instance.InputSystem.Player.Move.performed += OnMoveHandler;
            InputManager.Instance.InputSystem.Player.Move.canceled += OnMoveHandler;

            InputManager.Instance.InputSystem.Player.Shoot.started += OnShootHandler;
            InputManager.Instance.InputSystem.Player.Shoot.canceled += OnShootHandler;
        }

        private void OnGameStateOutHandler()
        {
            InputManager.Instance.InputSystem.Player.Move.performed -= OnMoveHandler;
            InputManager.Instance.InputSystem.Player.Move.canceled -= OnMoveHandler;

            InputManager.Instance.InputSystem.Player.Shoot.started -= OnShootHandler;
            InputManager.Instance.InputSystem.Player.Shoot.canceled += OnShootHandler;
        }

        private void OnMoveHandler(InputAction.CallbackContext context)
        {
            // move the player continuously from input direction
            if (context.performed)
            {
                Vector2 moveInput = context.ReadValue<Vector2>();
                _moveDirection = Vector2Int.RoundToInt(moveInput);                
                RestartCoroutine(ref _moveCoroutine, MovePlayerCoroutine());
            }
            else if (context.canceled)
            {
                StopAndClearCoroutine(ref _moveCoroutine);
                _moveDirection = Vector2Int.zero;
            }
        }

        private void OnShootHandler(InputAction.CallbackContext context)
        {
            // fire bullets continuously while pressing button
            if (context.started)
            {
                RestartCoroutine(ref _shootCoroutine, ShootCoroutine());
            }
            else if (context.canceled)
            {
                StopAndClearCoroutine(ref _shootCoroutine);
            }
        }

        private IEnumerator MovePlayerCoroutine()
        {
            // moving player by grid position
            var gridManager = GameManager.Instance.GridManager;
            var player = gridManager.PlayerWObject;

            while (true)
            {
                Vector2Int moveTarget = player.GridPosition + _moveDirection;

                // check bounds and mushrooms
                if (gridManager.IsWithinGridBounds(moveTarget, GameManager.Instance.PlayerMoveAreaY) &&
                    !gridManager.IsContainType(moveTarget, typeof(Mushroom)))
                {
                    gridManager.MoveWorldObject(player, moveTarget);

                    // check for collision at target position with the recently added object
                    if (gridManager.GridDataManager.WObjectsByGridPosition.TryGetValue(moveTarget, out var wObjs))
                    {
                        var targetObj = wObjs.LastOrDefault(wObj => wObj != player);
                        if (targetObj != null)
                        {
                            player.OnHit(targetObj);
                            targetObj.OnHit(player);
                        }
                    }
                }
                // delay movement on the grid (cells per second)
                yield return new WaitForSeconds(1f / Mathf.Max(0.001f, GameManager.Instance.PlayerMoveSpeed));
            }
        }

        private IEnumerator ShootCoroutine()
        {
            while (true)
            {
                GameManager.Instance.GridManager.SpawnBullet();
                // delay firing (bullet per second)
                yield return new WaitForSeconds(1 / Mathf.Max(0.001f, GameManager.Instance.PlayerFiringRate));
            }
        }

        #endregion
        #region helper method

        private void RestartCoroutine(ref Coroutine coroutine, IEnumerator routine)
        {
            StopAndClearCoroutine(ref coroutine);
            coroutine = StartCoroutine(routine);
        }

        private void StopAndClearCoroutine(ref Coroutine coroutine)
        {
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
                coroutine = null;
            }
        }

        #endregion
    }
}