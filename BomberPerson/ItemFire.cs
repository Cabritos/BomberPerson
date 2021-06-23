using SFML.Graphics;
using SFML.System;

namespace BomberPerson
{
    class ItemFire : Item
    {
        public ItemFire(Level level, float tileSize, int positionX, int positionY) : base(level, tileSize, positionX, positionY)
        {
            Collider.FillColor = Color.Magenta;

            Texture = new Texture("itemFireSprite.png");
            CreateSprite();
        }

        public override void PickUp(Player player)
        {
            base.PickUp(player);
            player.IncreaseFireRange();
        }
    }
}
