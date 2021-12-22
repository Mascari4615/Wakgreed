using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BanggalTree : InteractiveObject
{
    private SpriteRenderer treeSpriteRenderer;
    private SpriteRenderer shadowSpriteRenderer;
    [SerializeField] private Sprite[] treeSprites;
    [SerializeField] private Sprite[] shadowSprites;
    [SerializeField] private IntVariable power;
    private int bonusPower = 0;
    [SerializeField] private GameObject banggalTreePanel;
    [SerializeField] private BoolVariable isShowingSomething;

    private void Awake()
    {
        treeSpriteRenderer = GetComponent<SpriteRenderer>();
        shadowSpriteRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
        power.RuntimeValue += bonusPower = Mathf.Clamp(DataManager.Instance.CurGameData.deathCount / 2, 0, 4);
    }

    private void OnEnable()
    {
        power.RuntimeValue -= bonusPower;

        bonusPower = Mathf.Clamp(DataManager.Instance.CurGameData.deathCount / 2, 0, 4);
        treeSpriteRenderer.sprite = treeSprites[bonusPower];
        shadowSpriteRenderer.sprite = shadowSprites[bonusPower];

        power.RuntimeValue += bonusPower;
    }

    public override void Interaction()
    {
        isShowingSomething.RuntimeValue = true;
        banggalTreePanel.SetActive(true);
    }
}
