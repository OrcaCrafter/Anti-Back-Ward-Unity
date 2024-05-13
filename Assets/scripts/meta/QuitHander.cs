using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuitHander : MonoBehaviour
{
    
    enum QUIT_ACTION {
        MENU,
        CONFIRM,
        CANCEL
    }

    [SerializeField] CanvasRenderer quitConfirm;
    [SerializeField] QUIT_ACTION act;

    // Update is called once per frame
    public void Clicked ()
    {

        AudioManager.instance.PlaySound("menu_click");

        switch (act)
        {
            case QUIT_ACTION.MENU:

                quitConfirm.gameObject.SetActive(true);

            break;
            case QUIT_ACTION.CONFIRM:

                Application.Quit();

            break;
            case QUIT_ACTION.CANCEL:

                quitConfirm.gameObject.SetActive(false);

                break;
        }

    }
}
