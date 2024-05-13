using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EscapeHandler : MonoBehaviour
{

    public static EscapeHandler Handler;


    [SerializeField] string mainMenuName;
    [SerializeField] string levelSelectName;
    [SerializeField] string creditsName;
    [SerializeField] GameObject menuPrefab;

    public GameObject menu;

    Canvas canvas;

    private void Start()
    {
        DontDestroyOnLoad(gameObject);

        //Prevent duping
        if (Handler == null)
        {
            Handler = this;
        } else
        {

            Object.Destroy(gameObject);
        }

        //Load the main menu
        SceneManager.LoadScene(mainMenuName);
    }

    void Update()
    {

        //Freeze the game if the menu is open
        if (menu != null)
        {
            Time.timeScale = 0;
        } else
        {
            Time.timeScale = 1;
        }

        if (Input.GetKeyUp("escape"))
        {

            string sceneName = SceneManager.GetActiveScene().name;

            if (sceneName.Equals(mainMenuName))
            {
                //Close the game

                GameObject quitConfirm = Accesable.LookupDict.GetValueOrDefault("quit_confirm", null);

                if (quitConfirm != null)
                {
                    quitConfirm.SetActive(!quitConfirm.activeSelf);
                } else
                {
                    Debug.Log("Huh");
                }

            } else if (sceneName.Equals(levelSelectName) || sceneName.Equals(creditsName))
            {
                //Go to main menu
                SceneManager.LoadScene(mainMenuName);
            } else
            {
                //Toggle the game menu
                if (menu == null)
                {
                    menu = Instantiate(menuPrefab, new Vector2(0, 0), Quaternion.identity);
                } else
                {
                    Destroy(menu);
                    menu = null;
                }

            }
        }
    }
}
