using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxPieces : MonoBehaviour
{
    public bool boxDestroyed;
    float speed = 20;
    float A = 1;

    void Update()
    {
        if (boxDestroyed)
        {
            dissapear();
        }
        
    }
    void dissapear()
    {
        if(A <= 0)
        {
            Destroy(gameObject);
        }
        A -= 0.5f * Time.deltaTime;
        Color C = gameObject.GetComponent<SpriteRenderer>().color;
        gameObject.GetComponent<SpriteRenderer>().color = new Color(C.r, C.g, C.b, A);
        gameObject.transform.localScale = new Vector3(gameObject.transform.localScale.x - (gameObject.transform.localScale.x/100 * speed * Time.deltaTime), gameObject.transform.localScale.y - (gameObject.transform.localScale.y / 100 * speed * Time.deltaTime), gameObject.transform.localScale.z - (gameObject.transform.localScale.z / 100 * speed * Time.deltaTime));
    }
}
