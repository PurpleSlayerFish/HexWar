using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class CampController : MonoBehaviour
{
    GraphicRaycaster raycaster;
    PointerEventData pointerEventData;
    EventSystem eventSystem;

    void Start()
    {
        // Подготавливаем объекты для рейкаста UI
        raycaster = GetComponent<GraphicRaycaster>();
        eventSystem = GetComponent<EventSystem>();
    }

    void Update()
    {
        OnMouseClick();
    }

    private void OnMouseClick(){
        // При нажатии на кнопку мыши  производим рейкаст и активируем нажимаемый объект UI
        if (Input.GetMouseButtonDown(0))
        {
            pointerEventData = new PointerEventData(eventSystem);
            pointerEventData.position = Input.mousePosition;
            List<RaycastResult> results = new List<RaycastResult>();
            raycaster.Raycast(pointerEventData, results);
            foreach (RaycastResult result in results)
            {
                if (result.gameObject.name == "ToNavigation"){
                    SceneManager.LoadScene("HubNavigationScene");
                    break;
                }
            }
        }
    }
}
