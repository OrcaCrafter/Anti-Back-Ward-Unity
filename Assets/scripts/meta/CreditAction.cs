using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreditAction : MonoBehaviour
{


    [SerializeField] string URL;

    public void Clicked()
    {
        Application.OpenURL(URL);

        AudioManager.instance.PlaySound("menu_click");
    }
}
