using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class MainMenuScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void PlaySongScene()
    {
       SceneManager.LoadScene(2);

    }
    public void PlayEditScene()
    {
        SceneManager.LoadScene(1);

    }

}
