using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    //Script that controls the Tiles in the Board of the game.

    #region Variables;

    //Diferent images to change in the beginning of the game.
    SpriteRenderer rend;
    [SerializeField] Sprite[] tileGraphics;

    //Layer to diference what is obstacle.
    [SerializeField] LayerMask obstacleLayer;

    //Color when the Tile is highlighted
    [SerializeField] Color highlightedColor;

    //Bool to check if the Tile is Walkable.
    [SerializeField] bool isWalkable;

    //Reference to the Game Manager.
    GameMaster gm;

    //Color when the Tile can be created on.
    [SerializeField] Color creatableColor;

    //Bool to check if The Tile can be created on.
    [SerializeField] bool isCreatable;

    #endregion;

    #region Unity Methods;

    //Method called on the beginning of the game.
    void Start()
    {
        //Getting the Sprite Render and The Game Master object.
        rend = GetComponent<SpriteRenderer>();
        gm = FindObjectOfType<GameMaster>();

        //Setting the image to one of the Tile images we have randomly.
        int randTile = Random.Range(0, tileGraphics.Length);
        rend.sprite = tileGraphics[randTile];
    }   

    //Method called when the User Clicks with the left mouse button on the Tile.
    private void OnMouseDown()
    {
        //Moves the selected unit to this Tile.
        if(isWalkable && gm.selectedUnit != null)
        {
            gm.selectedUnit.Move(this.transform.position);
        }
        
        //Create a unit in this Tile.
        else if(isCreatable == true)
        {
            BarrackItem item = Instantiate(gm.purchasedItem, new Vector2(transform.position.x, transform.position.y), Quaternion.identity);
            gm.ResetTiles();
            Unit unit = item.GetComponent<Unit>();
            if(unit != null)
            {
                unit.hasMoved = true;
                unit.hasAttacked = true;
            }
        }
    }

    #endregion;

    #region Other Methods;

    //Method to check if the Tile has something on top of it.
    //Will return true if is empty and false if it is not.
    public bool IsClear()
    {
        Collider2D obstacle = Physics2D.OverlapCircle(transform.position, 0.2f, obstacleLayer);
        if (obstacle != null)
            return false;
        else 
            return true;
    }

    //Method to highlight the Tile.
    public void Highlight()
    {
        rend.color = highlightedColor;
        isWalkable = true;
    }

    //Method to Reset the Tile to its initial color and its booleans to false.
    public void Reset()
    {
        rend.color = Color.white;
        isWalkable = false;
        isCreatable = false;
    }

    //Method to Set the Tile as Creatable and change its color.
    public void SetCreatable()
    {
        rend.color = creatableColor;
        isCreatable = true;
    }

    #endregion;
}
