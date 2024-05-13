using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinSpawner : MonoBehaviour
{

    [SerializeField] GameObject coinPrefab;
    [SerializeField] SizeDefinition size;

    // Spawn all the coins
    void Start()
    {
        int children = transform.childCount;


        for (int i = 0; i < children; ++i)
        {
            //Spawn coin a location
            Transform target = transform.GetChild(i);

            newCoin(target.position);
        }
    }

    private void newCoin(Vector2 pos)
    {
        GameObject coin = Instantiate(coinPrefab, pos, Quaternion.identity);

        PickupWrap[] wrapComponents = coin.GetComponentsInChildren<PickupWrap>();

        //Itterate through the coins children and setup the level size
        for (int i = 0; i < wrapComponents.Length; ++i)
        {

            wrapComponents[i].size = size;
            
        }
    }
}
