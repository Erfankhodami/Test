using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;


public class Card : MonoBehaviour
{
    public int power;
    public bool isPlayerCard = true;

    [SerializeField] private Sprite top;
    [SerializeField] private Sprite button;
    [SerializeField] private float moveSpeed;
    private GameManager gameManager;
    private Image img;
    private bool isSelected;
    private bool isTop = false;
    private Vector3 pos;
    private Vector3 rot;
    private Vector3 scale;

    public void Awake()
    {
        scale = new Vector3(1, 1, 1);
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        img = GetComponent<Image>();
    }

    private void Update()
    {
        transform.localPosition = Vector3.Lerp(transform.localPosition, pos, moveSpeed * Time.deltaTime);
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(rot), moveSpeed * Time.deltaTime);
    }

    public void HandlePlayerTurn()
    {
        if (!gameManager.isPlayerPlayed)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit = default;
                if (Physics.Raycast(ray, out hit))
                {
                    print(hit.collider);
                    Debug.DrawRay(ray.origin, ray.direction * 10);
                    if (hit.collider == GetComponent<BoxCollider>())
                    {
                        if (!isSelected)
                        {
                            MoveUp();
                        }
                        else
                        {

                            gameManager.playerCardPower = power;
                            ThrowCard();
                        }
                    }
                    else
                    {
                        MoveDown();
                    }

                }

            }
        }
    }


    public void Move(Vector3 destination)
    {
        pos = destination;
    }

    public void MoveUp()
    {
        if (!isSelected)
        {
            pos.y += 150;
            isSelected = true;
        }
    }

    public void MoveDown()
    {
        if (isSelected)
        {
            pos.y -= 150;
            isSelected = false;
        }
    }

    public void Flip()
    {
        rot.y += 180;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
        
        scale = GetComponent<BoxCollider>().size;
        scale.x *= -1;
        GetComponent<BoxCollider>().size = scale;
        if (img != null && button != null && top != null)
        {
            if (isTop)
            {
                img.sprite = button;
                isTop = false;
            }
            else
            {
                img.sprite = top;
                isTop = true;
            }
            print("flipped");
        }
    }

    public void ThrowCard()
    {
        pos=Vector3.zero;
        rot = new Vector3(rot.x, rot.y, Random.Range(-100,100));
        gameManager.UpdateCardsOrder(gameObject);
    }
}