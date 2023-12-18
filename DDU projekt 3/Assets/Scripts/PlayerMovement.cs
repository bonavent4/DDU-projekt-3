using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 40f; // ajust the speed
    public float dashSpeed = 50f; // ajust the speed of the dash
    public float dashDistance = 70f;
    public float dashCooldown = 0.5f;
    public int consecutiveDashesToCooldown = 6;
    private float nextDashTime = 0f;
    private float CooldownTimer = 0f;
    private int consecutiveDashes = 0;
    
    private bool canDash = true;
    private Camera MainCamera;
    private bool isDashing = false;
    private Vector3 dashTarget;
    public Animator anim;
    public Animator playerAnim;
    public Image dashCooldownBar;
    private Vector3 originalScale;

    // Start is called before the first frame update
    private void Start()
    {
        originalScale = dashCooldownBar.rectTransform.localScale;
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
            if (Input.GetMouseButtonDown(0) && Time.time >= nextDashTime && canDash)
            {

                Debug.Log("the mouse buttion is being held down.");

                //nextDashTime = Time.time + dashCooldown;

                // Get the mouse position in the world space
                Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                mousePos.z = transform.position.z; // Ensure the z-coordinate remains the same as the player

                // Calculate the direction from the player to the mouse position
                Vector3 dashDirection = (mousePos - transform.position).normalized;

                // Set the target position for dashing by adding dashDistance in the calculated direction
                dashTarget = transform.position + dashDirection * dashDistance;
                isDashing = true;

                anim.SetTrigger("draw");
                playerAnim.SetTrigger("dash");

                consecutiveDashes++;

                if (consecutiveDashes >= consecutiveDashesToCooldown)
                {
                    canDash = false; // Disable futher dashing
                    Debug.Log("can't dash anymore until cooldown");
                    nextDashTime = Time.time + dashCooldown;
                }
            }
        }
        if (!canDash && Time.time >= nextDashTime)
        {
            canDash = true;
            consecutiveDashes = 0;
            Debug.Log("can't dash agian");
        }

        if (Time.time < nextDashTime)
        {
            CooldownTimer = nextDashTime - Time.time;
            float fillAmount = CooldownTimer / dashCooldown;
            float newScaleX = Mathf.Lerp(0f, 1f, fillAmount);
            dashCooldownBar.rectTransform.localScale = new Vector3(newScaleX, originalScale.y, originalScale.z);

        }
        else
        {
            CooldownTimer = 0f;
            dashCooldownBar.rectTransform.localScale = originalScale;
        }

    }
}
