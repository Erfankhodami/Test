using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIHandler : MonoBehaviour
{
    [SerializeField] private Animator mainMenuAnimator;
    [SerializeField] private Animator inGameMenuAnimator;
    [SerializeField] private GameObject inGameUI;
    [SerializeField] private GameObject finishMenu;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private Animator frameSelectMenuAnimator;
    [SerializeField] private List<Sprite> frames;
    [SerializeField] private GameObject frameIndicator;

    public Sprite selectedFrame;

    private GameManager gameManager;
    private bool inGameMenuOpen;


    private float InGameMenuPosX;

    private void Start()
    {
        Application.targetFrameRate = 60;
        SelectFrame(0);
        InGameMenuPosX = inGameMenuAnimator.transform.localPosition.x;
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    public void StartGame()
    {
        scoreText.gameObject.SetActive(true);
        inGameUI.SetActive(true);
        ResetInGameMenuPos();
        inGameMenuOpen = false;
        mainMenuAnimator.SetTrigger("MoveDown");
        gameManager.StartGame();
    }

    public void UpdateScore()
    {
        scoreText.text = gameManager.opponentScore +""+ gameManager.playerScore;
    }


    public IEnumerator OpenFinishMenu(bool isPlayerWon)
    {
        yield return new WaitForSeconds(0.1f);
        inGameUI.SetActive(false);
        GameObject resultText = finishMenu.transform.Find("Result").gameObject;
        GameObject backToMenuButton = finishMenu.transform.Find("BackToMainMenu").gameObject;

        resultText.transform.localPosition = new Vector3(0, 1300, 0);
        backToMenuButton.transform.localPosition = new Vector3(0, -300, 0);
        gameManager.EndGame();
        finishMenu.SetActive(true);
        string text = default;
        if (isPlayerWon)
        {
            text = "YouWon!";
        }
        else
        {
            text = "You Lost!";
        }

        resultText.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = text;
        resultText.GetComponent<Animator>().SetTrigger("MoveDown");
        backToMenuButton.GetComponent<Animator>().SetTrigger("MoveUp");
    }

    public void BackToMenu()
    {
        scoreText.gameObject.SetActive(false);
        finishMenu.SetActive(false);
        inGameUI.SetActive(false);
        mainMenuAnimator.SetTrigger("MoveUp");
        gameManager.EndGame();
    }

    public void OpenInGameMenu()
    {
        if (!inGameMenuOpen)
        {
            ResetInGameMenuPos();
            inGameMenuAnimator.SetTrigger("MoveLeft");
            inGameMenuOpen = true;
        }
        else
        {
            inGameMenuAnimator.SetTrigger("MoveRight");
            inGameMenuOpen = false;
        }
    }

    void ResetInGameMenuPos()
    {
        var pos = inGameMenuAnimator.transform.localPosition;
        pos.x = InGameMenuPosX;
        inGameMenuAnimator.transform.localPosition = pos;
    }

    public void OpenFrameSelectMenu()
    {
        mainMenuAnimator.transform.localPosition=Vector3.zero;
        mainMenuAnimator.SetTrigger("MoveDown");
        frameSelectMenuAnimator.SetTrigger("MoveUp");
    }

    public void CloseFrameSelectMenu()
    {
        mainMenuAnimator.SetTrigger("MoveUp");
        frameSelectMenuAnimator.SetTrigger("MoveDown");
    }

    public void SelectFrame(int index)
    {
        selectedFrame = frames[index];
        frameIndicator.transform.localPosition = new Vector3((index - 1) * 265, frameIndicator.transform.localPosition.y,0);
    }
}
