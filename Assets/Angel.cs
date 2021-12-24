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
    [SerializeField] private MaxHp maxhp;
    [SerializeField] private TextMeshProUGUI curGrowthText;
    [SerializeField] private Image[] curEffectStamps;
    [SerializeField] private Image treeImage;
    private int bonusPower = 0;
    private int bonusHp = 0;

    private void OnEnable() => UpdateTree();

    private void UpdateTree()
    {
        maxhp.RuntimeValue -= bonusHp;
        power.RuntimeValue -= bonusPower;

        bonusHp = 0;
        bonusPower = 0;
        curGrowthText.text = $"<color=#C6FF4C>���� ����ġ</color> : {DataManager.Instance.CurGameData.deathCount}";

        curEffectStamps[0].gameObject.SetActive(DataManager.Instance.CurGameData.deathCount >= 2);
        curEffectStamps[1].gameObject.SetActive(DataManager.Instance.CurGameData.deathCount >= 4);
        curEffectStamps[2].gameObject.SetActive(DataManager.Instance.CurGameData.deathCount >= 6);
        curEffectStamps[3].gameObject.SetActive(DataManager.Instance.CurGameData.deathCount >= 8);

        treeImage.sprite = treeSprites[Mathf.Clamp((DataManager.Instance.CurGameData.deathCount - 1) / 2, 0, 3)];
        treeSpriteRenderer.sprite = treeSprites[Mathf.Clamp((DataManager.Instance.CurGameData.deathCount - 1) / 2, 0, 3)];
        shadowSpriteRenderer.sprite = shadowSprites[Mathf.Clamp((DataManager.Instance.CurGameData.deathCount - 1) / 2, 0, 3)];

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

        maxhp.RuntimeValue += bonusHp;
        power.RuntimeValue += bonusPower;
    }
}
