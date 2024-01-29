using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorFollow : MonoBehaviour
{
    
    private void Awake()
    {
       
    }
    void Update()
    {
        Cursor.visible = false;
        transform.position = Input.mousePosition;
    }
}
