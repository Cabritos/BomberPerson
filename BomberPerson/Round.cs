using SFML.Graphics;
using System.Collections.Generic;
using BomberPerson.Assets;
using SFML.System;
using SFML.Window;

namespace BomberPerson
{

    class Round
    {
        public Game Game { get; }
        private readonly Level _level;

        public List<Player> Players { get; } = new List<Player>();
        private readonly List<Drawable> _playersRenderList = new List<Drawable>();

        private int _playersAlive = 0;

        private readonly List<LevelObject> _levelObjectUpdates = new List<LevelObject>();
        private readonly List<LevelObject> _toBeAddedFromUpdate = new List<LevelObject>();
        private readonly List<LevelObject> _toBeRemovedFromUpdate = new List<LevelObject>();

        private bool _playing;
        private bool _onPause;
        private float _cooldownRate = 1f;
        private float _nextPress;
        private float _pauseSet;
        private float _pausedTime;
        private readonly PauseMenu _pauseMenu;

        private readonly SoundManager _soundManager = Application.SoundManager;

        public Round(Game game, int numberOfPlayers, int size)
        {
            Game = game;
            _level = new Level(this, size);

            _playersAlive = numberOfPlayers;

            for (int i = 0; i < numberOfPlayers; i++)
            {
                Players.Add(new Player(_level, i));
            }

            foreach (var player in Players)
            {
                _playersRenderList.Add(player.GetDrawable());
            }

            _playing = false;
            _pauseMenu = new PauseMenu(this);
        }

        public void StartRound()
        {
            _playing = true;
            _soundManager.UnPause();
            _soundManager.LevelMusic();
        }
        
        public void Update()
        {
            if (!_playing) return;

            PauseInput();

            if (_onPause)
            {
                _pauseMenu.Update();
                return;
            }
            
            foreach (var player in Players)
            {
                player.Update();
            }

            RemoveFormUpdateList();

            foreach (var levelObject in _levelObjectUpdates)
            {
                levelObject.Update();
            }

            AddToUpdateList();

            if (_playersAlive <= 1) EndRound();
        }

        private void PauseInput()
        {
            if (Keyboard.IsKeyPressed(Keyboard.Key.Escape) && Application.Clock.ElapsedTime.AsSeconds() > _nextPress)
            {
                _nextPress = Application.Clock.ElapsedTime.AsSeconds() + _cooldownRate;
                SwitchPause();
            }
        }

        public void SwitchPause()
        {
            _onPause = !_onPause;

            if (_onPause)
            {
                _soundManager.Pause();
                _pauseSet = Application.Clock.ElapsedTime.AsSeconds();
            }
            else
            {
                _soundManager.UnPause();
                _pausedTime += Application.Clock.ElapsedTime.AsSeconds() - _pauseSet;
            }
        }

        public float GetElapsedTime()
        {
            return Application.Clock.ElapsedTime.AsSeconds() - _pausedTime;
        }

        //Para evitar las excepciones derivadas de incorporar y remover objetos a una única lista de Update de la ronda durante el gameplay,
        //se incorpora  una lista de ejecución anterior y posterior: _toBeAddedFromUpdate y _toBeRemovedFromUpdate.

        private void AddToUpdateList()
        {
            foreach (var levelObject in _toBeAddedFromUpdate)
            {
                _levelObjectUpdates.Add(levelObject);
            }
            _toBeAddedFromUpdate.Clear();
        }

        private void RemoveFormUpdateList()
        {
            foreach (var levelObject in _toBeRemovedFromUpdate)
            {
                _levelObjectUpdates.Remove(levelObject);
            }
            _toBeRemovedFromUpdate.Clear();
        }

        public void AddToUpdate(LevelObject levelObject)
        {
            _toBeAddedFromUpdate.Add(levelObject);
        }

        public void RemoveFromUpdate(LevelObject levelObject)
        {
            _toBeRemovedFromUpdate.Add(levelObject);
        }

        public void Render()
        {
            if (!_onPause)
            {
                Application.DrawList(_level.MapRenderList);
                Application.DrawList(_level.LevelObjectsRenderList);
                Application.DrawList(_playersRenderList);
            }

            if (_playing && _onPause) _pauseMenu.Render();
        }

        public void PlayerDeath(Player player)
        {
            _playersAlive--;
            _playersRenderList.Remove(player.GetDrawable());
        }

        public void EndRound()
        {
            _soundManager.EndRound();
            _soundManager.StopMusic();

            var winner = 5;

            foreach (var player in Players)
            {
                if (player.IsAlive)
                {
                    winner = player.PlayerNumber;
                }
            }

            Game.EndOfRound(winner);
        }
    }
}
