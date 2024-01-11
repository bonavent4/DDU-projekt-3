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
    public int consecutiveDashesToCooldown;
    private float nextDashTime = 0f;
    private float CooldownTimer = 0f;
    private int consecutiveDashes = 0;
    
    private bool canDash = true;
    private Camera MainCamera;
    private bool isDashing = false;
    private Vector3 dashTarget;
    public Animator anim;
    public Animator playerAnim;
    public Image[] dashCooldownBar;
    [SerializeField] Vector3[] originalScale;

    RaycastHit2D[] hit;

    TheManager manager;

    [SerializeField]int dashesLeft;

    public bool canMove = true;

    [SerializeField] AudioSource swordSway;
    [SerializeField] AudioSource swordCling;

    // Start is called before the first frame update

    [SerializeField] float distanceFromMouse;
    private void Start()
    {
        dashesLeft = consecutiveDashesToCooldown;
        for (int i = 0; i < dashCooldownBar.Length; i++)
        {
            originalScale[i] = dashCooldownBar[i].rectTransform.localScale;
        }
        MainCamera = Camera.main;
        manager = FindObjectOfType<TheManager>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (canMove)
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
                if(Vector3.Distance(transform.position, targetPosition) > distanceFromMouse)
                {
                    transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
                }
                
                //dash/attack
                if (Input.GetMouseButtonDown(0) && dashesLeft != 0)
                {

                    // Debug.Log("the mouse buttion is being held down.");

                    //nextDashTime = Time.time + dashCooldown;

                    // Get the mouse position in the world space
                    Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    mousePos.z = transform.position.z; // Ensure the z-coordinate remains the same as the player

                    // Calculate the direction from the player to the mouse position
                    // Vector3 dashDirection = (mousePos - transform.position).normalized;
                    //ændrede den til transform.right så den er mere end 0 når spilleren er ved musens position
                    Vector3 dashDirection = transform.right;

                    // Set the target position for dashing by adding dashDistance in the calculated direction
                    dashTarget = transform.position + dashDirection * dashDistance;
                    isDashing = true;

                    anim.SetTrigger("draw");
                    playerAnim.SetTrigger("dash");

                    // consecutiveDashes++;
                    if (dashesLeft != consecutiveDashesToCooldown)
                    {
                        dashCooldownBar[dashesLeft].rectTransform.localScale = new Vector3(0, originalScale[dashesLeft].y, originalScale[dashesLeft].z);
                    }
                    dashesLeft -= 1;
                    dashCooldownBar[dashesLeft].rectTransform.localScale = new Vector3(0, originalScale[dashesLeft].y, originalScale[dashesLeft].z);

                    swordSway.Play();
                    // check if you hit a shape and where it got hit.
                    hit = Physics2D.LinecastAll(gameObject.transform.position, dashTarget);
                    if (hit.Length != 0)
                    {
                        foreach (RaycastHit2D h in hit)
                        {
                            if (h.collider.gameObject.GetComponent<ShapesMovement>())
                            {
                                Debug.Log(Quaternion.FromToRotation(Vector3.up, h.normal).eulerAngles.z + " : " + h.collider.gameObject.transform.rotation.eulerAngles.z);
                                if (Quaternion.FromToRotation(Vector3.up, h.normal).eulerAngles.z >= h.collider.gameObject.transform.rotation.eulerAngles.z-1 && Quaternion.FromToRotation(Vector3.up, h.normal).eulerAngles.z <= h.collider.gameObject.transform.rotation.eulerAngles.z +1)
                                {
                                    h.collider.gameObject.GetComponent<ShapesMovement>().SplitBoxInHalf();
                                    //Debug.Log(hit.normal);
                                    
                                    
                                    if (h.collider.gameObject.GetComponent<ShapesMovement>().isGreen)
                                    {
                                        swordCling.Play();
                                        manager.GainPoints(200);
                                    }
                                    else
                                        manager.GainPoints(100);
                                }
                            }
                        }

                        // Debug.Log(Quaternion.FromToRotation(Vector3.up, hit.normal).eulerAngles + "  ::  " + new Vector3(0, 0, hit.collider.gameObject.transform.rotation.z));

                    }

                    /* if (consecutiveDashes >= consecutiveDashesToCooldown)
                     {
                         canDash = false; // Disable futher dashing
                        // Debug.Log("can't dash anymore until cooldown");
                         nextDashTime = Time.time + dashCooldown;
                     }*/
                    if (dashesLeft < consecutiveDashesToCooldown && Time.time >= nextDashTime)
                    {
                        nextDashTime = Time.time + dashCooldown;
                        // canDash = true;
                        // consecutiveDashes = 0;
                        //  Debug.Log("can't dash agian");
                    }
                }
            }


            if (Time.time < nextDashTime && dashesLeft != consecutiveDashesToCooldown)
            {
                CooldownTimer = Time.time - nextDashTime;
                float fillAmount = CooldownTimer / dashCooldown;
                float newScaleX = Mathf.Lerp(0f, 1f, fillAmount);
                dashCooldownBar[dashesLeft].rectTransform.localScale = new Vector3(newScaleX, originalScale[dashesLeft].y, originalScale[dashesLeft].z);

            }
            else if (dashesLeft != consecutiveDashesToCooldown)
            {
                dashCooldownBar[dashesLeft].rectTransform.localScale = new Vector3(originalScale[dashesLeft].x, originalScale[dashesLeft].y, originalScale[dashesLeft].z);
                CooldownTimer = 0f;
                dashesLeft++;
                nextDashTime = Time.time + dashCooldown;
                // dashCooldownBar.rectTransform.localScale = originalScale;
            }

        }
    }
       
}
