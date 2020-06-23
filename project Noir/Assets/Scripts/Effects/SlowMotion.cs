using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowMotion : MonoBehaviour
{
    private IEnumerator setTimeScaleAfterTime;

    public void SetTimeScale(float timeScale)
    {
        Time.timeScale = timeScale;
    }

    public void SetTimeScaleForTime(float timeScale, float time)
    {
        if (setTimeScaleAfterTime != null)
        {
            setTimeScaleAfterTime = SetTimeScaleAfterTime(Time.timeScale, time);
            StartCoroutine(setTimeScaleAfterTime);
            Time.timeScale = timeScale;
        }
    }

    private IEnumerator SetTimeScaleAfterTime(float timeScale, float time)
    {
        while (time > 0)
        {
            yield return null;
            time -= Time.deltaTime;
        }

        Time.timeScale = timeScale;
        setTimeScaleAfterTime = null;
    }
}
