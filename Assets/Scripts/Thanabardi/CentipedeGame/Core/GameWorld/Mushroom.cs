namespace Thanabardi.CentipedeGame.Core.GameWorld
{
    public class Mushroom : Character
    {
        public override void OnHit(WorldObject other)
        {
            switch (other)
            {
                case Bullet:
                    SetHealth(Health - 1);
                    break;
            }
        }
    }
}