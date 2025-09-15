using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Thanabardi.Generic.Core.StateSystem;
using Thanabardi.CentipedeGame.Core.GameWorld;
using Thanabardi.CentipedeGame.Core.GameWorld.GameCharacter;
using System.Collections;

namespace Thanabardi.CentipedeGame.Core.GameSystem.GridSystem
{
    public class GridManager : MonoBehaviour
    {
        #region field

        [Header("Grid Config")]
        [SerializeField]
        private Image _boardImage;

        [Space(10)]
        [Header("World Object Container")]
        [SerializeField]
        private Transform _mushroomContainer;
        [SerializeField]
        private Transform _centipedeContainer;
        [SerializeField]
        private Transform _playerContainer;
        [SerializeField]
        private Transform _bulletContainer;
        [SerializeField]
        private Transform _spiderContainer;

        [Space(10)]
        [Header("World Object Prefab")]
        [SerializeField]
        private Mushroom _mushroomPrefab;
        [SerializeField]
        private Centipede _centipedePrefab;
        [SerializeField]
        private Player _playerPrefab;
        [SerializeField]
        private Bullet _bulletPrefab;
        [SerializeField]
        private Spider _spiderPrefab;

        private GridDataManager _gridDataManager;
        public GridDataManager GridDataManager => _gridDataManager;

        public ObjPool<Centipede> CentipedeWObjectPool { get; private set; }
        public ObjPool<Mushroom> MushroomWObjectPool { get; private set; }
        public ObjPool<Bullet> BulletObjectPool { get; private set; }
        public Player PlayerWObject { get; private set; }
        public Spider SpiderWObject { get; private set; }

        public Vector2 CellSize { get; private set; }

        public event Action<WorldObject> OnWObjectDestroyed;

        private GridController _gridController;
        private Coroutine _spiderSpawnDelayCoroutine;

        #endregion
        #region lifecycle method

        private void Awake()
        {
            _gridDataManager = new();
            _gridController = new(this);
        }

        private void Update()
        {
            _gridController.UpdatePosition();
        }

        #endregion
        #region public method

        public void SetUpBoard()
        {
            // calculate cell size based on board dimensions and grid size
            Vector2 boardSize = _boardImage.rectTransform.sizeDelta;
            Vector2Int gridSize = GameManager.Instance.GridSize;
            CellSize = new(boardSize.x / gridSize.x, boardSize.y / gridSize.y);

            // reset and initialize grid data
            _gridDataManager.ResetData();
            _gridDataManager.SetUpData(gridSize);

            // initialize and clear object pools
            CentipedeWObjectPool ??= new(_centipedePrefab, _centipedeContainer.transform);
            MushroomWObjectPool ??= new(_mushroomPrefab, _mushroomContainer.transform);
            BulletObjectPool ??= new(_bulletPrefab, _bulletContainer.transform);

            CentipedeWObjectPool.Clear();
            MushroomWObjectPool.Clear();
            BulletObjectPool.Clear();

            // destroy existing player and spider
            if (PlayerWObject != null)
            {
                Destroy(PlayerWObject.gameObject);
                PlayerWObject = null;
            }
            if (SpiderWObject != null)
            {
                Destroy(SpiderWObject.gameObject);
                SpiderWObject = null;
            }

            // Spawn new entities
            SpawnPlayer();
            SpawnCentipede(GameManager.Instance.InitialCentipede);
            SpawnSpider();
            SpawnMushroom(GameManager.Instance.InitialMushroom);
        }

        public void OnGameStateIn()
        {
            _gridController.StartMoving();
        }

        public void OnGameStateOut()
        {
            _gridController.StopMoving();
        }

        public void SpawnMushroom(Vector2Int GridPosition)
        {
            SpawnWorldObject(MushroomWObjectPool, GridPosition);
        }



        public void SpawnBullet()
        {
            SpawnWorldObject(BulletObjectPool, PlayerWObject.GridPosition);
        }

        public void SpawnSpiderDelay(float delay)
        {
            // delay spawning spider
            if (_spiderSpawnDelayCoroutine != null) StopCoroutine(_spiderSpawnDelayCoroutine);
            _spiderSpawnDelayCoroutine = StartCoroutine(SpiderSpawnCoroutine(delay));
        }

        public void SpawnSpider()
        {
            // stop existing delay spawn
            if (_spiderSpawnDelayCoroutine != null)
            {
                StopCoroutine(_spiderSpawnDelayCoroutine);
                _spiderSpawnDelayCoroutine = null;
            }

            Vector2Int gridPosition;
            if (!GameManager.Instance.IsSpawnSpiderRandom)
            {
                // spawn at the center of the grid
                gridPosition = new(Mathf.RoundToInt(GameManager.Instance.GridSize.x / 2), Mathf.RoundToInt(GameManager.Instance.GridSize.y / 2));
            }
            else if (!_gridDataManager.TryGetRandomEmptyGrid(out gridPosition, Mathf.RoundToInt(GameManager.Instance.GridSize.y * GameManager.Instance.PlayerMoveAreaY)))
                return;

            if (SpiderWObject == null)
            {
                SpiderWObject = SpawnWorldObject(_spiderPrefab, gridPosition, _spiderContainer);
            }
            else
            {
                _gridDataManager.RemoveWorldObjectInGrid(SpiderWObject);
                SpawnObject(SpiderWObject, gridPosition);
            }
        }

