using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.AI;

public class HexagonMovementController : MonoBehaviour
{
    private TilemapService tilemapService;

    // FoW параметры
    public int vision = 4;
    private FoWController foWController;

    // Параметры перемещения
    private NavMeshAgent agent;
    private bool isMoving;
    private Vector3 targetPosition;
    
    // Параметры зоны передвижения
    private Tilemap movementZone;
    private bool isClear;
    public int movementRadius = 2;

    // Параметры зоны действия
    private Tilemap actionZone;
    private Vector3Int lastCursorTilePosition;
    private TileBase lastCursorTile;
    private TileBase currentCursorTile;
    private string[] actionLayerMask = {"Obstacles", "Objects", "MovementZone"};
    private string[] terrainLayerMask = {"Terrain"};


    // Инициализируем RaycastHit для работы с мышью
    private RaycastHit2D tempHit;
    private Tilemap tempTilemap;
    private Vector3 tempMousePosition;
    private Vector3Int tempMouseTilePosition;


    // ===== ======== =====
    //       Triggers 
    // ===== ======== =====

    void Start(){
        // Инициализируем переменные
        foWController =  foWController = Object.FindObjectOfType<FoWController>();
        actionZone = GameObject.FindGameObjectWithTag("ActionZone").GetComponent<Tilemap>();
        movementZone = GameObject.FindGameObjectWithTag("MovementZone").GetComponent<Tilemap>();
        tilemapService = Object.FindObjectOfType<TilemapService>();

        // Инициализируем параметры NavAgent
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        isMoving = false;
        isClear = false;

        // Инициализируем функции
        UpdateFogOfWar();
        LoadMovementZone();
    }

    void Update(){
        UpdateRaycastPoint();
        if (tempHit.collider != null){
            ListenMouseClick();
            ListenMouseMovement();
        }
    }

    void FixedUpdate(){
        UpdateMovement();
    }

    private void ListenMouseMovement(){
        InitCurrentCursorTile();
    }

    private void ListenMouseClick(){
        if (Input.GetMouseButtonDown(0)){
            MoveToMouseClick();
        }
    }
    
    // ===== ========== =====
    //       ActionZone 
    // ===== ========== =====

    private void UpdateRaycastPoint(){
        InitTempHit();
        if (tempHit.collider != null){
            Debug.Log(tempHit.collider.tag);
            UpdateCursorTile();
            tempTilemap = tempHit.collider.GetComponent<Tilemap>();
            tempMouseTilePosition = tempTilemap.WorldToCell(new Vector3(tempHit.point.x, tempHit.point.y, tempTilemap.transform.position.z));
            tempMousePosition = tempTilemap.CellToWorld(tempMouseTilePosition);
        }
    }

    private void InitTempHit(){
        tempHit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, Camera.main.farClipPlane, LayerMask.GetMask(actionLayerMask));
        if (tempHit.collider == null) {
            tempHit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, Camera.main.farClipPlane, LayerMask.GetMask(terrainLayerMask));
        }        
    }

    private void UpdateCursorTile(){
        if (lastCursorTilePosition.x != tempMouseTilePosition.x || lastCursorTilePosition.y != tempMouseTilePosition.y){
            actionZone.SetTile(lastCursorTilePosition, lastCursorTile);
            lastCursorTile = actionZone.GetTile(tempMouseTilePosition);
            lastCursorTilePosition = tempMouseTilePosition;
            actionZone.SetTile(tempMouseTilePosition, currentCursorTile);
        }
    }

    private void InitCurrentCursorTile(){
        if (tempHit.collider.tag == "Obstacles"){
            currentCursorTile = actionZone.GetComponent<ActionZoneController>().blockedTile;
        } else if (tempHit.collider.tag == "MovementZone"){
            currentCursorTile = actionZone.GetComponent<ActionZoneController>().allowedTile;
        } else if (tempHit.collider.tag == "Terrain"){
            currentCursorTile = actionZone.GetComponent<ActionZoneController>().cursor;
        }
    }

    // ===== ======== =====
    //       Movement 
    // ===== ======== =====

    // Перемещает объект на тайл по навигационной сетке
    private void MoveToMouseClick(){
        if(!isMoving && tempHit.collider.tag == "MovementZone") {
            targetPosition = tempMousePosition;
            agent.SetDestination(new Vector3(targetPosition.x, targetPosition.y, transform.position.z));
            isMoving = true;
            UpdateMovementZone();
        }
    }

    // Метод обновляет перемещение объекта
    public void UpdateMovement(){
        if (isMoving) {
            if (isMoving && targetPosition.x == Round(transform.position.x, 1) && targetPosition.z == Round(transform.position.z, 1)){
                transform.position = targetPosition;
                isMoving = false;
                UpdateMovementZone();
            } else {
                UpdateFogOfWar();
            }
        }
    }

    // // Телепортирует объект на тайл, по которому кликнули мышкой
    // private void TeleportToMouseClick(){
    //     targetPosition = tempMousePosition;
    //     SetNewPosition(tempTilemap.CellToWorld(tempMouseTilePosition));
    //     UpdateFogOfWar();
    // }

    // // Телепортирует объект к выбранным координатам
    // public void SetNewPosition(Vector3 newPosition){
    //     this.tempMousePosition = newPosition;
    //     transform.position = new Vector3(newPosition.x, newPosition.y, transform.position.z);
    // }

    private void UpdateMovementZone(){
        isClear = !isClear;
        if (isClear)
            ClearMovementZone();
        else 
            LoadMovementZone();
    }

    public void LoadMovementZone(){
        tilemapService.FillHex(movementZone, movementZone.GetComponent<ActionZoneController>().movementZoneTile, movementZone.WorldToCell(transform.position), movementRadius);
        movementZone.GetComponent<CompositeCollider2D>().GenerateGeometry();
        isClear = false;
    }

    public void ClearMovementZone(){
        movementZone.ClearAllTiles();
        isClear = true;
    }


    // ===== === =====
    //       FoW 
    // ===== === =====

    // Обновляет состояние тумана войны (чистить в радиусе)
    private void UpdateFogOfWar(){
        foWController.ClearFoW(transform.position, vision);
    }

    // ===== ===== =====
    //       Other 
    // ===== ===== =====

    private float Round(float value, int order){
        return ((float) Mathf.Round(value * Mathf.Pow(10, order))) / Mathf.Pow(10, order);
    }
}