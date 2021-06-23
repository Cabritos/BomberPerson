using System.Numerics;
using SFML.Graphics;
using SFML.System;

namespace BomberPerson
{
    class Bomb : AutoDestroy
    {
        public Player Player { get; private set; }

        private int _range;
        
        public Bomb(Level level, float tileSize, int positionX, int positionY, Player player, int range) : base(level, tileSize, positionX, positionY)
        {
            Type = LevelObjects.Bomb;
            Collider.FillColor = Color.Black;
            
            Texture = new Texture("bombSprite2.png"); ;
            CreateSprite();

            Level = level;
            Player = player;
            _range = range;

            Duration = 3.5f;

            Level.BombsGlobalBounds.Add(GetBounds());
        }

        public override void Destroy()
        {
            Explode();
        }

        public override void Explode()
        {
            base.Explode();

            Player.RemoveBomb();
            Level.Explosion(PositionX, PositionY, _range);
            SoundManager.Explotion();
            Level.BombsGlobalBounds.Remove(GetBounds());
        }
    }
}
