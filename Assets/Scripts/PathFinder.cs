using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System;

public class PathFinder 
{
    public List<OverlayTile> FindPath(OverlayTile start, OverlayTile end, List<OverlayTile> searchableTiles)
    {
        List<OverlayTile> openList = new List<OverlayTile>(); // List of Tiles to check
        List<OverlayTile> closedList = new List<OverlayTile>(); // List of checked Tiles

        openList.Add(start);

        while(openList.Count > 0)
        {
            // Find OverlayTile with lowest cost

            OverlayTile currentOverlayTile = openList.OrderBy(x => x.F).First(); // Gives overlay tile with the lowest f in the open list

            openList.Remove(currentOverlayTile); // Found tile in path, remove it
            closedList.Add(currentOverlayTile); // And add it to the closed list

            if (currentOverlayTile == end)
            {
                // finalize path

                return GetFinishedList(start, end);

            }

            var neighbourTiles = MapManager.Instance.GetNeighbourTiles(currentOverlayTile, searchableTiles);


            foreach(var neighbour in neighbourTiles)
            {
                if(neighbour.isBlocked || closedList.Contains(neighbour))
                {
                    continue;
                }

                neighbour.G = GetManhattenDistance(start, neighbour);
                neighbour.H = GetManhattenDistance(end, neighbour);

                neighbour.previous = currentOverlayTile;

                if (!openList.Contains(neighbour))
                {
                    openList.Add(neighbour);
                }

            }
        }

        return new List<OverlayTile>();
    }

    private List<OverlayTile> GetFinishedList(OverlayTile start, OverlayTile end)
    {
        List<OverlayTile> finishedList = new List<OverlayTile>();

        OverlayTile currentTile = end;

        while(currentTile != start) // loop through past nodes and return node
        {
            finishedList.Add(currentTile);
            currentTile = currentTile.previous;
        }

        finishedList.Reverse();
        return finishedList;
;
    }

    private int GetManhattenDistance(OverlayTile start, OverlayTile neighbour)
    {
        return Mathf.Abs(start.gridLocation.x - neighbour.gridLocation.x) + Mathf.Abs(start.gridLocation.y - neighbour.gridLocation.y);
    }

    
}
