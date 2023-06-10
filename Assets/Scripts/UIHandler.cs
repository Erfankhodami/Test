using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIHandler : MonoBehaviour
{
    [SerializeField] private Animator mainMenuAnimator;
    [SerializeField] private Animator inGameMenuAnimator;
    [SerializeField] private GameObject InGameMenu;

    public float InGameMenuPosX;
    private GameManager gameManager;
    private bool inGameMenuOpen;

    private void Start()
    {
        InGameMenuPosX = inGameMenuAnimator.transform.localPosition.x;
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    public void StartGame()
    {
        InGameMenu.SetActive(true);
        mainMenuAnimator.SetTrigger("MoveDown");
        gameManager.StartGame();
    }

    public void BackToMenu()
    {
        InGameMenu.SetActive(false);
        mainMenuAnimator.SetTrigger("MoveUp");
        gameManager.EndGame();
        inGameMenuAnimator.SetTrigger("MoveRight");
    }

    public void OpenInGameMenu()
    {
        if (!inGameMenuOpen)
        {
            var pos = inGameMenuAnimator.transform.localPosition;
            pos.x = InGameMenuPosX;
            inGameMenuAnimator.transform.localPosition = pos;
            
            inGameMenuAnimator.SetTrigger("MoveLeft");
            inGameMenuOpen = true;
        }
        else
        {
            inGameMenuOpen = false;
        }
    }
}
