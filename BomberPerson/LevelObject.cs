using BomberPerson.Assets;
using SFML.Graphics;
using SFML.System;

namespace BomberPerson
{
    enum LevelObjects
    {
        Empty,
        Wall,
        Block,
        Bomb,
        Fire,
        ItemFire,
        ItemBomb,
        ItemSpeed,
    }

    abstract class LevelObject : GameObject
    {
        protected Round Round;
        public LevelObjects Type { get; protected set; }

        protected int PositionX;
        protected int PositionY;

        protected LevelObject(Level level, float tileSize, int positionX, int positionY) : base(level)
        {
            Round = Level.Round;

            PositionX = positionX;
            PositionY = positionY;

            FloatPosition = level.GridToFloat(PositionX, PositionY);
            Collider = new RectangleShape(new Vector2f(tileSize, tileSize));
            Collider.Position = FloatPosition;
        }

        public virtual void Update()
        {
            //
        }

        public virtual void Destroy()
        {
            Level.RemoveFromLevelDictionary(PositionX, PositionY);
            Level.LevelObjectsRenderList.Remove(GetDrawable());
        }
    }
}
