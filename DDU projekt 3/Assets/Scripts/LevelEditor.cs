using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using NAudio;


public class LevelEditor : MonoBehaviour
{
    [SerializeField] TMP_Dropdown saveFilesDropDown;
    [SerializeField] TMP_Dropdown mp3DropDown;
    [SerializeField] TMP_InputField newFileNameInput;

    [SerializeField] List<string> listOfFiles = new List<string>();
    [SerializeField] List<string> listOfMp3 = new List<string>();

    string currentFile;

    TheManager manager;

    [SerializeField] GameObject startMenu;
    [SerializeField] GameObject leveleditorMenu;
    [SerializeField] GameObject createNewMenu;

    [SerializeField] AudioSource music;
    [SerializeField] float audioTimer;
    [SerializeField] Scrollbar scrollBar;
    [SerializeField] Scrollbar SpeedScrollBar;

    [SerializeField] bool canedit;

    [SerializeField] GameObject[] DifferentShapes;

    [SerializeField] List<GameObject> testShapes = new List<GameObject>();

    [SerializeField] float timeToFadeIn;
    bool moveShape;
   // bool rotateShape;
    [SerializeField] GameObject shapeToMove;
    GameObject shapeToRotate;

    bool haveSaved;


    // place little thing
    [SerializeField] List<GameObject> littleThings = new List<GameObject>();

    [SerializeField] float XMax;
    [SerializeField] GameObject littleThingPrefab;
    [SerializeField] GameObject littleThingParent;

    [SerializeField] GameObject[] shapeSprites;

