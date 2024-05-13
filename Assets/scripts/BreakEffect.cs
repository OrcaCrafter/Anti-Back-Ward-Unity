using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakEffect : MonoBehaviour
{


    [SerializeField] GameObject topLeft;
    [SerializeField] GameObject topRight;
    [SerializeField] GameObject bottomLeft;
    [SerializeField] GameObject bottomRight;

    [SerializeField] float force;
    [SerializeField] float TTL;

    float timeAlive;

    public void FixedUpdate()
    {

        timeAlive -= Time.deltaTime;

        float alpha = timeAlive / TTL;

        fadeCorner(topLeft, alpha);
        fadeCorner(topRight, alpha);
        fadeCorner(bottomLeft, alpha);
        fadeCorner(bottomRight, alpha);

        if (TTL < 0)
        {
            Destroy(gameObject);
        }

    }

    public void playEffect(Vector3 scale)
    {

        timeAlive = TTL;

        transform.localScale = scale;

        wakeCorner(topLeft, new Vector2(-force, force));
        wakeCorner(topRight, new Vector2(force, force));
        wakeCorner(bottomLeft, new Vector2(-force, -force));
        wakeCorner(bottomRight, new Vector2(force, -force));

    }

    private static void wakeCorner (GameObject corner, Vector2 vel)
    {

        Rigidbody2D body = corner.GetComponent<Rigidbody2D>();
        SpriteRenderer sprite = corner.GetComponent<SpriteRenderer>();

        if (body != null && sprite != null)
        {
            body.WakeUp();
            sprite.enabled = true;

            body.AddForce(vel);
        }

    }

    private static void fadeCorner (GameObject corner, float alpha)
    {
        
        SpriteRenderer sprite = corner.GetComponent<SpriteRenderer>();

        if (sprite != null)
        {
            Color set = sprite.color;

            set.a = alpha;

            sprite.color = set;
        }

    }
}
