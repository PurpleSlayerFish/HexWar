using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameLoadingController : MonoBehaviour
{
    void FixedUpdate()
    {
       if (Input.anyKey)
        {
            SceneManager.LoadScene("HubNavigationScene");

            // SceneManager.UnloadSceneAsync("GameLoadingScene");
        } 
    }
}
