using UnityEngine;

public class DashUI : MonoBehaviour
{
    [SerializeField] private IntVariable curDashStack;
    [SerializeField] private GameObject[] dashStackUIs;

    public void UpdateUI()
    {
        for (int i = 0; i < dashStackUIs.Length; i++)
        {
            if (i < curDashStack.RuntimeValue) dashStackUIs[i].transform.GetChild(0).gameObject.SetActive(true);
            else dashStackUIs[i].transform.GetChild(0).gameObject.SetActive(false);
        }
    }
}
