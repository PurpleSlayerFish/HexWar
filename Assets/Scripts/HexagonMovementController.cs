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
    private Vector3 newPosition;
    private Vector3Int newTilePosition;
    private Tilemap terrain;
    private bool isMoving;
    
    // Параметры зоны действия
    private Tilemap actionZone;
    private bool isCleared;
    public int actionRadius = 2;


    

    // Инициализируем RaycastHit для работы с мышью
    private RaycastHit2D hit => Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

    void Start(){
        // Инициализируем переменные
        foWController =  foWController = Object.FindObjectOfType<FoWController>();
        actionZone = GameObject.FindGameObjectWithTag("ActionZone").GetComponent<Tilemap>();
        tilemapService = Object.FindObjectOfType<TilemapService>();

        // Инициализируем параметры NavAgent
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        isMoving = false;
        isCleared = true;

        // Инициализируем функции
        UpdateFogOfWar();
        LoadActionZone();
    }

    void Update(){
        if (Input.GetMouseButtonDown(0)){
            OnMouseClick();
        }
    }

    void FixedUpdate(){
        UpdateMovement();
    }

    private void OnMouseClick(){
        MoveToMouseClick();
    }

    // Перемещает объект на тайл по навигационной сетке
    private void MoveToMouseClick(){
        if(!isMoving && hit.collider != null /*&& hit.collider is TilemapCollider2D*/)
        {
            // Debug.Log(hit.transform.tag);
            if(hit.transform.tag == "Terrain"){
                terrain = hit.collider.GetComponent<Tilemap>();
                newTilePosition = terrain.WorldToCell(new Vector3(hit.point.x, hit.point.y, terrain.transform.position.z));
                newPosition = terrain.CellToWorld(newTilePosition);
                agent.SetDestination(new Vector3(newPosition.x, newPosition.y, transform.position.z));
                isMoving = true;
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

    // Метод обновляет перемещение объекта
    public void UpdateMovement(){
        if (isMoving) {
            if (isMoving && newPosition.x == Round(transform.position.x, 1) && newPosition.z == Round(transform.position.z, 1)){
                transform.position = newPosition;
                isMoving = false;
            } else {
                UpdateFogOfWar();
            }
        }
    }

    public void LoadActionZone(){
        tilemapService.FillHex(actionZone, actionZone.GetComponent<ActionZoneController>().movementZoneTile, actionZone.WorldToCell(transform.position), actionRadius);
    }

    public void ClearActionZone(){
        actionZone.ClearAllTiles();
    }

    // Обновляет состояние тумана войны (чистить в радиусе)
    private void UpdateFogOfWar(){
        foWController.ClearFoW(transform.position, vision);
    }

    private float Round(float value, int order){
        return ((float) Mathf.Round(value * Mathf.Pow(10, order))) / Mathf.Pow(10, order);
    }
}