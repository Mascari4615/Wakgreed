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
                //ad += 10;
            break;
            case 1 :
                //ad += 10;
            break;
            case 2 :
                //ad += 10;
            break;
            default :
                Debug.Log("???");
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
