using UnityEngine;
using UnityEngine.UI;

public class MasteryManager : MonoBehaviour
{
    [SerializeField] private MasteryInventory MasteryInventory;
    [SerializeField] private KnightMasteryDataBuffer KnightMasteryDataBuffer;
    [SerializeField] private GameEvent OnChooseMastery;
    public GameObject selectMasteryPanel;
    [SerializeField] private Image[] buttonImages;
    [SerializeField] private ToolTipTrigger[] toolTipTriggers;
    private AudioSource audioSource;
    private int selectMasteryStack = 0;
    
    private Mastery[] randomMasteries = new Mastery[3];

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Initialize()
    {
        randomMasteries[0] = KnightMasteryDataBuffer.Items[Random.Range(0, KnightMasteryDataBuffer.Items.Length)];
        randomMasteries[1] = KnightMasteryDataBuffer.Items[Random.Range(0, KnightMasteryDataBuffer.Items.Length)];
        randomMasteries[2] = KnightMasteryDataBuffer.Items[Random.Range(0, KnightMasteryDataBuffer.Items.Length)];
        buttonImages[0].sprite = randomMasteries[0].sprite;
        buttonImages[1].sprite = randomMasteries[1].sprite;
        buttonImages[2].sprite = randomMasteries[2].sprite;
        toolTipTriggers[0].SetText(randomMasteries[0].name, randomMasteries[0].description, randomMasteries[0].comment);
        toolTipTriggers[1].SetText(randomMasteries[1].name, randomMasteries[1].description, randomMasteries[1].comment);
        toolTipTriggers[2].SetText(randomMasteries[2].name, randomMasteries[2].description, randomMasteries[2].comment);
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
        audioSource.Play();
        ToolTipManager.Hide();
        selectMasteryStack--;

        MasteryInventory.Add(randomMasteries[i]);
        randomMasteries[i].OnEquip();
        OnChooseMastery.Raise();

        if (selectMasteryStack > 0)
        {
            Initialize();
            selectMasteryPanel.SetActive(true);
        }
        else selectMasteryPanel.SetActive(false);
    }
}
