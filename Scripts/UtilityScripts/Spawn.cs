using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DefenderNameSpace;

//call this script when we are in the spawn action to spawn children
public class Spawn : MonoBehaviour
{
    [SerializeField]
    private GameObject spawnPrefab;
    private bool spawn = false;

    // if spawn then update and set spawn to false
    void Update()
    {
        if (spawn) //if spawn is true spawn the baby
        {
            spawnBaby();
            spawn = false;
        }

    }


    private void spawnBaby()
    {
        Instantiate(spawnPrefab, transform.position, transform.rotation); //spawn baby
    }
}