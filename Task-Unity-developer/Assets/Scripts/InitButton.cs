using UnityEngine;
using UnityEngine.UI;
using System;

public class InitButton : MonoBehaviour
{
    public Slider slider;
    public Text textInfo;
    public ScrollLogic scrollLogic;

    private int mode = 0; // 0 - move to open card, 1 - move to close card

    public int GetMode()
    {
        return mode;
    }

    public void LoadPref()
    {
        Preferences pref = Util.LoadFromJson<Preferences>(Application.persistentDataPath + "/preferences.json");

        /// for tests
        if(pref == null)
        {
            pref = JsonUtility.FromJson<Preferences>(Resources.Load<TextAsset>("preferences").text);
            Debug.Log("You use test preferences");
        }
        ///

        if(pref != null)
        {
            mode = pref.mode;
            slider.value = mode;
            ChangeTextInfo();

            double sec = TimeSpan.FromTicks(DateTime.UtcNow.Ticks - pref.lastTime).TotalSeconds;
            Util.SetTimePassed((long)sec);
        }
        else
        {
            Util.SetTimePassed(0);
        }
    }

    public void OnClickCallback()
    {
        slider.value = slider.value == 0 ? 1 : 0;
        mode = (int)slider.value;
        ChangeTextInfo();
    }

    public void OnSlideCallback()
    {
        mode = (int)slider.value;
        ChangeTextInfo();
    }

    private void ChangeTextInfo()
    {
        switch(mode)
        {
            case 0:
                textInfo.text = "when you open the app next time it will smoothly move gallery to the <b>first opened card</b>";
                break;
            case 1:
                textInfo.text = "when you open the app next time it will smoothly move gallery to the <b>first closed card</b>";
                break;
            default:
                Debug.LogError("Caboom");
                break;
        }
    }
    public void SavePref()
    {
        Preferences pref = new Preferences();
        pref.mode = mode;
        pref.lastTime = DateTime.UtcNow.Ticks;
        Util.ToJsonAndCreateFile(pref, Application.persistentDataPath + "/preferences.json");
    }

    [Serializable]
    public class Preferences
    {
        public int mode;
        public long lastTime;
    }
}
