using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using TMPro;

public class TheManager : MonoBehaviour
{
    [SerializeField] TMP_Dropdown saveFilesDropDown;
    [SerializeField] List<string> listOfFiles = new List<string>();
    [SerializeField] AudioSource music;

    [SerializeField] GameObject[] DifferentShapes;

    [SerializeField] GameObject StartMenu;
    [SerializeField] GameObject deadMenu;
    [SerializeField] GameObject everythingElse;

    [SerializeField] GameObject[] ShapesInAction;

    public float lengthFromOriginalShape;

    int currentNumber = 0;

     SaveObject saveObject;

    [SerializeField] GameObject[] livesImages;
    [SerializeField] TextMeshProUGUI pointText;
    int health = 5;
    int points;

    PlayerMovement playerMovement;

    bool haveStartedMusic;
    [SerializeField] TextMeshProUGUI endPointsText;

    private void Awake()
    {
        playerMovement = FindObjectOfType<PlayerMovement>();
        var fileinfo = new DirectoryInfo(Application.streamingAssetsPath + "/SongsData").GetFiles("*.txt");
        foreach (FileInfo f in fileinfo)
        {
            listOfFiles.Add(f.Name);
        }
        saveFilesDropDown.AddOptions(listOfFiles);
    }
    private void Update()
    {
       if(!music.isPlaying && health != 0 && haveStartedMusic)
        {
            deadMenu.SetActive(true);
            everythingElse.SetActive(false);
            playerMovement.canMove = false;
            endPointsText.text = "Points: " + points.ToString();
        }
    }
    void SpawnShape()
    {
        ShapesInAction[currentNumber].GetComponent<ShapesMovement>().startMoving();
        currentNumber++;
    }
    public void startGame()
    {
        StartMenu.SetActive(false);

        saveObject = JsonUtility.FromJson<SaveObject>(File.ReadAllText(Application.streamingAssetsPath + "/SongsData/" + listOfFiles[saveFilesDropDown.value]));
        ShapesInAction = new GameObject[saveObject.StartPoints.Count];
        for (int i = 0; i < saveObject.TimeToSpawn.Count; i++)
        {
            float length = Mathf.Sqrt((saveObject.StartPoints[i].x * saveObject.StartPoints[i].x) + (saveObject.StartPoints[i].y * saveObject.StartPoints[i].y));
            float percentage = lengthFromOriginalShape / length;
           // Debug.Log(percentage);

            Vector2 offset = new Vector2(saveObject.StartPoints[i].x * percentage, saveObject.StartPoints[i].y * percentage);

            GameObject g = Instantiate(DifferentShapes[saveObject.WhichShapeToSpawn[i]], saveObject.StartPoints[i] + offset, Quaternion.Euler(0, 0, saveObject.Rotation[i]));

            ShapesInAction[i] = g;

            Invoke("SpawnShape", saveObject.TimeToSpawn[i] - 0.1666667f);
        }
        WWW www = new WWW(Application.streamingAssetsPath + "/SongsData/" + saveObject.mp3File);
        music.clip = NAudioPlayer.FromMp3Data(www.bytes);
        
        Invoke("StartMusic", 5.5f);

    }

    void StartMusic()
    {

        
        music.Play();
        haveStartedMusic = true;
    }
    public class SaveObject
    {
        public string mp3File = "";
        public List<Vector2> StartPoints = new List<Vector2>();
        public List<float> Rotation = new List<float>();
        public List<float> TimeToSpawn = new List<float>();
        public List<int> WhichShapeToSpawn = new List<int>();

    }

    public void LoseHealth()
    {
        
        health -= 1;
        if (health == 0)
        {
            Debug.Log("Dead");
            deadMenu.SetActive(true);
            everythingElse.SetActive(false);
            playerMovement.canMove = false;

            endPointsText.text = "Points: " + points.ToString();

            for (int i = 0; i < ShapesInAction.Length; i++)
            {
                if(ShapesInAction[i] != null)
                {
                    ShapesInAction[i].GetComponent<ShapesMovement>().enabled = false;
                }
                
            }
            music.Stop();
        }
          

        if(health >= 0)
         Destroy(livesImages[health]);
    }
    public void GainPoints(int pointsToGain)
    {
        points += pointsToGain;
        pointText.text = "Points: " + points.ToString();
    }
}
