using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;  // menu.cs

namespace PacmanGame
{
    public class Menu
    {
        private SpriteFont font; // Шрифт для текста
        private bool isActive;   // Активно ли меню

        public Menu()
        {
            isActive = true;
        }

        public void LoadContent(ContentManager content)
        {
            font = content.Load<SpriteFont>("MenuFont"); // Загрузите шрифт в папку Content
        }

        public void Update(GameTime gameTime, out bool startGame)
        {
            startGame = false;

            if (isActive && Keyboard.GetState().IsKeyDown(Keys.Space))
            {
                isActive = false;
                startGame = true;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (isActive)
            {
                string message = "Нажмите ПРОБЕЛ, чтобы начать!";
                Vector2 position = new Vector2(200, 200); // Позиция текста
                spriteBatch.DrawString(font, message, position, Color.White);
            }
        }
    }
}
