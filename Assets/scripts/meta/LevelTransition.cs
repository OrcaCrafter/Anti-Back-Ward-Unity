using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelTransition : MonoBehaviour
{

    public static LevelTransition instance;

    private Animator anim;

    // Start is called before the first frame update
    void Start ()
    {

        if (instance == null)
        {
            instance = this;
        } else
        {
            GameObject.Destroy(gameObject);
        }

        anim = GetComponentInChildren<Animator>();

    }
    

    public void EndLevel ()
    {
        anim.SetTrigger("End Level");
    }
}
