using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageIcon : MonoBehaviour
{
    //Script that handle the icons to show the ammount of damage dealt in the battle.

    #region Variables;

    //Array of Sprites that have different values depending on the amount of damage dealt.
    [SerializeField] Sprite[] damageSprites;

    //Duration that the Icon will appear.
    [SerializeField] float lifetime;

    //Effect when the Icon is destroyed.
    [SerializeField] GameObject effect;

    #endregion;

    #region Methods;

    //Method called on the beginning of the game.
    private void Start()
    {
        Invoke("Destruction", lifetime);
    }

    //Method that defines the Image of the Icon depending on the damage.
    public void Setup(int damage)
    {
        GetComponent<SpriteRenderer>().sprite = damageSprites[damage - 1];
    }

    //Method that destroys the Icon and create the effect.
    void Destruction()
    {
        Instantiate(effect, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    #endregion;
}
