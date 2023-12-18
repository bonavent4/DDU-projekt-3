using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShapesMovement : MonoBehaviour
{
    public GameObject Home;
    [SerializeField] float speed;
    [SerializeField] float rotateSpeed;

    bool move;
    Animator anim;
    private void Awake()
    {
        anim = gameObject.GetComponent<Animator>();
        Home = FindObjectOfType<TheManager>().gameObject;
    }
    void Update()
    {
        if (move)
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

    public void startMoving()
    {
        move = true;
        Invoke("startanim", Home.GetComponent<TheManager>().lengthFromOriginalShape / speed - 0.16666667f);
        Debug.Log(Home.GetComponent<TheManager>().lengthFromOriginalShape / speed - 0.16666667f);
    }
    void startanim()
    {
        anim.SetTrigger("GoGreen");
    }
}

