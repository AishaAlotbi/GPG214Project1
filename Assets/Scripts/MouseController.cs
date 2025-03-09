using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static ArrowTranslator;

public class MouseController : MonoBehaviour
{
    public float speed;
    public GameObject characterPrefab;
    private CharacterInfo character;

    
    private PathFinder pathFinder;
    private RangeFinder rangeFinder;
    private ArrowTranslator arrowTranslator;
    private List<OverlayTile> path = new List<OverlayTile>();
    private List<OverlayTile> inRangeTiles = new List<OverlayTile>();

    [System.Obsolete]
    private void Start()
    {
        pathFinder = new PathFinder();
        rangeFinder = new RangeFinder();
        arrowTranslator = new ArrowTranslator();


    }

    bool isMoving = false;

    void LateUpdate()
    {


        var focusedTileHit = GetFocusedOnTile();

        if (focusedTileHit.HasValue)
        {
            OverlayTile overlayTile = focusedTileHit.Value.collider.gameObject.GetComponent<OverlayTile>();
            transform.position = overlayTile.transform.position;
            gameObject.GetComponent<SpriteRenderer>().sortingOrder = overlayTile.GetComponent<SpriteRenderer>().sortingOrder;

            if (inRangeTiles.Contains(overlayTile) && !isMoving)
            {
                path = pathFinder.FindPath(character.activeTile, overlayTile, inRangeTiles);
                Debug.Log(path.Count);

                foreach(var item in inRangeTiles)
                {
                    item.SetArrowSprite(ArrowDirection.None);
                }

                for (int i = 0; i < path.Count; i++)
                { 
                    var previousTile = i > 0 ? path[i - 1] : character.activeTile;
                    var futureTile = i < path.Count - 1 ? path[i + 1] : null;

                    var arrowDir = arrowTranslator.TranslateDirection(previousTile, path[i], futureTile);
                    path[i].SetArrowSprite(arrowDir);
                }
            }


            if (Input.GetMouseButtonDown(0))
            {            
                if(character == null)
                {
                    character = Instantiate(characterPrefab).GetComponent<CharacterInfo>();
                    PositionCharacterOnTile(overlayTile);
                    GetInRangeTiles();
                }
                else
                {
                    isMoving = true;
                }  
                                   
            }

           
        }

        if(path.Count > 0 && isMoving)
        { 
            
          MoveAlongPath();
              
        }
    }

    private void GetInRangeTiles()
    {
        foreach (var item in inRangeTiles)
        {
            item.HideTile();
        }

        inRangeTiles = rangeFinder.GetTilesInRange(character.activeTile, 3);

        foreach (var item in inRangeTiles)
        {
            item.ShowTile();
        }
    }

    private void MoveAlongPath()
    {
        var step = speed * Time.deltaTime;
        
        character.transform.position = Vector2.MoveTowards(character.transform.position, path[0].transform.position,step);

        if(Vector2.Distance(character.transform.position, path[0].transform.position) < 0.0001f)
        {
            PositionCharacterOnTile(path[0]);
            path.RemoveAt(0);
        }

        if(path.Count == 0)
        {
            GetInRangeTiles();
            isMoving = false;
        }
    }

    public RaycastHit2D? GetFocusedOnTile()
    {

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mousePos2d = new Vector2(mousePos.x, mousePos.y);
        RaycastHit2D[] hits = Physics2D.RaycastAll(mousePos2d, Vector2.zero);

        if(hits.Length > 0)
        {
            return hits.OrderByDescending(i => i.collider.transform.position.z).First();
        }

        return null;

    }

    private void PositionCharacterOnTile(OverlayTile tile)
    {
       character.transform.position = new Vector2(tile.transform.position.x, tile.transform.position.y);
       character.GetComponent<SpriteRenderer>().sortingOrder = tile.GetComponent<SpriteRenderer>().sortingOrder +2;
       character.activeTile = tile;
    }
}
