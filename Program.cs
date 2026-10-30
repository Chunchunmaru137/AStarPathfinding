using System;
using System.Collections.Generic;

public static class Program
{
    public static void Main()
    {
        string[] map =
        {
            "S...#.......",
            ".##.#.#####.",
            ".#..#.....#.",
            ".#.######.#.",
            ".#........#.",
            ".########.#.",
            "..........#G",
        };

        Grid grid = Grid.FromRows(map, out Point start, out Point goal);
        List<Point>? path = AStar.FindPath(grid, start, goal);

        if (path == null)
        {
            Console.WriteLine("Path not found.");
            return;
        }

        Console.WriteLine($"Path length: {path.Count - 1} steps\n");
        Print(map, path);
    }

    private static void Print(string[] map, List<Point> path)
    {
        char[][] canvas = Array.ConvertAll(map, row => row.ToCharArray());

        foreach (Point p in path)
        {
            if (canvas[p.Y][p.X] == '.')
                canvas[p.Y][p.X] = '*';
        }

        foreach (char[] row in canvas)
            Console.WriteLine(new string(row));
    }
}
