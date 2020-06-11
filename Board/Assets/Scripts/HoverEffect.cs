using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoverEffect : MonoBehaviour
{
    //Script that handles the effect when the User hover the Tiles or Units with the mouse.

    #region Variables;

    //Value that will be used to make interaction when hovering the collider.
    [SerializeField] float hoverAmount;

    #endregion;

    #region Methods;

    //Method called when the mouse Enter the collider.
    private void OnMouseEnter()
    {
        transform.localScale += Vector3.one * hoverAmount;
    }

    //Method called when the mouse Exit the collider.
    private void OnMouseExit()
    {
        transform.localScale -= Vector3.one * hoverAmount;
    }

    #endregion;
}
