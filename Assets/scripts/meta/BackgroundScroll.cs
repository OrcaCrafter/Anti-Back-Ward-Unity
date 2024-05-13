using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BackgroundScroll : MonoBehaviour
{

    [SerializeField] int BackgroundHeight;
    [SerializeField] int BackgroundWidth;
    [SerializeField] float scrollSpeed;

    private Tilemap write;

    // Start is called before the first frame update
    void Start()
    {
        write = GetComponent<Tilemap>();

        int halfw = BackgroundWidth / 2;
        int halfh = BackgroundHeight / 2;

        int offset = BackgroundHeight;

        if (scrollSpeed < 0)
        {
            offset *= -1;
        }

        //Copy every thing to the top
        for (int i = -halfw - 2; i < halfw; i++)
        {
            for (int j = -halfh - 2; j < halfh; j++)
            {

                TileBase t = write.GetTile(new Vector3Int(i, j, 0));

                if (t != null)
                {
                    write.SetTile(new Vector3Int(i, j + offset, 0), t);
                }

            }
        }
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {


        Vector3 backgroundPos = transform.position;

        backgroundPos.y -= scrollSpeed/50.0f;

        if (scrollSpeed > 0)
        {

            if (backgroundPos.y < -BackgroundHeight)
            {
                backgroundPos.y = 0;
            }
        } else
        {
            if (backgroundPos.y > BackgroundHeight)
            {
                backgroundPos.y = 0;
            }
        }

        transform.position = backgroundPos;
    }
}
