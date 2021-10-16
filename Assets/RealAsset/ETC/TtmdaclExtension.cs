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

    public static IEnumerator ChangeWithDelay(bool changeValue, float coolTime, Action<bool> makeResult, Image image)
    {
        image.fillAmount = 1f;
        float curCoolTime = coolTime;     
        do
        {
            image.fillAmount = curCoolTime / coolTime;
            yield return null;
        } while ((curCoolTime -= Time.deltaTime) > 0);
        image.fillAmount = 0f;
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
