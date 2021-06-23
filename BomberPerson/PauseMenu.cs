using System;
using System.Collections.Generic;
using System.Text;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace BomberPerson
{
    class PauseMenu : Menu
    {
        private Round _round;

        private readonly List<Drawable> _renderList = new List<Drawable>();
        private List<Text> _optionsList = new List<Text>();

        private RectangleShape _border;
        private RectangleShape _background;

        private Text _pausedText;
        private Text _resumeText;
        private Text _mainMenuText;
        private Text _exitText;

        public PauseMenu(Round round)
        {
            _round = round;

            GenerateMenu();

            CurrentOptionsList = _optionsList;
            UpdateSelection(0, true);

            NextPress = Application.Clock.ElapsedTime.AsSeconds() + 0.3f;
        }

        private void GenerateMenu()
        {
            _border = new RectangleShape(new Vector2f(Application.WindowWidth / 2, Application.WindowHeight / 8 * 5));
            _border.FillColor = Color.White;
            _border.Origin = new Vector2f(_border.GetGlobalBounds().Width / 2, _border.GetGlobalBounds().Height / 2);
            _border.Position = new Vector2f(Application.WindowWidth / 2, Application.WindowHeight / 2);
            _renderList.Add(_border);

            _background = new RectangleShape(new Vector2f(Application.WindowWidth / 2 * 0.95f, Application.WindowHeight / 8 * 5 * 0.95f));
            _background.FillColor = Color.Black;
            _background.Origin = new Vector2f(_background.GetGlobalBounds().Width / 2, _background.GetGlobalBounds().Height / 2);
            _background.Position = new Vector2f(Application.WindowWidth / 2, Application.WindowHeight / 2);
            _renderList.Add(_background);

            _pausedText = new Text("Paused", Application.Font, Application.WindowHeight / 20);
            _pausedText.Origin = new Vector2f(_pausedText.GetGlobalBounds().Width / 2, _pausedText.GetGlobalBounds().Height / 2);
            _pausedText.Position = new Vector2f(Application.WindowWidth / 2, Application.WindowHeight / 2 - 2 * (Application.WindowHeight / 10));
            _renderList.Add(_pausedText);

            _resumeText = new Text("Resume", Application.Font, Application.WindowHeight / 20);
            _resumeText.Origin = new Vector2f(_resumeText.GetGlobalBounds().Width / 2, _resumeText.GetGlobalBounds().Height / 2);
            _resumeText.Position = new Vector2f(Application.WindowWidth / 2, Application.WindowHeight / 2);
            _optionsList.Add(_resumeText);

            _mainMenuText = new Text("Main Menu", Application.Font, Application.WindowHeight / 20);
            _mainMenuText.Origin = new Vector2f(_mainMenuText.GetGlobalBounds().Width / 2, _mainMenuText.GetGlobalBounds().Height / 2);
            _mainMenuText.Position = new Vector2f(Application.WindowWidth / 2, Application.WindowHeight / 2 + (Application.WindowHeight / 10));
            _optionsList.Add(_mainMenuText);

            _exitText = new Text("Exit", Application.Font, Application.WindowHeight / 20);
            _exitText.Origin = new Vector2f(_exitText.GetGlobalBounds().Width / 2, _exitText.GetGlobalBounds().Height / 2);
            _exitText.Position = new Vector2f(Application.WindowWidth / 2, Application.WindowHeight / 2 + (Application.WindowHeight / 10 * 2));
            _optionsList.Add(_exitText);

            foreach (var option in _optionsList)
            {
                _renderList.Add(option);
            }
        }

        public override void Update()
        {
            if (Keyboard.IsKeyPressed(Keyboard.Key.Up)) UpdateSelection(-1);
            if (Keyboard.IsKeyPressed(Keyboard.Key.Down)) UpdateSelection(1);
            if (Keyboard.IsKeyPressed(Keyboard.Key.Enter)) Select();
        }

        public override void Render()
        {
            Application.DrawList(_renderList);
        }

        protected override void Select()
        {
            if (Application.Clock.ElapsedTime.AsSeconds() < NextPress) return;
            NextPress = Application.Clock.ElapsedTime.AsSeconds() + CooldownRate;

            SoundManager.UiSelect();

            switch (CurrentOption)
            {
                case 0:
                    _round.SwitchPause();
                    break;

                case 1:
                    Application.MainMenu();
                    break;

                case 2:
                    Application.EndApplication(this, EventArgs.Empty);
                    break;

                default:
                    return;
            }
        }
    }
}