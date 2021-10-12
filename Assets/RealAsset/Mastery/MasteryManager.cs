using UnityEngine;
using UnityEngine.UI;
using FMODUnity;

public class MasteryManager : MonoBehaviour
{
    private static MasteryManager instance;
    public static MasteryManager Instance { get { return instance; } }
    [SerializeField] private MasteryInventory MasteryInventory;
    [SerializeField] private WakduMasteryDataBuffer WakduMasteryDataBuffer;
    public GameObject selectMasteryPanel;
    [SerializeField] private Image[] buttonImages;
    [SerializeField] private ToolTipTrigger[] toolTipTriggers;
    private int selectMasteryStack = 0;
    private Mastery[] randomMasteries = new Mastery[3];
    [SerializeField] private GameEvent MasterySelect;

    private void Awake()
    {
        instance = this;
    }

    private void Initialize()
    {
        for (int i = 0; i < 3; i++)
        {
            randomMasteries[i] = WakduMasteryDataBuffer.Items[Random.Range(0, WakduMasteryDataBuffer.Items.Length)];
            buttonImages[i].sprite = randomMasteries[i].sprite;
            toolTipTriggers[i].SetToolTip(randomMasteries[i]);
        }
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
        RuntimeManager.PlayOneShot("event:/SFX/UI/Test", transform.position);
        ToolTipManager.Hide();
        selectMasteryStack--;

        MasteryInventory.Add(randomMasteries[i]);
        randomMasteries[i].OnEquip();
        MasterySelect.Raise();

        if (selectMasteryStack > 0)
        {
            Initialize();
            selectMasteryPanel.SetActive(true);
        }
        else selectMasteryPanel.SetActive(false);
    }
}
