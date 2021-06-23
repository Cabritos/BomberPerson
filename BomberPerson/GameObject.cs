using System;
using System.Collections.Generic;
using System.Text;
using BomberPerson.Assets;
using SFML.Graphics;
using SFML.System;

namespace BomberPerson
{
    class GameObject
    {
        //Case padre de Level Object y Player

        protected Level Level;
        protected SoundManager SoundManager;

        protected Vector2f FloatPosition;
        protected RectangleShape Collider;

        protected Texture Texture;
        protected Sprite Sprite;

        public GameObject(Level level)
        {
            Level = level;
            SoundManager = Application.SoundManager;
        }

        protected void CreateSprite(bool smooth = true)
        {
            if (smooth) Texture.Smooth = true;
            Sprite = new Sprite(Texture);
            Sprite.Scale = new Vector2f(Collider.GetGlobalBounds().Width / Sprite.GetGlobalBounds().Width, Collider.GetGlobalBounds().Height / Sprite.GetGlobalBounds().Height);
            Sprite.Position = FloatPosition;
            Level.LevelObjectsRenderList.Add(GetDrawable());
        }

        public Drawable GetDrawable()
        {
            if (Application.Debug) return Collider;
            return Sprite;
        }

        public FloatRect GetBounds()
        {
            return Collider.GetGlobalBounds();
        }
    }
}
