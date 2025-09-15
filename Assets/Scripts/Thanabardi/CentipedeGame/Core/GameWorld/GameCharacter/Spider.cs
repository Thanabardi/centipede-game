using System;
using System.Collections.Generic;
using System.Linq;
using Thanabardi.CentipedeGame.Core.GameSystem;
using Thanabardi.CentipedeGame.Core.Interface;
using UnityEngine;

namespace Thanabardi.CentipedeGame.Core.GameWorld.GameCharacter
{
    public class Spider : Character, IMoveable
    {
        #region move method

        public Vector2Int CalculateMoveTarget()
        {
            // stop moving when player is destroyed
            if (GameManager.Instance.GridManager.PlayerWObject.IsDestroyed)
                return GridPosition;

            // perform A* Pathfinding
            PathNode origin = new(GridPosition);
            PathNode target = new(GameManager.Instance.GridManager.PlayerWObject.GridPosition);

            List<PathNode> openNode = new();   // nodes to be searched
            List<PathNode> closedNode = new(); // nodes already searched and part of the explored path

            openNode.Add(origin);

            while (openNode.Count > 0)
            {
                // sort and pick lowest total estimated cost
                openNode.Sort((x, y) => x.TotalCost.CompareTo(y.TotalCost));
                PathNode currentNode = openNode[0];

                openNode.Remove(currentNode);
                closedNode.Add(currentNode);

                // reached destination
                if (currentNode.GridPosition == target.GridPosition)
                {
                    PathNode nextNode = GetNextOriginNode(currentNode, origin);
                    return nextNode.GridPosition;
                }

                foreach (PathNode neighbor in GetNeighborNode(currentNode))
                {
                    // excluded node that already searched
                    if (closedNode.Any(node => node.GridPosition == neighbor.GridPosition))
                        continue;

                    float costToNeighbor = currentNode.CostFromOrigin;

                    if (costToNeighbor < neighbor.CostFromOrigin || !openNode.Any(node => node.GridPosition == neighbor.GridPosition))
                    {
                        neighbor.CostFromOrigin = costToNeighbor;
                        neighbor.CostToTarget = Vector2.Distance(target.GridPosition, neighbor.GridPosition);
                        neighbor.ParentNode = currentNode;

                        if (!openNode.Any(node => node.GridPosition == neighbor.GridPosition))
                            openNode.Add(neighbor);
                    }
                }
            }

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
                case Bullet:
                    SetHealth(Health - 1);
                    break;
            }
        }

        #endregion
        #region private method

        private List<PathNode> GetNeighborNode(PathNode node)
        {
            List<PathNode> neighbors = new();
            Vector2Int[] directions = new Vector2Int[]
            {
                new(0, 1),   // North
                new(1, 1),   // NorthEast
                new(1, 0),   // East
                new(1, -1),  // SouthEast
                new(0, -1),  // South
                new(-1, -1), // SouthWest
                new(-1, 0),  // West
                new(-1, 1)   // NorthWest
            };

            foreach (var dir in directions)
            {
                Vector2Int neighborPos = node.GridPosition + dir;
                // check bounds and mushrooms
                if (GameManager.Instance.GridManager.IsWithinGridBounds(neighborPos) &&
                    !GameManager.Instance.GridManager.IsContainType(neighborPos, typeof(Mushroom)))
                {
                    PathNode neighborNode = new(neighborPos);
                    neighbors.Add(neighborNode);
                }
            }
            return neighbors;
        }

        private PathNode GetNextOriginNode(PathNode node, PathNode origin)
        {
            // get the next node to move toward from the origin
            if (node.ParentNode == origin)
            {
                return node;
            }
            return GetNextOriginNode(node.ParentNode, origin);
        }

        #endregion

        private class PathNode
        {
            public Vector2Int GridPosition;
            public float CostFromOrigin;
            public float CostToTarget;
            public PathNode ParentNode;
            public float TotalCost => CostFromOrigin + CostToTarget;
            public PathNode(Vector2Int gridPosition)
            {
                GridPosition = gridPosition;
            }
        }
    }
}