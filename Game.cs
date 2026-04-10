using System;
using System.Threading;


class Program
{
    static bool isRunning = true;
    static bool isPaused = false;

    static int playerX = 30;
    static int playerY = 10;

    static object locker = new object();
    static List<(int x, int y)> snowflakes = new List<(int, int)>();
    static Random rand = new Random();

    static void Main()
    {
        Console.CursorVisible = false;

        Thread snowThread = new Thread(Snow);
        Thread inputThread = new Thread(Input);

        snowThread.Start();
        inputThread.Start();

        while (isRunning)
        {
            if (!isPaused)
            {
                Draw();
            }
            Thread.Sleep(50);
        }
    }

    static void Snow()
    {
        while (isRunning)
        {
            if (!isPaused)
            {
                lock (locker)
                {
                    snowflakes.Add((rand.Next(0, Console.WindowWidth), 0));

                    for (int i = 0; i < snowflakes.Count; i++)
                    {
                        var s = snowflakes[i];
                        snowflakes[i] = (s.x, s.y + 1);
                    }

                    snowflakes.RemoveAll(s => s.y >= Console.WindowHeight - 1);
                }
            }
            Thread.Sleep(100);
        }
    }

    static void Input()
    {
        while (isRunning)
        {
            if (Console.KeyAvailable) 
            {
                var key = Console.ReadKey(true).Key;

                if (key == ConsoleKey.Escape)
                {
                    isPaused = !isPaused;
                }

                
                
                    if (key == ConsoleKey.W) playerY--;
                    if (key == ConsoleKey.S) playerY++;
                    if (key == ConsoleKey.A) playerX--;
                    if (key == ConsoleKey.D) playerX++;
                

                
                playerX = Math.Clamp(playerX, 0, Console.WindowWidth - 3);
                playerY = Math.Clamp(playerY, 0, Console.WindowHeight - 4);
            }

            Thread.Sleep(10); 
        }
    }

    static void Draw()
    {
        lock (locker)
        {
            Console.Clear();

            foreach (var s in snowflakes)
            {
                Console.SetCursorPosition(s.x, s.y);
                Console.Write("*");
            }

            Console.SetCursorPosition(playerX, playerY);
            Console.Write(" O ");
            Console.SetCursorPosition(playerX, playerY + 1);
            Console.Write("/|\\");
            Console.SetCursorPosition(playerX, playerY + 2);
            Console.Write("/ \\");

            for (int i = 0; i < Console.WindowWidth; i++)
            {
                Console.SetCursorPosition(i, Console.WindowHeight - 1);
                Console.Write("$");
            }

            if (isPaused)
            {
                Console.SetCursorPosition(10, 5);
                Console.Write("PAUSE");
            }
        }
    }
}
