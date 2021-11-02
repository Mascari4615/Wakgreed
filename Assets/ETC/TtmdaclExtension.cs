using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public static class TtmdaclExtension
{
    // 참고 : https://velog.io/@sonohoshi/10.-Unity에서-일정-시간-이후-값을-바꾸는-방법

    public static IEnumerator ChangeWithDelay(bool changeValue, float delay, Action<bool> makeResult)
    {
        yield return new WaitForSeconds(delay);
        makeResult(changeValue);
    }

    public static IEnumerator ChangeWithDelay(this float origin, float coolTime, Action<float> makeResult)
    {
        float now = 0f;
        while (now < coolTime)
        {
            now += Time.deltaTime;
            makeResult(coolTime - now);
            yield return null;
        }
    }
}
