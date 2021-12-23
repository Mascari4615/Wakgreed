using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Angel : NPC
{
    [SerializeField] private SpriteRenderer treeSpriteRenderer;
    [SerializeField] private SpriteRenderer shadowSpriteRenderer;
    [SerializeField] private Sprite[] treeSprites;
    [SerializeField] private Sprite[] shadowSprites;
    [SerializeField] private IntVariable power;
    [SerializeField] private IntVariable hp;
    [SerializeField] private TextMeshProUGUI curGrowthText;
    [SerializeField] private Image[] curEffectStamps;
    private int bonusPower = 0;
    private int bonusHp = 0;

    private void OnEnable()
    {
        Debug.Log("OnEnable");
        UpdateTree();
    }

    private void UpdateTree()
    {
        hp.RuntimeValue -= bonusHp;
        power.RuntimeValue -= bonusPower;

        bonusHp = 0;
        bonusPower = 0;
        curGrowthText.text = $"<color=#C6FF4C>현재 성장치</color> - {DataManager.Instance.CurGameData.deathCount}";

        curEffectStamps[0].gameObject.SetActive(DataManager.Instance.CurGameData.deathCount >= 2);
        curEffectStamps[1].gameObject.SetActive(DataManager.Instance.CurGameData.deathCount >= 4);
        curEffectStamps[2].gameObject.SetActive(DataManager.Instance.CurGameData.deathCount >= 6);
        curEffectStamps[3].gameObject.SetActive(DataManager.Instance.CurGameData.deathCount >= 8);

        treeSpriteRenderer.sprite = treeSprites[(DataManager.Instance.CurGameData.deathCount - 1) / 2];
        shadowSpriteRenderer.sprite = shadowSprites[(DataManager.Instance.CurGameData.deathCount- 1) / 2];

        if (DataManager.Instance.CurGameData.deathCount >= 2)
        {
            bonusHp += 10;
        }

        if (DataManager.Instance.CurGameData.deathCount >= 4)
        {
            bonusPower += 10;
        }

        if (DataManager.Instance.CurGameData.deathCount >= 6)
        {
            bonusHp += 5;
            bonusPower += 5;
        }

        if (DataManager.Instance.CurGameData.deathCount >= 8)
        {
            bonusHp += 10;
            bonusPower += 10;
        }

        hp.RuntimeValue += bonusHp;
        power.RuntimeValue += bonusPower;
    }
}
