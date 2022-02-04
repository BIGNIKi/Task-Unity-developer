using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;

public class MainInstance : MonoBehaviour
{
    public GameObject prefOfCard;

    public InitButton initButton;

    public ScrollLogic scrollLogic;


    void Start()
    {
        OpenGallery();
    }

    private void OpenGallery()
    {
        initButton.LoadPref();

        LoadAndCreateAllCards();

        scrollLogic.MakeInitialMove(initButton.GetMode());
    }

    private void LoadAndCreateAllCards()
    {
        Cards cards = Util.LoadFromJson<Cards>(Application.persistentDataPath + "/save.json");

        /// for tests
        if(cards == null)
        {
            cards = JsonUtility.FromJson<Cards>(Resources.Load<TextAsset>("save").text);
            Debug.Log("You use test save");
        }
        ///

        if(cards != null)
        {
            foreach(CardInfo cardInfo in cards.cards)
            {
                CreateCard(cardInfo);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void CreateCard(CardInfo cardInfo)
    {
        GameObject neww = Instantiate(prefOfCard, transform);
        // neww.transform.Find("UpPlashka/Name").GetComponent<Text>().text = cardInfo.name;
        neww.transform.Find("UpPlashka/Name").GetComponent<TextMeshProUGUI>().text = cardInfo.name;
        // neww.transform.Find("Info/Text").GetComponent<Text>().text = cardInfo.description;
        neww.transform.Find("Info/Text").GetComponent<TextMeshProUGUI>().text = cardInfo.description;
        GameObject ob3d = Instantiate(Resources.Load<GameObject>(cardInfo.resourceGameObjectName), neww.transform.Find("3dObject"));
        ob3d.name = cardInfo.resourceGameObjectName;
        ob3d.transform.localPosition = cardInfo.position;
        ob3d.transform.rotation = cardInfo.rotation;
        ob3d.transform.localScale = cardInfo.scale;
        if(cardInfo.isOpen)
        {
            MakeCardOpened(neww);
        }

        neww.transform.GetComponent<CardState>().SetTimeToOpenSeconds(cardInfo.timeToOpenSeconds);
        if(cardInfo.isOpeningProcessStarted == true)
        {
            if(Util.GetTimePassed() >= cardInfo.timeRemainToOpen)
            {
                MakeCardOpened(neww);
            }
            else
            {
                int realRemain = cardInfo.timeRemainToOpen - (int)Util.GetTimePassed();
                neww.transform.GetComponent<CardState>().SetIsOpeneningProcessStarted(true);
                neww.transform.GetComponent<CardState>().delayTimer.SetActive(true);
                StartCoroutine(neww.transform.GetComponent<CardState>().TimerBusinessLogic(realRemain));
            }
        }
    }

    private void MakeCardOpened(GameObject card)
    {
        card.transform.Find("dark").GetComponent<DarkLogic>().ChangeOpenState();
        card.transform.Find("dark").GetComponent<DarkLogic>().canvas.alpha = 0;
        card.transform.GetComponent<CardState>().delayTimer.SetActive(false);
        card.transform.GetComponent<CardState>().SetIsOpened(true);
        card.transform.GetComponent<CardState>().SetIsOpeneningProcessStarted(false);
    }

    private Cards GetAllCardsInfo()
    {
        Cards cards = new Cards();
        cards.cards = new List<CardInfo>();

        for(int i = 0, id = 0; i< transform.childCount; i++)
        {
            if(transform.GetChild(i).gameObject.activeSelf == false)
            {
                continue;
            }

            CardInfo newCard = new CardInfo();
            var child = transform.GetChild(i);
            newCard.id = id;
            id++;
            newCard.name = child.Find("UpPlashka/Name").GetComponent<TextMeshProUGUI>().text;

            newCard.description = child.Find("Info/Text").GetComponent<TextMeshProUGUI>().text;

            var ob3d = child.Find("3dObject").GetChild(0);
            newCard.resourceGameObjectName = ob3d.name;
            newCard.position = ob3d.transform.localPosition;
            newCard.scale = ob3d.transform.localScale;
            newCard.rotation = ob3d.transform.rotation;

            newCard.isOpen = child.GetComponent<CardState>().IsOpened();
            newCard.isOpeningProcessStarted = child.GetComponent<CardState>().IsOpeneningProcessStarted();
            if(newCard.isOpeningProcessStarted == true)
            {
                newCard.timeRemainToOpen = child.GetComponent<CardState>().GetTimeRemainToOpen();
            }
            newCard.timeToOpenSeconds = child.GetComponent<CardState>().GetTimeToOpenSeconds();

            cards.cards.Add(newCard);
        }
        return cards;
    }

    void OnApplicationQuit()
    {
        CloseGallery();
    }

    // true - pause, false - re-opened
    void OnApplicationPause(bool pauseStatus)
    {
        if(pauseStatus)
        {
            /// костыль, чтобы пофиксить след. баг:
            /// ѕри первом запуске прилож просит у ќ— андроида разрешение на сохранение данных
            /// из-за этого приложение уходит в паузу и после сохран€ет данные, чего по задумке быть не должно
            if(Util.getChildCountActive(transform) == 0)
            {
                return;
            }
            ///

            CloseGallery();
        }
        else
        {
            var pref = Util.LoadFromJson<InitButton.Preferences>(Application.persistentDataPath + "/preferences.json");
            if(pref != null)
            {
                double sec = TimeSpan.FromTicks(DateTime.UtcNow.Ticks - pref.lastTime).TotalSeconds;
                Util.SetTimePassed((long)sec);
                foreach(Transform child in transform)
                {
                    if(child.gameObject.activeSelf == false)
                    {
                        continue;
                    }
                    if(child.GetComponent<CardState>().IsOpeneningProcessStarted())
                    {
                        if(Util.GetTimePassed() >= child.GetComponent<CardState>().GetTimeRemainToOpen())
                        {
                            MakeCardOpened(child.gameObject);
                        }
                        else
                        {
                            int realRemain = child.GetComponent<CardState>().GetTimeRemainToOpen() - (int)Util.GetTimePassed();
                            child.transform.GetComponent<CardState>().SetTimeRemainToOpen(realRemain);
                        }
                    }
                }
            }
        }
    }

    private void CloseGallery()
    {
        Util.ToJsonAndCreateFile(GetAllCardsInfo(), Application.persistentDataPath + "/save.json");
        initButton.SavePref();
    }

        [Serializable]
    public struct CardInfo
    {
        public int id;
        public string name;
        public string description;
        public string resourceGameObjectName;
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 scale;
        public bool isOpen;
        public bool isOpeningProcessStarted;
        public int timeToOpenSeconds;
        public int timeRemainToOpen;
    }

    [Serializable]
    public class Cards
    { 
        public List<CardInfo> cards;
    }
}
