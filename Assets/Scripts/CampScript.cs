using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class CampScript : MonoBehaviour
{
    GraphicRaycaster m_Raycaster;
    PointerEventData m_PointerEventData;
    EventSystem m_EventSystem;

    void Start()
    {
        // Подготавливаем объекты для рейкаста UI
        m_Raycaster = GetComponent<GraphicRaycaster>();
        m_EventSystem = GetComponent<EventSystem>();
    }

    void FixedUpdate()
    {
        onMouseClick();
    }

    private void onMouseClick(){
        // При нажатии на кнопку мыши  производим рейкаст и активируем нажимаемый объект UI
        if (Input.GetKey(KeyCode.Mouse0))
        {
            m_PointerEventData = new PointerEventData(m_EventSystem);
            m_PointerEventData.position = Input.mousePosition;
            List<RaycastResult> results = new List<RaycastResult>();
            m_Raycaster.Raycast(m_PointerEventData, results);
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
