using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace PacmanGame
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private Texture2D pacmanTexture;
        private Texture2D wallTexture;
        private Texture2D floorTexture;
        private Texture2D dotTexture;

        private Vector2 pacmanPosition;

        private int score = 0; // Очки игрока
        private int totalDots; // Общее количество круглешков

        int mapWidth = 10;
        int mapHeight = 10;

        int[,] map = new int[10, 10]
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

        private bool gameWon = false; // Флаг победы

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            pacmanPosition = new Vector2(32, 32);
            totalDots = CountDots(); // Подсчет общего количества круглешков
        }

        private int CountDots()
        {
            int dots = 0;
            for (int y = 0; y < mapHeight; y++)
                for (int x = 0; x < mapWidth; x++)
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
        // Если игра завершена, ждем нажатия пробела для выхода
        if (gameWon)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Space))
            {
                Exit(); // Закрываем игру
            }
            return; // Не обновляем игровую логику
        }

        Vector2 newPosition = pacmanPosition;

        // Управление Pacman
        if (Keyboard.GetState().IsKeyDown(Keys.Up)) newPosition.Y -= 2;
        if (Keyboard.GetState().IsKeyDown(Keys.Down)) newPosition.Y += 2;
        if (Keyboard.GetState().IsKeyDown(Keys.Left)) newPosition.X -= 2;
        if (Keyboard.GetState().IsKeyDown(Keys.Right)) newPosition.X += 2;

        if (IsValidMove(newPosition))
        {
            pacmanPosition = newPosition;
            CollectDot(); // Проверяем, съел ли Pacman точку
        }

        base.Update(gameTime);
    }



    private void CollectDot()
    {
        int mapX = (int)(pacmanPosition.X / 32);
        int mapY = (int)(pacmanPosition.Y / 32);

        if (map[mapY, mapX] == 2)
        {
            map[mapY, mapX] = 0; // Съедаем круглешок
            score++;
            if (score == totalDots)
            {
                gameWon = true; // Устанавливаем флаг победы
            }
        }
    }


        private bool IsValidMove(Vector2 position)
        {
            int pacmanSize = 32;
            int mapX1 = (int)(position.X / 32);
            int mapY1 = (int)(position.Y / 32);
            int mapX2 = (int)((position.X + pacmanSize - 1) / 32);
            int mapY2 = (int)((position.Y + pacmanSize - 1) / 32);

            if (mapX1 < 0 || mapX2 >= mapWidth || mapY1 < 0 || mapY2 >= mapHeight)
                return false;

            return map[mapY1, mapX1] != 1 &&
                   map[mapY1, mapX2] != 1 &&
                   map[mapY2, mapX1] != 1 &&
                   map[mapY2, mapX2] != 1;
        }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        _spriteBatch.Begin();

        if (gameWon)
        {
            // Рисуем экран победы
            string message = "YOU   WIN!\nPress   SPACE   to   exit";
            SpriteFont font = Content.Load<SpriteFont>("DefaultFont"); // Подключите шрифт
            Vector2 textSize = font.MeasureString(message);
            Vector2 position = new Vector2(
                (_graphics.PreferredBackBufferWidth - textSize.X) / 2,
                (_graphics.PreferredBackBufferHeight - textSize.Y) / 2
            );
            _spriteBatch.DrawString(font, message, position, Color.White);
        }
        else
        {
            // Отрисовка карты
            for (int y = 0; y < mapHeight; y++)
            {
                for (int x = 0; x < mapWidth; x++)
                {
                    Vector2 position = new Vector2(x * 32, y * 32);
                    if (map[y, x] == 1)
                        _spriteBatch.Draw(wallTexture, position, Color.White);
                    else if (map[y, x] == 2)
                        _spriteBatch.Draw(dotTexture, position, Color.White);
                    else
                        _spriteBatch.Draw(floorTexture, position, Color.White);
                }
            }

            // Отрисовка Pacman
            _spriteBatch.Draw(pacmanTexture, pacmanPosition, Color.White);
        }

        _spriteBatch.End();

        base.Draw(gameTime);
    }

}
}
