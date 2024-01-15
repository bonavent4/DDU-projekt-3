using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class MainMenuScript : MonoBehaviour
{
    [SerializeField]int sceneNumber;
    [SerializeField] GameObject escapeMenu;
    public void LoadTheScene()
    {
       SceneManager.LoadScene(sceneNumber);

    }

    private void Update()
    {
        if(escapeMenu != null)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (escapeMenu.activeSelf)
                {
                    escapeMenu.SetActive(false);
                }
                else
                {
                    escapeMenu.SetActive(true);
                }
            }
        }   
    }


}
