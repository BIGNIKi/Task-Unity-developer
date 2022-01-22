using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System;
using UnityEngine.UI;

public class MainInstance : MonoBehaviour
{
    public GameObject prefOfCard;

    // Start is called before the first frame update
    void Start()
    {
        Cards cards = Util.LoadFromJson<Cards>("save.json");
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
        neww.transform.Find("UpPlashka/Name").GetComponent<Text>().text = cardInfo.name;
        neww.transform.Find("Info/Text").GetComponent<Text>().text = cardInfo.description;
        GameObject ob3d = Instantiate(Resources.Load<GameObject>(cardInfo.resourceGameObjectName), neww.transform.Find("3dObject"));
        ob3d.name = cardInfo.resourceGameObjectName;
        ob3d.transform.localPosition = cardInfo.position;
        ob3d.transform.rotation = cardInfo.rotation;
        ob3d.transform.localScale = cardInfo.scale;
    }

    private Cards GetAllCardsInfo()
    {
        Cards cards = new Cards();
        cards.cards = new List<CardInfo>();

        for(int i = 0; i< transform.childCount; i++)
        {
            if(transform.GetChild(i).gameObject.activeSelf == false)
            {
                continue;
            }

            CardInfo newCard = new CardInfo();
            var child = transform.GetChild(i);
            newCard.id = i;
            newCard.name = child.Find("UpPlashka/Name").GetComponent<Text>().text;
            newCard.description = child.Find("Info/Text").GetComponent<Text>().text;

            var ob3d = child.Find("3dObject").GetChild(0);
            newCard.resourceGameObjectName = ob3d.name;
            newCard.position = ob3d.transform.localPosition;
            newCard.scale = ob3d.transform.localScale;
            newCard.rotation = ob3d.transform.rotation;

            // newCard.isOpen = 

            cards.cards.Add(newCard);
        }
        return cards;
    }

    void OnApplicationQuit()
    {
        Util.ToJsonAndCreateFile(GetAllCardsInfo(), "save.json");
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
        //public bool isOpen;
        //public float timeToOpen;
        //public float timeLeftToOpen;
    }

    [Serializable]
    public class Cards
    { 
        public List<CardInfo> cards;
    }
}
