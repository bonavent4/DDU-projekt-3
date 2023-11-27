using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TheManager : MonoBehaviour
{
    [SerializeField] GameObject HouseToAttack;
    [SerializeField] GameObject[] DifferentShapes;

    [SerializeField] Vector2[] startPoints;
    [SerializeField] float[] timeToSpawn;
    [SerializeField] int[] whichShapeToSpawn;
    [SerializeField] GameObject[] ShapesInAction;

    int currentNumber = 0;

    private void Start()
    {
        for (int i = 0; i < startPoints.Length; i++)
        {
            ShapesInAction[i] = Instantiate(DifferentShapes[whichShapeToSpawn[i]]);
            ShapesInAction[i].transform.position = startPoints[i];
            ShapesInAction[i].GetComponent<ShapesMovement>().Home = HouseToAttack;
            Invoke("SpawnShape", timeToSpawn[i]);
        }
    }
    void SpawnShape()
    {
        ShapesInAction[currentNumber].GetComponent<ShapesMovement>().startMoving();
        currentNumber++;
    }
}
