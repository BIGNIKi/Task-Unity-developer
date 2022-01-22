using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DarkLogic : MonoBehaviour
{
    public CanvasGroup canvas;

    private int isShouldOpen = 1; // 1 - will open, -1 - will close
    private int numProcesses = 0;

    public void ChangeOpenState()
    {
        isShouldOpen = isShouldOpen == -1 ? 1 : -1;
    }

    public void OnClickCallback()
    {
        ChangeOpenState();
        if(numProcesses == 0)
        {
            numProcesses++;
            StartCoroutine(OpeningCloseDark());
        }
    }

    private IEnumerator OpeningCloseDark()
    {
        float time = canvas.alpha;
        do
        {
            time += Time.deltaTime * isShouldOpen;
            canvas.alpha = time;
            yield return null;
        } while(canvas.alpha > 0 && canvas.alpha < 1);
        numProcesses--;
        yield break;
    }
}
