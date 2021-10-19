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
            if (i < curDashStack.RuntimeValue) dashStackUIs[i].GetChild(0).gameObject.SetActive(true);
            else dashStackUIs[i].GetChild(0).gameObject.SetActive(false);
        }
    }

    public void UpdateMaxDashStackUI()
    {
        for (int i = 0; i < dashStackUIs.Length; i++)
        {
            if (i < maxDashStack.RuntimeValue) dashStackUIs[i].gameObject.SetActive(true);
            else dashStackUIs[i].gameObject.SetActive(false);
        }
    }
}
