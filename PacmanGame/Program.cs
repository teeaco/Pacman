using PacmanGame;  // Подключаем пространство имен с классом Game1

class Program
{
    static void Main()
    {
        using var game = new Game1();  // Создаем объект класса Game1
        game.Run();  // Запускаем игру
    }
}
