using System;
using System.Collections.Generic;
using System.Text;
using BomberPerson.Assets;
using SFML.Graphics;

namespace BomberPerson
{
    abstract class Menu
    {
        //Clase padre de MainMenu y PauseMenu

        protected readonly float CooldownRate = 0.2f;
        protected float NextPress;

        protected int CurrentOption;
        protected List<Text> CurrentOptionsList;

        protected SoundManager SoundManager = Application.SoundManager;

        public abstract void Update();
        public abstract void Render();

        protected abstract void Select();

        protected void UpdateSelection(int direction, bool screenChange = false)
        {
            if (!screenChange)
            {
                if (Application.Clock.ElapsedTime.AsSeconds() < NextPress) return;
                NextPress = Application.Clock.ElapsedTime.AsSeconds() + CooldownRate;

                SoundManager.UiMove();
            }

            CurrentOption += direction;

            if (CurrentOption > CurrentOptionsList.Count - 1) CurrentOption = 0;
            else if (CurrentOption < 0) CurrentOption = CurrentOptionsList.Count - 1;

            foreach (var option in CurrentOptionsList) option.FillColor = Color.White;

            CurrentOptionsList[(int) CurrentOption].FillColor = Color.Red;
        }
    }
}
