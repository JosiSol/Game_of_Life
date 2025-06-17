using UnityEngine;

[CreateAssetMenu(fileName = "New_Pattern", menuName = "Game_of_Life/Pattern")]
public class Pattern : ScriptableObject
{
    public Vector2Int[] cells;

    public Vector2Int getCenter()
    {
        if (cells == null || cells.Length == 0)
            return Vector2Int.zero;

        Vector2Int min = Vector2Int.zero;
        Vector2Int max = Vector2Int.zero;

        foreach (Vector2Int cell in cells)
        {
            min.x = Mathf.Min(min.x, cell.x);
            min.x = Mathf.Min(min.y, cell.y);
            max.x = Mathf.Max(max.x, cell.x);
            max.y = Mathf.Max(max.y, cell.y);
        }

        return (min + max) / 2;
    }
}
