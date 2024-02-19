using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPickup : MonoBehaviour
{

    public GameObject[] weapons;
    public GameObject PickThisWeapon;

    //public int WeaponNumber;


    void Start()
    {
        //weapons[0].SetActive(true);
    }

    

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if(collision.CompareTag("Player"))
        {
            Debug.Log("weapon is here");

            foreach(GameObject weapon in weapons)
            {
                weapon.SetActive(false);
            }

            PickThisWeapon.SetActive(true);
        }
        

    }
}
