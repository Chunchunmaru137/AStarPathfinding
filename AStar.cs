using System;
using System.Collections.Generic;

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
