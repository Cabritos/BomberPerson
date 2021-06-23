using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using BomberPerson.Assets;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace BomberPerson
{
    enum MenuScreens
    {
        Intro,
        Main,
        Configuration,
        Tutorial,
        Controllers
    }

    class MainMenu : Menu
    {
        private MenuScreens _currentScreen;
        private int _currentTutorialScreen;

        private readonly List<Drawable> _introRenderList = new List<Drawable>();
        private readonly List<Drawable> _mainRenderList = new List<Drawable>();
        private readonly List<Drawable> _configurationRenderList = new List<Drawable>();
        private readonly List<Drawable> _tutorialRenderList1 = new List<Drawable>();
        private readonly List<Drawable> _tutorialRenderList2 = new List<Drawable>();
        private readonly List<Drawable> _tutorialRenderList3 = new List<Drawable>();
        private readonly List<Drawable> _controllersRenderList = new List<Drawable>();

        private readonly List<Text> _mainOptionsList = new List<Text>();
        private readonly List<Text> _configurationOptionsList = new List<Text>();
        private List<Drawable>[] _tutorialScreens;

        private Text _intro;
        private Text _intro2;

        private Text _gameTitle;
        private Text _newGame;
        private Text _tutorial;
        private Text _controllers;
        private Text _exit;

        private Text _enter;
        private Text _tutorial1;
        private Text _tutorial2;
        private Text _tutorial3;
        private Text _tutorial4;
        private Text _tutorial5;
        private Text _tutorial6;
        private Text _tutorial7;
        private Text _tutorial8;
        private Text _tutorial9;

        private Sprite _sprite1;
        private Sprite _sprite2;
        private Sprite _sprite3;
        private Sprite _sprite4;
        private Sprite _sprite5;
        
        //private Text _playersText;
        private Text _mapSizeText;
        private Text _pointsText;
        private Text _startText;
        //private Text _playersValueText;
        private Text _mapSizeValueText;
        private Text _pointsValueText;

        private int _numberOfPlayers = 2;
        private int _mapSize = 15;
        private int _points = 5;

        //private int _minPlayers = 2;
        //private int _maxPlayers = 4;

        private const int MinSize = 9;
        private const int MaxSize = 25;

        private const int MinPoints = 1;
        private const int MaxPoints = 10;

        private Text _controllersTitle;
        private Text _p1;
        private Text _p2;
        private Text _move;
        private Text _moveP1;
        private Text _moveP2;
        private Text _action;
        private Text _actionP1;
        private Text _actionP2;
        private Text _pause;
        private Text _return;

        public MainMenu()
        {
            GenerateIntroScreen();
            GenerateMainScreen();
            GenerateConfigurationScreen();
            GenerateControllersScreen();
            GenerateTutorialScreen();

            CurrentOptionsList = _mainOptionsList;
            UpdateSelection(0, true);

            if (!Application.HasPlayedIntro)
            {
                _currentScreen = MenuScreens.Intro;
                SoundManager.PlayIntro();
            }
            else
            {
                _currentScreen = MenuScreens.Main;
                SoundManager.MainMenuScreen();
            }

            NextPress = Application.Clock.ElapsedTime.AsSeconds() + 0.5f;
        }

        public override void Update()
        {
            if (_currentScreen == MenuScreens.Intro) IntroUpdate();

            if (_currentScreen == MenuScreens.Main || _currentScreen == MenuScreens.Configuration)
            {
                if (Keyboard.IsKeyPressed(Keyboard.Key.Up)) UpdateSelection(-1);
                if (Keyboard.IsKeyPressed(Keyboard.Key.Down)) UpdateSelection(1);
                if (Keyboard.IsKeyPressed(Keyboard.Key.Enter)) Select();
            }

            if (_currentScreen == MenuScreens.Configuration)
            {
                if (Keyboard.IsKeyPressed(Keyboard.Key.Left)) IncrementSelection(-1);
                if (Keyboard.IsKeyPressed(Keyboard.Key.Right)) IncrementSelection(1);
            }

            if (_currentScreen != MenuScreens.Main && Keyboard.IsKeyPressed(Keyboard.Key.Escape))
            {
                _currentScreen = MenuScreens.Main;
                CurrentOptionsList = _mainOptionsList;
                CurrentOption = 0;
                _currentTutorialScreen = 0;
                UpdateSelection(0, true);
            }

            if (_currentScreen == MenuScreens.Tutorial && Keyboard.IsKeyPressed(Keyboard.Key.Enter)) NextTutorialScreen();
        }

        public override void Render()
        {
            switch (_currentScreen)
            {
                case MenuScreens.Intro:
                    Application.DrawList(_introRenderList);
                    break;

                case MenuScreens.Main:
                     Application.DrawList(_mainRenderList);
                    break;

                case MenuScreens.Configuration:
                    Application.DrawList(_configurationRenderList);
                    break;

                case MenuScreens.Tutorial:
                    Application.DrawList(_tutorialScreens[_currentTutorialScreen]);
                    break;

                case MenuScreens.Controllers:
                    Application.DrawList(_controllersRenderList);
                    break;

                default:
                    return;
            }
        }

        private void GenerateIntroScreen()
        {
            _intro = new Text("Fabricio Delboni", Application.Font, Application.WindowHeight / 20);
            _intro.Origin = new Vector2f(_intro.GetGlobalBounds().Width / 2f, _intro.GetGlobalBounds().Height / 2f);
            _intro.Position = new Vector2f(Application.WindowWidth / 2f, Application.WindowHeight / 9 * 4);

            _intro2 = new Text("presents", Application.Font, Application.WindowHeight / 20);
            _intro2.Origin = new Vector2f(_intro2.GetGlobalBounds().Width / 2f, _intro2.GetGlobalBounds().Height / 2f);
            _intro2.Position = new Vector2f(Application.WindowWidth / 2f, Application.WindowHeight / 9 * 5);
        }

        private bool _displayedIntro;
        private void IntroUpdate()
        {
            if (Application.Clock.ElapsedTime.AsSeconds() < 2.2f) return;

            if (!_displayedIntro)
            { 
                _introRenderList.Add(_intro);
                _introRenderList.Add(_intro2);
                _displayedIntro = true;
            }

            if (Application.Clock.ElapsedTime.AsSeconds() < 4.81f) return;

            SoundManager.MainMenuScreen();
            _currentScreen = MenuScreens.Main;
            Application.HasPlayedIntro = true;
        }

        private void GenerateMainScreen()
        {
            _gameTitle = new Text("Bomber Person", Application.Font, Application.WindowHeight / 12);
            _gameTitle.Origin = new Vector2f(_gameTitle.GetGlobalBounds().Width / 2, _gameTitle.GetGlobalBounds().Height / 2);
            _gameTitle.Position = new Vector2f(Application.WindowWidth / 2f, Application.WindowHeight / 9f * 2);
            _mainRenderList.Add(_gameTitle);

            _newGame = new Text("New Game", Application.Font, Application.WindowHeight / 20);
            _newGame.Origin = new Vector2f(_newGame.GetGlobalBounds().Width / 2, _newGame.GetGlobalBounds().Height / 2);
            _newGame.Position = new Vector2f(Application.WindowWidth / 2f, Application.WindowHeight / 9f * 4);
            _mainOptionsList.Add(_newGame);

            _tutorial = new Text("Tutorial", Application.Font, Application.WindowHeight / 20);
            _tutorial.Origin = new Vector2f(_tutorial.GetGlobalBounds().Width / 2, _tutorial.GetGlobalBounds().Height / 2);
            _tutorial.Position = new Vector2f(Application.WindowWidth / 2f, Application.WindowHeight / 9f * 5);
            _mainOptionsList.Add(_tutorial);

            _controllers = new Text("Controllers", Application.Font, Application.WindowHeight / 20);
            _controllers.Origin = new Vector2f(_controllers.GetGlobalBounds().Width / 2, _controllers.GetGlobalBounds().Height / 2);
            _controllers.Position = new Vector2f(Application.WindowWidth / 2f, Application.WindowHeight / 9f * 6);
            _mainOptionsList.Add(_controllers);

            _exit = new Text("Exit", Application.Font, Application.WindowHeight / 20);
            _exit.Origin = new Vector2f(_exit.GetGlobalBounds().Width / 2, _exit.GetGlobalBounds().Height / 2);
            _exit.Position = new Vector2f(Application.WindowWidth / 2f, Application.WindowHeight / 9f * 7);
            _mainOptionsList.Add(_exit);

            foreach (var option in _mainOptionsList)
            {
                _mainRenderList.Add(option);
            }
        }
        
        private void GenerateControllersScreen()
        {
            _controllersTitle = new Text("Controllers", Application.Font, Application.WindowHeight / 18);
            _controllersTitle.Origin = new Vector2f(_controllersTitle.GetGlobalBounds().Width / 2, _controllersTitle.Origin.Y);
            _controllersTitle.Position = new Vector2f(Application.WindowWidth / 2f, Application.WindowHeight / 10f);
            _controllersRenderList.Add(_controllersTitle);

            _p1 = new Text("   P1", Application.Font, Application.WindowHeight / 20);
            _p1.Position = new Vector2f(Application.WindowWidth / 9 * 3, Application.WindowHeight / 8 * 2);
            _controllersRenderList.Add(_p1);

            _p2 = new Text("   P2", Application.Font, Application.WindowHeight / 20);
            _p2.Position = new Vector2f(Application.WindowWidth / 9 * 6, Application.WindowHeight / 8 * 2);
            _controllersRenderList.Add(_p2);

            _move = new Text("Move: ", Application.Font, Application.WindowHeight / 20);
            _move.Position = new Vector2f(Application.WindowWidth / 9f, Application.WindowHeight / 8 * 3);
            _controllersRenderList.Add(_move);

            _moveP1 = new Text("Arrows", Application.Font, Application.WindowHeight / 20);
            _moveP1.Position = new Vector2f(Application.WindowWidth / 9 * 3, Application.WindowHeight / 8 * 3);
            _controllersRenderList.Add(_moveP1);

            _moveP2 = new Text("WASD", Application.Font, Application.WindowHeight / 20);
            _moveP2.Position = new Vector2f(Application.WindowWidth / 9 * 6, Application.WindowHeight / 8 * 3);
            _controllersRenderList.Add(_moveP2);

            _action = new Text("Bomb: ", Application.Font, Application.WindowHeight / 20);
            _action.Position = new Vector2f(Application.WindowWidth / 9f, Application.WindowHeight / 8 * 4);
            _controllersRenderList.Add(_action);

            _actionP1 = new Text("LCtrl", Application.Font, Application.WindowHeight / 20);
            _actionP1.Position = new Vector2f(Application.WindowWidth / 9 * 3, Application.WindowHeight / 8 * 4);
            _controllersRenderList.Add(_actionP1);

            _actionP2 = new Text("E", Application.Font, Application.WindowHeight / 20);
            _actionP2.Position = new Vector2f(Application.WindowWidth / 9 * 6, Application.WindowHeight / 8 * 4);
            _controllersRenderList.Add(_actionP2);

            _pause = new Text("Esc to pause", Application.Font, Application.WindowHeight / 20);
            _pause.Position = new Vector2f(Application.WindowWidth / 9f, Application.WindowHeight / 8 * 5);
            _controllersRenderList.Add(_pause);

            _return = new Text("Press Esc to return", Application.Font, Application.WindowHeight / 23);
            _return.Origin = new Vector2f(_return.GetGlobalBounds().Width / 2, _return.Origin.Y);
            _return.Position = new Vector2f(Application.WindowWidth / 2f, Application.WindowHeight / 9 * 8);
            _controllersRenderList.Add(_return);
        }

        private void GenerateConfigurationScreen()
        {
            /*
            _playersText = new Text("Map size:", Application.Font, Application.WindowHeight / 20);
            _playersText.Position = new Vector2f(Application.WindowWidth / 9 * 2, Application.WindowHeight / 8 * 1);
            _configurationRenderList.Add(_playersText);
            */

            _mapSizeText = new Text("Map size:", Application.Font, Application.WindowHeight / 20);
            _mapSizeText.Position = new Vector2f(Application.WindowWidth / 9 * 2, Application.WindowHeight / 8 * 2);
            _configurationRenderList.Add(_mapSizeText);

            _pointsText = new Text("Points goal:", Application.Font, Application.WindowHeight / 20);
            _pointsText.Position = new Vector2f(Application.WindowWidth / 9 * 2, Application.WindowHeight / 8 * 3);
            _configurationRenderList.Add(_pointsText);

            /*
            _playersValueText = new Text(_mapSize.ToString(), Application.Font, Application.WindowHeight / 20);
            _playersValueText.Position = new Vector2f(Application.WindowWidth / 9 * 6, Application.WindowHeight / 8 * 1);
            _configurationOptionsList.Add(_playersValueText);
            */

            _mapSizeValueText = new Text(_mapSize.ToString(), Application.Font, Application.WindowHeight / 20);
            _mapSizeValueText.Position = new Vector2f(Application.WindowWidth / 9 * 6, Application.WindowHeight / 8 * 2);
            _configurationOptionsList.Add(_mapSizeValueText);

            _pointsValueText = new Text(_points.ToString(), Application.Font, Application.WindowHeight / 20);
            _pointsValueText.Position = new Vector2f(Application.WindowWidth / 9 * 6, Application.WindowHeight / 8 * 3);
            _configurationOptionsList.Add(_pointsValueText);

            _startText = new Text("Start game", Application.Font, Application.WindowHeight / 20);
            _startText.Position = new Vector2f(Application.WindowWidth / 2f, Application.WindowHeight / 8 * 5);
            _startText.Origin = new Vector2f(_startText.GetGlobalBounds().Width / 2, _startText.Origin.Y);
            _configurationOptionsList.Add(_startText);

            foreach (var option in _configurationOptionsList)
            {
                _configurationRenderList.Add(option);
            }
        }

        private void GenerateTutorialScreen()
        {
            _tutorialScreens = new[] {_tutorialRenderList1, _tutorialRenderList2, _tutorialRenderList3};

            _enter = new Text("Press Enter to continue", Application.Font, Application.WindowHeight / 23);
            _enter.Origin = new Vector2f(_enter.GetGlobalBounds().Width / 2, _enter.Origin.Y);
            _enter.Position = new Vector2f(Application.WindowWidth / 2f, Application.WindowHeight / 9 * 8);
            _tutorialRenderList1.Add(_enter);
            _tutorialRenderList2.Add(_enter);
            _tutorialRenderList3.Add(_enter);

            _tutorial1 = new Text("Drop bombs to unleash\ndestruction power", Application.Font, Application.WindowHeight / 20);
            _tutorial1.Position = new Vector2f(Application.WindowWidth / 9f, Application.WindowHeight / 9f);
            _tutorialRenderList1.Add(_tutorial1);

            float scale1 = Application.WindowWidth / 1400f;
            _sprite1 = new Sprite(new Texture("tutorial1.jpg"));
            _sprite1.Scale = new Vector2f(scale1, scale1);
            _sprite1.Position = new Vector2f(Application.WindowWidth / 9 * 3, Application.WindowHeight / 10 * 3);
            _tutorialRenderList1.Add(_sprite1);

            _sprite2 = new Sprite(new Texture("tutorial2.jpg"));
            _sprite2.Scale = new Vector2f(scale1, scale1);
            _sprite2.Position = new Vector2f(Application.WindowWidth / 9 * 5, Application.WindowHeight / 10 * 3);
            _tutorialRenderList1.Add(_sprite2);

            _tutorial2 = new Text("Fire will destroy brick\nwalls, items, your\nrivals and you!", Application.Font, Application.WindowHeight / 20);
            _tutorial2.Position = new Vector2f(Application.WindowWidth / 9f, Application.WindowHeight / 9 * 5);
            _tutorialRenderList1.Add(_tutorial2);

            _tutorial3 = new Text("Items will boost your skills", Application.Font, Application.WindowHeight / 20);
            _tutorial3.Origin = new Vector2f(_tutorial3.GetGlobalBounds().Width / 2, _tutorial3.GetGlobalBounds().Height / 2);
            _tutorial3.Position = new Vector2f(Application.WindowWidth / 2f, Application.WindowHeight / 9f);
            _tutorialRenderList2.Add(_tutorial3);

            float scale2 = Application.WindowWidth / 300f;
            _sprite3 = new Sprite(new Texture("itemFireSprite.png"));
            _sprite3.Scale = new Vector2f(scale2, scale2);
            _sprite3.Position = new Vector2f(Application.WindowWidth / 9f, Application.WindowHeight / 9 * 2);
            _tutorialRenderList2.Add(_sprite3);

            _sprite4 = new Sprite(new Texture("itemBombSprite.png"));
            _sprite4.Scale = new Vector2f(scale2, scale2);
            _sprite4.Position = new Vector2f(Application.WindowWidth / 9f, Application.WindowHeight / 9 * 4);
            _tutorialRenderList2.Add(_sprite4);

            _sprite5 = new Sprite(new Texture("itemSpeedSprite.png"));
            _sprite5.Scale = new Vector2f(scale2, scale2);
            _sprite5.Position = new Vector2f(Application.WindowWidth / 9f, Application.WindowHeight / 9 * 6);
            _tutorialRenderList2.Add(_sprite5);

            _tutorial4 = new Text("Fire range +1", Application.Font, Application.WindowHeight / 20);
            _tutorial4.Origin = new Vector2f(_tutorial4.Origin.X, _tutorial4.GetGlobalBounds().Height / 2);
            _tutorial4.Position = new Vector2f(Application.WindowWidth / 10 * 3, Application.WindowHeight / 9f * 2.6f);
            _tutorialRenderList2.Add(_tutorial4);

            _tutorial5 = new Text("Droppable bombs +1", Application.Font, Application.WindowHeight / 20);
            _tutorial5.Origin = new Vector2f(_tutorial5.Origin.X, _tutorial5.GetGlobalBounds().Height / 2);
            _tutorial5.Position = new Vector2f(Application.WindowWidth / 10 * 3, Application.WindowHeight / 9f * 4.6f);
            _tutorialRenderList2.Add(_tutorial5);

            _tutorial6 = new Text("Increased speed", Application.Font, Application.WindowHeight / 20);
            _tutorial6.Origin = new Vector2f(_tutorial6.Origin.X, _tutorial6.GetGlobalBounds().Height / 2);
            _tutorial6.Position = new Vector2f(Application.WindowWidth / 10 * 3, Application.WindowHeight / 9f * 6.6f);
            _tutorialRenderList2.Add(_tutorial6);

            _tutorial7 = new Text("Last player standing\nwins the round", Application.Font, Application.WindowHeight / 20);
            _tutorial7.Position = new Vector2f(Application.WindowWidth / 9f, Application.WindowHeight / 9f);
            _tutorialRenderList3.Add(_tutorial7);

            _tutorial8 = new Text("Reach the goal and\nand become the new", Application.Font, Application.WindowHeight / 20);
            _tutorial8.Position = new Vector2f(Application.WindowWidth / 9f, Application.WindowHeight / 9 * 3);
            _tutorialRenderList3.Add(_tutorial8);

            _tutorial9 = new Text("Champerson", Application.Font, Application.WindowHeight / 10);
            _tutorial9.Origin = new Vector2f(_tutorial9.GetGlobalBounds().Width / 2, _tutorial9.GetGlobalBounds().Height / 2);
            _tutorial9.Position = new Vector2f(Application.WindowWidth / 2f, Application.WindowHeight / 9 * 6);
            _tutorial9.FillColor = Color.Yellow;
            _tutorialRenderList3.Add(_tutorial9);
        }

        protected override void Select()
        {
            if (Application.Clock.ElapsedTime.AsSeconds() < NextPress) return;
            NextPress = Application.Clock.ElapsedTime.AsSeconds() + CooldownRate;

            SoundManager.UiSelect();

            if (_currentScreen == MenuScreens.Main)
            {
                switch (CurrentOption)
                {
                    case 0:
                        _currentScreen = MenuScreens.Configuration;
                        CurrentOptionsList = _configurationOptionsList;
                        CurrentOption = 0;
                        UpdateSelection(0, true);
                        break;

                    case 1:
                        _currentTutorialScreen = 0;
                        _currentScreen = MenuScreens.Tutorial;
                        break;

                    case 2:
                        _currentScreen = MenuScreens.Controllers;
                        break;

                    case 3:
                        Application.EndApplication(this, EventArgs.Empty);
                        break;

                    default:
                        return;
                }
            }
            else
            {
                if (CurrentOption == _configurationOptionsList.Count - 1)
                {
                    SoundManager.StopMusic();
                    Application.NewGame(_numberOfPlayers, _mapSize + 2, _points);
                }
            }
        }

        private void IncrementSelection(int direction)
        {
            if (Application.Clock.ElapsedTime.AsSeconds() < NextPress) return;
            NextPress = Application.Clock.ElapsedTime.AsSeconds() + CooldownRate;

            switch (CurrentOption)
            {
                //El juego está diseñado para ser jugado de hasta 4 jugadores, pero lo estoy limitando a dos
                //porque no llegué a implementar el indispensable uso de joysticks como input

                /*
                    case 0:
                    _numberOfPlayers += direction * 2;

                    if (_numberOfPlayers < _minPlayers) _numberOfPlayers = _maxPlayers;
                    else if (_numberOfPlayers > _maxPlayers) _numberOfPlayers = _minPlayers;

                    _mapSizeValueText.DisplayedString = _numberOfPlayers.ToString();
                    break;
                */

                case 0:
                    _mapSize += direction * 2;

                    if (_mapSize < MinSize) _mapSize = MaxSize;
                    else if (_mapSize > MaxSize) _mapSize = MinSize;

                    _mapSizeValueText.DisplayedString = _mapSize.ToString();
                    break;

                case 1:
                    _points += direction;
                    if (_points < MinPoints) _points = MaxPoints;
                    else if (_points > MaxPoints) _points = MinPoints;

                    _pointsValueText.DisplayedString = _points.ToString();
                    break;

                default:
                    return;
            }
        }

        private void NextTutorialScreen()
        {
            if (Application.Clock.ElapsedTime.AsSeconds() < NextPress) return;
            NextPress = Application.Clock.ElapsedTime.AsSeconds() + CooldownRate;

            if (_currentTutorialScreen == _tutorialScreens.Length - 1) _currentScreen = MenuScreens.Main;
            else _currentTutorialScreen++;
        }
    }
}
