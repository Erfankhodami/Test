using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIHandler : MonoBehaviour
{
    //تعریف کردن متغیر ها 
    //متغیر های animator مقدار هایی هستند که انیمیشن حرکت هارا مدیریت و اجرا میکنند
    [SerializeField] private Animator mainMenuAnimator;
    [SerializeField] private Animator inGameMenuAnimator;

    [SerializeField] private Animator frameSelectMenuAnimator;

    //متغیر های gameobject مقدار هایی هستند که مشخصات یک گیم ابجکت مثلا منو را مدریریت میکنند
    [SerializeField] private GameObject inGameUI;
    [SerializeField] private GameObject finishMenu;

    [SerializeField] private GameObject frameIndicator;

    //متغیر هایTextMeshProUGUI مقدار هایی هستند که متن و تکست توی بازی را ذخیره و مدیریت میکنند
    [SerializeField] private TextMeshProUGUI scoreText;

    //متغیر های list<> دقیقا مثل آرایه عمل میکنند فقط با چند تفاوت جزئی
    //متغیر List<Sprite> یک آرایه از نوع sprite میسازد که صرفا تصویر است
    [SerializeField] private List<Sprite> frames;

    //ساخت یک متغیر sprite که قاب انتخاب شده را ذخیره میکند
    public Sprite selectedFrame;

    //ساخت یک مقدار GameManager که یکی از اسکریپت های ما است برای دسترسی راحت تر به کد های آن اسکریپت 
    private GameManager gameManager;
    private bool inGameMenuOpen;

    private void Start()
    {
        //تعیین کردن نرخ تازه سازی صفحه به 60 فریم بر ثانیه
        Application.targetFrameRate = 60;
        SelectFrame(0);
        //مقدار دهی کردن متغیر gameobject که در بالا ساختیم
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    //این تابع بازی را شروع میکند
    public void StartGame()
    {
        //فعال کردن متن امتیاز های بازی و رابط کاربری داخل بازی
        scoreText.gameObject.SetActive(true);
        inGameUI.SetActive(true);
        //ریست کردن مکان کلید بازگشت به منوی اصلی
        inGameMenuAnimator.transform.localPosition = new Vector3(658, 287, 0);
        inGameMenuOpen = false;
        //حرکت دادن کلید های منو اصلی به پایین
        mainMenuAnimator.SetTrigger("MoveDown");
        gameManager.StartGame();
    }

    //این تابع متن امتیاز های بازی را آپدیت میکند
    public void UpdateScore()
    {
        scoreText.text = gameManager.opponentScore +""+ gameManager.playerScore;
    }


    //این تابع موقعی که بازی تمام میشود اجرا میشود و منوی پایان بازی را فعال میکند
    public IEnumerator OpenFinishMenu(bool isPlayerWon)
    {
        //تاخیر به مدت 1 ثانیه
        yield return new WaitForSeconds(1);
        //غیر فعال کردن منوی داخل بازی و فعال کردن منوی پایان بازی
        inGameUI.SetActive(false);
        finishMenu.SetActive(true);
        
        gameManager.EndGame();
        //ساخت دو مقدار از نوع گیم ابجکت یکی برای برای متن نشانگر برد و باخت و دیگری برای دکمه بازگشت به منو اصلی
        GameObject resultText = finishMenu.transform.Find("Result").gameObject;
        GameObject backToMenuButton = finishMenu.transform.Find("BackToMainMenu").gameObject;
        resultText.transform.localPosition = new Vector3(0, 1300, 0);
        backToMenuButton.transform.localPosition = new Vector3(0, -300, 0);
        string text = default;
        //مقدار دهی text
        if (isPlayerWon)
        {
            text = "YouWon!";
        }
        else
        {
            text = "You Lost!";
        }
        //ریختن مقدار text داخل متن برد و باخت
        resultText.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = text;
        //حرکت دادن متن و دکمه بازگشت به منو
        resultText.GetComponent<Animator>().SetTrigger("MoveDown");
        backToMenuButton.GetComponent<Animator>().SetTrigger("MoveUp");
    }

    //این تابع بازی را به منوی اصلی بازمیگرداند
    public void BackToMenu()
    {
        //غیر فعال کردن متن امتیاز های بازی، منوی پایان بازی و رابط کاربری داخب بازی
        scoreText.gameObject.SetActive(false);
        finishMenu.SetActive(false);
        inGameUI.SetActive(false);
        //حرکت دکمه های منوی اصلی به بالا
        mainMenuAnimator.SetTrigger("MoveUp");
        gameManager.EndGame();
    }

    //بازکردن منوی داخل بازی
    public void OpenInGameMenu()
    {
        if (!inGameMenuOpen)
        {
            inGameMenuAnimator.transform.localPosition = new Vector3(658, 287, 0);
            inGameMenuAnimator.SetTrigger("MoveLeft");
            inGameMenuOpen = true;
        }
        else
        {
            inGameMenuAnimator.SetTrigger("MoveRight");
            inGameMenuOpen = false;
        }
    }

    //بازکردن منوی انتخاب قاب
    public void OpenFrameSelectMenu()
    {
        mainMenuAnimator.transform.localPosition=Vector3.zero;
        mainMenuAnimator.SetTrigger("MoveDown");
        frameSelectMenuAnimator.SetTrigger("MoveUp");
    }


    //بستن منوی انتخاب قاب
    public void CloseFrameSelectMenu()
    {
        mainMenuAnimator.SetTrigger("MoveUp");
        frameSelectMenuAnimator.SetTrigger("MoveDown");
    }

    //مشخص کردن قاب انتخاب شده و ریختن مقدار آن داخل متغیر selectedframe که در بالا ساختیم
    public void SelectFrame(int index)
    {
        selectedFrame = frames[index];
        frameIndicator.transform.localPosition = new Vector3((index - 1) * 265, frameIndicator.transform.localPosition.y,0);
    }
}
