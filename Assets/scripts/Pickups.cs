using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;

public class Pickups : MonoBehaviour {
    [SerializeField] string nextLevel;



    static Vector2 attractionPoint = new Vector2(0, 0.5f);
    static float attractionFactor = 0.05f;

    PlayerMovement move;

    private void Start () {
        move = GetComponent<PlayerMovement>();
    }

    Vector2 mirrorPos;
    bool won = false;
    float winTimer = 1f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Goal"))
        {

            if (!won)
            {
                move.win();
                AudioManager.instance.PlaySound("victory");

                LevelTransition.instance.EndLevel();

                won = true;

                mirrorPos = collision.gameObject.transform.position;

                mirrorPos = mirrorPos + attractionPoint;
            }
        }
        else if (collision.gameObject.CompareTag("Coin"))
        {

            AudioManager.instance.PlaySound("coin_collect");

            Destroy(collision.gameObject);

        }
    }

    private void FixedUpdate ()  {
        if (won) {
            winTimer -= Time.deltaTime;

            //Pull player towards mirror
            Vector2 playerPos = move.gameObject.transform.position;

            Vector2 offset = mirrorPos - playerPos;

            offset *= attractionFactor;

            playerPos += offset;

            move.gameObject.transform.position = playerPos;


            if (winTimer < 0) {
                SceneManager.LoadScene(nextLevel);
            }
        }
    }
}
