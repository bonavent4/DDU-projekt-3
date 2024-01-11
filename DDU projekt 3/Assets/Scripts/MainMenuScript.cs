using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class MainMenuScript : MonoBehaviour
{
    [SerializeField]int sceneNumber;
    // Start is called before the first frame update
   // [SerializeField] GameObject exitButton;

    // Update is called once per frame
    /*void Update()
    {
       // EnableExitButton();
    }*/
    public void LoadTheScene()
    {
       SceneManager.LoadScene(sceneNumber);

    }
    /*public void PlayEditScene()
    {
        SceneManager.LoadScene(1);

    }
    public void ReturnToMainScene()
    {
        SceneManager.LoadScene(0);

    }/*
   /* public void EnableExitButton()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (exitButton.activeSelf)
            {
                exitButton.SetActive(false);
            }
            else
            {
                exitButton.SetActive(true);
            }
        }
        

    }*/


}
