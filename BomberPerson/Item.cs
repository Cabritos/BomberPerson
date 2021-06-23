using System.Security.Cryptography.X509Certificates;
using SFML.Audio;

namespace BomberPerson
{
    abstract class Item : LevelObject
    {
        protected Item(Level level, float tileSize, int positionX, int positionY) : base(level, tileSize, positionX, positionY)
        {
            Level.ItemsGlobalBounds.Add(GetBounds(), this);
        }

        public override void Destroy()
        {
            base.Destroy();
            Level.ItemsGlobalBounds.Remove(GetBounds());
        }

        public virtual void PickUp(Player player)
        {
            SoundManager.PickItem();
            Destroy();
        }
    }
}
