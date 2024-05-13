using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Persistent : MonoBehaviour
{

    static List<string> Existing = new List<string>();

    [SerializeField] string identifier;

    public void Awake ()
    {
        DontDestroyOnLoad(gameObject);

        //Prevent duplicates with the same ID
        if (Existing.Contains(identifier))
        {
            Destroy(gameObject);
        } else
        {
            Existing.Add(identifier);
        }
    }

}
