using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShapesMovement : MonoBehaviour
{
    [SerializeField] GameObject Home;
    [SerializeField] float speed;
    [SerializeField] float rotateSpeed;
    void Update()
    {
        if (gameObject.transform.position != Home.transform.position)
        {
            gameObject.transform.position = Vector3.MoveTowards(gameObject.transform.position, Home.transform.position, speed * Time.deltaTime);
            transform.Rotate(Vector3.forward * rotateSpeed * Time.deltaTime);
        }
        else
        {
            Debug.Log("hit home");
        }

    }
}

