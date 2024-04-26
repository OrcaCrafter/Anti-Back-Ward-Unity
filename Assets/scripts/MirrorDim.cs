using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MirrorDim : MonoBehaviour {

    [SerializeField] private int setActiveRegion = -1;
    [SerializeField] private MirrorRegions regions;
    [SerializeField] private SizeDefinition size;
    [SerializeField] private Tilemap mirrorRefrence;
    [SerializeField] private TilemapRenderer terrainRender;
    [SerializeField] private TilemapCollider2D terrainCollide;
    [SerializeField] private TilemapRenderer warpRender;
    [SerializeField] private TilemapCollider2D warpCollide;
    [SerializeField] public int setXScreenOff;
    [SerializeField] public int setYScreenOff;
    [SerializeField] private SpriteRenderer fog;
    [SerializeField] private Color fogColor;
    [SerializeField] private Color mirrorFogColor;
    [SerializeField] private bool flipX;
    [SerializeField] private bool flipY;

    private Tilemap mirrorMap;
    private int activeRegion = -1;
    private int xScreenOff;
    private int yScreenOff;

    void Start () {
        mirrorMap = GetComponent<Tilemap>();
        fog.color = fogColor;
    }
    
    void Update () {
        if ((activeRegion != -1) && (xScreenOff != setXScreenOff || yScreenOff != setYScreenOff)) {
            xScreenOff = setXScreenOff;
            yScreenOff = setYScreenOff;

            //Reload the dim
            clearDim();
            makeDim();
        }


        if (activeRegion != setActiveRegion) {
            activeRegion = setActiveRegion;
            
            if (activeRegion != -1) {
                //Establish mirror dim

                AudioManager.instance.PlaySound("mirror");

                makeDim();

            } else {
                //Clear mirror dim

                AudioManager.instance.PlaySound("mirror_rev");

                clearDim();
            }

        }
    }

    public bool isActive () {
        return activeRegion != -1;
    }

    public void setActive (int index) {
        if (setActiveRegion == -1) {
            setActiveRegion = index;
        } else  {
            setActiveRegion = -1;
        }

        setXScreenOff = 0;
        xScreenOff = 0;
        setYScreenOff = 0;
        yScreenOff = 0;
    }

    public void reload () {
        if ((activeRegion != -1) && (xScreenOff != setXScreenOff || yScreenOff != setYScreenOff)) {
            xScreenOff = setXScreenOff;
            yScreenOff = setYScreenOff;

            //Reload the dim
            clearDim();
            makeDim();
        }
    }

    private void makeDim() {

        fog.color = mirrorFogColor;

        terrainCollide.enabled = false;
        terrainRender.sortingLayerName = "Background";
        warpCollide.enabled = false;
        warpRender.sortingLayerName = "Background";
        warpRender.enabled = false;

        int minX = regions.startX[activeRegion];
        int minY = regions.startY[activeRegion];
        int mirrorWidth = regions.width[activeRegion];
        int mirrorHeight = regions.height[activeRegion];

        for (int i = -size.width; i < size.width*2; i++) {
            for (int j = -size.height; j < size.height*2; j++) {
                
                int refrenceX = mirrorMod((i - minX) + mirrorWidth*size.width, mirrorWidth, flipX) + minX;
                int refrenceY = mirrorMod((j - minY) + mirrorHeight*size.height, mirrorHeight, flipY) + minY;

                TileBase t = mirrorRefrence.GetTile(new Vector3Int(refrenceX, refrenceY, 0));

                int nudgeX = (xScreenOff + mirrorWidth*2*size.width)%(mirrorWidth*2);
                int nudgeY = (yScreenOff + mirrorHeight*2*size.height)%(mirrorHeight*2);

                int setX = (i + nudgeX + size.width)%(size.width*3) - size.width;
                int setY = (j + nudgeY + size.height)%(size.height*3) - size.height;

                mirrorMap.SetTile(new Vector3Int(setX, setY, 0), t);

            }
        }
    }

    private static int mirrorMod (int number, int modulator, bool fix) {
        if (fix) {
            if (number%(modulator*2) < modulator) {
                return (modulator - 1) - (number % modulator);
            }
        } else {
            if (number % (modulator * 2) >= modulator) {
                return (modulator - 1) - (number % modulator);
            }
        }

        return number % modulator;
    }


    private void clearDim () {

        fog.color = fogColor;

        terrainCollide.enabled = true;
        terrainRender.sortingLayerName = "Default";
        warpCollide.enabled = true;
        warpRender.sortingLayerName = "Default";
        warpRender.enabled = true;

        for (int i = -size.width; i < size.width *2; i++) {
            for (int j = -size.height; j < size.height *2; j++) {

                mirrorMap.SetTile(new Vector3Int(i, j, 0), null);

            }
        }
    }
}
