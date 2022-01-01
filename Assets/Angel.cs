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
    [SerializeField] private TextMeshProUGUI powerText;
    [SerializeField] private TextMeshProUGUI hpText;
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

        treeImage.sprite = treeSprites[Mathf.Clamp((DataManager.Instance.CurGameData.deathCount - 1) / 2, 0, 3)];
        treeSpriteRenderer.sprite = treeSprites[Mathf.Clamp((DataManager.Instance.CurGameData.deathCount - 1) / 2, 0, 3)];
        shadowSpriteRenderer.sprite = shadowSprites[Mathf.Clamp((DataManager.Instance.CurGameData.deathCount - 1) / 2, 0, 3)];

        powerText.text = $"- ��, <color=#C6FF4C>����ġ</color> X 2 = {DataManager.Instance.CurGameData.deathCount * 2} �߰�";
        hpText.text = $"- ü��, <color=#C6FF4C>����ġ</color> X 1 = {DataManager.Instance.CurGameData.deathCount * 1} �߰�";

        maxhp.RuntimeValue += DataManager.Instance.CurGameData.deathCount * 2;
        power.RuntimeValue += DataManager.Instance.CurGameData.deathCount * 1;
    }
}
