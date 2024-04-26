using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    [SerializeField] private Transform player;
    [SerializeField] private float followRate = 0.005f;

    void Update () {

        Vector2 delta = new Vector2(player.position.x - transform.position.x, player.position.y + 2 - transform.position.y);

        delta.x *= followRate;
        delta.y *= followRate;

        transform.position = new Vector3(transform.position.x + delta.x, transform.position.y + delta.y, transform.position.z);

        
    }
}
