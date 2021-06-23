using System;
using System.Collections.Generic;
using SFML.Graphics;
using SFML.System;

namespace BomberPerson
{
    class Fire : AutoDestroy
    {
        private readonly List<IntRect> _frames;
        private int _frameIndex;

        public Fire(Level level, float tileSize, int positionX, int positionY) : base(level, tileSize, positionX, positionY)
        {
            Type = LevelObjects.Fire;
            Collider.FillColor = Color.Red;

            Texture = new Texture("fireSpritesheet.png");
            CreateSprite(false);
            Sprite.Scale = new Vector2f(tileSize / Texture.Size.X, tileSize / Texture.Size.X);
           
            _frames = new List<IntRect>();
            _frames.Add(new IntRect(0, 0, 31, 31));
            _frames.Add(new IntRect(0, 31, 31, 31));
            _frames.Add(new IntRect(0, 62, 31, 31));

            var random = new Random();
            _frameIndex = random.Next(0, _frames.Count - 1);
            Sprite.TextureRect = _frames[_frameIndex];

            Duration = 1.5f;

            Level.FireGlobalBounds.Add(GetBounds());
        }

        public override void Update()
        {
            Animate();
            WaitAndDestroy();
        }

        private void Animate()
        {
            _frameIndex++;

            if (_frameIndex >= _frames.Count - 1)
            { 
                _frameIndex = 0;
            }

            Sprite.TextureRect = _frames[_frameIndex];
        }

        public override void Explode()
        {
            base.Explode();
            Level.FireGlobalBounds.Remove(GetBounds());
        }
    }
}
