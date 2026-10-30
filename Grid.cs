using System;

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
