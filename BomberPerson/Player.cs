using System.Collections.Generic;
using BomberPerson.Assets;
using SFML.Audio;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace BomberPerson
{
    class Player : GameObject
    {
        public int PlayerNumber { get; }

        private float _scale = 0.8f; //escala del pj en relación al tilesize (0-1)
        private readonly Vector2f _scaleOffset;

        private FloatRect _globalBounds;

        private readonly List<IntRect> _sprites;

        private float _speed;
        private float _speedFactor = 35;

        private int _bombRange = 2;

        private int _maxBombs = 2;
        private int _droppedBombs;
        private Bomb _lastBomb;
        private Bomb _publicBomb1;
        private Bomb _publicBomb2;

        public bool IsAlive { get; private set; }

        private readonly InputManager _inputManager;

        public Player(Level level, int playerNumber) : base(level)
        {
            PlayerNumber = playerNumber;

            _scaleOffset = new Vector2f(Level.TileSize / 2 * (1 - _scale), Level.TileSize / 2 * (1 - _scale));
            FloatPosition = GetStartingPosition(Level, playerNumber);

            var sideLength = Level.TileSize * _scale;

            SetSpeed();

            Collider = new RectangleShape(new Vector2f(sideLength, sideLength));
            Collider.FillColor = SFML.Graphics.Color.Blue;
            Collider.Position = FloatPosition;

            Texture = new Texture("player.png");
            CreateSprite();
            Sprite.Scale = new Vector2f(sideLength / Texture.Size.X, sideLength / Texture.Size.X);

            _sprites = new List<IntRect>();
            _sprites.Add(new IntRect(0, 0, 31, 31));  //0: down
            _sprites.Add(new IntRect(0, 31, 31, 31)); //1: left
            _sprites.Add(new IntRect(0, 62, 31, 31)); //2: right
            _sprites.Add(new IntRect(0, 93, 31, 31)); //3: up

            Sprite.TextureRect = _sprites[0];

            IsAlive = true;

            _inputManager = playerNumber switch
            {
                0 => new InputManager(Keyboard.Key.Up, Keyboard.Key.Down, Keyboard.Key.Left, Keyboard.Key.Right, Keyboard.Key.RControl),
                1 => new InputManager(Keyboard.Key.W, Keyboard.Key.S, Keyboard.Key.A, Keyboard.Key.D, Keyboard.Key.E),
                _ => new InputManager(Keyboard.Key.W, Keyboard.Key.S, Keyboard.Key.A, Keyboard.Key.D, Keyboard.Key.E)
            };
        }

        private Vector2f GetStartingPosition(Level level, int startingPosition)
        {
            return startingPosition switch
            {
                0 => level.GridToFloat(1, 1) + _scaleOffset,
                1 => level.GridToFloat(level.MapSize - 2, level.MapSize - 2) + _scaleOffset,
                2 => level.GridToFloat(level.MapSize - 2, 1) + _scaleOffset,
                _ => level.GridToFloat(1, level.MapSize - 2) + _scaleOffset,
            };
        }

        public void Update()
        {
            if (!IsAlive) return;

            Move();
            DropBomb();
            //if (_inputManager.Pause()) Level.Round.SwitchPause();

            _globalBounds = GetBounds();
            CheckFire();
            CheckItems();
        }

        private void Move()
        {
            if (_inputManager.Up())
            {
                FloatPosition.Y -= _speed;
                if (CheckCollisions()) FloatPosition.Y += _speed;
                else Sprite.TextureRect = _sprites[3];
            }

            if (_inputManager.Down())
            {
                FloatPosition.Y += _speed;
                if (CheckCollisions()) FloatPosition.Y -= _speed;
                else Sprite.TextureRect = _sprites[0];
            }

            if (_inputManager.Left())
            {
                FloatPosition.X -= _speed;
                if (CheckCollisions()) FloatPosition.X += _speed;
                else Sprite.TextureRect = _sprites[1];
            }

            if (_inputManager.Right())
            {
                FloatPosition.X += _speed;
                if (CheckCollisions()) FloatPosition.X -= _speed;
                else Sprite.TextureRect = _sprites[2];
            }

            Collider.Position = FloatPosition;
            Sprite.Position = FloatPosition;
        }

        private bool CheckCollisions()
        {
            Collider.Position = FloatPosition;

            var playerGlobalBounds = GetBounds();

            return CheckWallCollisions(playerGlobalBounds) || CheckBombCollisions(playerGlobalBounds);
        }

        private bool CheckWallCollisions(FloatRect playerGlobalBounds)
        {
            foreach (var collider in Level.MapGlobalBounds)
            {
                if (playerGlobalBounds.Intersects(collider)) return true;
            } 

            return false;
        }

        private bool CheckBombCollisions(FloatRect playerGlobalBounds)
        {
            /*
             * La última bomba dejada por el juegador queda identificada como _lastBomb.
             * Si otro jugador deja una bomba que interseca con el jugador, este la registra como _publicBomb1.
             * No es posible estar ubicado sobre más de dos bombas a la vez.
             * Sí es posible que ambas bombas pertenezcan a enemigos.
             * En ese caso, la segunda bomba se registra como _publicBomb2.
             *
             * El jugador puede caminar sobre cualquiera de esas bombas en tanto interseque con ellas.
             * Cuando deja de estar encima, la variable se nullea y la bomba se considera un obstáculo más.
             */

            var touchingBombs = 0;

            var touchesOwn = false;
            var touchesPublic1 = false;
            var touchesPublic2 = false;

            foreach (var collider in Level.BombsGlobalBounds)
            {
                if (!playerGlobalBounds.Intersects(collider)) continue;

                touchingBombs++;
                
                if (_lastBomb != null && collider == _lastBomb.GetBounds())
                {
                    touchingBombs--;
                    touchesOwn = true;
                }
                else

                if (_publicBomb1 != null && collider == _publicBomb1.GetBounds())
                {
                    touchingBombs--;
                    touchesPublic1 = true;
                }

                if (_publicBomb2 != null && collider == _publicBomb2.GetBounds())
                {
                    touchingBombs--;
                    touchesPublic2 = true;
                }
            }

            if (!touchesOwn) { _lastBomb = null; }
            if (!touchesPublic1) { _publicBomb1 = null; }
            if (!touchesPublic2) { _publicBomb2 = null; }

            return touchingBombs > 0;
        }

        public void RegisterPublicBomb(Bomb bomb)
        {
            if (_publicBomb1 == null) _publicBomb1 = bomb;
            else _publicBomb2 = bomb;
        }


        private void DropBomb()
        {
            if (!_inputManager.Action()) return;

            if (_droppedBombs >= _maxBombs) return;

            //Este bloque evita el dropeo mientras se esté situado encima de otra bomba
            var collides = false;

            Collider.Position = FloatPosition;

            var playerGlobalBounds = GetBounds();

            foreach (var collider in Level.BombsGlobalBounds)
            {
                if (playerGlobalBounds.Intersects(collider))
                {
                    collides = true;
                    break;
                }
            }

            if (collides) return;
            //

            var bomb = Level.DropBomb(this, FloatPosition - _scaleOffset, _bombRange);

            if (bomb == null) return;

            _droppedBombs++;

            _lastBomb = bomb;
        }

        public void RemoveBomb()
        {
            _droppedBombs--;
        }
        
        private void CheckFire()
        {
            foreach (var fireCollider in Level.FireGlobalBounds)
            {
                if (_globalBounds.Intersects(fireCollider)) Burn();
            }
        }

        private void CheckItems()
        {
            foreach (var itemCollider in Level.ItemsGlobalBounds)
            {
                if (_globalBounds.Intersects(itemCollider.Key))
                {
                    itemCollider.Value.PickUp(this);
                    break;
                }
            }
        }

        private void SetSpeed()
        {
            _speed = Level.TileSize / _speedFactor;
        }
        
        public void IncreaseSpeed()
        {
            _speedFactor -= 2;
            SetSpeed();
        }

        public void IncreaseFireRange()
        {
            _bombRange++;
        }

        public void IncreaseBombCapacity()
        {
            _maxBombs++;
        }
        
        private void Burn()
        {
            IsAlive = false;
            SoundManager.PlayerDie(); //TODO evento
            Level.Round.PlayerDeath(this);
        }
    }
}
