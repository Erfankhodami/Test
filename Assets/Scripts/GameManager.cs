using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;


//این اسکریپت کنترل کننده خود بازی است
public class GameManager : MonoBehaviour
{
    //ساخت یک enum برای مشخص کردن حالتی که بازی در آن قرار دارد
    public enum GameState
    {
        PlayerTurn,
        OpponentTurn,
        RoundOver
    }

    //ساخت یک متغیر از نوع همان انوم که در بالا ساختیم
    public GameState currentState;

    //ساخت یک آرایه از نوع گیم ابجکت که کارت های قابل بازی کردن را ذخیره میکند
    [SerializeField] private List<GameObject> cards;

    //ساخت دو آرایه یکی برای کارت های بازیکن و دیگری برای کارت های حریف
    public Card[] playerCards;

    public Card[] opponentCards;

    //ساخت دو متغیر از نوع کلاس card برای ذخیره کردن کارت بازی شده توسط بازیکن و حریف
    public Card playerThrownCard;

    public Card opponentThrownCard;

    //سه متغیر بول برای مشخص کردن اینکه حریف نوبتش را بازی کرده یا خیر و اینکه بازی فعال است یا خیر
    public bool isPlayerPlayed;
    public bool isOpponentPlayed;

    public bool isGameActive;

    //دو متغیر اینت برای ذخیره امتیاز پلیر و بازیکن
    public int playerScore;

    public int opponentScore;

    //ساخت یک متغیر برای زیاد کردن فاصله کارت ها به ازای هر بار که یک دست بازی شد
    private float dis;

    //ساخت سه متغیر گیم ابجکت برای زیرمجموعه کردن کارت ها به آنها
    private GameObject opponent;
    private GameObject playedCards;

    private GameObject player;

    //ساخت یک متغیر برای مدیریت ایجاد کارت ها به صورت رندوم
    private List<GameObject> spawnedCards;

    //ساخت یک متغیر از نوع ui handler برای دسترسی راحت تر به اسکریپت UIHandler
    private UIHandler uiHandler;

    private void Start()
    {
        //مقدار دهی متغیر های ایجاد شده توسط خود یونیتی
        uiHandler = GameObject.Find("UIHandler").GetComponent<UIHandler>();
        playedCards = GameObject.Find("PlayedCards");
        opponent = GameObject.Find("Opponent");
        player = GameObject.Find("Player");
    }

    private void Update()
    {
        //این کد همواره اجرا میشود و نوبت بازیکن و حریف را مدیریت میکند
        if (isGameActive)
        {
            switch (currentState)
            {
                case GameState.PlayerTurn:
                    foreach (var card in playerCards)
                    {
                        card.HandlePlayerTurn();
                    }

                    break;
                case GameState.OpponentTurn:
                    StartCoroutine(ThrowRandomOpponetCard());
                    break;
            }
        }

    }

    //این متد برای شروع بازی است
    public void StartGame()
    {
        isGameActive = true;
        dis = 0;
        isOpponentPlayed = false;
        isPlayerPlayed = false;
        opponentScore = 0;
        playerScore = 0;
        currentState = GameState.PlayerTurn;
        spawnedCards = new List<GameObject>(cards);
        for (int i = 0; i < cards.Count / 2; i++)
        {
            int x = Random.Range(0, spawnedCards.Count);
            GameObject card = spawnedCards[x];
            Instantiate(card, transform.position, card.transform.rotation, player.transform);
            spawnedCards.RemoveAt(x);
        }

        for (int i = 0; i < cards.Count / 2; i++)
        {
            int x = Random.Range(0, spawnedCards.Count);
            GameObject card = spawnedCards[x];
            Instantiate(card, transform.position, card.transform.rotation, opponent.transform);
            spawnedCards.RemoveAt(x);
        }


        playerCards = player.transform.GetComponentsInChildren<Card>();
        opponentCards = opponent.transform.GetComponentsInChildren<Card>();

        for (int i = 0; i < playerCards.Length; i++)
        {

            playerCards[i].Move(new Vector3((i - (((float)playerCards.Length - 1) / 2)) * 85, 0, i * -10));
            playerCards[i].Flip();
            playerCards[i].isPlayerCard = true;
            playerCards[i].transform.Find("Frame").GetComponent<Image>().sprite = uiHandler.selectedFrame;
        }

        for (int i = 0; i < opponentCards.Length; i++)
        {
            opponentCards[i].Move(new Vector3((i - (((float)playerCards.Length - 1) / 2)) * 85, 0, i * -10));
            opponentCards[i].isPlayerCard = false;
            opponentCards[i].transform.Find("Frame").GetComponent<Image>().sprite = uiHandler.selectedFrame;
        }
        uiHandler.UpdateScore();
    }


