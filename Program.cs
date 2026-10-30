using System;
using System.Collections.Generic;

public readonly record struct Point(int X, int Y);

/// <summary>Grid with walkable cells and walls.</summary>
public sealed class Grid
{
    private readonly bool[,] _walls;

    public int Width { get; }
    public int Height { get; }

    public Grid(int width, int height)
    {
        if (width <= 0) throw new ArgumentOutOfRangeException(nameof(width));
        if (height <= 0) throw new ArgumentOutOfRangeException(nameof(height));

        Width = width;
        Height = height;
        _walls = new bool[width, height];
    }

    public bool IsInside(Point p) => p.X >= 0 && p.X < Width && p.Y >= 0 && p.Y < Height;

    public bool IsWalkable(Point p) => IsInside(p) && !_walls[p.X, p.Y];

    public void SetWall(Point p)
    {
        if (!IsInside(p)) throw new ArgumentOutOfRangeException(nameof(p));
        _walls[p.X, p.Y] = true;
    }

    /// <summary>Builds a grid from text: '#' wall, 'S' start, 'G' goal, anything else is free.</summary>
    public static Grid FromRows(string[] rows, out Point start, out Point goal)
    {
        if (rows.Length == 0) throw new ArgumentException("Map is empty", nameof(rows));

        var grid = new Grid(rows[0].Length, rows.Length);
        Point? s = null, g = null;

        for (int y = 0; y < rows.Length; y++)
        {
            if (rows[y].Length != grid.Width)
                throw new ArgumentException($"Row {y} has a different length", nameof(rows));

            for (int x = 0; x < grid.Width; x++)
            {
                switch (rows[y][x])
                {
                    case '#': grid.SetWall(new Point(x, y)); break;
                    case 'S': s = new Point(x, y); break;
                    case 'G': g = new Point(x, y); break;
                }
            }
        }

        start = s ?? throw new ArgumentException("Map has no start 'S'");
        goal = g ?? throw new ArgumentException("Map has no goal 'G'");
        return grid;
    }
}

/// <summary>A* pathfinding (4 directions, Manhattan heuristic).</summary>
public static class AStar
{
    private static readonly Point[] Directions = { new(0, -1), new(1, 0), new(0, 1), new(-1, 0) };

    /// <returns>Path from start to goal inclusive, or null if no path exists.</returns>
    public static List<Point>? FindPath(Grid grid, Point start, Point goal)
    {
        if (!grid.IsWalkable(start) || !grid.IsWalkable(goal))
            return null;

        var open = new PriorityQueue<Point, int>();
        var cameFrom = new Dictionary<Point, Point>();
        var cost = new Dictionary<Point, int> { [start] = 0 };
        var closed = new HashSet<Point>();

        open.Enqueue(start, Heuristic(start, goal));

        while (open.TryDequeue(out Point current, out _))
        {
            if (current == goal)
                return BuildPath(cameFrom, current);

            // The queue may contain stale duplicates of a cell, skip them
            if (!closed.Add(current))
                continue;

            foreach (Point dir in Directions)
            {
                var next = new Point(current.X + dir.X, current.Y + dir.Y);

                if (!grid.IsWalkable(next) || closed.Contains(next))
                    continue;

                int newCost = cost[current] + 1;

                if (cost.TryGetValue(next, out int oldCost) && newCost >= oldCost)
                    continue;

                cost[next] = newCost;
                cameFrom[next] = current;
                open.Enqueue(next, newCost + Heuristic(next, goal));
            }
        }

        return null;
    }

    private static int Heuristic(Point a, Point b) => Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y);

    private static List<Point> BuildPath(Dictionary<Point, Point> cameFrom, Point end)
    {
        var path = new List<Point> { end };

        while (cameFrom.TryGetValue(end, out Point previous))
        {
            path.Add(previous);
            end = previous;
        }

        path.Reverse();
        return path;
    }
}

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
