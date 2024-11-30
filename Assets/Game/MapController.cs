using UnityEngine;

public class MapController : MonoBehaviour
{
    public static MapController Instance { get; private set; }
    private int[,] _grid;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void Initialize(int width, int height)
    {
        _grid = new int[width, height];
    }

    public void UpdateCell(Vector2Int position, int ownerID)
    {
        _grid[position.x, position.y] = ownerID;
        Debug.Log($"Cell updated at {position}, Owner: {ownerID}");
    }
}
