using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 40f; // ajust the speed
    public float dashSpeed = 50f; // ajust the speed of the dash
    public float dashDistance = 70f;
    private Camera MainCamera;
    private bool isDashing = false;
    private Vector3 dashTarget;
    public Animator anim;

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
        //transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
        Vector3 moveDirection = targetPosition - transform.position;

        // Rotate the player to face the mouse direction
        if (moveDirection != Vector3.zero)
        {
            float angle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
        if (isDashing)
        {
            transform.position = Vector3.MoveTowards(transform.position, dashTarget, dashSpeed * Time.deltaTime);
            if (transform.position == dashTarget)
                isDashing = false;
        }
        else
        {
            // Move the player towards the mouse position
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            //dash/attack
            if (Input.GetMouseButtonDown(0))
            {
                Debug.Log("the mouse buttion is being held down.");

                // Get the mouse position in the world space
                Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                mousePos.z = transform.position.z; // Ensure the z-coordinate remains the same as the player

                // Calculate the direction from the player to the mouse position
                Vector3 dashDirection = (mousePos - transform.position).normalized;

                // Set the target position for dashing by adding dashDistance in the calculated direction
                dashTarget = transform.position + dashDirection * dashDistance;
                isDashing = true;

                anim.SetTrigger("draw");
            }
        }

    }
}
