using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Collections.Generic;
using FMODUnity;

public class MasteryManager : MonoBehaviour
{
    [SerializeField] private MasteryInventory MasteryInventory;
    [SerializeField] private GameObject selectMasteryPanel;
    [SerializeField] private Image[] buttonImages;
    [SerializeField] private ToolTipTrigger[] toolTipTriggers;
    private int selectMasteryStack = 0;
    private readonly Mastery[] randomMasteries = new Mastery[3];

    private void Initialize()
    {
        List<Mastery> temp = DataManager.Instance.wdMasteryBuffer.items.ToList();
        for (int i = 0; i < 3; i++)
        {
            int random = Random.Range(0, temp.Count);
            randomMasteries[i] = temp[random];
            buttonImages[i].sprite = randomMasteries[i].sprite;
            toolTipTriggers[i].SetToolTip(randomMasteries[i]);
            temp.RemoveAt(random);
        }
    }

    public void SetSelectMasteryPanelOff() => selectMasteryPanel.SetActive(false);

    public void LevelUp()
    {
        selectMasteryStack++;

        if (selectMasteryPanel.activeSelf == false)
        {
            Initialize();
            selectMasteryPanel.SetActive(true);
        }
    }

    public void ChooseAbility(int i)
    {
        RuntimeManager.PlayOneShot("event:/SFX/UI/Test", transform.position);
        ToolTipManager.Instance.Hide();
        selectMasteryStack--;

        MasteryInventory.Add(randomMasteries[i]);

        if (selectMasteryStack > 0)
        {
            Initialize();
            selectMasteryPanel.SetActive(true);
        }
        else selectMasteryPanel.SetActive(false);
    }
}
