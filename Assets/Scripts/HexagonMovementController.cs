using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class HexagonMovementController : MonoBehaviour
{

    public Tilemap fogOfWar;
    public int vision = 1;

    private Vector3 newPosition;
    private Vector3Int newCellPos;
    private Tilemap battleField;
    private RaycastHit2D hit => Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
    private bool hasMoved;

    void Update(){
        if (Input.GetMouseButtonDown(0)){
            OnMouseClick();
        }
    }

    void FixedUpdate(){
    }

    private void OnMouseClick(){
        MoveToMouseClick();
    }

    private void MoveToMouseClick(){
        if(hit.collider != null && hit.collider is TilemapCollider2D)
        {
            battleField = hit.collider.GetComponent<Tilemap>();
            newCellPos = battleField.WorldToCell(new Vector3(hit.point.x, hit.point.y, battleField.transform.position.z));
            SetNewPosition(battleField.CellToWorld(newCellPos)); 
            UpdateFogOfWar();
        }
    }

    private void UpdateFogOfWar(){
        Vector3Int currentPlayerTile = fogOfWar.WorldToCell(transform.position);

        for (int x =- vision; x <= vision; x++){
            for (int y =- vision; y <= vision; y++){
                //if (fogOfWar.GetTile(currentPlayerTile + new Vector3Int(x, y, 0)) != null)
                    fogOfWar.SetTile(currentPlayerTile + new Vector3Int(x, y, 0), null);
                Debug.Log(currentPlayerTile + new Vector3Int(x, y, 0));
            }
        }
    }

    public void SetNewPosition(Vector3 newPosition){
        this.newPosition = newPosition;
        transform.position = new Vector3(newPosition.x, newPosition.y, transform.position.z);
    }
}
