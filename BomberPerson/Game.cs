using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Text;
using BomberPerson.Assets;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace BomberPerson
{
    enum GameState
    {
        Start,
        Play,
        EndRound,
        EndGame
    }

    class Game
    {
        //Gestiona la partida, entendida como conjunto de rondas hasta alcanzar el puntaje objetivo

        private int _numberOfPlayers;
        private int _mapSize;

        private int _goalPoints;
        private Score[] _scores;
        private List<Drawable> _uiRenderList = new List<Drawable>();

        private Round _currentRound;
        private bool _renderRound = true;

        private Text _centralText;

        private GameState _currentState;

        private float _markTime;

        private SoundManager _soundManager;
        private bool _startClipPlayed;

        public Game(int numberOfPlayers, int mapSize, int goalPoints)
        {
            //La clase hace su propio control de los parámetros, con independencia del que se hace desde MainMenu
            if (numberOfPlayers < 2) _numberOfPlayers = 2;
            else if (numberOfPlayers > 4) _numberOfPlayers = 4;
            else _numberOfPlayers = numberOfPlayers;

            if (mapSize < 9)
            {
                mapSize = 9;
            }

            if (mapSize % 2 == 1)
            {
                _mapSize = mapSize;
            }
            else
            {
                _mapSize = mapSize + 1;
            }
            //
            
            float offset = ((Application.WindowWidth - (Application.WindowHeight / (_mapSize + 2f) * _mapSize)) / 8);
            _goalPoints = goalPoints;
            _scores = new Score[_numberOfPlayers];
            for (int i = 0; i < _numberOfPlayers; i++)
            {
                _scores[i] = new Score(i, goalPoints, offset);
            }

            GenerateUI();

            NewRound();

            _soundManager = Application.SoundManager;
        }

        private void NewRound()
        {
            _currentState = GameState.Start;
            _currentRound = new Round(this, _numberOfPlayers, _mapSize);
            _renderRound = true;
            _markTime = Application.Clock.ElapsedTime.AsSeconds();
            SetCentralText("");
            _centralText.FillColor = Color.Black;
            _startClipPlayed = false;
        }

        public void Update()
        {
            if (Application.Debug) _centralText.DisplayedString = _currentRound.GetElapsedTime().ToString();

            switch (_currentState)
            {
                case GameState.Start:
                    StartCountdown();
                    break;

                case GameState.Play:
                    _currentRound.Update();
                    break;

                case GameState.EndRound:
                    EndRound();
                    break;

                case GameState.EndGame:
                    EndGameUpdate();
                    break;
            }
        }

        public void Render()
        {
            if (_renderRound)_currentRound.Render();
            Application.DrawList(_uiRenderList);
        }

        public void EndOfRound(int winner)
        {
            _markTime = Application.Clock.ElapsedTime.AsSeconds();
            _renderRound = false;
            _centralText.FillColor = Color.White;

            if (winner == 5)
            {
                _currentState = GameState.EndRound;
                SetCentralText("Tie");
                _soundManager.EndRound();
                return;
            }
            
            if (_scores[winner].AddPoint()) EndGame();
            else
            {
                _currentState = GameState.EndRound;
                SetCentralText($"Player\n {winner + 1} wins");
                _soundManager.EndRound();
            }
        }

        private void EndGame()
        {
            _currentState = GameState.EndGame;
            _soundManager.EndGame();

            for (int i = 0; i < _numberOfPlayers; i++)
            {
                if (_scores[i].Points == _goalPoints)
                {
                    _uiRenderList.Clear();

                    var line1 = new Text($"Player {i + 1}", Application.Font, Application.WindowHeight / 7);
                    line1.FillColor = Color.Yellow;
                    line1.Origin = new Vector2f(line1.GetGlobalBounds().Width / 2, line1.GetGlobalBounds().Height / 2);
                    line1.Position = new Vector2f(Application.WindowWidth / 2, Application.WindowHeight / 8 * 3);
                    _uiRenderList.Add(line1);

                    var line2 = new Text("is the new", Application.Font, Application.WindowHeight / 9);
                    line2.FillColor = Color.Yellow;
                    line2.Position = new Vector2f(Application.WindowWidth / 2, Application.WindowHeight / 2);
                    line2.Origin = new Vector2f(line2.GetGlobalBounds().Width / 2, line2.GetGlobalBounds().Height / 2);
                    _uiRenderList.Add(line2);

                    var line3 = new Text("champerson", Application.Font, Application.WindowHeight / 10);
                    line3.FillColor = Color.Yellow;
                    line3.Origin = new Vector2f(line3.GetGlobalBounds().Width / 2, line3.GetGlobalBounds().Height / 2);
                    line3.Position = new Vector2f(Application.WindowWidth / 2, Application.WindowHeight / 8 * 5);
                    _uiRenderList.Add(line3);
                }

                _centralText.FillColor = Color.Yellow;
            }
        }

        private bool _playedVictoryMusic;
        private void EndGameUpdate()
        {
            if (Keyboard.IsKeyPressed(Keyboard.Key.Escape) ||
                Keyboard.IsKeyPressed(Keyboard.Key.Enter) ||
                Keyboard.IsKeyPressed(Keyboard.Key.E) ||
                Keyboard.IsKeyPressed(Keyboard.Key.LControl))
            {
                Application.MainMenu();
            }

            if (!_playedVictoryMusic)
            {
                if (Application.Clock.ElapsedTime.AsSeconds() - _markTime > 4.5f)
                {
                    _soundManager.VictoryMusic();
                    _playedVictoryMusic = true;
                }
            }
        }

        private void GenerateUI()
        {
            for (int i = 0; i < _numberOfPlayers; i++)
            {
                _uiRenderList.Add(_scores[i].ScoreText);
            }

            _centralText = new Text("", Application.Font, Application.WindowHeight / 8);
            _centralText.FillColor = Color.Black;
            _centralText.Origin = new Vector2f(_centralText.GetGlobalBounds().Width / 2, _centralText.GetGlobalBounds().Height / 2);
            _centralText.Position = new Vector2f(Application.WindowWidth / 2, Application.WindowHeight / 2);
            _uiRenderList.Add(_centralText);
        }

        private void SetCentralText(string text)
        {
            _centralText.DisplayedString = text;
            _centralText.Origin = new Vector2f(_centralText.GetGlobalBounds().Width / 2, _centralText.GetGlobalBounds().Height / 2);
        }

        private void StartCountdown()
        {
            //este método es un tanto feo, repite en cada frame cosas que deberían ocurrir una única vez

            if (Application.Clock.ElapsedTime.AsSeconds() -_markTime > 1) SetCentralText("3");
            if (Application.Clock.ElapsedTime.AsSeconds() - _markTime > 2) SetCentralText("2");
            if (Application.Clock.ElapsedTime.AsSeconds() - _markTime > 3) SetCentralText("1");
            if (Application.Clock.ElapsedTime.AsSeconds() - _markTime > 4)
            {
                SetCentralText("Start");
                if (!_startClipPlayed)
                {
                    _soundManager.StartRound();
                    _startClipPlayed = true;
                }
            }

            if (Application.Clock.ElapsedTime.AsSeconds() - _markTime < 5) return;
            
            SetCentralText("");
            _currentState = GameState.Play;
            _currentRound.StartRound();
        }

        private void EndRound()
        {
            if (Application.Clock.ElapsedTime.AsSeconds() - _markTime > 3) NewRound();
        }
    }
}