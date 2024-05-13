using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuAction : MonoBehaviour {
    
    [SerializeField] string levelToLoad;
    

    public void Clicked ()
    {
        if (levelToLoad.Equals("%CURRENT%"))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        else
        {
            SceneManager.LoadScene(levelToLoad);
        }

        

        AudioManager.instance.PlaySound("menu_click");
    }
    
}