        public void SpawnPlayer()
        {
            Vector2Int gridPosition = new(Mathf.RoundToInt(GameManager.Instance.GridSize.x / 2), 0);
            if (PlayerWObject == null)
            {
                PlayerWObject = SpawnWorldObject(_playerPrefab, gridPosition, _playerContainer);
            }
            else
            {
                _gridDataManager.RemoveWorldObjectInGrid(PlayerWObject);
                SpawnObject(PlayerWObject, gridPosition);
            }
        }

        public void SpawnCentipede(int amount)
        {
            // clear existing centipede parts
            foreach (var item in CentipedeWObjectPool)
            {
                item.DestroyWObject();
            }

            int availableSpace = Mathf.Min(amount, GameManager.Instance.GridSize.x * GameManager.Instance.GridSize.y);
            Vector2Int direction = Vector2Int.right + Vector2Int.down;
            int startY = GameManager.Instance.GridSize.y - 1;
            Centipede centipedeInstance = null;

            for (int i = 0; i < availableSpace; i++)
            {
                int col = i % GameManager.Instance.GridSize.x;
                int row = Mathf.CeilToInt(i / GameManager.Instance.GridSize.x);

                int x = col;
                int y = startY - row;

                // flip direction at odd row
                if (row % 2 != 0)
                {
                    direction.x *= -1;
                    x = (GameManager.Instance.GridSize.x - 1) - col;
                }

                Centipede centipede = SpawnWorldObject(CentipedeWObjectPool, new(x, y));
                // set previous next node
                if (centipedeInstance != null) centipedeInstance.SetNextNode(centipede);
                centipede.SetDirection(direction);
                centipede.SetPreviousNode(centipedeInstance);
                centipedeInstance = centipede;
            }
            centipedeInstance.SetNextNode(null);
        }

        public void RemoveWorldObject(WorldObject worldObject)
        {
            _gridDataManager.RemoveWorldObjectInGrid(worldObject);
            worldObject.gameObject.SetActive(false);
            OnWObjectDestroyed?.Invoke(worldObject);
        }

        public void MoveWorldObject(WorldObject worldObject, Vector2Int gridPosition)
        {
            _gridDataManager.MoveWorldObjectInGrid(worldObject, gridPosition);
        }

        public Vector3 GetWorldGridPosition(Vector2 gridPosition)
        {
            return new Vector3((gridPosition.x * CellSize.x) + (CellSize.x / 2), (gridPosition.y * CellSize.y) + (CellSize.y / 2), 0);
        }

        public bool IsContainType(Vector2Int gridPosition, params Type[] types)
        {
            // checks if the grid position contains a world object with type
            if (_gridDataManager.WObjectsByGridPosition.TryGetValue(gridPosition, out var wObjects))
            {
                foreach (var wObject in wObjects)
                {
                    foreach (var type in types)
                    {
                        if (type.IsAssignableFrom(wObject.GetType()))
                            return true;
                    }
                }
            }
            return false;
        }

        public bool IsWithinGridBounds(Vector2Int gridPosition, float heightScale = 1f, float widthScale = 1f)
        {
            return gridPosition.x >= 0 && gridPosition.x < GameManager.Instance.GridSize.x * widthScale &&
                gridPosition.y >= 0 && gridPosition.y < GameManager.Instance.GridSize.y * heightScale;
        }

        public bool IsAllCentipedeDestroyed()
        {
            foreach (var centipede in CentipedeWObjectPool)
            {
                if (!centipede.IsDestroyed) return false;
            }
            return true;
        }

        #endregion
        #region private method

        private void SpawnMushroom(int amount)
        {
            for (int i = 0; i < amount; i++)
            {
                if (_gridDataManager.TryGetRandomEmptyGrid(out var gridPosition, 1))
                    SpawnWorldObject(MushroomWObjectPool, gridPosition);
            }
        }

        private T SpawnWorldObject<T>(T prefab, Vector2Int gridPosition, Transform parent) where T : WorldObject
        {
            // spawn world object from prefab
            T worldObject = Instantiate(prefab, parent, false);
            SpawnObject(worldObject, gridPosition);
            return worldObject;
        }

        private T SpawnWorldObject<T>(ObjPool<T> objPool, Vector2Int gridPosition) where T : WorldObject
        {
            // spawn world object from pool
            T worldObject = objPool.Get();
            SpawnObject(worldObject, gridPosition);
            return worldObject;
        }

        private void SpawnObject<T>(T instance, Vector2Int gridPosition) where T : WorldObject
        {
            instance.transform.localPosition = GetWorldGridPosition(gridPosition);
            instance.Initialize(gridPosition, CellSize);
            _gridDataManager.AddWorldObjectInGrid(instance, gridPosition);
            instance.gameObject.SetActive(true);
        }

        private IEnumerator SpiderSpawnCoroutine(float delay)
        {
            yield return new WaitForSeconds(delay);
            SpawnSpider();
        }

        #endregion
    }
}