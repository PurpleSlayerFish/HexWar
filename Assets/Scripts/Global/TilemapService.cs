using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.AI;
public class TilemapService : MonoBehaviour{
    
    // Заполняет гексагон
    public void FillHex(Tilemap tilemap, Tile tile, Vector3Int startPosition, int radius){
        FillCone(tilemap, tile, startPosition, 1, radius);
        FillConeOpposide(tilemap, tile, startPosition, -1, radius);
        FillRhombus(tilemap, tile, startPosition, 1, radius);
        FillRhombus(tilemap, tile, startPosition, -1, radius);
    }

    // Заполняет конус в указанном направлении (сверху и снизу)
    private void FillConeOpposide(Tilemap tilemap, Tile tile, Vector3Int startPosition, int direction, int length){
        for (int y = 1; y <= length; y++){
            for (int x = 0; x <= y; x++){
                if ((startPosition.y % 2) == 0)
                    FillTile(tilemap, tile, startPosition + new Vector3Int(-x + (y / 2), direction * y, 0));
                else
                    FillTile(tilemap, tile, startPosition + new Vector3Int(-x + (y / 2) + (y % 2), direction * y, 0));
            }
        }
    }

    // Заполняет конус в указанном направлении (сверху и снизу) и текущую точку
    private void FillCone(Tilemap tilemap, Tile tile, Vector3Int startPosition, int direction, int length){
        FillTile(tilemap, tile, startPosition);
        FillConeOpposide(tilemap, tile, startPosition, 1, length);
    }

    
    // Заполняет вертикальный ромб (слева или справа от координат)
    private void FillRhombus(Tilemap tilemap, Tile tile, Vector3Int startPosition, int direction, int length){
        if (direction < 0)
            startPosition = startPosition + new Vector3Int(length + 1, 0, 0);
        int y = 0;
        for (int x = 0; x < length; x++){
            if((startPosition.y % 2) == 0)
                FillDiagonalLine(tilemap, tile, startPosition + new Vector3Int(x - (y / 2) - (y % 2), y, 0), 1, length);
            else
                FillDiagonalLine(tilemap, tile, startPosition + new Vector3Int(x - (y / 2), y, 0), 1, length);
            y++;
        }
    }

    // Заполняет диагональную линию (координаты смещены, нужно учесть)
    private void FillDiagonalLine(Tilemap tilemap, Tile tile, Vector3Int startPosition, int direction, int length){
        int y = 0;
        for (int x = length; x > 0; x--){
            if ((startPosition.y % 2) == 0)
                FillTile(tilemap, tile, startPosition + new Vector3Int(- x + (y / 2) + (y % 2), direction * y, 0));
            else
                FillTile(tilemap, tile, startPosition + new Vector3Int(- x + (y / 2), direction * y, 0));
            y--;
        }
    }

    // Заполняет тайл. Раньше было оптимизировано, но тогда actionZone не работает
    private void FillTile(Tilemap tilemap, Tile tile, Vector3Int tilePosition){
        // if (tilemap.GetTile(tilePosition) != null)
            tilemap.SetTile(tilePosition, tile);
    }
}