using UnityEngine;
using UnityEngine.UI;
using Thanabardi.CentipedeGame.Core.GameSystem;

namespace Thanabardi.CentipedeGame.Core.GameWorld
{
    [DisallowMultipleComponent]
    public abstract class WorldObject : MonoBehaviour
    {
        [SerializeField]
        protected Image ObjectImage;

        public Vector2Int GridPosition {get; private set;}

        public bool IsDestroyed {get; private set;}

        public virtual void Initialize(Vector2Int position, Vector2 size)
        {
            IsDestroyed = false;
            ObjectImage.rectTransform.sizeDelta = size;
        }

        public void SetGridPosition(Vector2Int position)
        {
            GridPosition = position;
        }

        public abstract void OnHit(WorldObject other);

        public void DestroyWObject()
        {
            IsDestroyed = true;
            GameManager.Instance.GridManager.RemoveWorldObject(this);
        }
    }
}