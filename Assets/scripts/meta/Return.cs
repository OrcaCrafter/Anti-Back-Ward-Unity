using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Return : MonoBehaviour {
    
    public void Clicked ()
    {

        //Remove the menu
        Destroy(EscapeHandler.Handler.menu);
        EscapeHandler.Handler.menu = null;

        AudioManager.instance.PlaySound("menu_click");
    }
    
}
