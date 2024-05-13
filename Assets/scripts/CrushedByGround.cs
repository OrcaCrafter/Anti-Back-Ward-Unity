using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CrushedByGround : MonoBehaviour {

    //I don't know why, but triple checking is necessary
    static int TotalCheckCount = 2;

    [SerializeField] BoxCollider2D collide;

    List<Tilemap> intersecting = new List<Tilemap>();

    int checkCount = 0;

    public void OnTriggerEnter2D (Collider2D other) {

        Tilemap map = other.gameObject.GetComponent<Tilemap>();

        if (map == null) return;

        intersecting.Add(map);
    }

    public void OnTriggerExit2D (Collider2D other)
    {
        Tilemap map = other.gameObject.GetComponent<Tilemap>();

        if (map == null) return;

        intersecting.Remove(map);
    }

    public void FixedUpdate()
    {


        //Debug.Log(intersecting.Count);

        if (intersecting.Count > 0)
        {

            if (checkCount < TotalCheckCount)
            {
                checkCount++;
                return;
            }


            PlayerMovement move = collide.gameObject.GetComponentInParent<PlayerMovement>();

            if (move != null)
            {
                move.kill();
                
            }
            else
            {
                
                EnemyInstance inst = GetComponentInParent<EnemyInstance>();
                EnemyController wrap = inst.GetComponentInParent<EnemyController>();

                wrap.KillCopy(inst.x, inst.y);
            }
        } else
        {
            checkCount = 0;
        }

    }
}
