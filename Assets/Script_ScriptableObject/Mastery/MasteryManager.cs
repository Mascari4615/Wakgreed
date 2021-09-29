using UnityEngine;
using UnityEngine.UI;
using FMODUnity;

public class MasteryManager : MonoBehaviour
{
    [SerializeField] private MasteryInventory MasteryInventory;
    [SerializeField] private KnightMasteryDataBuffer KnightMasteryDataBuffer;
    public GameObject selectMasteryPanel;
    [SerializeField] private Image[] buttonImages;
    [SerializeField] private ToolTipTrigger[] toolTipTriggers;
    private int selectMasteryStack = 0;
    private int selectedMasteryStack = 0;
    private Mastery[] randomMasteries = new Mastery[3];
    [SerializeField] private VerticalLayoutGroup masteryGrid;


    private void Update() 
    {
        if (Input.GetKey(KeyCode.X))   
        {
            masteryGrid.transform.parent.gameObject.SetActive(true);
        } 
        else
        {
            masteryGrid.transform.parent.gameObject.SetActive(false);
        }
    }
    private void Initialize()
    {
        randomMasteries[0] = KnightMasteryDataBuffer.Items[Random.Range(0, KnightMasteryDataBuffer.Items.Length)];
        randomMasteries[1] = KnightMasteryDataBuffer.Items[Random.Range(0, KnightMasteryDataBuffer.Items.Length)];
        randomMasteries[2] = KnightMasteryDataBuffer.Items[Random.Range(0, KnightMasteryDataBuffer.Items.Length)];
        buttonImages[0].sprite = randomMasteries[0].sprite;
        buttonImages[1].sprite = randomMasteries[1].sprite;
        buttonImages[2].sprite = randomMasteries[2].sprite;
        toolTipTriggers[0].SetToolTip(randomMasteries[0]);
        toolTipTriggers[1].SetToolTip(randomMasteries[1]);
        toolTipTriggers[2].SetToolTip(randomMasteries[2]);
    }

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
        RuntimeManager.PlayOneShot("event:/SFX/ETC/UI", transform.position);
        ToolTipManager.Hide();
        selectMasteryStack--;
        selectedMasteryStack++;

        MasteryInventory.Add(randomMasteries[i]);
        randomMasteries[i].OnEquip();

        if (selectMasteryStack > 0)
        {
            Initialize();
            selectMasteryPanel.SetActive(true);
        }
        else selectMasteryPanel.SetActive(false);

        GameObject temp = masteryGrid.transform.GetChild(selectedMasteryStack - 1).gameObject;
        temp.SetActive(true);
        temp.transform.GetChild(i).GetComponent<Slot>().SetSlot(randomMasteries[i]);
    }
}