    public void UpdateCardsOrder(GameObject card)
    {
        card.transform.SetParent(playedCards.transform);
        if (card.GetComponent<Card>().isPlayerCard)
        {
            playerCards = player.transform.GetComponentsInChildren<Card>();
            MoveCard(playerCards, dis);
            isPlayerPlayed = true;
            EvaluateRound();
        }
        else
        {
            opponentCards = opponent.transform.GetComponentsInChildren<Card>();
            MoveCard(opponentCards, dis);
        }
    }


    public IEnumerator ThrowRandomOpponetCard()
    {
        if (!isOpponentPlayed)
        {
            isOpponentPlayed = true;
            int index = Random.Range(0, opponentCards.Length);
            opponentThrownCard = opponentCards[index];
            yield return new WaitForSeconds(1);
            opponentCards[index].Flip();
            yield return new WaitForSeconds(0.1f);
            opponentCards[index].ThrowCard();
            EvaluateRound();
        }
    }

    void MoveCard(Card[] cards, float dis)
    {
        for (int i = 0; i < cards.Length; i++)
        {
            cards[i].Move(new Vector3((i - (((float)cards.Length - 1) / 2)) * (85 + dis), 0, i * -10));
        }
    }


    public void StartNewRound()
    {
        isOpponentPlayed = false;
        isPlayerPlayed = false;
        if (playerThrownCard.power > opponentThrownCard.power)
        {
            currentState = GameState.PlayerTurn;
        }
        else
        {
            currentState = GameState.OpponentTurn;
        }
    }

    public void EvaluateRound()
    {
        if (isPlayerPlayed && isOpponentPlayed)
        {
            if (playerThrownCard.power > opponentThrownCard.power)
            {
                playerScore++;
                playerThrownCard.Impact();
            }
            else
            {
                opponentScore++;
                opponentThrownCard.Impact();
            }
            print("RoundIsOver");
            StartCoroutine(FinishRound());
        }

        if (isPlayerPlayed && !isOpponentPlayed)
        {
            currentState = GameState.OpponentTurn;
        }

        if (!isPlayerPlayed && isOpponentPlayed)
        {
            currentState = GameState.PlayerTurn;
        }
    }

    IEnumerator FinishRound()
    {
        yield return new WaitForSeconds(3);
        uiHandler.UpdateScore();
        Card[] _playedCards = playedCards.transform.GetComponentsInChildren<Card>();
        foreach (var card in _playedCards)
        {
            card.Move(new Vector3(7, 0, 0));
        }

        currentState = GameState.RoundOver;
        if (playerCards.Length > 0)
        {
            StartNewRound();
            dis += 16;
        }
        else
        {
            bool isplayerwon = default;
            if (playerScore > opponentScore)
            {
                isplayerwon = true;
            }
            else
            {
                isplayerwon = false;
            }
            StartCoroutine(uiHandler.OpenFinishMenu(isplayerwon));
            print("The Game Is Finished");
        }
    }

    public void EndGame()
    {
        isGameActive = false;
        if (playerCards.Length != 0)
        {
            foreach (Transform card in GameObject.Find("Player").transform)
            {
                Destroy(card.gameObject);
            }
        }

        if (opponentCards.Length != 0)
        {
            foreach (Transform card in GameObject.Find("Opponent").transform)
            {
                Destroy(card.gameObject);
            }
        }

        foreach (Transform card in playedCards.transform)
        {
            Destroy(card.gameObject);
        }
    }
}

