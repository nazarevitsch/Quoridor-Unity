using System;
using UnityEngine;
using UnityEngine.UI;

public class UIControllerScript : MonoBehaviour
{
    public GameObject scores;
    public GameObject mainMenu;
    public GameObject win;
    public GameObject pause;
    public GameObject restart;

    private Game _game;
    
    private void Awake()
    {
        _game = FindObjectOfType<Game>();
        _game.OnEndGame += HideUI;
    }

    public void HideUI(string winner)
    {
        scores.SetActive(false);
        mainMenu.SetActive(true);
        win.SetActive(true);
        pause.SetActive(false);
        restart.SetActive(false);
        win.GetComponent<Text>().text = winner;
    }
}
