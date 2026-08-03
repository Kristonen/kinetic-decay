using System.Numerics;
using Core.Ecs;
using Raylib_cs;

public class SpatialGrid
{
    public Dictionary<(int x, int y), List<EntityId>> Grid { get => _grid; }
    public int CellSize { get => CELL_SIZE; }
    private const int CELL_SIZE = 64;
    private readonly Dictionary<(int x, int y), List<EntityId>> _grid = new();

    public (int x, int y) GetCellCoords(Vector2 pos)
    {
        return (
            (int)Math.Floor(pos.X / CELL_SIZE),
            (int)Math.Floor(pos.Y / CELL_SIZE)
        );
    }

    public void Clear()
    {
        foreach (var list in _grid.Values)
        {
            list.Clear();
        }
        _grid.Clear();
    }

    public void Insert(EntityId id, Transform t, PhysicsBody body)
    {
        int minX, maxX, minY, maxY;

        if (body.Shape == ShapeType.Circle)
        {
            Vector2 pos = body.GetCirPos(t.Pos);
            float radius = body.Width / 2;

            minX = (int)MathF.Floor((pos.X - radius) / CELL_SIZE);
            maxX = (int)MathF.Floor((pos.X + radius) / CELL_SIZE);
            minY = (int)MathF.Floor((pos.Y - radius) / CELL_SIZE);
            maxY = (int)MathF.Floor((pos.Y + radius) / CELL_SIZE);
        }
        else
        {
            Rectangle rec = body.GetRec(t);
            minX = (int)MathF.Floor(rec.X / CELL_SIZE);
            maxX = (int)MathF.Floor((rec.X + rec.Width) / CELL_SIZE);
            minY = (int)MathF.Floor(rec.Y / CELL_SIZE);
            maxY = (int)MathF.Floor((rec.Y + rec.Height) / CELL_SIZE);
        }

        for (int x = minX; x <= maxX; x++)
        {
            for (int y = minY; y <= maxY; y++)
            {
                var cell = (x, y);
                if (!_grid.TryGetValue(cell, out var list))
                {
                    list = new();
                    _grid[cell] = list;
                }
                list.Add(id);
            }
        }
    }

    public List<EntityId> GetEntitiesInSameCell(Vector2 pos)
    {
        int cellX = (int)MathF.Floor(pos.X / CELL_SIZE);
        int cellY = (int)MathF.Floor(pos.Y / CELL_SIZE);

        if (_grid.TryGetValue((cellX, cellY), out var list))
        {
            return list;
        }

        return new();
    }

    public List<EntityId> GetNeighbors(Vector2 pos)
    {
        var centerCell = GetCellCoords(pos);
        List<EntityId> neighbors = new();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                var neighborCell = (centerCell.x + x, centerCell.y + y);
                if (_grid.TryGetValue(neighborCell, out var list))
                {
                    neighbors.AddRange(list);
                }
            }
        }
        return neighbors;
    }
}
