using UnityEngine;

public class DashUI : MonoBehaviour
{
    [SerializeField] private IntVariable curDashStack;
    [SerializeField] private IntVariable maxDashStack;
    private Transform[] dashStackUIs;

    private void Awake() 
    {
        dashStackUIs = new Transform[transform.childCount];

        for (int i = 0; i < transform.childCount; i++)
        {
            dashStackUIs[i] = transform.GetChild(i);
        }

        UpdateCurDashStackUI();
        UpdateMaxDashStackUI();
    }

    public void UpdateCurDashStackUI()
    {
        for (int i = 0; i < maxDashStack.RuntimeValue; i++)
        {
            dashStackUIs[i].GetChild(1).gameObject.SetActive(i < curDashStack.RuntimeValue);
        }
    }

    public void UpdateMaxDashStackUI()
    {
        for (int i = 0; i < dashStackUIs.Length; i++)
        {
            dashStackUIs[i].gameObject.SetActive(i < maxDashStack.RuntimeValue);
        }
    }
}
