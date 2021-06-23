using SFML.Graphics;
using SFML.System;
using Color = SFML.Graphics.Color;

namespace BomberPerson
{
    class ItemBomb : Item
    {
        public ItemBomb(Level level, float tileSize, int positionX, int positionY) : base(level, tileSize, positionX, positionY)
        {
            Collider.FillColor = Color.White;

            Texture = new Texture("itemBombSprite.png");
            CreateSprite();
        }

        public override void PickUp(Player player)
        {
            base.PickUp(player);
            player.IncreaseBombCapacity();
        }
    }
}
