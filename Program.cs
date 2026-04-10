using System;
using System.Threading;

namespace ConsoleApp2
{
    class Program
    {
        static object locker = new object();
        static CancellationToken token;
        static bool isPaused = false;

        static void Main(string[] args)
        {
            Console.Write("Введіть кількість прогрес-барів: ");
            int count = int.Parse(Console.ReadLine());

            CancellationTokenSource cts = new CancellationTokenSource();
            token = cts.Token;

            Console.WriteLine("Якщо хочете відмінити,натисніть Q,пауза - F,L - продовжити \n");
            Console.ReadLine();

            Thread[] threads = new Thread[count];

            for (int i = 0; i < count; i++)
            {
                threads[i] = new Thread(ProgressBar);
                threads[i].Start(i);
            }

            Thread inputThread = new Thread(Cancel);
            inputThread.IsBackground = true;
            inputThread.Start(cts);

            foreach (var t in threads)
                t.Join();

            Console.SetCursorPosition(0, count + 2);

            if (cts.IsCancellationRequested)
                Console.WriteLine("Виконання скасовано.");
            else
                Console.WriteLine("Всі прогресбари завершено!");
        }

        static void Cancel(object obj)
        {
            CancellationTokenSource cts = (CancellationTokenSource)obj;

            while (true)
            {
                ConsoleKeyInfo key = Console.ReadKey(true);

                if (key.Key == ConsoleKey.Q)
                {
                    cts.Cancel();
                    break;
                }
                else if (key.Key == ConsoleKey.F)
                {
                    isPaused = true;
                }
                else if (key.Key == ConsoleKey.L)
                {
                    isPaused = false;
                }
            }
        }

        static void WriteAt(string s, int x, int y)
        {
            lock (locker)
            {
                try
                {
                    Console.SetCursorPosition(x, y);
                    Console.Write(s);
                }
                catch (ArgumentOutOfRangeException) { }
            }
        }

        static void ProgressBar(object obj)
        {
            int row = (int)obj;
            Random rand = new Random();
            int progress = 0;
            int speed = rand.Next(30, 100);

            while (progress <= 100)
            {
                while (isPaused)
                {
                    Thread.Sleep(50);
                }

                if (token.IsCancellationRequested)
                {
                    int blocks = progress / 2;
                    string cancelled = new string('#', blocks) + new string('-', 50 - blocks);
                    WriteAt($"Бар {row + 1}: [{cancelled}] {progress}%", 0, row);
                    return;
                }

                int filled = progress / 2;
                string bar = new string(':', filled) + new string('-', 50 - filled);
                WriteAt($"Бар {row + 1}: [{bar}] {progress}%", 0, row);

                progress = progress + rand.Next(1, 5);
                if (progress > 100) progress = 100;

                Thread.Sleep(speed);
            }

            string finalBar = new string('-', 50);
            WriteAt($"Бар {row + 1,2}: [{finalBar}] 100% ", 0, row);
        }
    }
}
