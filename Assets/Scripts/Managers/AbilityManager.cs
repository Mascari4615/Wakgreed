using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityManager : MonoBehaviour
{
    private static AbilityManager instance;
    public static AbilityManager Instance { get { return instance; } }

    [SerializeField] private GameObject selectAbilityPanel;
    [SerializeField] private GameObject[] selectAbilityButton;
    [SerializeField] private AudioSource selectAbilityAudioSource;
    private int selectAbilityStack = 0;

    private void Awake()
    {
        instance = this;
    }

    public void LevelUp()
    {
        selectAbilityStack++;
        
        if (selectAbilityPanel.activeSelf == false)
        {
            selectAbilityPanel.SetActive(true);
        }
    }

    public void ChooseAbility(int i)
    {
        selectAbilityAudioSource.Play();
        selectAbilityStack--;

        switch (i)
        {
            case 0 :
                Traveller.Instance.ad += 2;
            break;
            case 1 :
                Traveller.Instance.criticalChance += 20;
            break;
            case 2 :
                Traveller.Instance.basicAttackCoolDown = Traveller.Instance.basicAttackCoolDown * 0.7f;
            break;
        }

        if (selectAbilityStack > 0)
        {
            selectAbilityPanel.SetActive(true);
        }
        else
        {
            selectAbilityPanel.SetActive(false);
        }  
    }
}
