using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Linq;
using PacmanGame;

namespace PacmanGame{
    public class Ghost
    {
        public enum Mode
        {
            Normal,     // Обычное поведение
            Frightened, // Испуганный режим
            Chase,      // Преследование
            Scatter     // Рассеивание
        }
        
        public Vector2 position;
        public Vector2 direction;
        private int[,] map;
        private Texture2D texture;
        private float speed;
        private float targetX;  // Целевая позиция по X
        private float targetY;  // Целевая позиция по Y
        private bool isMovingToTarget;
        private bool isAtGridCell;  // Флаг, указывающий, что призрак на границе клетки

        public Mode CurrentMode { get; private set; } // Добавляем свойство для режима

        public Ghost(Vector2 startPosition, Texture2D texture, int[,] map, Vector2 position)//initialize
        {
            this.position = startPosition;
            this.map = map;
            this.texture = texture;
            this.speed = 2.0f;
            this.direction = new Vector2(0, 0);
            this.CurrentMode = Mode.Normal; // Устанавливаем начальный режим
            this.isMovingToTarget = false;
            this.isAtGridCell = true; // Изначально считаем, что призрак находится на границе клетки
        }
        private float behaviorTimer = 0f;
        public Vector2 ScatterTarget { get; private set; }
        private float behaviorChangeInterval = 10f; // Интервал смены поведения (в секундах)
        //private desiredDirection currentBehavior = GhostBehavior.Scatter; // Начальное поведение
        public void Update(GameTime gameTime, Vector2 pacmanPosition, Vector2 scatterTarget) //изменения за кадр+логика
        {
            ScatterTarget = scatterTarget;
            behaviorTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (behaviorTimer >= behaviorChangeInterval)
                {
                    behaviorTimer = 0f; // Сбрасываем таймер

                    // Переключаем режим
                    if (CurrentMode == Mode.Scatter) //жалкие попытки
                    {
                        CurrentMode = Mode.Chase;
                    }
                    else if (CurrentMode == Mode.Chase)
                    {
                        CurrentMode = Mode.Scatter;
                    }
                }
            // Если мы находимся на границе клетки, выбираем новое направление
            if (isAtGridCell)
            {
                //Console.WriteLine($"Mode: {CurrentMode}, Direction: {direction}, Position: {position}");
                // Выбираем новое направление в зависимости от режима
                Vector2 desiredDirection = direction;
                switch (CurrentMode)
                {
                    case Mode.Normal:
                        desiredDirection = ChooseDirectionRandomly();
                        break;
                    case Mode.Chase:
                        desiredDirection = GetDirectionToPacman(pacmanPosition);
                        break;
                    case Mode.Scatter:
                        desiredDirection = GetScatterDirection();
                        break;
                    case Mode.Frightened:
                        desiredDirection = GetOppositeDirection();
                        break;
                }

                // Проверяем, если новое направление допустимо
                Vector2 nextPosition = position + desiredDirection * speed;
                if (IsValidMove(nextPosition, 32)) // 32 - размер шага
                {
                    direction = desiredDirection; // Обновляем направление
                }
            }

            // Двигаем призрака в выбранном направлении
            Vector2 movePosition = position + direction * speed;
            if (IsValidMove(movePosition, 32))
            {
                position = movePosition; // Обновляем позицию
            }

            // Проверяем, если мы достигли конца клетки, чтобы изменить направление
            if (IsAtGridCell())
            {
                isAtGridCell = true;
            }
            else
            {
                isAtGridCell = false;
            }
        }

        private bool IsAtGridCell()
        {
            // Проверка, что призрак находится на границе клетки (целое число координат)
            Console.WriteLine($"Mode: {CurrentMode}, Direction: {direction}, Position: {position}");
            
            return (Math.Abs(position.X % 32) < 0.5f && Math.Abs(position.Y % 32) < 0.5f);

        }

        private Vector2 ChooseDirectionRandomly()
        {
            // Список возможных направлений: вверх, вниз, влево, вправо
            Vector2[] possibleDirections = {
                new Vector2(1, 0),  // Вправо
                new Vector2(-1, 0), // Влево
                new Vector2(0, 1),  // Вниз
                new Vector2(0, -1)  // Вверх
            };

            Random rand = new Random();
            return possibleDirections[rand.Next(possibleDirections.Length)];
        }

        private Vector2 GetDirectionToPacman(Vector2 pacmanPosition)
        {
            Vector2 delta = pacmanPosition - position;
            if (Math.Abs(delta.X) > Math.Abs(delta.Y))
            {
                return new Vector2(Math.Sign(delta.X), 0); // Двигаемся по X
            }
            else
            {
                return new Vector2(0, Math.Sign(delta.Y)); // Двигаемся по Y
            }
        }


        private Vector2 GetScatterDirection()
        {
            Vector2 delta = ScatterTarget - position;
            if (Math.Abs(delta.X) > Math.Abs(delta.Y))
            {
                return new Vector2(Math.Sign(delta.X), 0);
            }
            return new Vector2(0, Math.Sign(delta.Y));
        }


        private Vector2 GetOppositeDirection()
        {
            return -direction;
        }

        private bool IsValidMove(Vector2 position, int textureSize)
        {
            // Границы карты
            float minX = 0;
            float minY = 0;
            float maxX = map.GetLength(1) * textureSize;
            float maxY = map.GetLength(0) * textureSize;

            // Проверяем, что позиция призрака не выходит за пределы карты
            if (position.X < minX || position.Y < minY || position.X + textureSize > maxX || position.Y + textureSize > maxY)
                return false;

            // Проверяем, не выходит ли призрак за границы карты с учетом размера
            int startX = (int)(position.X / textureSize);
            int startY = (int)(position.Y / textureSize);
            int endX = (int)((position.X + textureSize - 1) / textureSize); // Последний элемент по X
            int endY = (int)((position.Y + textureSize - 1) / textureSize); // Последний элемент по Y

            // Проверяем, что призрак не столкнется со стенами (предположим, что 1 — это стена)
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

        public void SetMode(Mode mode)
        {
            CurrentMode = mode;
        }

        public void Draw(SpriteBatch spriteBatch) //отрисовка
        {
            spriteBatch.Draw(texture, position, Color.White);
        }

        public bool CheckCollision(Vector2 pacmanPosition)
        {
            float collisionDistance = 16; // Задаём допустимое расстояние для столкновения
            return Vector2.Distance(position, pacmanPosition) < collisionDistance;
        }
    }
}