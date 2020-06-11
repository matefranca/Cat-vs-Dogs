using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorFollow : MonoBehaviour
{
    //Script that control the Cursor image.

    #region Methods;

    //Method called on the beginning of the game.
    void Start()
    {
        //Disable the visibility of the cursor.
        Cursor.visible = false; 
    }

    //Method called each frame.
    void Update()
    {
        //Sets the position of the Cursor image to the position of the mouse Cursor.
        transform.position = Input.mousePosition;
    }

    #endregion;
}
