using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Thanabardi.CentipedeGame.Core.GameWorld;

namespace Thanabardi.CentipedeGame.Core.GameSystem.GridSystem
{
    public class GridDataManager
    {
        #region field

        private Dictionary<Vector2Int, List<WorldObject>> _wObjectsByGridPosition;
        public Dictionary<Vector2Int, List<WorldObject>> WObjectsByGridPosition => _wObjectsByGridPosition;

        private List<Vector2Int> _emptyGridPosition;

        private Vector2Int _gridSize;

        #endregion
        #region public method

        public void ResetData()
        {
            if (_wObjectsByGridPosition != null)
            {
                foreach (var wObjects in _wObjectsByGridPosition.Values)
                {
                    if (wObjects == null) continue;

                    foreach (var wObject in wObjects)
                    {
                        Object.Destroy(wObject.gameObject);
                    }
                }
            }

            _wObjectsByGridPosition?.Clear();
            _emptyGridPosition?.Clear();
        }

        public void SetUpData(Vector2Int gridSize)
        {
            _gridSize = gridSize;
            _wObjectsByGridPosition ??= new();
            _emptyGridPosition ??= new();

            for (int y = 0; y < _gridSize.y; y++)
            {
                for (int x = 0; x < _gridSize.x; x++)
                {
                    _wObjectsByGridPosition[new(x, y)] = new List<WorldObject>();
                    _emptyGridPosition.Add(new(x, y));
                }
            }
        }

        public void AddWorldObjectInGrid(WorldObject worldObject, Vector2Int gridPosition)
        {
            // add world object into grid data
            _emptyGridPosition.Remove(gridPosition);
            _wObjectsByGridPosition[gridPosition].Add(worldObject);
            worldObject.SetGridPosition(gridPosition);
        }

        public void RemoveWorldObjectInGrid(WorldObject worldObject)
        {
            // remove world object from grid data
            Vector2Int gridPosition = worldObject.GridPosition;
            if (_wObjectsByGridPosition.TryGetValue(gridPosition, out var wObjs))
            {
                wObjs.Remove(worldObject);
                if (wObjs.Count == 0) _emptyGridPosition.Add(gridPosition);
            }
        }

        public void MoveWorldObjectInGrid(WorldObject worldObject, Vector2Int gridPosition)
        {
            // move world object in grid data
            RemoveWorldObjectInGrid(worldObject);
            AddWorldObjectInGrid(worldObject, gridPosition);
        }

        public bool TryGetRandomEmptyGrid(out Vector2Int gridPosition, int minYPosition = 0)
        {
            // get a random empty grid cell with Y-axis offset
            gridPosition = Vector2Int.zero;
            if (_emptyGridPosition.Count == 0)
                return false;

            var filteredPositions = _emptyGridPosition.Where(position => position.y >= minYPosition).ToList();
            gridPosition = filteredPositions[Random.Range(0, filteredPositions.Count)];

            return true;
        }

        #endregion
    }
}