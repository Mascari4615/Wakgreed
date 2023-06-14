using UnityEngine;
using UnityEngine.UI;

public class MasteryManager : MonoBehaviour
{
    [SerializeField] private GameObject selectMasteryPanel;
    [SerializeField] private Image[] buttonImages;
    [SerializeField] private ToolTipTrigger[] toolTipTriggers;
    private readonly Mastery[] randomMasteries = new Mastery[3];
    private int selectMasteryStack = 0;
}