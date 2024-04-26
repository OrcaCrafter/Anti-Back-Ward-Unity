using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;

public class Goal : MonoBehaviour {
    [SerializeField] string nextLevel;

    PlayerMovement move;

    private void Start () {
        move = GetComponent<PlayerMovement>();
    }

    bool won = false;
    float winTimer = 1.5f;

    private void OnTriggerEnter2D (Collider2D collision)  {
        if (collision.gameObject.CompareTag("Goal")) {
            AudioManager.instance.PlaySound("victory");
            won = true;

            move.freeze = true;
        }
    }

    private void Update()  {
        if (won) {
            winTimer -= Time.deltaTime;

            if (winTimer < 0) {
                SceneManager.LoadScene(nextLevel);
            }
        }
    }
}
