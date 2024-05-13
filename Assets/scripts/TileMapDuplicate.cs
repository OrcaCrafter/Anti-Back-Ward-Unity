using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileMapDuplicate : MonoBehaviour {

    private static int[] xMult = new int[] { 0, 1, 1, 1, 0, -1, -1, -1};
    private static int[] yMult = new int[] { 1, 1, 0, -1, -1, -1, 0, 1};

    [SerializeField] private Tilemap read;
    [SerializeField] private TilemapCollider2D readCollide;
    [SerializeField] private TilemapRenderer readRender;
    [SerializeField] private Transform readPos;
    [SerializeField] private SizeDefinition size;
    [SerializeField] private int index;

    private Tilemap write;
    private TilemapCollider2D writeCollide;
    private TilemapRenderer writeRender;

    void Start () {
        write = GetComponent<Tilemap>();
        writeCollide = GetComponent<TilemapCollider2D>();
        writeRender = GetComponent<TilemapRenderer>();

        //Ensure that its in the correct position

        float xOff = size.width * xMult[index % 8];
        float yOff = size.height * yMult[index % 8];

        transform.position = new Vector2(readPos.position.x + xOff, readPos.position.y + yOff);


        for (int i = 0; i < size.width; i++) {
            for (int j = 0; j < size.height; j++) {

                TileBase t = read.GetTile(new Vector3Int(i, j, 0));

                if (t != null) {
                    write.SetTile(new Vector3Int(i, j, 0), t);
                }
            }
        }
    }
    
    void FixedUpdate () {
        if (!readRender.sortingLayerName.Equals(writeRender.sortingLayerName)) {
            writeRender.sortingLayerName = readRender.sortingLayerName;
        }

        writeRender.enabled = readRender.enabled;

        if (readCollide != null) {
            writeCollide.enabled = readCollide.enabled;
        }
    }
}
