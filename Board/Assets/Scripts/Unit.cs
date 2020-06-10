using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Unit : MonoBehaviour
{
    public bool selected;
    GameMaster gm;

    public int tileSpeed;
    public bool hasMoved;

    public float moveSpeed;

    public int playerNumber;

    public int attackRange;
    List<Unit> enemiesInRange = new List<Unit>();
    public bool hasAttacked;

    public GameObject weaponIcon;

    public int health, attackDamage, defenseDamage, armor;

    Animator camAnim;

    public DamageIcon damageIcon;
    public GameObject deathEffect;

    public TextMeshProUGUI kingHealth;
    public bool isKing;

    private void Start()
    {
        gm = FindObjectOfType<GameMaster>();
        camAnim = Camera.main.GetComponent<Animator>();
        UpdateKingHealth();
    }

    public void UpdateKingHealth()
    {
        if (isKing == true)
        {
            kingHealth.text = health.ToString();
        }
    }

    private void OnMouseOver()
    {
        if(Input.GetMouseButtonDown(1))
        {
            gm.ToggleStatsPanel(this);
        }
    }

    private void OnMouseDown()
    {
        ResetWeaponIcon();

        if(selected == true)
        {
            selected = false;
            gm.selectedUnit = null;
            gm.ResetTiles();
        } 
        else
        {
            if(playerNumber == gm.playerTurn)
            {
                if(gm.selectedUnit != null)
                {
                    gm.selectedUnit.selected = false;
                }
            
                selected = true;
                gm.selectedUnit = this;

                gm.ResetTiles();
                GetEnemies();
                GetWalkableTiles();

            }
        }

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

    void Attack(Unit enemy)
    {
        camAnim.SetTrigger("Shake");

        hasAttacked = true;
        int enemyDamage = attackDamage - enemy.armor;
        int myDamage = enemy.defenseDamage - armor;

        if(enemyDamage >= 1)
        {
            DamageIcon instance = Instantiate(damageIcon, enemy.transform.position, Quaternion.identity);
            instance.Setup(enemyDamage);
            enemy.health -= enemyDamage;
            enemy.UpdateKingHealth();
        }

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

        if(enemy.health <= 0)
        {
            Instantiate(deathEffect, enemy.transform.position, Quaternion.identity);
            Destroy(enemy.gameObject);
            GetWalkableTiles();
            gm.RemoveStatsPanel(enemy);
        }

        if(health <= 0)
        {
            Instantiate(deathEffect, transform.position, Quaternion.identity);
            gm.ResetTiles();
            gm.RemoveStatsPanel(this);
            Destroy(this.gameObject);
        }

        gm.UpdateStatsPanel();
    }
    
    void GetWalkableTiles()
    {
        if(hasMoved)
        {
            return;
        }

        foreach(Tile tile in FindObjectsOfType<Tile>())
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

    void GetEnemies()
    {
        enemiesInRange.Clear();

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

    public void ResetWeaponIcon()
    {
        foreach (Unit unit in FindObjectsOfType<Unit>())
        {
            unit.weaponIcon.SetActive(false);
        }
    }

    public void Move(Vector2 tilePos)
    {
        gm.ResetTiles();
        StartCoroutine(StartMovement(tilePos));
    }    

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
}
