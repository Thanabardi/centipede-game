using System;
using Thanabardi.CentipedeGame.Core.Interface;
using UnityEngine;
using UnityEngine.UI;

namespace Thanabardi.CentipedeGame.Core.GameWorld
{
    public abstract class Character : WorldObject
    {
        [SerializeField]
        [Min(1)]
        private int _maxHealth = 1;

        public int Health { get; private set; }

        public override void Initialize(Vector2Int position, Vector2 size)
        {
            base.Initialize(position, size);
            SetHealth(_maxHealth);
        }

        protected void SetHealth(int health)
        {
            if (health <= 0)
            {
                DestroyWObject();
                return;
            }
            Health = health;
            Color ImageColor = ObjectImage.color;
            ImageColor.a = health / (float)Mathf.Max(1, _maxHealth); // set max health minimum value
            ObjectImage.color = ImageColor;
        }
    }
}