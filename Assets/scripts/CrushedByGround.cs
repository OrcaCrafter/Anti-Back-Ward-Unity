using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CrushedByGround : MonoBehaviour {

    [SerializeField] private Tilemap jumpableGround;
    [SerializeField] private TilemapCollider2D jumpableGroundCollide;
    [SerializeField] private Tilemap warpableGround;
    [SerializeField] private TilemapCollider2D warpableGroundCollide;
    [SerializeField] private Tilemap looperGround;
    [SerializeField] private TilemapCollider2D looperCollide;

    private BoxCollider2D collide;
    private SpriteRenderer sprite;

    void Start () {
        collide = GetComponent<BoxCollider2D>();
        sprite = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update () {
        if (!collide.enabled)  {
            return;
        }

        int x = Mathf.FloorToInt(transform.position.x);
        int y = Mathf.FloorToInt(transform.position.y - 0.25f);

        Vector3Int checkPos = new Vector3Int(x, y, 0);

        if (jumpableGroundCollide.enabled) {
            TileBase t = jumpableGround.GetTile(checkPos);

            if (t != null) {
                squish();
            }
        }

        if (warpableGround.enabled) {
            TileBase t = warpableGround.GetTile(checkPos);

            if (t != null) {
                squish();
            }
        }

        if (looperCollide.enabled) {
            TileBase t = looperGround.GetTile(checkPos);

            if (t != null) {
                squish();
            }
        }
    }

    private void squish () {

        PlayerMovement move = collide.gameObject.GetComponent<PlayerMovement>();

        if (move != null) {
            move.dead = true;
        } else  {
            AudioManager.instance.PlaySound("die");

            collide.enabled = false;
            sprite.enabled = false;
        }
    }
}
