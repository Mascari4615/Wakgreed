using UnityEngine;
using TMPro;

public class GoodsUI : MonoBehaviour
{
    [SerializeField] bool isMonsterKill = false;
    [SerializeField] IntVariable IntVariable;
    [SerializeField] FloatVariable floatVariable;
    [SerializeField] TextMeshProUGUI text;

    public void UpdateText()
    {
        if (isMonsterKill)
        {
            text.text = (++IntVariable.RuntimeValue).ToString();
        }
        else
        {
            Debug.Log("asd");
            if (IntVariable != null) text.text = IntVariable.RuntimeValue.ToString();
            else text.text = floatVariable.RuntimeValue.ToString();
        }
    }
}
