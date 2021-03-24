using UnityEngine;
using UnityEngine.UI;

public class MasteryManager : MonoBehaviour
{
    [SerializeField] private MasteryInventory MasteryInventory;
    [SerializeField] private KnightMasteryDataBuffer KnightMasteryDataBuffer;
    public GameObject selectMasteryPanel;
    [SerializeField] private Image[] buttonImages;
    [SerializeField] private ToolTipTrigger[] toolTipTriggers;
    private int selectMasteryStack = 0;
    [SerializeField] private AudioClip soundEffect;
    private Mastery[] randomMasteries = new Mastery[3];

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
        SoundManager.Instance.PlayAudioClip(soundEffect);
        ToolTipManager.Hide();
        selectMasteryStack--;

        MasteryInventory.Add(randomMasteries[i]);
        randomMasteries[i].OnEquip();

        if (selectMasteryStack > 0)
        {
            Initialize();
            selectMasteryPanel.SetActive(true);
        }
        else selectMasteryPanel.SetActive(false);
    }
}
