using SFML.Graphics;
using System;
using SFML.System;

namespace BomberPerson
{
    class Block : LevelObject
    {
        public float Duration { get; protected set; }

        private float _setTime;

        public Block(Level level, float tileSize, int positionX, int positionY) : base(level, tileSize, positionX, positionY)
        {
            Type = LevelObjects.Block;
            Collider.FillColor = Color.Yellow;
            Duration = 1.55f;

            Texture = new Texture("blockSprite.png");
            CreateSprite();

            Level.MapGlobalBounds.Add(GetBounds());
        }

        public override void Destroy()
        {
            base.Destroy();
            _setTime = Application.Clock.ElapsedTime.AsSeconds();
            Level.Round.AddToUpdate(this);
            Level.MapGlobalBounds.Remove(GetBounds());
        }

        public override void Update()
        {
            if (Round.GetElapsedTime() - _setTime > Duration) DropItem();
        }

        public virtual void DropItem()
        {
            Level.Round.RemoveFromUpdate(this);

            Random random = new Random();

            switch (random.Next(0, 8))
            {
                case 0:
                    Level.CreateLevelObject(LevelObjects.ItemBomb, PositionX, PositionY);
                    break;

                case 1:
                    Level.CreateLevelObject(LevelObjects.ItemFire, PositionX, PositionY);
                    break;

                case 2:
                    Level.CreateLevelObject(LevelObjects.ItemSpeed, PositionX, PositionY);
                    break;

                default:
                    return;
            }
        }
    }
}
