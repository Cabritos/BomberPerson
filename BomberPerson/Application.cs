using System;
using SFML.Graphics;
using SFML.Window;
using System.Collections.Generic;
using BomberPerson.Assets;
using SFML.System;

namespace BomberPerson
{
    static class Application
    {
        //Gestiona la ventana, el game loop y objetos de uso global

        public static uint WindowHeight { get; private set; }
        public static uint WindowWidth { get; private set; }

        private const string GameTitle = "Bomber Person";

        private static RenderWindow _renderWindow;

        private static bool _onApplication = true;

        public static bool HasPlayedIntro;

        private static MainMenu _menu;
        private static Game _game;
        
        private static bool _onGame; //versión micro de un state manager general

        public static SoundManager SoundManager { get; } = new SoundManager();
        public static Clock Clock { get; private set; }

        public static bool Debug = false;

        public static Font Font;


        public static void Start()
        {
            WindowSetup(1024, 768);

            Clock = new Clock();
            Font = new Font("font.ttf");

            MainMenu();

            Update();
            }


        private static void WindowSetup(uint width, uint height)
        {
            WindowWidth = width;
            WindowHeight = height;

            _renderWindow = new RenderWindow(new VideoMode(WindowWidth, WindowHeight), GameTitle);
            _renderWindow.SetFramerateLimit(60);

            _renderWindow.Closed += EndApplication;
        }

        public static void MainMenu()
        {
            _onGame = false;
            _menu = new MainMenu();
        }

        public static void NewGame(int numberOfPlayers, int mapSize, int pointsGoal)
        {
            _onGame = true;
            _game = new Game(numberOfPlayers, mapSize, pointsGoal);
        }

        private static void Update()
        {
            while (_onApplication)
            {
                _renderWindow.DispatchEvents();
                TakeScreenshot();
                StateUpdate();
                Render();
            }
        }
        
        private static void StateUpdate()
        {
            if (_onGame)
            {
                _game.Update();
            }
            else
            {
                _menu.Update();
            }
        }

        private static void Render()
        {
            _renderWindow.Clear(Color.Black);

            if (_onGame)
            {
                _game.Render();
            }
            else
            {
                _menu.Render();
            }

            _renderWindow.Display();
        }

        public static void DrawList(List<Drawable> list)
        {
            foreach (var drawable in list)
            {
                _renderWindow.Draw(drawable);
            }
        }

        private static float _cooldownRate = 0.5f;
        private static float _nextPress;
        private static int _screenId;

        private static void TakeScreenshot()
        {
            //Secret feature!
            if (!Keyboard.IsKeyPressed(Keyboard.Key.P)) return;

            if (Clock.ElapsedTime.AsSeconds() < _nextPress) return;

            _nextPress = Clock.ElapsedTime.AsSeconds() + _cooldownRate;

            var texture = new Texture(WindowWidth, WindowHeight);

            texture.Update(_renderWindow);

            texture.CopyToImage().SaveToFile($"screenshot{_screenId}.jpg");

            _screenId++;
        }

        public static void EndApplication(object sender, EventArgs e)
        {
            _onApplication = false;
            _renderWindow.Close();
        }
    }
}