    SaveObject saveObject = new SaveObject
    {
        /* StartPoints = startPoints,
         TimeToSpawn = timeToSpawn,
         WhichShapeToSpawn = whichShapeToSpawn*/
    };
    private void Awake()
    {
        manager = FindObjectOfType<TheManager>();

        if (!Directory.Exists(Application.streamingAssetsPath + "/SongsData"))
        {
            Directory.CreateDirectory(Application.streamingAssetsPath + "/SongsData");
        }
        listOfFiles.Add("Create New");
        var fileinfo = new DirectoryInfo(Application.streamingAssetsPath + "/SongsData").GetFiles("*.txt");
        foreach (FileInfo f in fileinfo)
        {
            listOfFiles.Add(f.Name);
        }
        saveFilesDropDown.AddOptions(listOfFiles);
        var mp3info = new DirectoryInfo(Application.streamingAssetsPath + "/SongsData").GetFiles("*.mp3");
        foreach (FileInfo f in mp3info)
        {
            listOfMp3.Add(f.Name);
        }
        mp3DropDown.AddOptions(listOfMp3);
    }
    private void Update()
    {
        
        if (music.isPlaying)
        {
            audioTimer += Time.deltaTime * music.pitch;
            scrollBar.value = 1 / music.clip.length * audioTimer;
            
        }
        
        createShapes();

        
    }
    public void takeCareOfShapes()
    {
        if (!music.isPlaying)
        {
            audioTimer = scrollBar.value * music.clip.length;
        }
        if (canedit)
        {
            for (int i = 0; i < saveObject.TimeToSpawn.Count; i++)
            {
                if (scrollBar.value * music.clip.length > saveObject.TimeToSpawn[i] && scrollBar.value * music.clip.length < saveObject.TimeToSpawn[i] + timeToFadeIn)
                {
                    testShapes[i].SetActive(true);
                    SpriteRenderer sprite = testShapes[i].GetComponent<SpriteRenderer>();

                    sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, 1 - ((scrollBar.value * music.clip.length) - saveObject.TimeToSpawn[i]) / timeToFadeIn);

                }
                else
                {
                    testShapes[i].SetActive(false);
                    //testShapes[i].GetComponent<SpriteRenderer>().enabled = false;
                }
            }
        }
        
    }
    public void ChangeMusicSpeed()
    {
        if (canedit)
        {
            music.pitch = SpeedScrollBar.value;
        }
    }
    void createShapes()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            stopShapeMovement();
        }
        
        if (Input.GetMouseButtonDown(0) && canedit && !EventSystem.current.IsPointerOverGameObject())
        {
            
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if(hit.collider != null && hit.collider.gameObject.CompareTag("XButton"))
            {

                stopShapeMovement();
                int number = testShapes.IndexOf(hit.collider.gameObject.transform.parent.gameObject.transform.parent.gameObject);
                saveObject.StartPoints.Remove(saveObject.StartPoints[number]);
                saveObject.TimeToSpawn.Remove(saveObject.TimeToSpawn[number]);
                saveObject.Rotation.Remove(saveObject.Rotation[number]);
                saveObject.WhichShapeToSpawn.Remove(saveObject.WhichShapeToSpawn[number]);

                Destroy(testShapes[number]);
                testShapes.Remove(testShapes[number]);

                Destroy(littleThings[number]);
                littleThings.Remove(littleThings[number]);
                
            }
            else if(hit.collider != null && hit.collider.gameObject.CompareTag("triangle"))
            {
                int number = testShapes.IndexOf(hit.collider.gameObject.transform.parent.gameObject.transform.parent.gameObject.transform.parent.gameObject);
                hit.collider.gameObject.transform.parent.gameObject.transform.parent.gameObject.transform.parent.gameObject.GetComponent<SpriteRenderer>().sprite = shapeSprites[0].GetComponent<SpriteRenderer>().sprite;
                saveObject.WhichShapeToSpawn[number] = 1;
            }
            else if (hit.collider != null && hit.collider.gameObject.CompareTag("sixThingy"))
            {
                int number = testShapes.IndexOf(hit.collider.gameObject.transform.parent.gameObject.transform.parent.gameObject.transform.parent.gameObject);
                hit.collider.gameObject.transform.parent.gameObject.transform.parent.gameObject.transform.parent.gameObject.GetComponent<SpriteRenderer>().sprite = shapeSprites[1].GetComponent<SpriteRenderer>().sprite;
                saveObject.WhichShapeToSpawn[number] = 2;
            }
            else if (hit.collider != null && hit.collider.GetComponent<EditShape>())
            {
                if (moveShape && hit.collider != shapeToMove)
                {
                    shapeToMove.GetComponent<EditShape>().editorMenu.SetActive(false);
                }
                moveShape = true;
                shapeToMove = hit.collider.gameObject;
                shapeToMove.GetComponent<EditShape>().editorMenu.SetActive(true);
            }
            else
            {
                stopShapeMovement();


                GameObject shape = Instantiate(DifferentShapes[0], new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y), Quaternion.identity);

                if (saveObject.TimeToSpawn.Count == 0 || saveObject.TimeToSpawn[saveObject.TimeToSpawn.Count - 1] <= audioTimer)
                {
                    saveObject.TimeToSpawn.Add(audioTimer);
                    saveObject.StartPoints.Add(shape.transform.position);
                    saveObject.Rotation.Add(0);
                    saveObject.WhichShapeToSpawn.Add(0);
                    testShapes.Add(shape);
                    PlaceLittleThing(saveObject.TimeToSpawn[saveObject.TimeToSpawn.Count - 1], saveObject.TimeToSpawn.Count - 1);
                }
                else
                {
                    for (int i = 0; i < saveObject.TimeToSpawn.Count; i++)
                    {
                        if (saveObject.TimeToSpawn[i] > audioTimer)
                        {
                            saveObject.TimeToSpawn.Insert(i, audioTimer);
                            saveObject.StartPoints.Insert(i, shape.transform.position);
                            saveObject.Rotation.Insert(i, 0);
                            saveObject.WhichShapeToSpawn.Insert(i, 0);
                            testShapes.Insert(i, shape);
                            PlaceLittleThing(saveObject.TimeToSpawn[i], i);
                            break;
                        }
                    }
                }


                
            }

            save();
            Debug.Log("saved");

        }
        if (Input.GetMouseButtonDown(1) && canedit && !EventSystem.current.IsPointerOverGameObject() && moveShape)
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if(hit.collider == null || !hit.collider.gameObject.GetComponent<EditShape>())
            {
                stopShapeMovement();
            }
        }
        if(Input.GetMouseButton(1) && moveShape)
        {
           // Debug.Log(Camera.main.ScreenToWorldPoint(Input.mousePosition));

            shapeToMove.transform.up = new Vector3(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y, 0) - new Vector3(shapeToMove.transform.position.x, shapeToMove.transform.position.y, 0);
            haveSaved = true;
        }
        else if (Input.GetMouseButton(0) && moveShape)
        {
            shapeToMove.transform.position = new Vector3(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y, shapeToMove.transform.position.z);
            haveSaved = true;
        }
        else if (moveShape && haveSaved && saveObject.StartPoints.Count != 0)
        {
            for (int i = 0; i < saveObject.StartPoints.Count; i++)
            {
                saveObject.StartPoints[i] = testShapes[i].transform.position;
                saveObject.Rotation[i] = testShapes[i].transform.rotation.eulerAngles.z;
                Debug.Log(testShapes[i].transform.rotation.eulerAngles.z);
            }
            haveSaved = false;
            save();
        }
       
    }
    void stopShapeMovement()
    {
        if (moveShape)
        {
            shapeToMove.GetComponent<EditShape>().editorMenu.SetActive(false);
            shapeToMove = null;
            moveShape = false;
            shapeToRotate = null;
        }
    }
    void PlaceLittleThing(float time, int index)
    {
        float X = 0;
        if(time > music.clip.length / 2)
        {
            X = XMax / (music.clip.length / 2) * (time - music.clip.length/2);
           
        }
        else if(time < music.clip.length /2)
        {
            X = -XMax + (XMax / (music.clip.length / 2) * time);
        }
       // Debug.Log(X + ", " + music.clip.length + ", " + time + ", ");
        GameObject thing = Instantiate(littleThingPrefab, gameObject.transform);
        thing.transform.localPosition = new Vector3(X, -456, 0);
        thing.transform.parent = littleThingParent.transform;
        if (index == saveObject.TimeToSpawn.Count)
        {
            littleThings.Add(thing);
        }
        else
        {
            littleThings.Insert(index, thing);
        }
    }
    public void startEditing()
    {
        if(saveFilesDropDown.value == 0)
        {
            createNewMenu.SetActive(true);
        }
        else
        {
            saveObject = JsonUtility.FromJson<SaveObject>(File.ReadAllText(Application.streamingAssetsPath + "/SongsData/" + listOfFiles[saveFilesDropDown.value]));
            //Debug.Log(saveObject.StartPoints[0]);
            currentFile = listOfFiles[saveFilesDropDown.value];
            

            
            leveleditorMenu.SetActive(true);

            WWW www = new WWW(Application.streamingAssetsPath + "/SongsData/" + saveObject.mp3File);
            music.clip = NAudioPlayer.FromMp3Data(www.bytes);
            music.Play();

            canedit = true;

            for (int i = 0; i < saveObject.TimeToSpawn.Count; i++)
            {
                testShapes.Add(Instantiate(DifferentShapes[0], saveObject.StartPoints[i], Quaternion.Euler(0,0,saveObject.Rotation[i])));
                if(saveObject.WhichShapeToSpawn[i] > 0)
                {
                    testShapes[i].GetComponent<SpriteRenderer>().sprite = shapeSprites[saveObject.WhichShapeToSpawn[i] - 1].GetComponent<SpriteRenderer>().sprite;
                }
                
                PlaceLittleThing(saveObject.TimeToSpawn[i], i);
            }
        }
        startMenu.SetActive(false);
    }
    public void CreateNewFile()
    {
        saveObject.mp3File = listOfMp3[mp3DropDown.value];
        File.WriteAllText(Application.streamingAssetsPath + "/SongsData/" + newFileNameInput.text + ".txt", JsonUtility.ToJson(saveObject));
        // Debug.Log(JsonUtility.ToJson(saveObject));
        currentFile = newFileNameInput.text + ".txt";

        

        createNewMenu.SetActive(false);
        leveleditorMenu.SetActive(true);
        WWW www = new WWW(Application.streamingAssetsPath + "/SongsData/" + saveObject.mp3File);
        music.clip = NAudioPlayer.FromMp3Data(www.bytes);
        music.Play();

        canedit = true;
        if(saveObject.TimeToSpawn.Count != 0)
        {
            for (int i = 0; i < saveObject.TimeToSpawn.Count; i++)
            {
                testShapes.Add(Instantiate(DifferentShapes[saveObject.WhichShapeToSpawn[i]], saveObject.StartPoints[i], Quaternion.identity));
            }
        }
        
        

    }
    void save()
    {
        File.WriteAllText(Application.streamingAssetsPath + "/SongsData/" + currentFile, JsonUtility.ToJson(saveObject));
    }
    public class SaveObject
    {
        public string mp3File = "";
        public List<Vector2> StartPoints = new List<Vector2>();
        public List<float> Rotation = new List<float>();
        public List<float> TimeToSpawn = new List<float>();
        public List<int> WhichShapeToSpawn = new List<int>();

    }

    
    public void PauseButton()
    {
        if (music.isPlaying)
        {
            music.Stop();
        }
        else if(scrollBar.value != 1)
        {
            audioTimer = scrollBar.value * music.clip.length;
            music.time = audioTimer;
            music.Play();
        }
    }

    public void DeleteSaveFile()
    {
        if(saveFilesDropDown.value != 0)
        {
            
            File.Delete(Application.streamingAssetsPath + "/SongsData/" + listOfFiles[saveFilesDropDown.value]);
            listOfFiles.Remove(listOfFiles[saveFilesDropDown.value]);
            saveFilesDropDown.ClearOptions();
            saveFilesDropDown.AddOptions(listOfFiles);
            Debug.Log(saveFilesDropDown.value);
        }
        
    }
    
    
}
