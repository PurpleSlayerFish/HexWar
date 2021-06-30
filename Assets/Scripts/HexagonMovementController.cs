using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.AI;

public class HexagonMovementController : MonoBehaviour
{
    // FoW параметры
    public Tilemap fogOfWar;
    public int vision = 1;

    // 
    private Vector3 newPosition;
    private Vector3Int newTilePosition;
    private Tilemap terrain;

    // Инициализируем RaycastHit для работы с мышью
    private RaycastHit2D hit => Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

    private NavMeshAgent agent;
    private Vector3 target;


    void Start(){
        // Инициализируем параметры NavAgent
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    void Update(){
        if (Input.GetMouseButtonDown(0)){
            OnMouseClick();
        }
    }

    void FixedUpdate(){
        UpdateFogOfWar();
    }

    private void OnMouseClick(){
        MoveToMouseClick();
    }

    // Перемещает объект на тайл по навигационной сетке
    private void MoveToMouseClick(){
        if(hit.collider != null /*&& hit.collider is TilemapCollider2D*/)
        {
            Debug.Log(hit.transform.tag);
            if(hit.transform.tag == "Terrain"){
                terrain = hit.collider.GetComponent<Tilemap>();
                newTilePosition = terrain.WorldToCell(new Vector3(hit.point.x, hit.point.y, terrain.transform.position.z));
                newPosition = terrain.CellToWorld(newTilePosition);
                agent.SetDestination(new Vector3(newPosition.x, newPosition.y, transform.position.z));
            }
        }
    }

    // Телепортирует объект на тайл, по которому кликнули мышкой
    private void TeleportToMouseClick(){
        if(hit.collider != null /*&& hit.collider is TilemapCollider2D*/)
        {
            if(hit.transform.tag == "Terrain"){
                terrain = hit.collider.GetComponent<Tilemap>();
                newTilePosition = terrain.WorldToCell(new Vector3(hit.point.x, hit.point.y, terrain.transform.position.z));
                SetNewPosition(terrain.CellToWorld(newTilePosition)); 
                UpdateFogOfWar();
            }
        }
    }

    // Телепортирует объект к выбранным координатам
    public void SetNewPosition(Vector3 newPosition){
        this.newPosition = newPosition;
        transform.position = new Vector3(newPosition.x, newPosition.y, transform.position.z);
    }

    // Обновляет состояние тумана войны (чистить в радиусе)
    private void UpdateFogOfWar(){
        Vector3Int currentPlayerTile = fogOfWar.WorldToCell(transform.position);
        FillHex(fogOfWar, null, currentPlayerTile, vision);
    }

    // Заполняет гексагон
    private void FillHex(Tilemap tilemap, Tile tile, Vector3Int startPosition, int radius){
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

    // Заполняет тайл
    private void FillTile(Tilemap tilemap, Tile tile, Vector3Int tilePosition){
        if (tilemap.GetTile(tilePosition) != null)
            tilemap.SetTile(tilePosition, tile);
    }
}