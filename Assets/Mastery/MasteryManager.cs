using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Collections.Generic;
using FMODUnity;

public class MasteryManager : MonoBehaviour
{
    [SerializeField] private GameObject selectMasteryPanel;
    [SerializeField] private Image[] buttonImages;
    [SerializeField] private ToolTipTrigger[] toolTipTriggers;
    private int selectMasteryStack = 0;
    private readonly Mastery[] randomMasteries = new Mastery[3];

}
