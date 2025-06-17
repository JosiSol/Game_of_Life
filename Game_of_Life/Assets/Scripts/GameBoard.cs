using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameBoard : MonoBehaviour
{
    public Tilemap currentState;
    public Tilemap nextState;
    public Tile aliveTile;
    public Tile deadTile;
    private float updateInterval = 0.05f;
    public Pattern pattern;
    private HashSet<Vector3Int> aliveCells;
    private HashSet<Vector3Int> checkCells;
    public int population {get; private set; }
    public int iterations {get; private set; }
    public float time {get; private set; }

    List<Vector2Int> drxn = new List<Vector2Int>
    {
        new Vector2Int(0, 1), new Vector2Int(1, 0), new Vector2Int(-1, 0), new Vector2Int(0, -1),
        new Vector2Int(1, 1), new Vector2Int(-1, -1), new Vector2Int(1, -1), new Vector2Int(-1, 1)
    };

    private void Awake()
    {
        aliveCells = new HashSet<Vector3Int>();
        checkCells = new HashSet<Vector3Int>();
    }
    private void Start()
    {
        SetPattern(pattern);
    }
    private void SetPattern(Pattern pattern)
    {
        Clear();

        Vector2Int center = pattern.getCenter();

        foreach (Vector2Int cell in pattern.cells)
        {
            Vector3Int adjusted = (Vector3Int)(cell - center);
            currentState.SetTile(adjusted, aliveTile);
            aliveCells.Add(adjusted);
        }
        population = aliveCells.Count;

    }
    private void Clear()
    {
        currentState.ClearAllTiles();
        nextState.ClearAllTiles();
        aliveCells.Clear();
        checkCells.Clear();
        population = 0;
        iterations = 0;
        time = 0f;
    }
    private void OnEnable()
    {
        StartCoroutine(Animate());
    }
    private IEnumerator Animate()
    {
        var interval = new WaitForSeconds(updateInterval);
        yield return interval;

        while (enabled)
        {
            UpdateState();
            population = aliveCells.Count;
            iterations++;
            time += updateInterval;

            yield return interval;
        }
    }
    private void UpdateState()
    {
        checkCells.Clear();
        foreach (Vector3Int cell in aliveCells)
        {
            foreach (Vector2Int d in drxn)
            {
                int x = cell.x + d.x;
                int y = cell.y + d.y;
                Vector3Int neighbor = new Vector3Int(x, y);
                checkCells.Add(neighbor);
            }
            checkCells.Add(cell);
        }

        HashSet<Vector3Int> nextAliveCells = new HashSet<Vector3Int>();

        foreach (Vector3Int cell in checkCells)
        {
            int neighbors = countNeighbors(cell);
            bool alive = isAlive(cell);
            if (!alive && neighbors == 3)
            {
                // Reproduction, cell becomes alive
                nextState.SetTile(cell, aliveTile);
                nextAliveCells.Add(cell);
            }
            else if (alive && (neighbors == 2 || neighbors == 3))
            {
                // Survival, cell stays alive
                nextState.SetTile(cell, aliveTile);
                nextAliveCells.Add(cell);
            }
        }

        aliveCells = nextAliveCells;

        Tilemap temp = currentState;
        currentState = nextState;
        nextState = temp;
        nextState.ClearAllTiles();
    }
    private int countNeighbors(Vector3Int cell)
    {
        int count = 0;
        foreach (Vector2Int d in drxn)
        {
            int x = cell.x + d.x;
            int y = cell.y + d.y;
            Vector3Int neighbor = new Vector3Int(x, y);

            count += isAlive(neighbor) ? 1 : 0;
        }

        return count;

    }
    private bool isAlive(Vector3Int cell)
    {
        return currentState.GetTile(cell) == aliveTile;
    }

}
