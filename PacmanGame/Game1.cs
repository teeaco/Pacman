using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace PacmanGame
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private Texture2D pacmanTexture, wallTexture, floorTexture, dotTexture;
        private List<Ghost> ghosts = new List<Ghost>();

        private Vector2 pacmanPosition;
        private int score = 0;
        private int totalDots;
        private bool gameWon = false;
        private bool gameLost = false;

        private int[,] map = new int[10, 10]
        {
            {1, 1, 1, 1, 1, 1, 1, 1, 1, 1},
            {1, 2, 2, 2, 1, 2, 2, 2, 1, 1},
            {1, 2, 1, 2, 1, 2, 1, 2, 1, 1},
            {1, 2, 1, 2, 1, 2, 1, 2, 1, 1},
            {1, 2, 1, 2, 2, 2, 1, 2, 1, 1},
            {1, 2, 1, 1, 1, 2, 1, 2, 1, 1},
            {1, 2, 2, 2, 2, 2, 2, 2, 2, 1},
            {1, 2, 1, 1, 1, 1, 1, 1, 2, 1},
            {1, 2, 2, 2, 2, 2, 2, 2, 2, 1},
            {1, 1, 1, 1, 1, 1, 1, 1, 1, 1}
        };

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            pacmanPosition = new Vector2(32, 32);
            totalDots = CountDots();
        }

        private int CountDots()
        {
            int dots = 0;
            for (int y = 0; y < map.GetLength(0); y++)
                for (int x = 0; x < map.GetLength(1); x++)
                    if (map[y, x] == 2) dots++;
            return dots;
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            pacmanTexture = CreateTexture(Color.Yellow);
            wallTexture = CreateTexture(Color.Blue);
            floorTexture = CreateTexture(Color.White);
            dotTexture = CreateTexture(Color.Orange);

            // Добавляем 4 призраков с разными цветами
            ghosts.Add(new Ghost(new Vector2(128, 128), CreateTexture(Color.Red), map));
            ghosts.Add(new Ghost(new Vector2(256, 256), CreateTexture(Color.Purple), map));
            ghosts.Add(new Ghost(new Vector2(64, 192), CreateTexture(Color.Green), map));
            ghosts.Add(new Ghost(new Vector2(192, 64), CreateTexture(Color.Cyan), map));
        }

        private Texture2D CreateTexture(Color color)
        {
            Texture2D texture = new Texture2D(GraphicsDevice, 32, 32);
            Color[] data = new Color[32 * 32];
            for (int i = 0; i < data.Length; i++) data[i] = color;
            texture.SetData(data);
            return texture;
        }

        protected override void Update(GameTime gameTime)
        {
            if (gameLost || gameWon)
            {
                if (Keyboard.GetState().IsKeyDown(Keys.Space))
                    Exit();
                return;
            }

            // Обновляем Pacman
            Vector2 newPosition = pacmanPosition;
            if (Keyboard.GetState().IsKeyDown(Keys.Up)) newPosition.Y -= 2;
            if (Keyboard.GetState().IsKeyDown(Keys.Down)) newPosition.Y += 2;
            if (Keyboard.GetState().IsKeyDown(Keys.Left)) newPosition.X -= 2;
            if (Keyboard.GetState().IsKeyDown(Keys.Right)) newPosition.X += 2;

            if (IsValidMove(newPosition, pacmanTexture.Width))
            {
                pacmanPosition = newPosition;
                CollectDot();
            }

            // Обновляем призраков
            foreach (var ghost in ghosts)
            {
                ghost.Update();
                if (ghost.CheckCollision(pacmanPosition))
                {
                    gameLost = true;
                }
            }

            base.Update(gameTime);
        }

        private bool IsValidMove(Vector2 position, int textureSize)
        {
            // Границы карты
            float minX = 0;
            float minY = 0;
            float maxX = map.GetLength(1) * textureSize;
            float maxY = map.GetLength(0) * textureSize;

            // Проверяем, что позиция Пакмэна не выходит за пределы карты
            if (position.X < minX || position.Y < minY || position.X + textureSize > maxX || position.Y + textureSize > maxY)
                return false;

            // Проверяем, не выходит ли Пакмэн за границы карты с учетом размера
            int startX = (int)(position.X / textureSize);
            int startY = (int)(position.Y / textureSize);
            int endX = (int)((position.X + textureSize - 1) / textureSize); // Последний элемент по X
            int endY = (int)((position.Y + textureSize - 1) / textureSize); // Последний элемент по Y

            // Проверяем, не выходит ли Пакмэн за границы карты
            if (startX < 0 || startY < 0 || endX >= map.GetLength(1) || endY >= map.GetLength(0))
                return false;

            // Проверяем, что Пакмэн не столкнется со стенами (предположим, что 1 — это стена)
            for (int x = startX; x <= endX; x++)
            {
                for (int y = startY; y <= endY; y++)
                {
                    if (map[y, x] == 1) // Если в какой-то клетке стена, возвращаем false
                        return false;
                }
            }

            return true; // Все проверки пройдены
        }



        private void CollectDot()
        {
            int mapX = (int)(pacmanPosition.X / 32);
            int mapY = (int)(pacmanPosition.Y / 32);

            if (map[mapY, mapX] == 2)
            {
                map[mapY, mapX] = 0;
                score++;
                if (score == totalDots)
                {
                    gameWon = true;
                }
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin();

            // Отрисовка карты
            for (int y = 0; y < map.GetLength(0); y++)
                for (int x = 0; x < map.GetLength(1); x++)
                {
                    Vector2 position = new Vector2(x * 32, y * 32);
                    if (map[y, x] == 1)
                        _spriteBatch.Draw(wallTexture, position, Color.White);
                    else if (map[y, x] == 2)
                        _spriteBatch.Draw(dotTexture, position, Color.White);
                    else
                        _spriteBatch.Draw(floorTexture, position, Color.White);
                }

            // Отрисовка Pacman
            _spriteBatch.Draw(pacmanTexture, pacmanPosition, Color.White);

            // Отрисовка призраков
            foreach (var ghost in ghosts)
            {
                ghost.Draw(_spriteBatch);
            }

            if (gameWon)
                DrawMessage("YOU WIN!");
            else if (gameLost)
                DrawMessage("YOU LOSE!");

            _spriteBatch.End();

            base.Draw(gameTime);
        }

        private void DrawMessage(string message)
        {
            SpriteFont font = Content.Load<SpriteFont>("DefaultFont");
            Vector2 size = font.MeasureString(message);
            Vector2 position = new Vector2(
                (_graphics.PreferredBackBufferWidth - size.X) / 2,
                (_graphics.PreferredBackBufferHeight - size.Y) / 2
            );
            _spriteBatch.DrawString(font, message, position, Color.White);
        }
    }

    public class Ghost
    {
        private Vector2 position;
        private Texture2D texture;
        private Vector2 direction;
        private int[,] map;
        private static Random random = new Random(); // Сделаем Random статическим

        public Ghost(Vector2 position, Texture2D texture, int[,] map)
        {
            this.position = position;
            this.texture = texture;
            this.map = map;
            direction = GetRandomDirection(); // Сразу задаем случайное направление
        }

        public void Update()
        {
            Vector2 newPosition = position + direction * 2;

            if (!IsValidMove(newPosition))  // Параметр textureSize больше не нужен
            {
                direction = GetRandomDirection();
            }
            else
            {
                position = newPosition;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, position, Color.White);
        }

        public bool CheckCollision(Vector2 pacmanPosition)
        {
            return Vector2.Distance(position + new Vector2(texture.Width / 2, texture.Height / 2),
                pacmanPosition + new Vector2(texture.Width / 2, texture.Height / 2)) < texture.Width;
        }

        private bool IsValidMove(Vector2 position)
        {
            // Границы карты
            float minX = 0;
            float minY = 0;
            float maxX = map.GetLength(1) * texture.Width;  // Используем texture.Width вместо textureSize
            float maxY = map.GetLength(0) * texture.Height; // Используем texture.Height вместо textureSize

            // Проверяем, что позиция Пакмэна не выходит за пределы карты
            if (position.X < minX || position.Y < minY || position.X + texture.Width > maxX || position.Y + texture.Height > maxY)
                return false;

            // Проверяем, не выходит ли Пакмэн за границы карты с учетом размера
            int startX = (int)(position.X / texture.Width);
            int startY = (int)(position.Y / texture.Height);
            int endX = (int)((position.X + texture.Width - 1) / texture.Width); // Последний элемент по X
            int endY = (int)((position.Y + texture.Height - 1) / texture.Height); // Последний элемент по Y

            // Проверяем, не выходит ли Пакмэн за границы карты
            if (startX < 0 || startY < 0 || endX >= map.GetLength(1) || endY >= map.GetLength(0))
                return false;

            // Проверяем, что Пакмэн не столкнется со стенами (предположим, что 1 — это стена)
            for (int x = startX; x <= endX; x++)
            {
                for (int y = startY; y <= endY; y++)
                {
                    if (map[y, x] == 1) // Если в какой-то клетке стена, возвращаем false
                        return false;
                }
            }

            return true; // Все проверки пройдены
        }


        private Vector2 GetRandomDirection()
        {
            Vector2[] directions = { new Vector2(1, 0), new Vector2(-1, 0), new Vector2(0, 1), new Vector2(0, -1) };
            return directions[random.Next(directions.Length)];
        }
    }

}