using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CardState : MonoBehaviour
{
    private bool isOpened = false;
    private bool isOpeneningProcessStarted = false;

    public GameObject delayTimer;

    [SerializeField]
    private int timeToOpenSeconds = 0;
    private int timeRemainToOpen;

    public DarkLogic darkLogic;
    public Text textTime;
    public Image circleBar;

    public int GetTimeToOpenSeconds()
    {
        return timeToOpenSeconds;
    }

    public int GetTimeRemainToOpen()
    {
        return timeRemainToOpen;
    }

    public void SetTimeRemainToOpen(int val)
    {
        timeRemainToOpen = val;
    }
    
    public void SetTimeToOpenSeconds(int val)
    {
        timeToOpenSeconds = val;
    }

    public bool IsOpened()
    {
        return isOpened;
    }

    public bool IsOpeneningProcessStarted()
    {
        return isOpeneningProcessStarted;
    }

    public void SetIsOpeneningProcessStarted(bool val)
    {
        isOpeneningProcessStarted = val;
    }

    public void SetIsOpened(bool val)
    {
        isOpened = val;
    }

    public void OnClickCallback1()
    {
        if(isOpeneningProcessStarted)
        {
            return;
        }
        if(timeToOpenSeconds == 0 || isOpened)
        {
            OpenCard();
        }
        else
        {
            isOpeneningProcessStarted = true;
            delayTimer.SetActive(true);
            StartCoroutine(TimerBusinessLogic(timeToOpenSeconds));
        }
        
    }

    private void OpenCard()
    {
        isOpened = !isOpened;
        darkLogic.StartOpenCloseOpertation();
    }

    public IEnumerator TimerBusinessLogic(int setTime)
    {
        timeRemainToOpen = setTime;
        while(timeRemainToOpen >= 0)
        {
            if(timeRemainToOpen <= 3599)
            {
                textTime.text = $"{timeRemainToOpen / 60:00} : {timeRemainToOpen % 60:00}";
            }
            else
            {
                textTime.text = ">1hr";
            }
            circleBar.fillAmount = Mathf.InverseLerp(0, timeToOpenSeconds, timeRemainToOpen);
            timeRemainToOpen--;
            yield return new WaitForSeconds(1f);
        }

        delayTimer.SetActive(false);
        isOpeneningProcessStarted = false;
        OpenCard();

        yield break;
    }
}
