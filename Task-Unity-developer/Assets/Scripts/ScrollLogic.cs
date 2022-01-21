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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        numOfCard = this.transform.childCount;
        // should we fix the camera? && no touches
        if(!hasSnapped && Input.touchCount == 0 && !Input.GetMouseButton(0)) // I check Mouse button for debuggin in editor
        {
            // if scrolling has finished
            if(scrollValueNew == scrollValueOld)
            {
                hasSnapped = true;

                float[] pos = new float[numOfCard];
                float space = 1f / (numOfCard - 1f); // space between two cards in terms of scrollbar value
                for(int i = 0; i < numOfCard; i++)
                {
                    pos[i] = space * i; // value of scrollbar value for ith card
                    if(scrollValueNew > pos[i] - (space / 2) && scrollValueNew < pos[i] + (space / 2))
                    {
                        StartCoroutine(SmoothSlide(pos[i]));
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

    private IEnumerator SmoothSlide(float to)
    {
        float from = scrollbar.value;
        float progress = 0;
        while(Mathf.Abs(scrollbar.value - to) > 0.01f)
        {
            if(!hasSnapped || scrollbar.value <= 0 || scrollbar.value >= 1)
            {
                yield break;
            }
            progress += Time.deltaTime;
            scrollbar.value = Mathf.Lerp(from, to, progress);

            yield return null;
        }
        

        yield break;
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
