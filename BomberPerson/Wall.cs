using SFML.Graphics;

namespace BomberPerson
{
    class Wall : LevelObject
    {
        public Wall(Level level, float tileSize, int positionX, int positionY) : base(level, tileSize, positionX, positionY)
        {
            Type = LevelObjects.Wall;
            Collider.FillColor = Color.Magenta;

            Texture = new Texture("wallSprite.png");
            CreateSprite();

            Level.MapGlobalBounds.Add(GetBounds());
        }   
    }
}
