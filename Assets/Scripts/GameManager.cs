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


public class GameManager : MonoBehaviour
{
    public enum GameState
    {
        PlayerTurn,
        OpponentTurn,
        RoundOver
    }

    public GameState currentState;
    [SerializeField] private List<GameObject> cards;
    public Card[] playerCards;
    public Card[] opponentCards;
    public int playerCardPower;
    public int opponentCardPower;

    public bool isPlayerPlayed;
    public bool isOpponentPlayed;

    public int playerScore;
    public int opponentScore;

    public bool isGameActive;
    private float dis;
    private GameObject opponent;
    private GameObject playedCards;
    private GameObject player;
    private SFXController SFXController;
    private List<GameObject> spawnedCards;

    private UIHandler uiHandler;

    private void Start()
    {
        uiHandler = GameObject.Find("UIHandler").GetComponent<UIHandler>();
        SFXController = GameObject.Find("SFX").GetComponent<SFXController>();
        playedCards = GameObject.Find("PlayedCards");
        opponent = GameObject.Find("Opponent");
        player = GameObject.Find("Player");
    }

    private void Update()
    {
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
            opponentCardPower = opponentCards[index].power;
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
        if (playerCardPower > opponentCardPower)
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
            if (playerCardPower > opponentCardPower)
            {
                playerScore++;

            }
            else
            {
                opponentScore++;
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
        SFXController.PlayClip(SFXController.cardThrowClip);
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

