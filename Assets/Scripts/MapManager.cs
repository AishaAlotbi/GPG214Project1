using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapManager : MonoBehaviour

{
    private static MapManager _instance;
    public static MapManager Instance { get { return _instance; } }

    public OverlayTile overlayTilePrefab;
    public GameObject overlayContainer;

    public Dictionary<Vector2Int, OverlayTile> map;



    private void Awake()
    {
        if (_instance !=null && _instance != this)
        {
            Destroy(this.gameObject);

        }
        else
        {
            _instance = this;
        }
    }

    private void Start()
    {
        var tileMap = gameObject.GetComponentInChildren<Tilemap>();
        map = new Dictionary<Vector2Int, OverlayTile>();

        BoundsInt bounds = tileMap.cellBounds;

        // Loop through all tiles 
        
        
            for (int y = bounds.min.y; y < bounds.max.y; y++)
            {
                for (int x = bounds.min.x; x < bounds.max.x; x++)
                {
                    Vector3Int tileLocation = new Vector3Int(x, y, (int)tileMap.transform.position.y);
                    Vector3 place = tileMap.CellToWorld(tileLocation);
                    var tileKey = new Vector2Int(x, y);

                    if (tileMap.HasTile(tileLocation) && !map.ContainsKey(tileKey))
                    // instantiates cursor 
                    {
                        var overlayTile = Instantiate(overlayTilePrefab, overlayContainer.transform);
                        var cellWorldPosition = tileMap.GetCellCenterWorld(tileLocation);

                        overlayTile.transform.position = new Vector3(cellWorldPosition.x, cellWorldPosition.y, 0);
                        overlayTile.GetComponent<SpriteRenderer>().sortingOrder = tileMap.GetComponent<TilemapRenderer>().sortingOrder +1;
                        overlayTile.gridLocation = tileLocation;

                        map.Add(tileKey, overlayTile);
                    }

                }
            }
        
    }

    public List<OverlayTile> GetNeighbourTiles(OverlayTile currentOverlayTile, List<OverlayTile> searchableTiles)
    {
        Dictionary<Vector2Int, OverlayTile> tileToSearch = new Dictionary<Vector2Int, OverlayTile>();

        if(searchableTiles.Count > 0)
        {
            foreach(var item in searchableTiles)
            {
                tileToSearch.Add(item.grid2DLocation, item);
            }
            
            
        }
        else
        {
            tileToSearch = map;
        }



        List<OverlayTile> neighbours = new List<OverlayTile>();

        // Check for neighbour tiles

        // Top Neighbour
        Vector2Int locationToCheck = new Vector2Int(
            currentOverlayTile.gridLocation.x,
            currentOverlayTile.gridLocation.y + 1
            );

        if (tileToSearch.ContainsKey(locationToCheck))
        {
            neighbours.Add(tileToSearch[locationToCheck]); // If tile exists add to neighbour
        }

        // Bottom Neighbour
        locationToCheck = new Vector2Int(
        currentOverlayTile.gridLocation.x,
        currentOverlayTile.gridLocation.y - 1
        );

        if (tileToSearch.ContainsKey(locationToCheck))
        {
            neighbours.Add(tileToSearch[locationToCheck]); // If tile exists add to neighbour
        }


        // Right Neighbour
        locationToCheck = new Vector2Int(
        currentOverlayTile.gridLocation.x + 1,
        currentOverlayTile.gridLocation.y
        );

        if (tileToSearch.ContainsKey(locationToCheck))
        {
            neighbours.Add(tileToSearch[locationToCheck]); // If tile exists add to neighbour
        }


        // Left Neighbour
        locationToCheck = new Vector2Int(
        currentOverlayTile.gridLocation.x - 1,
        currentOverlayTile.gridLocation.y
        );

        if (tileToSearch.ContainsKey(locationToCheck))
        {
            neighbours.Add(tileToSearch[locationToCheck]); // If tile exists add to neighbour
        }

        return neighbours;
    }

}


