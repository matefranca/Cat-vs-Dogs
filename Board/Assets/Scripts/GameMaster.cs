using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameMaster : MonoBehaviour
{
    //Script that Manages the game.

    #region Variables;

    //Variables that holds the Unit that is current selected in the Game.
    [HideInInspector] public Unit selectedUnit;

    //Holds which team`s turn it is.
    [HideInInspector] public int playerTurn = 1;  

    //References the object that marks the selected unit visually.
    [SerializeField] GameObject selectedUnitSquare;

    //Changes the image that shows the player turn.
    [SerializeField] Image playerIndicator;
    [SerializeField] Sprite player1Indicator;
    [SerializeField] Sprite player2Indicator;

    //Variable with the gold from each team.
    public int player1Gold = 100;
    public int player2Gold = 100;

    //Text that shows the ammount of gold to the Players.
    [SerializeField] TextMeshProUGUI player1GoldText;
    [SerializeField] TextMeshProUGUI player2GoldText;

    //Variable with the item the Player wants to buy.
    [HideInInspector] public BarrackItem purchasedItem; 

    //Panel that show the stats of the selected unit.
    [SerializeField] GameObject statsPanel;
    //A value that shifts the Stats Panel from the center of the Unit.
    [SerializeField] Vector2 statsPanelShift;
    
    //Current viewed Unit.
    Unit viewedUnit;

    //UI Elements.
    [SerializeField] Text healthText;
    [SerializeField] Text armorText;
    [SerializeField] Text attackDamageText;
    [SerializeField] Text defenseDamageText;

    #endregion;

    #region Unity Methods;

    //Method called on the beginning of the game.
    private void Start()
    {
        GetGoldIncome(1);
    }
    
    //Method called each frame.
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            EndTurn();
        }

        if(selectedUnit != null)
        {
            selectedUnitSquare.SetActive(true);
            selectedUnitSquare.transform.position = selectedUnit.transform.position;
        }
        else
        {
            selectedUnitSquare.SetActive(false);
        }
    }

    #endregion;

    #region Other Methods;

    //Method that turns on or off the stats panel.
    public void ToggleStatsPanel (Unit unit)
    {
        if (unit.Equals(viewedUnit) == false)
        {
            statsPanel.SetActive(true);
            statsPanel.transform.position = (Vector2)unit.transform.position + statsPanelShift;
            viewedUnit = unit;
            UpdateStatsPanel();
        }
        else
        {
            statsPanel.SetActive(false);
            viewedUnit = null;
        }
    }

    //Method that update the values in the UI of the Panel.
    public void UpdateStatsPanel()
    {
        if(viewedUnit != null)
        {
            healthText.text = viewedUnit.health.ToString();
            armorText.text = viewedUnit.armor.ToString();
            attackDamageText.text = viewedUnit.attackDamage.ToString();
            defenseDamageText.text = viewedUnit.defenseDamage.ToString();
        }
    }

    //Method that move the Panel to the Unit selected.
    public void MoveStatsPanel(Unit unit)
    {
        if(unit.Equals(viewedUnit))
        {
            statsPanel.transform.position = (Vector2)unit.transform.position + statsPanelShift;
        }
    }

    //Method that Remove the Panel.
    public void RemoveStatsPanel(Unit unit)
    {
        if(unit.Equals(viewedUnit))
        {
            statsPanel.SetActive(false);
            viewedUnit = null;
        }
    }

    //Method that Update the UI text for the gold of each player.
    public void UpdateGoldText()
    {
        player1GoldText.text = player1Gold.ToString();
        player2GoldText.text = player2Gold.ToString();
    }

    //Method that updates the gold of the players.
    public void GetGoldIncome(int playerTurn)
    {
        foreach (Village village in FindObjectsOfType<Village>())
        {
            if(village.playerNumber == playerTurn)
            {
                if(playerTurn == 1)
                {
                    player1Gold += village.goldPerTurn;
                }
                else
                {
                    player2Gold += village.goldPerTurn;
                }
            }
        }

        UpdateGoldText();
    }

    //Method that reset all the Tiles.
    public void ResetTiles()
    {
        foreach(Tile tile in FindObjectsOfType<Tile>())
        {
            tile.Reset();
        }
    }


    //Method to change the turn of each player.
    public void EndTurn()
    {
        if (playerTurn == 1)
        {
            playerTurn = 2;
            playerIndicator.sprite = player2Indicator;
        }

        else if (playerTurn == 2)
        {
            playerTurn = 1;
            playerIndicator.sprite = player1Indicator;
        }

        GetGoldIncome(playerTurn);

        if(selectedUnit != null)
        {
            selectedUnit.selected = false;
            selectedUnit = null;
        }

        ResetTiles();

        foreach (Unit unit in FindObjectsOfType<Unit>())
        {
            unit.hasMoved = false;
            unit.weaponIcon.SetActive(false);
            unit.hasAttacked = false;
        }

        GetComponent<Barrack>().CloseMenus();
    }

    #endregion;
}
