using System;
using System.Collections.Generic;
using System.Linq;

class MazeGenerator
{
    private readonly int width;
    private readonly int height;
    private readonly char[,] maze;
    private readonly Random random;

    private int playerX;
    private int playerY;

    private int exitX;
    private int exitY;

    private List<(int x, int y)> shortestPath = new List<(int x, int y)>();
    private bool showPath = false;

    public MazeGenerator(int width, int height)
    {
        this.width = width % 2 == 0 ? width + 1 : width;
        this.height = height % 2 == 0 ? height + 1 : height;
        this.maze = new char[this.height, this.width];
        this.random = new Random();
    }

    public void Generate()
    {
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                maze[y, x] = '#';
            }
        }

        int startX = 1;
        int startY = 1;
        maze[startY, startX] = ' ';

        CarvePassage(startX, startY);

        maze[1, 0] = ' ';
        maze[height - 2, width - 1] = ' ';

        playerX = 0;
        playerY = 1;

        exitX = width - 1;
        exitY = height - 2;
    }

    private void CarvePassage(int x, int y)
    {
        int[] dx = { 0, 2, 0, -2 };
        int[] dy = { -2, 0, 2, 0 };

        int[] directions = { 0, 1, 2, 3 };
        Shuffle(directions);

        foreach (int direction in directions)
        {
            int newX = x + dx[direction];
            int newY = y + dy[direction];

            if (IsInBounds(newX, newY) && maze[newY, newX] == '#')
            {
                maze[y + dy[direction] / 2, x + dx[direction] / 2] = ' ';
                maze[newY, newX] = ' ';

                CarvePassage(newX, newY);
            }
        }
    }

    private bool IsInBounds(int x, int y)
    {
        return x > 0 && x < width - 1 && y > 0 && y < height - 1;
    }

    private void Shuffle(int[] array)
    {
        for (int i = array.Length - 1; i > 0; i--)
        {
            int j = random.Next(i + 1);
            (array[i], array[j]) = (array[j], array[i]);
        }
    }
    private void FindShortestPath()
    {
        shortestPath.Clear();
        showPath = true;

        var queue = new Queue<(int x, int y, List<(int x, int y)> path)>();
        var visited = new bool[height, width];

        var startPath = new List<(int x, int y)> { (playerX, playerY) };
        queue.Enqueue((playerX, playerY, startPath));
        visited[playerY, playerX] = true;

        int[] dx = { 0, 1, 0, -1 };
        int[] dy = { -1, 0, 1, 0 };

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();

            if (current.x == exitX && current.y == exitY)
            {
                shortestPath = current.path;
                return;
            }

            for (int i = 0; i < 4; i++)
            {
                int newX = current.x + dx[i];
                int newY = current.y + dy[i];

                if (newX >= 0 && newX < width && newY >= 0 && newY < height &&
                    maze[newY, newX] == ' ' && !visited[newY, newX])
                {
                    visited[newY, newX] = true;
                    var newPath = new List<(int x, int y)>(current.path) { (newX, newY) };
                    queue.Enqueue((newX, newY, newPath));
                }
            }
        }

        showPath = false;
        Console.Clear();
        Console.WriteLine("Путь до выхода не найден!");
        Console.WriteLine("Нажмите любую клавишу для продолжения...");
        Console.ReadKey();
    }

    public void Print()
    {
        Console.Clear();
        Console.WriteLine("ЛАБИРИНТ");
        if (showPath && shortestPath.Count > 0)
        {
            Console.WriteLine($"Кратчайший путь: {shortestPath.Count - 1} шагов");
        }

        Console.WriteLine("\nУправление:");
        Console.WriteLine("Стрелки - движение");
        Console.WriteLine("P - показать/скрыть кратчайший путь");
        Console.WriteLine("ESC - выход");
        Console.WriteLine();

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (showPath && shortestPath.Contains((x, y)) && !(x == playerX && y == playerY) && !(x == exitX && y == exitY))
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write("··");
                }
                else if (x == playerX && y == playerY)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write('P');
                    Console.Write('P');
                }
                else if (x == exitX && y == exitY)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write('E');
                    Console.Write('E');
                }
                else
                {
                    if (maze[y, x] == '#')
                    {
                        Console.ForegroundColor = ConsoleColor.Blue;
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    Console.Write(maze[y, x]);
                    Console.Write(maze[y, x]);
                }
                Console.ResetColor();
            }
            Console.WriteLine();
        }
    }

    public bool MovePlayer(ConsoleKey key)
    {
        int newX = playerX;
        int newY = playerY;

        switch (key)
        {
            case ConsoleKey.UpArrow:
                newY--;
                break;
            case ConsoleKey.DownArrow:
                newY++;
                break;
            case ConsoleKey.LeftArrow:
                newX--;
                break;
            case ConsoleKey.RightArrow:
                newX++;
                break;
            case ConsoleKey.P:
                if (showPath)
                {
                    showPath = false;
                }
                else
                {
                    FindShortestPath();
                }
                return true;
        }

        if (newX >= 0 && newX < width && newY >= 0 && newY < height && maze[newY, newX] == ' ')
        {
            playerX = newX;
            playerY = newY;
            if (showPath)
            {
                FindShortestPath();
            }
            return true;
        }

        return false;
    }

    public bool CheckWin()
    {
        return playerX == exitX && playerY == exitY;
    }
}

class Program
{
    static void Main()
    {
        Console.Title = "Лабиринт";
        Console.CursorVisible = false;

        int width = 35;
        int height = 25;

        MazeGenerator maze = new MazeGenerator(width, height);
        maze.Generate();

        bool playing = true;

        while (playing)
        {
            maze.Print();

            if (maze.CheckWin())
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Поздравляю! Вы прошли лабиринт!");

                Console.WriteLine("\nНажмите любую клавишу для выхода...");
                Console.ReadKey();
                break;
            }

            ConsoleKeyInfo keyInfo = Console.ReadKey(true);

            if (keyInfo.Key == ConsoleKey.Escape)
            {
                playing = false;
            }
            else
            {
                maze.MovePlayer(keyInfo.Key);
            }
        }

        Console.CursorVisible = true;
    }
}