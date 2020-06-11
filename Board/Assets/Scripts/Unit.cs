using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Unit : MonoBehaviour
{
    //Script that controls the units in the game.

    #region Variables;

    //Bool that tells when the unit is selected by the Player.
    public bool selected;

    //Reference to the GameMaster.
    GameMaster gm;

    //Distance the unit can walk.
    [SerializeField] int tileSpeed;

    //Check if the unit has moved.
    public bool hasMoved;

    //Speed the unit moves;
    [SerializeField] float moveSpeed;

    //Which team the unit is on.
    [SerializeField] int playerNumber;

    //Check the distance the unit can attack and what it can attack.
    [SerializeField] int attackRange;
    List<Unit> enemiesInRange = new List<Unit>();
    [HideInInspector] public bool hasAttacked;

    //Image that represents if the unit can be attacked.
    public GameObject weaponIcon;

    //Stats of each unit.
    public int health, attackDamage, defenseDamage, armor;

    //Animator from the camera.
    Animator camAnim;

    //Particles and image for the battles.
    [SerializeField] DamageIcon damageIcon;
    [SerializeField] GameObject deathEffect;

    //Text that shows to the Player each kings health.
    [SerializeField] TextMeshProUGUI kingHealth;

    //To see if the unit is a King unit.
    [SerializeField] bool isKing;

    #endregion;

    #region Unity Methods;

    //Called in the beginning of the game.
    private void Start()
    {
        //Getting the references to the Game Master and the Animator from the Camera.
        gm = FindObjectOfType<GameMaster>();
        camAnim = Camera.main.GetComponent<Animator>();

        UpdateKingHealth();
    }

    //Method called when the mouse is over the Collider.
    private void OnMouseOver()
    {
        //Checking if the Player clicks the left mouse button.
        if(Input.GetMouseButtonDown(1))
        {
            //Updating the Stats Pannel with this units stats.
            gm.ToggleStatsPanel(this);
        }
    }

    //Called when the Player has pressed the mouse button while over the Collider.
    private void OnMouseDown()
    {
        ResetWeaponIcon();

        //Reset the selection of a unit.
        if(selected == true)
        {
            selected = false;
            gm.selectedUnit = null;
            gm.ResetTiles();
        } 
        else
        {
            //Change the selected unit to the one the Player clicked. 
            if(playerNumber == gm.playerTurn)
            {
                if(gm.selectedUnit != null)
                {
                    gm.selectedUnit.selected = false;
                }
            
                selected = true;
                gm.selectedUnit = this;

                //Reset the board so only shows the information of the new selected unit.
                gm.ResetTiles();
                GetEnemies();
                GetWalkableTiles();

            }
        }

        //Attacking the unit clicked if it is a possible unit to be attacked from the other team.
        Collider2D col = Physics2D.OverlapCircle(Camera.main.ScreenToWorldPoint(Input.mousePosition), 0.15f);
        Unit unit = col.GetComponent<Unit>();

        if (gm.selectedUnit != null)
        {
             if(gm.selectedUnit.enemiesInRange.Contains(unit) && gm.selectedUnit.hasAttacked == false)
            {
                gm.selectedUnit.Attack(unit);
            }
        }
    }

    #endregion;

    #region Other Methods;

    //Method that updates the Text element to the health from the King unit.
    public void UpdateKingHealth()
    {
        if (isKing == true)
        {
            kingHealth.text = health.ToString();
        }
    }

    //Method called to begin combat in the game.
    void Attack(Unit enemy)
    {
        //Shake the camera in the game.
        camAnim.SetTrigger("Shake");

        //Deals the attack damage to both the units. The one the player is selecting and the one he chooses to attack.
        hasAttacked = true;
        int enemyDamage = attackDamage - enemy.armor;
        int myDamage = enemy.defenseDamage - armor;

        //If the attack is effective, updating the visuals and the right values to the UI.
        if(enemyDamage >= 1)
        {
            DamageIcon instance = Instantiate(damageIcon, enemy.transform.position, Quaternion.identity);
            instance.Setup(enemyDamage);
            enemy.health -= enemyDamage;
            enemy.UpdateKingHealth();
        }

        //Calculations made to deal with units that can attack from a distance and units that cannot.
        if(transform.tag == "Ranged" && enemy.tag != "Ranged")
        {
            if(Mathf.Abs(transform.position.x - enemy.transform.position.x)+ Mathf.Abs(transform.position.y - enemy.transform.position.y) <= 1)
            {
                if(myDamage >= 1)
                {
                    DamageIcon instance = Instantiate(damageIcon, transform.position, Quaternion.identity);
                    instance.Setup(myDamage);
                    health -= myDamage;
                    UpdateKingHealth();
                }

            }
        }
        else
        {
            if (myDamage >= 1)
            {
                DamageIcon instance = Instantiate(damageIcon, transform.position, Quaternion.identity);
                instance.Setup(myDamage);
                health -= myDamage;
                UpdateKingHealth();
            }
        }

        //Checking if the enemy`s unit is dead.
        if(enemy.health <= 0)
        {
            Instantiate(deathEffect, enemy.transform.position, Quaternion.identity);
            Destroy(enemy.gameObject);
            GetWalkableTiles();
            gm.RemoveStatsPanel(enemy);
        }

        //Checking if the player`s unit is dead.
        if(health <= 0)
        {
            Instantiate(deathEffect, transform.position, Quaternion.identity);
            gm.ResetTiles();
            gm.RemoveStatsPanel(this);
            Destroy(this.gameObject);
        }

        gm.UpdateStatsPanel();
    }
    
    //Method called to check which tiles the selected unit can walk on and changing the visuals accordingly.
    void GetWalkableTiles()
    {
        //Can only move once per turn.
        if(hasMoved)
        {
            return;
        }

        //Checking for the distance for each tile if is smaller than the unit`s walking distance.
        foreach (Tile tile in FindObjectsOfType<Tile>())
        {
            if(Mathf.Abs(transform.position.x - tile.transform.position.x) + Mathf.Abs(transform.position.y - tile.transform.position.y) <= tileSpeed)
            {
                if (tile.IsClear())
                {
                    tile.Highlight();
                }
            }
        }
    }

    //Method that gets the enemies inside the Unit`s attack range.
    void GetEnemies()
    {
        enemiesInRange.Clear();

        //Checking for the distance for each tile if is smaller than the unit`s walking distance.
        foreach (Unit unit in FindObjectsOfType<Unit>())
        {
            if (Mathf.Abs(transform.position.x - unit.transform.position.x) + Mathf.Abs(transform.position.y - unit.transform.position.y) <= attackRange)
            {
                if(unit.playerNumber != gm.playerTurn && hasAttacked == false)
                {
                    enemiesInRange.Add(unit);
                    unit.weaponIcon.SetActive(true);
                }
            }
        }
    }

    //Method to set the weapon`s icon false.
    public void ResetWeaponIcon()
    {
        foreach (Unit unit in FindObjectsOfType<Unit>())
        {
            unit.weaponIcon.SetActive(false);
        }
    }

    //Method to move the unit.
    public void Move(Vector2 tilePos)
    {
        gm.ResetTiles();
        StartCoroutine(StartMovement(tilePos));
    }    

    //Sequence to move the unit.
    IEnumerator StartMovement(Vector2 tilePos)
    {
        while(transform.position.x != tilePos.x)
        {
            transform.position = Vector2.MoveTowards(transform.position, new Vector2(tilePos.x, transform.position.y), moveSpeed * Time.deltaTime);
            yield return null;
        }

        while (transform.position.y != tilePos.y)
        {
            transform.position = Vector2.MoveTowards(transform.position, new Vector2(transform.position.x, tilePos.y), moveSpeed * Time.deltaTime);
            yield return null;
        }

        hasMoved = true;

        ResetWeaponIcon();
        GetEnemies();
        gm.MoveStatsPanel(this);
    }

    #endregion;
}
