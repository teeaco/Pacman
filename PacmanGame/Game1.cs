using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Linq;


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

        //private int[,] map;
        private int[,] map = new int[21, 19]
        {
            {1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1},
            {1, 2, 2, 2, 2, 2, 2, 2, 2, 1, 2, 2, 2, 3, 2, 2, 2, 2, 1},
            {1, 2, 1, 1, 1, 2, 1, 1, 2, 1, 2, 1, 1, 1, 1, 1, 1, 2, 1},
            {1, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 1},
            {1, 2, 1, 1, 1, 2, 1, 1, 1, 1, 1, 1, 1, 2, 1, 1, 1, 2, 1},
            {1, 3, 2, 2, 2, 2, 2, 2, 2, 1, 2, 2, 2, 2, 2, 2, 2, 2, 1},
            {1, 1, 1, 1, 1, 2, 1, 1, 2, 1, 2, 1, 1, 2, 1, 1, 1, 1, 1},
            {0, 0, 0, 0, 1, 2, 1, 1, 2, 1, 2, 1, 1, 2, 1, 0, 0, 0, 0},
            {0, 0, 0, 0, 1, 2, 2, 2, 2, 0, 2, 2, 2, 2, 1, 0, 0, 0, 0},
            {1, 1, 1, 1, 1, 2, 1, 1, 1, 0, 1, 1, 1, 2, 1, 1, 1, 1, 1},
            {2, 2, 2, 2, 2, 2, 1, 1, 0, 0, 0, 1, 1, 2, 2, 2, 2, 2, 2},
            {1, 1, 1, 1, 1, 2, 1, 1, 1, 1, 1, 1, 1, 2, 1, 1, 1, 1, 1},
            {0, 0, 0, 0, 1, 2, 2, 2, 2, 2, 2, 2, 2, 2, 1, 0, 0, 0, 0},
            {0, 0, 0, 0, 1, 2, 1, 1, 2, 1, 2, 1, 1, 2, 1, 0, 0, 0, 0},
            {1, 1, 1, 1, 1, 2, 1, 1, 2, 1, 2, 1, 1, 2, 1, 1, 1, 1, 1},
            {1, 2, 2, 2, 2, 2, 2, 2, 2, 1, 2, 2, 2, 2, 2, 2, 2, 2, 1},
            {1, 2, 1, 1, 1, 2, 1, 1, 1, 1, 1, 1, 1, 2, 1, 1, 1, 2, 1},
            {1, 2, 2, 3, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 1},
            {1, 2, 1, 1, 1, 2, 1, 1, 2, 1, 2, 1, 1, 2, 1, 1, 1, 2, 1},
            {1, 2, 2, 2, 2, 2, 2, 2, 2, 1, 2, 2, 2, 2, 2, 2, 2, 3, 1},
            {1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1}
        };



        private bool ghostsReleased = false;
        private float ghostReleaseTime = 5.0f; // Время до выхода призраков (в секундах)
        private float elapsedTime = 0.0f;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

                // Устанавливаем размеры окна
            _graphics.PreferredBackBufferWidth = 608;  // 19 * 32 пикселей (ширина карты)
            _graphics.PreferredBackBufferHeight = 672; // 21 * 32 пикселей (высота карты)
            //_graphics.ApplyChanges();

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
        bool isBigDotActive = false;

        private void CollectBigDot()
        {
            isBigDotActive = true; // Активируем эффект для больших пиллетов
            // Дополнительная логика, например, сделать призраков уязвимыми
        }

        private Texture2D bigDotTexture;
        public Vector2 StartPosition(int x, int y)
        {
            int cellSize = 32; // Assuming each cell is 32x32 pixels
            return new Vector2(x * cellSize, y * cellSize);
        }
        protected override void LoadContent()
        {
            
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            //bigDotTexture = Content.Load<Texture2D>("bigDot"); // Путь к текстуре
            bigDotTexture = CreateTexture(Color.Black); // Путь к текстуре
            pacmanTexture = CreateTexture(Color.Yellow);
            wallTexture = CreateTexture(Color.Blue);
            floorTexture = CreateTexture(Color.White);
            dotTexture = CreateTexture(Color.Gray);

            // Добавляем 4 призраков с разными цветами
            ghosts.Add(new Ghost(StartPosition(9, 9), CreateTexture(Color.Red), map, new Vector2(0, 0))); // Угол карты вверху слева
            ghosts.Add(new Ghost(StartPosition(9, 10), CreateTexture(Color.Purple), map, new Vector2(map.GetLength(1) - 1, 0))); // Угол карты вверху справа
            ghosts.Add(new Ghost(StartPosition(10, 10), CreateTexture(Color.Orange), map, new Vector2(0, map.GetLength(0) - 1))); // Угол карты внизу слева
            ghosts.Add(new Ghost(StartPosition(8, 10), CreateTexture(Color.Cyan), map, new Vector2(map.GetLength(1) - 1, map.GetLength(0) - 1))); // Угол карты внизу справа

        }

        private Texture2D CreateTexture(Color color)
        {
            Texture2D texture = new Texture2D(GraphicsDevice, 32, 32);
            Color[] data = new Color[32 * 32];
            for (int i = 0; i < data.Length; i++) data[i] = color;
            texture.SetData(data);
            return texture;
        }

        private Vector2 pacmanDirection = new Vector2(1, 0); // Начальное направление вправо
        private bool gameStarted = false; // Флаг для проверки, началась ли игра
        protected override void Update(GameTime gameTime)
        {
            if (gameLost || gameWon)
            {
                if (Keyboard.GetState().IsKeyDown(Keys.Space))
                    Exit();
                return;
            }
            if (!gameStarted)
            {
                if (Keyboard.GetState().GetPressedKeys().Length > 0) // Проверка на нажатие любой клавиши
                {
                    gameStarted = true; // Игра началась
                }
                return;
            }

            KeyboardState keyboardState = Keyboard.GetState();

            // Выбираем новое направление, если пользователь нажал клавишу
            Vector2 desiredDirection = pacmanDirection;
            if (keyboardState.IsKeyDown(Keys.Up)) desiredDirection = new Vector2(0, -1);
            if (keyboardState.IsKeyDown(Keys.Down)) desiredDirection = new Vector2(0, 1);
            if (keyboardState.IsKeyDown(Keys.Left)) desiredDirection = new Vector2(-1, 0);
            if (keyboardState.IsKeyDown(Keys.Right)) desiredDirection = new Vector2(1, 0);

            // Вычисляем новую позицию на основе направления
            Vector2 nextPosition = pacmanPosition + desiredDirection * 2;

            // Если новое направление допустимо, обновляем pacmanDirection
            if (IsValidMove(nextPosition, 32))
            {
                pacmanDirection = desiredDirection;
            }

            // Пытаемся двигаться в текущем направлении
            Vector2 movePosition = pacmanPosition + pacmanDirection * 2;
            if (IsValidMove(movePosition, 32))
            {
                pacmanPosition = movePosition;
                CollectDot();
            }

            // Обновляем призраков
            foreach (var ghost in ghosts)
            {
                ghost.Update(pacmanPosition); // Передаем позицию Pac-Man для логики преследования

                if (ghost.CheckCollision(pacmanPosition))
                {
                    if (ghost.CurrentMode == Ghost.Mode.Frightened) // Если призрак испуган
                    {
                        ghosts.Remove(ghost); // Удаляем пойманного призрака
                    }
                    else
                    {
                        gameLost = true; // Столкновение с обычным призраком — проигрыш
                        break;
                    }
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


        public int count = 0;
        private void CollectDot()
        {
            int mapX = (int)(pacmanPosition.X / 32);
            int mapY = (int)(pacmanPosition.Y / 32);

            // Check if the position contains a dot (2)
            if (map[mapY, mapX] == 2)
            {
                map[mapY, mapX] = 0; // Remove the dot
                score += 10;         // Increase score
                totalDots--;         // Decrease the total number of dots
            }
            // Check if the position contains a big dot (3)
            else if (map[mapY, mapX] == 3)
            {
                map[mapY, mapX] = 0; // Remove the big dot
                score += 50;         // Increase score for big dot
                CollectBigDot();     // Trigger big dot effects
                totalDots--;         // Decrease the total number of dots
            }

            // Check for win condition
            if (totalDots == 0)
            {
                gameWon = true; // Mark the game as won
            }
        }



        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin();

            // Отрисовка карты
            for (int y = 0; y < map.GetLength(0); y++){
                for (int x = 0; x < map.GetLength(1); x++)
                {
                    Vector2 position = new Vector2(x * 32, y * 32);
                    if (map[y, x] == 1)
                        _spriteBatch.Draw(wallTexture, position, Color.White);
                    else if (map[y, x] == 2)
                        _spriteBatch.Draw(dotTexture, position, Color.White);
                    else if (map[y, x] == 3) // Для больших пиллетов
                        _spriteBatch.Draw(bigDotTexture, position, Color.White);
                    else
                        _spriteBatch.Draw(floorTexture, position, Color.White);
                }
            }


            // Отрисовка Pacman
            _spriteBatch.Draw(pacmanTexture, pacmanPosition, Color.White);

            // Отрисовка призраков
            foreach (var ghost in ghosts)
            {
                ghost.Draw(_spriteBatch);
            }
                        // Отрисовка текста с очками
            string scoreText = $"Score: {score}";
            var font = Content.Load<SpriteFont>("DefaultFont"); // Ensure you have a font file in Content
            _spriteBatch.DrawString(font, scoreText, new Vector2(10, 10), Color.White);

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
    private static Random random = new Random();

    public Mode currentMode; // Режим поведения
    private Vector2 homeCorner; // Угол карты для режима рассеяния
    private Vector2 scatterTarget; // Целевая точка для Scatter
    private float modeSwitchTimer; // Таймер для переключения режимов
    private Vector2 pacmanPosition; // Позиция Pac-Man (передается извне)

    public enum Mode
    {
        Scatter,
        Chase,
        Frightened
    }
    public bool IsVulnerable { get; set; } = false;
    private Vector2 startPosition; // Начальная позиция
    private bool isOutOfBox; // Флаг, указывающий, что призрак вышел из коробки

    // Конструктор класса Ghost
    public Ghost(Vector2 position, Texture2D texture, int[,] map, Vector2 homeCorner)
    {
        this.position = position;
        this.texture = texture;
        this.map = map;
        this.homeCorner = homeCorner;
        this.currentMode = Mode.Scatter;
        this.modeSwitchTimer = 7f; // Каждые 7 секунд режим переключается
        this.startPosition = position; // Сохраняем начальную позицию
        direction = GetRandomDirection();
        this.isOutOfBox = false; // Изначально призрак в коробке
    }

    public Mode CurrentMode
    {
        get { return currentMode; }
    }

    // Метод обновления состояния призрака
// Метод обновления состояния призрака
    public void Update(Vector2 pacmanPosition)
    {
        this.pacmanPosition = pacmanPosition;

        // Переключение режимов по таймеру
        modeSwitchTimer -= 0.016f; // Предполагается, что кадр = 16 мс
        if (modeSwitchTimer <= 0)
        {
            if (currentMode == Mode.Scatter)
                currentMode = Mode.Chase;
            else if (currentMode == Mode.Chase)
                currentMode = Mode.Scatter;
            modeSwitchTimer = 7f; // Сброс таймера
        }

        // Логика движения в зависимости от текущего режима
        if (currentMode == Mode.Chase)
        {
            direction = GetNextDirection(pacmanPosition); // Движение к Pac-Man
        }
        else if (currentMode == Mode.Scatter)
        {
            direction = GetNextDirection(homeCorner); // Движение к "домашнему углу"
        }
        else if (currentMode == Mode.Frightened)
        {
            direction = GetRandomDirection(); // Случайное движение
        }

        Vector2 newPosition = position + direction * 1.6f; // применяем скорость

        // Проверка на валидность движения
        if (IsValidMove(newPosition))
        {
            position = newPosition;
        }
        else
        {
            direction = GetRandomDirection(); // Если движение невозможно, выбираем случайное направление
        }

        // Логика выхода из коробки
        if (!isOutOfBox)
        {
            Vector2 boxExit = new Vector2(7, 8); // Пример координаты выхода
            direction = GetNextDirection(boxExit); // Направляем призрака к выходу

            if (Vector2.Distance(position, boxExit) < 60f) // Если близко к выходу
            {
                isOutOfBox = true; // Призрак выходит из коробки
                currentMode = Mode.Scatter; // Сразу переключаемся в режим погони
            }
        }
    }


    // Метод активации режима испуга
    public void EnterFrightenedMode()
    {
        currentMode = Mode.Frightened;
        modeSwitchTimer = 5f; // Испуганное состояние длится 5 секунд
    }

    // Метод для получения следующего направления для призрака
    public Vector2 GetNextDirection(Vector2 target)
    {
        // Получаем возможные направления
        List<Vector2> possibleDirections = new List<Vector2>();

        Vector2[] directions = { Vector2.UnitX, -Vector2.UnitX, Vector2.UnitY, -Vector2.UnitY };

        foreach (var dir in directions)
        {
            Vector2 newPos = position + dir * texture.Width;
            if (IsValidMove(newPos))  // Проверяем, не упирается ли в стену
            {
                possibleDirections.Add(dir);
            }
        }

        // Выбираем направление, которое будет наиболее подходящим для движения в сторону цели (target)
        Vector2 bestDirection = possibleDirections.OrderBy(d => Vector2.Distance(target, position + d * texture.Width)).First();
        return bestDirection;
    }


    // Метод для получения случайного направления
    private Vector2 GetRandomDirection()
    {
        Vector2[] directions = { Vector2.UnitX, -Vector2.UnitX, Vector2.UnitY, -Vector2.UnitY };
        return directions[random.Next(directions.Length)];
    }

    // Метод проверки валидности движения
    private bool IsValidMove(Vector2 position)
    {
        float minX = 0;
        float minY = 0;
        float maxX = map.GetLength(1) * texture.Width;
        float maxY = map.GetLength(0) * texture.Height;

        if (position.X < minX || position.Y < minY || position.X + texture.Width > maxX || position.Y + texture.Height > maxY)
            return false;

        int startX = (int)(position.X / texture.Width);
        int startY = (int)(position.Y / texture.Height);
        int endX = (int)((position.X + texture.Width - 1) / texture.Width);
        int endY = (int)((position.Y + texture.Height - 1) / texture.Height);

        if (startX < 0 || startY < 0 || endX >= map.GetLength(1) || endY >= map.GetLength(0))
            return false;

        for (int x = startX; x <= endX; x++)
        {
            for (int y = startY; y <= endY; y++)
            {
                if (map[y, x] == 1)
                    return false;
            }
        }

        return true;
    }

    // Метод отрисовки призрака
    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(texture, position, currentMode == Mode.Frightened ? Color.Blue : Color.White);
    }

    // Метод проверки столкновения с Pac-Man
    public bool CheckCollision(Vector2 pacmanPosition)
    {
        return Vector2.Distance(position + new Vector2(texture.Width / 2, texture.Height / 2),
            pacmanPosition + new Vector2(texture.Width / 2, texture.Height / 2)) < texture.Width;
    }
}
}