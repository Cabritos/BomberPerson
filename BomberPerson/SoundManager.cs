using System;
using System.Collections.Generic;
using System.Text;
using SFML.Audio;

namespace BomberPerson.Assets
{
    class SoundManager
    {
        private int _musicVolume = 70;
        private int _soundVolume = 100;

        private readonly Music _levelMusic = new Music("levelMusic.ogg");
        private readonly Music _menuMusic = new Music("menuMusic.ogg");
        private readonly Music _victoryMusic = new Music("victoryMusic.ogg");

        private Music _currentMusic;

        private readonly SoundBuffer _intro = new SoundBuffer("intro.ogg");

        private readonly SoundBuffer _uiMove = new SoundBuffer("uiMove.ogg");
        private readonly SoundBuffer _uiSelect = new SoundBuffer("uiSelect.ogg");

        private readonly SoundBuffer _playerDie = new SoundBuffer("playerDie.ogg");
        private readonly SoundBuffer _itemPick = new SoundBuffer("itemPick.ogg");

        private readonly SoundBuffer _explosion1 = new SoundBuffer("explosion1.ogg");
        private readonly SoundBuffer _explosion2 = new SoundBuffer("explosion2.ogg");
        private readonly SoundBuffer _explosion3 = new SoundBuffer("explosion3.ogg");
        private readonly SoundBuffer _explosion4 = new SoundBuffer("explosion4.ogg");

        private readonly SoundBuffer _startRound = new SoundBuffer("fluteIntro.ogg");
        private readonly SoundBuffer _endRound = new SoundBuffer("endRound.ogg");
        private readonly SoundBuffer _endGame = new SoundBuffer("endGame.ogg");

        private readonly Sound _introClip;
        private readonly Sound _uiMoveClip;
        private readonly Sound _uiSelectClip;
        private readonly Sound _playerDieClip;
        private readonly Sound _pickUpClip;
        private readonly Sound _explosionClip1;
        private readonly Sound _explosionClip2;
        private readonly Sound _explosionClip3;
        private readonly Sound _explosionClip4;
        private readonly Sound _startRoundClip;
        private readonly Sound _endRoundClip;
        private readonly Sound _endGameClip;

        private readonly List<Sound> _soundsList;
        private readonly List<Sound> _introsList;
        private readonly List<Music> _musicList;

        public SoundManager()
        {
            _currentMusic = _menuMusic;

            _soundsList = new List<Sound>();
            _introsList = new List<Sound>();
            _musicList = new List<Music>();

            _introClip = new Sound(_intro);
            _introsList.Add(_introClip);

            _endGameClip = new Sound(_endGame);
            _introsList.Add(_endGameClip);

            _uiMoveClip = new Sound(_uiMove);
            _soundsList.Add(_uiMoveClip);

            _uiSelectClip = new Sound(_uiSelect);
            _soundsList.Add(_uiSelectClip);

            _playerDieClip = new Sound(_playerDie);
            _soundsList.Add(_playerDieClip);

            _pickUpClip = new Sound(_itemPick);
            _soundsList.Add(_pickUpClip);

            _explosionClip1 = new Sound(_explosion1);
            _soundsList.Add(_explosionClip1);

            _explosionClip2 = new Sound(_explosion2);
            _soundsList.Add(_explosionClip2);

            _explosionClip3 = new Sound(_explosion3);
            _soundsList.Add(_explosionClip3);

            _explosionClip4 = new Sound(_explosion4);
            _soundsList.Add(_explosionClip4);

            _startRoundClip = new Sound(_startRound);
            _soundsList.Add(_startRoundClip);

            _endRoundClip = new Sound(_endRound);
            _soundsList.Add(_endRoundClip);

            foreach (var sound in _soundsList)
            {
                sound.Volume = _soundVolume;
            }

            foreach (var sound in _introsList)
            {
                sound.Volume = _musicVolume;
            }

            _musicList.Add(_menuMusic);
            _musicList.Add(_levelMusic);
            _musicList.Add(_victoryMusic);

            foreach (var music in _musicList)
            {
                music.Volume = _musicVolume;
                music.Loop = true;
            }
        }
        
        public void PickItem()
        {
            _pickUpClip.Play();
        }

        public void PlayerDie()
        {
            _playerDieClip.Play();
        }

        public void Explotion()
        {
            Random random = new Random();

            Sound explosion;

            switch (random.Next(0, 3))
            {
                case 0:
                    explosion = _explosionClip1;
                    break;

                case 1:
                    explosion = _explosionClip2;
                    break;

                case 2:
                    explosion = _explosionClip3;
                    break;

                default:
                    explosion = _explosionClip4;
                    return;
            }

            explosion.Play();
        }

        public void PlayIntro()
        {
            _introClip.Play();
        }

        public void StartRound()
        {
            foreach (var sound in _soundsList) { sound.Volume = _soundVolume; }
            _startRoundClip.Play();
        }

        public void EndRound()
        {
            foreach (var sound in _soundsList) sound.Volume = _soundVolume * 0.1f;
            _endRoundClip.Volume = _soundVolume * 1.5f;
            _endRoundClip.Play();
        }

        public void EndGame()
        {
           foreach (var sound in _soundsList) sound.Volume = _soundVolume * 0.1f;
           _endGameClip.Play();
        }

        public void Pause()
        {
            foreach (var sound in _soundsList) if (sound.Status == SoundStatus.Playing) sound.Pause();
            _levelMusic.Volume = _musicVolume * 0.7f;
        }

        public void UnPause()
        {
            foreach (var sound in _soundsList) if (sound.Status == SoundStatus.Paused) sound.Play();
            _levelMusic.Volume = _musicVolume;
        }

        public void UiMove()
        {
            _uiMoveClip.Play();
        }

        public void UiSelect()
        {
            _uiSelectClip.Play();
        }

        public void LevelMusic()
        {
            StopMusic();
            _levelMusic.Play();
            _currentMusic = _levelMusic;
        }

        public void MainMenuScreen()
        {
            StopMusic();
            _menuMusic.Play();
            _currentMusic = _menuMusic;

            foreach (var sound in _soundsList) { sound.Volume = _soundVolume; }
        }

        public void VictoryMusic()
        {
            StopMusic();
            _victoryMusic.Play();
            _currentMusic = _victoryMusic;
        }

        public void StopMusic()
        {
            _currentMusic.Stop();
        }
    }
}
