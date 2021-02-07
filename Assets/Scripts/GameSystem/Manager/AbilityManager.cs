using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityManager : MonoBehaviour
{
    private static AbilityManager instance;
    public static AbilityManager Instance { get { return instance; } }

    public GameObject selectAbilityPanel;
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
                // TravellerController.Instance.ad += 1;
            break;
            case 1 :
                // TravellerController.Instance.criticalChance += 15;
            break;
            case 2 :
                // TravellerController.Instance.attackCoolDown = TravellerController.Instance.attackCoolDown * 0.9f;
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
