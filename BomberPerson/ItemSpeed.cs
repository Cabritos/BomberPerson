using SFML.Graphics;
using Color = SFML.Graphics.Color;

namespace BomberPerson
{
    class ItemSpeed : Item
    {
        public ItemSpeed(Level level, float tileSize, int positionX, int positionY) : base(level, tileSize, positionX, positionY)
        {
            Collider.FillColor = Color.Cyan;

            Texture = new Texture("itemSpeedSprite.png");
            CreateSprite();
        }

        public override void PickUp(Player player)
        {
            base.PickUp(player);
            player.IncreaseSpeed();
        }
    }
}
