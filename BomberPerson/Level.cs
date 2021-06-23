using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using BomberPerson.Assets;

namespace BomberPerson
{
    class Level
    {
        //Genera y gestiona el mapa y los objetos que lo pueblan

        public Round Round { get; }

        public int MapSize { get; }
        private LevelObjects[,] _mapLayout;
        private Dictionary<Tuple<int, int>, LevelObject> _level;

        private float _offsetX;
        public float TileSize { get; private set; }

        public List<FloatRect> MapGlobalBounds { get; private set; }

        public List<FloatRect> BombsGlobalBounds { get; private set; }
        public List<FloatRect> FireGlobalBounds { get; private set; }

        public Dictionary<FloatRect, Item> ItemsGlobalBounds { get; private set; }

        public List<Drawable> MapRenderList { get; }
        public List<Drawable> LevelObjectsRenderList { get; }

        public Level(Round round, int size)
        {
            Round = round;
            MapSize = size;

            MapRenderList = new List<Drawable>();
            LevelObjectsRenderList = new List<Drawable>();

            GenerateLevel();
        }

        private void GenerateLevel()
        {
            /*
            Distingo Map como conjunto de bordes, columnas y espacio vacíos que se generan a partir del tamaño de mapa, se mantienen invariables
            y se renderizan en una primera capa (_mapRenderList), de Level como el conjunto dinámico de LevelObjects (Blocks, Bombs, Items, etc.), 
            los cuales se renderizan en una lista posterior (_levelObjectsRenderList) y además son registrados en un diccionario (_level) según su
            posición en grilla.
            */
            
            _level = new Dictionary<Tuple<int, int>, LevelObject>();

            MapGlobalBounds = new List<FloatRect>();
            BombsGlobalBounds = new List<FloatRect>();
            FireGlobalBounds = new List<FloatRect>();
            ItemsGlobalBounds = new Dictionary<FloatRect, Item>();;

            GenerateMap();
            GenerateBlocks();
        }

        private void GenerateMap()
        {
            _mapLayout = new LevelObjects[MapSize, MapSize];

            //define la posición de las columnas
            for (int i = 0; i < MapSize; i++)
            {
                for (int j = 0; j < MapSize; j++)
                {
                    if (i % 2 == 0 && j % 2 == 0)
                    {
                        _mapLayout[i, j] = LevelObjects.Wall;
                    }
                }
            }

            //define, por encima, la posición de los bordes
            for (int i = 0; i < MapSize; i++)
            {
                _mapLayout[i, 0] = LevelObjects.Wall;
                _mapLayout[i, MapSize - 1] = LevelObjects.Wall;
                _mapLayout[0, i] = LevelObjects.Wall;
                _mapLayout[MapSize - 1, i] = LevelObjects.Wall;
            }

            TileSize = Application.WindowHeight / (MapSize + 2f);
            _offsetX = ((Application.WindowWidth - (TileSize * MapSize)) / 2);

            //instancia los objetos indexados, y los agrega a su lista de renderizado
            for (int i = 0; i < MapSize; i++)
            {
                for (int j = 0; j < MapSize; j++)
                {

                    RectangleShape rectangle = new RectangleShape(new Vector2f(TileSize, TileSize));
                    rectangle.Position = new Vector2f(i * TileSize + _offsetX, j * TileSize + TileSize);

                    if (_mapLayout[i, j] == LevelObjects.Wall)
                    {
                        CreateMapObject(LevelObjects.Wall, i, j);
                    }
                    else
                    {
                        CreateMapObject(LevelObjects.Empty, i, j);
                    }
                }
            }
        }

        private void GenerateBlocks()
        {
            for (int i = 0; i < MapSize; i++)
            {
                for (int j = 0; j < MapSize; j++)
                {
                    if ( //excluye las esquinas de la generación de bloques según el layout tradicional
                        (i == 1 && j == 1) ||
                        (i == 1 && j == 2) ||
                        (i == 2 && j == 1) ||

                        (i == MapSize - 2 && j == 1) ||
                        (i == MapSize - 2 && j == 2) ||
                        (i == MapSize - 3 && j == 1) ||

                        (i == 1 && j == MapSize - 2) ||
                        (i == 1 && j == MapSize - 3) ||
                        (i == 2 && j == MapSize - 2) ||

                        (i == MapSize - 2 && j == MapSize - 2) ||
                        (i == MapSize - 2 && j == MapSize - 3) ||
                        (i == MapSize - 3 && j == MapSize - 2)
                    )
                    {
                        CreateLevelObject(LevelObjects.Empty, i, j);
                    }
                    else
                    {
                        if (_mapLayout[i, j] != 0) continue;

                        Random random = new Random();

                        if (random.Next(0, 3) != 0)
                        {
                            _mapLayout[i, j] = LevelObjects.Block;
                            CreateLevelObject(LevelObjects.Block, i, j);
                        }
                        else
                        {
                            CreateLevelObject(LevelObjects.Empty, i, j);
                        }
                    }
                }
            }
        }

