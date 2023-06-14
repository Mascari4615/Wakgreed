using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
    private int bonusHp;
    private int bonusPower = 0;

    protected override void Awake()
    {
        base.Awake();
        UpdateTree();
    }

    public void UpdateTree()
    {
        bonusHp = 0;
        // bonusPower = 0;
        curGrowthText.text = $"<color=#C6FF4C>현재 성장치</color> : {DataManager.Instance.CurGameData.deathCount}";

        curEffectStamps[0].gameObject.SetActive(DataManager.Instance.CurGameData.deathCount >= 3);
        curEffectStamps[1].gameObject.SetActive(DataManager.Instance.CurGameData.deathCount >= 6);
        curEffectStamps[2].gameObject.SetActive(DataManager.Instance.CurGameData.deathCount >= 9);
        curEffectStamps[3].gameObject.SetActive(DataManager.Instance.CurGameData.deathCount >= 12);

        treeImage.sprite = treeSprites[Mathf.Clamp((DataManager.Instance.CurGameData.deathCount - 1) / 2, 0, 3)];
        treeSpriteRenderer.sprite =
            treeSprites[Mathf.Clamp((DataManager.Instance.CurGameData.deathCount - 1) / 2, 0, 3)];
        shadowSpriteRenderer.sprite =
            shadowSprites[Mathf.Clamp((DataManager.Instance.CurGameData.deathCount - 1) / 2, 0, 3)];

        if (DataManager.Instance.CurGameData.deathCount >= 3)
        {
            bonusHp += 3;
        }

        if (DataManager.Instance.CurGameData.deathCount >= 6)
        {
            bonusHp += 3;
            // bonusPower += 3;
        }

        if (DataManager.Instance.CurGameData.deathCount >= 9)
        {
            bonusHp += 3;
            // bonusPower += 5;
        }

        if (DataManager.Instance.CurGameData.deathCount >= 12)
        {
            bonusHp += 3;
            // bonusPower += 10;
        }

        maxhp.RuntimeValue += bonusHp;
        // power.RuntimeValue += bonusPower;
    }
}