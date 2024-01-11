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

    [SerializeField] GameObject[] boxParts;
    [SerializeField] float force;

    public bool isGreen;

    [SerializeField] GameObject SlashTrail;
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
                //  Debug.Log("hit home");
                Home.GetComponent<TheManager>().LoseHealth();
                Destroy(gameObject);
            }
        }
    }

    public void startMoving()
    {
        move = true;
        Invoke("startanim", Home.GetComponent<TheManager>().lengthFromOriginalShape / speed - 0.16666667f);
       // Debug.Log(Home.GetComponent<TheManager>().lengthFromOriginalShape / speed - 0.16666667f); 
    }
    void startanim()
    {
        anim.SetTrigger("GoGreen");
    }
    public void SplitBoxInHalf()
    {
        SlashTrail.GetComponent<Animator>().SetTrigger("Slash");
        SlashTrail.transform.parent = null;
        Invoke("destroyTrail", 3);
        foreach (GameObject g in boxParts)
        {
            g.SetActive(true);
            g.transform.parent = null;
            Vector3 direction = (g.transform.position - transform.position).normalized;
            g.GetComponent<Rigidbody2D>().AddForce(direction * force);
            g.GetComponent<BoxPieces>().boxDestroyed = true;
        }
        Destroy(gameObject);
    }
    void destroyTrail()
    {

    }

    void GoGreen()
    {
        isGreen = true;
    }
    void GoBack()
    {
        isGreen = false;
    }
}

