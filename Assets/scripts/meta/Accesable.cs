using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Accesable : MonoBehaviour
{

    public static Dictionary<string, GameObject> LookupDict = new Dictionary<string, GameObject>();

    [SerializeField] string globalName;

    // Start is called before the first frame update
    void Awake()
    {

        
         
        if (LookupDict.ContainsKey(globalName))
        {
            LookupDict.Remove(globalName);
        }


        LookupDict.Add(globalName, gameObject);

    }

    
}
