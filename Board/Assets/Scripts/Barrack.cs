using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Barrack : MonoBehaviour
{
    //Script that controls the Buy Item Menu.

    #region Variables;

    //Buttons that will turn the menu active or not.
    [SerializeField] Button player1ToggleButton;
    [SerializeField] Button player2ToggleButton;

    //The menu objects that will be activated and deactivated.
    [SerializeField] GameObject player1Menu;
    [SerializeField] GameObject player2Menu;

    //Reference to the Game Master.
    GameMaster gm;

    #endregion;

    #region Unity Methods;

    //Method called on the beginning of the game.
    private void Start()
    {
        gm = GetComponent<GameMaster>();
    }

    //Method called each frame.
    private void Update()
    {
        ChangeInteractableTurn();        
    }

    #endregion;

    #region Other Methods;

    //Method that changes the activation of the menu.
    public void ToggleMenu(GameObject menu)
    {
        menu.SetActive(!menu.activeSelf);
    }

    //Method that closes both menus.
    public void CloseMenus()
    {
        player1Menu.SetActive(false);
        player2Menu.SetActive(false);
    }

    //Method to make the buying system work.
    public void BuyItem(BarrackItem item)
    {
        //Check if the player has the amount of gold needed to buy the Unit.
        if(gm.playerTurn == 1 && item.cost <= gm.player1Gold)
        {
            gm.player1Gold -= item.cost;
            player1Menu.SetActive(false);
        }
        else if (gm.playerTurn == 2 && item.cost <= gm.player2Gold)
        {
            gm.player2Gold -= item.cost;
            player2Menu.SetActive(false);
        }
        else
        {
            print("Not enough Gold!");
            return;
        }

        gm.UpdateGoldText();

        gm.purchasedItem = item;

        if(gm.selectedUnit != null)
        {
            gm.selectedUnit.selected = false;
            gm.selectedUnit = null;
        }

        GetCreatableTiles();
    }

    //Method that highlights the Tiles that the Unit can be created.
    void GetCreatableTiles()
    {
        foreach (Tile tile in FindObjectsOfType<Tile>())
        {
            if(tile.IsClear())
            {
                tile.SetCreatable();
            }
        }
    }

    //Method that change the button that can be used to open the menu depending on the Player turn.
    void ChangeInteractableTurn ()
    {
        if (gm.playerTurn == 1)
        {
            player1ToggleButton.interactable = true;
            player2ToggleButton.interactable = false;
        }
        else
        {
            player1ToggleButton.interactable = false;
            player2ToggleButton.interactable = true;
        }
    }

    #endregion;
}
