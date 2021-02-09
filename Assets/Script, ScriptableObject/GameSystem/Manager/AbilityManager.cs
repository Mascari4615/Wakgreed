using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MasteryManager : MonoBehaviour
{
    private static MasteryManager instance;
    public static MasteryManager Instance { get { return instance; } }

    public GameObject selectMasteryPanel;
    [SerializeField] private Sprite[] buttonSprites;
    private AudioSource audioSource;
    private int selectMasteryStack = 0;
    [SerializeField] private KnightMasteryDataBuffer KnightMasteryDataBuffer;
    [SerializeField] private Mastery[] randomMasteries = new Mastery[3];

    private void Awake()
    {
        instance = this;
        audioSource = GetComponent<AudioSource>();
    }

    private void Initialize()
    {
        randomMasteries[0] = KnightMasteryDataBuffer.Items[Random.Range(0, KnightMasteryDataBuffer.Items.Length)];
        randomMasteries[1] = KnightMasteryDataBuffer.Items[Random.Range(0, KnightMasteryDataBuffer.Items.Length)];
        randomMasteries[2] = KnightMasteryDataBuffer.Items[Random.Range(0, KnightMasteryDataBuffer.Items.Length)];
        buttonSprites[0] = randomMasteries[0].sprite;
        buttonSprites[1] = randomMasteries[1].sprite;
        buttonSprites[2] = randomMasteries[2].sprite;
    }

    public void LevelUp()
    {
        selectMasteryStack++;

        Initialize();
        if (selectMasteryPanel.activeSelf == false) selectMasteryPanel.SetActive(true);
    }

    public void ChooseAbility(int i)
    {
        audioSource.Play();
        selectMasteryStack--;

        randomMasteries[i].OnEquip();

        if (selectMasteryStack > 0)
        {
            Initialize();
            selectMasteryPanel.SetActive(true);
        }
        else selectMasteryPanel.SetActive(false);
    }
}
