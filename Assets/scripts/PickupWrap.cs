using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupWrap : MonoBehaviour {

    private static int[] xMult = new int[] { 0, 1, 1, 1, 0, -1, -1, -1 };
    private static int[] yMult = new int[] { 1, 1, 0, -1, -1, -1, 0, 1 };

    [SerializeField] private SpriteRenderer parentRender;
    [SerializeField] private BoxCollider2D parentCollide;
    [SerializeField] private Transform parentTransform;
    [SerializeField] public SizeDefinition size;
    [SerializeField] private int index = 0;

    private BoxCollider2D collide;
    private SpriteRenderer render;

    void Start () {
        float xOff = xMult[index % 8] * size.width;
        float yOff = yMult[index % 8] * size.height;

        transform.position = new Vector3(parentTransform.position.x + xOff, parentTransform.position.y + yOff, 0);

        collide = GetComponent<BoxCollider2D>();
        render = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void FixedUpdate () {
        if (collide != null)
        {
            collide.enabled = parentCollide.enabled;
        }

        render.enabled = parentRender.enabled;
    }
}
