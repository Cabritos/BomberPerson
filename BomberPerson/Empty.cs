using SFML.Graphics;
using SFML.System;

namespace BomberPerson
{
    class Empty : LevelObject
    {
        public Empty(Level level, float tileSize, int positionX, int positionY) : base(level, tileSize, positionX, positionY)
        {
            Type = LevelObjects.Empty;
            Collider.FillColor = Color.Green;

            Texture = new Texture("emptySprite.png");
            CreateSprite();

            Level.LevelObjectsRenderList.Add(GetDrawable());
        }

        //Constructor alternativo para no generar sprites innecesarios. Cumple el rol de null level object en el diccionario de level objects
        public Empty(Level level, float tileSize, int positionX, int positionY, bool noSprite) : base(level, tileSize, positionX, positionY)
        {
        }
    }
}
