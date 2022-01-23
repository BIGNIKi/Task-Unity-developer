using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollLogic : MonoBehaviour
{
    private bool hasSnapped = true; // have we already snapped camera to a center of a card?

    private float scrollValueOld, scrollValueNew;

    public Scrollbar scrollbar; // reference to the scrollbar

    private int numOfCard;

    // Update is called once per frame
    void Update()
    {
        // should we fix the camera? && no touches
        if(!hasSnapped && Input.touchCount == 0 && !Input.GetMouseButton(0)) // I check Mouse button for debuggin in editor
        {
            // if scrolling has finished
            if(scrollValueNew == scrollValueOld)
            {
                hasSnapped = true;

                //numOfCard = this.transform.childCount;
                numOfCard = Util.getChildCountActive(transform);
                float[] pos = GetPositionVector();
                //float[] pos = new float[numOfCard];
                float space = 1f / (numOfCard - 1f); // space between two cards in terms of scrollbar value
                for(int i = 0; i < numOfCard; i++)
                {
                    //pos[i] = space * i; // value of scrollbar value for ith card
                    if(scrollValueNew > pos[i] - (space / 2) && scrollValueNew < pos[i] + (space / 2))
                    {
                        StartCoroutine(SmoothSlide(pos[i], false));
                        break;
                    }
                }
            }
            else
            {
                scrollValueOld = scrollValueNew;
            }
        }
    }

    private float[] GetPositionVector()
    {
        numOfCard = Util.getChildCountActive(transform);
        float[] pos = new float[numOfCard];
        float space = 1f / (numOfCard - 1f); // space between two cards in terms of scrollbar value
        for(int i = 0; i < numOfCard; i++)
        {
            pos[i] = space * i; // value of scrollbar value for ith card
        }
        return pos;
    }

    private IEnumerator SmoothSlide(float to, bool isInitialMove)
    {
        float from = scrollbar.value;
        float progress = 0;
        while(Mathf.Abs(scrollbar.value - to) > 0.01f)
        {
            if(!hasSnapped || !isInitialMove && (scrollbar.value <= 0 || scrollbar.value >= 1))
            {
                yield break;
            }
            progress += Time.deltaTime;
            scrollbar.value = Mathf.Lerp(from, to, progress);

            yield return null;
        }

        // ниже - отладка бага параллелизма
        // когда значение value scrollbar'а на прошлом кадре совпало с (to)
        // но на след кадр инерция меняет значение, сдвигая галерею в неверное место
        // код ниже убирает появляющееся не нужное смещение
        yield return null;
        if(Mathf.Abs(scrollbar.value - to) > 0.01f)
        {
            scrollbar.value = to;
        }
        yield break;
    }

    public void MakeInitialMove(int mode)
    {
        bool shouldOpen = mode == 0 ? true : false;
        numOfCard = transform.childCount;
        if(numOfCard == 0)
        {
            return;
        }
        int id = 0; // id of card to move
        for(int i = 0; i < numOfCard; i++)
        {
            if(transform.GetChild(i).gameObject.activeSelf == false)
            {
                continue;
            }

            if(transform.GetChild(i).GetComponent<CardState>().IsOpened() == shouldOpen)
            {
                break;
            }
            id++;

            if(i == numOfCard - 1)
            {
                return;
            }
        }

        float[] pos = GetPositionVector();
        StartCoroutine(SmoothSlide(pos[id], true));
    }

    public void ScrollbarCallback()
    {
        if(Input.touchCount > 0 || Input.GetMouseButton(0))
        {
            hasSnapped = false;
            scrollValueNew = scrollbar.value;
        }
    }
}