        private void CreateMapObject(LevelObjects type, int positionX, int positionY)
        {
            switch (type)
            {
                case LevelObjects.Empty:
                    var empty = new Empty(this, TileSize, positionX, positionY);
                    AddToLevelDictionary(empty, positionX, positionY);
                    MapRenderList.Add(empty.GetDrawable());
                    break;

                case LevelObjects.Wall:
                    var wall = new Wall(this, TileSize, positionX, positionY);
                    AddToLevelDictionary(wall, positionX, positionY);
                    MapRenderList.Add(wall.GetDrawable());
                    break;
            }
        }

        public void CreateLevelObject(LevelObjects type, int positionX, int positionY)
        {
            switch (type)
            {
                case LevelObjects.Empty:
                    var empty = new Empty(this, TileSize, positionX, positionY, true);
                    AddToLevelDictionary(empty, positionX, positionY);
                    break;

                case LevelObjects.Block:
                    var block = new Block(this, TileSize, positionX, positionY);
                    AddToLevelDictionary(block, positionX, positionY);
                    break;

                case LevelObjects.Fire:
                    var fire = new Fire(this, TileSize, positionX, positionY);
                    AddToLevelDictionary(fire, positionX, positionY);
                    break;

                case LevelObjects.ItemBomb:
                    var itemBomb = new ItemBomb(this, TileSize, positionX, positionY);
                    AddToLevelDictionary(itemBomb, positionX, positionY);
                    break;

                case LevelObjects.ItemFire:
                    var itemFire = new ItemFire(this, TileSize, positionX, positionY);
                    AddToLevelDictionary(itemFire, positionX, positionY);
                    break;

                case LevelObjects.ItemSpeed:
                    var itemSpeed = new ItemSpeed(this, TileSize, positionX, positionY);
                    AddToLevelDictionary(itemSpeed, positionX, positionY);
                    break;
            }
        }

        public LevelObject GetDictionaryValue(int positionX, int positionY)
        {
            if (!_level.ContainsKey(Tuple.Create(positionX, positionY))) return null;

            return _level[Tuple.Create(positionX, positionY)];
        }

        private void AddToLevelDictionary(LevelObject levelObject, int positionX, int positionY)
        {
            if (_level.ContainsKey(Tuple.Create(positionX, positionY)))
            {
                _level.Remove(Tuple.Create(positionX, positionY));
                _level.Add(Tuple.Create(positionX, positionY), levelObject);
            }
            else
            {
                _level.Add(Tuple.Create(positionX, positionY), levelObject);
            }
        }

        public void RemoveFromLevelDictionary(int positionX, int positionY)
        {
            CreateLevelObject(LevelObjects.Empty, positionX, positionY);
        }

        public Vector2f GridToFloat(int positionX, int positionY)
        {
            return new Vector2f(
                positionX * TileSize + _offsetX,
                positionY * TileSize + TileSize);
        }

        public Vector2i FloatToGrid(Vector2f position)
        {
            return new Vector2i(
                (int)MathF.Round((position.X - _offsetX) / TileSize),
                (int)MathF.Round((position.Y - TileSize) / TileSize));
        }

        public Bomb DropBomb(Player bombOwner, Vector2f position, int range)
        {
            var posToInt = FloatToGrid(position);

            if (!(GetDictionaryValue(posToInt.X, posToInt.Y) is Empty)) return null;

            var bomb = new Bomb(this, TileSize, posToInt.X, posToInt.Y, bombOwner, range);
            AddToLevelDictionary(bomb, posToInt.X, posToInt.Y);

            foreach (var player in Round.Players)
            {
                if (player.GetBounds().Intersects(bomb.GetBounds())) player.RegisterPublicBomb(bomb);
            }

            return bomb;
        }

        public void Explosion(int positionX, int positionY, int range)
        {
            CreateLevelObject(LevelObjects.Fire, positionX, positionY);

            if (positionY % 2 == 1)
            {
                for (int i = 1; i <= range; i++)
                {
                    if (positionX + i >= MapSize - 1) break;

                    if (Burn(positionX + i, positionY)) break;
                }

                for (int i = 1; i <= range; i++)
                {
                    if (positionX - i <= 0) break;

                    if (Burn(positionX - i, positionY)) break;
                }
            }

            if (positionX % 2 == 1)
            {
                for (int i = 1; i < range + 1; i++)
                {
                    if (positionY + i >= MapSize - 1) break;

                    if (Burn(positionX, positionY + i)) break;

                }

                for (int i = 1; i < range + 1; i++)
                {
                    if (positionY - i <= 0) break;

                    if (Burn(positionX, positionY - i)) break;
                }
            }
        }

        private bool Burn(int positionX, int positionY) // devuelve true si destruyó algo que detiene la explosión
        {
            if (GetDictionaryValue(positionX, positionY) is Empty)
            {
                CreateLevelObject(LevelObjects.Fire, positionX, positionY);
                return false;
            }

            GetDictionaryValue(positionX, positionY).Destroy();
            CreateLevelObject(LevelObjects.Fire, positionX, positionY);

            return true;
        }
    }
}