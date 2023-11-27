using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f; // ajustthe speed
    private Camera MainCamera;

    // Start is called before the first frame update
    private void Start()
    {
        MainCamera = Camera.main;
    }

    // Update is called once per frame
    private void Update()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = transform.position.z - MainCamera.transform.position.z;
        Vector3 targetPosition = MainCamera.ScreenToWorldPoint(mousePosition);
        targetPosition.z = transform.position.z;

        // Move the player towards the mouse position
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
    }
}
