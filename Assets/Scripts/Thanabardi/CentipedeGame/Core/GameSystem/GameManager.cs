using System;
using UnityEngine;
using Thanabardi.CentipedeGame.Core.GameSystem.GridSystem;
using Thanabardi.Generic.Utility;
using Thanabardi.CentipedeGame.Core.GameWorld;
using Thanabardi.CentipedeGame.Core.GameWorld.GameCharacter;
using Thanabardi.CentipedeGame.Core.GameState;

namespace Thanabardi.CentipedeGame.Core.GameSystem
{
    public class GameManager : MonoSingleton<GameManager>
    {
        #region field

        [SerializeField]
        private GridManager _gridManager;
        public GridManager GridManager => _gridManager;

        [SerializeField]
        private Vector2Int _gridSize = new(25, 25);
        public Vector2Int GridSize => _gridSize;

        [SerializeField]
        [Range(0f, 1f)]
        private float _playerMoveAreaY = 0.15f;
        public float PlayerMoveAreaY => _playerMoveAreaY;

        [Space(10)]
        [Header("Initial Values")]
        [SerializeField]
        private int _initialPlayerLife = 3;
        [SerializeField]
        private int _initialCentipede = 15;
        public int InitialCentipede => _initialCentipede;
        [SerializeField]
        private int _initialMushroom = 50;
        public int InitialMushroom => _initialMushroom;

        [Space(10)]
        [Header("Speed Configuration")]
        [Tooltip("A speed when moving player in cells per second")]
        [SerializeField]
        [Min(0.1f)]
        private float _playerMoveSpeed = 8f;
        public float PlayerMoveSpeed => _playerMoveSpeed;

        [Tooltip("A speed when moving bullet in cells per second")]
        [SerializeField]
        [Min(0.1f)]
        private float _bulletMoveSpeed = 20f;
        public float BulletMoveSpeed => _bulletMoveSpeed;

        [Tooltip("A speed when moving centipede in cells per second")]
        [SerializeField]
        [Min(0.1f)]
        private float _centipedeMoveSpeed = 5f;
        public float CentipedeMoveSpeed => _centipedeMoveSpeed;

        [Tooltip("A speed when moving spider in cells per second")]
        [SerializeField]
        [Min(0.1f)]
        private float _spiderMoveSpeed = 2f;
        public float SpiderMoveSpeed => _spiderMoveSpeed;

        [Tooltip("Number of bullets fired per second")]
        [SerializeField]
        [Min(0.1f)]
        private float _playerFiringRate = 5f;
        public float PlayerFiringRate => _playerFiringRate;

        [Space(10)]
        [Tooltip("Enable random spider spawning")]
        [SerializeField]
        private bool _isSpawnSpiderRandom;
        public bool IsSpawnSpiderRandom => _isSpawnSpiderRandom;

        [SerializeField]
        private float _spiderSpawnDelay = 10f;

        [Space(10)]
        [Header("Score")]
        [SerializeField]
        private int _mushroomScore = 1;
        [SerializeField]
        private int _centipedeScore = 100;
        [SerializeField]
        private int _spiderScore = 500;

        public int PlayerLife { get; private set; }
        public int Score { get; private set; }
        public bool IsEndless { get; private set; }

        public event Action OnGameStateIn;
        public event Action OnGameStateOut;
        public event Action<int> OnScoreUpdate;
        public event Action<int> OnLifeUpdate;

        #endregion
        #region public method

        public void StartGame(bool isEndless = false)
        {
            IsEndless = isEndless;
            _gridManager.SetUpBoard();
            PlayerLife = _initialPlayerLife;
            Score = 0;
        }

        public void OnGameStateInHandler()
        {
            _gridManager.OnGameStateIn();
            _gridManager.OnWObjectDestroyed += OnWObjectDestroyedHandler;

            OnGameStateIn?.Invoke();
            OnScoreUpdate?.Invoke(Score);
            OnLifeUpdate?.Invoke(PlayerLife);
        }

        public void OnGameStateOutInHandler()
        {
            _gridManager.OnGameStateOut();
            _gridManager.OnWObjectDestroyed -= OnWObjectDestroyedHandler;

            OnGameStateOut?.Invoke();
        }

        private void OnWObjectDestroyedHandler(WorldObject worldObject)
        {
            switch (worldObject)
            {
                case Player:
                    HandlePlayerDeath();
                    break;

                case Mushroom:
                    AddScore(_mushroomScore);
                    break;

                case Centipede:
                    AddScore(_centipedeScore);
                    HandleCentipedeDeath();
                    break;

                case Spider:
                    AddScore(_spiderScore);
                    // respawn spider
                    _gridManager.SpawnSpiderDelay(_spiderSpawnDelay);
                    break;
            }
        }

        #endregion
        #region private method

        private void HandlePlayerDeath()
        {
            PlayerLife -= 1;
            OnLifeUpdate?.Invoke(PlayerLife);

            if (PlayerLife <= 0)
            {
                GameStateManager.Instance.GoToState((int)GameStates.State.GameOver);
            }
            else
            {
                // respawn player and reset enemy
                _gridManager.OnWObjectDestroyed -= OnWObjectDestroyedHandler;
                _gridManager.SpawnPlayer();
                _gridManager.SpawnSpider();
                _gridManager.SpawnCentipede(_initialCentipede);
                _gridManager.OnWObjectDestroyed += OnWObjectDestroyedHandler;
            }
        }

        private void HandleCentipedeDeath()
        {
            if (_gridManager.IsAllCentipedeDestroyed())
            {
                if (IsEndless)
                {
                    // respawn centipede in endless mode
                    _gridManager.OnWObjectDestroyed -= OnWObjectDestroyedHandler;
                    _gridManager.SpawnCentipede(_initialCentipede);
                    _gridManager.OnWObjectDestroyed += OnWObjectDestroyedHandler;
                }
                else
                {
                    GameStateManager.Instance.GoToState((int)GameStates.State.GameOver);
                }
            }
        }

        private void AddScore(int amount)
        {
            Score += amount;
            OnScoreUpdate?.Invoke(Score);
        }

        #endregion
    }
}