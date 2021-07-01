using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class FoWController : MonoBehaviour
{
    private Tilemap fowTilemap;
    private TilemapService tilemapService;

    void Awake(){
        tilemapService = Object.FindObjectOfType<TilemapService>();
        fowTilemap = GetComponent<Tilemap>();
    }
    public void ClearFoW(Vector3 position, int radius){
        tilemapService.FillHex(fowTilemap, null, fowTilemap.WorldToCell(position), radius);
    }
}
