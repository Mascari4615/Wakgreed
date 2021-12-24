using UnityEngine;
using TMPro;

public class GoodsUI<T> : MonoBehaviour
{
    [SerializeField] CustomVariable<T> Variable;
    [SerializeField] TextMeshProUGUI text;

    private void OnEnable()
    {
        UpdateText();
    }

    public void UpdateText()
    {
        text.text = Variable.RuntimeValue.ToString();
    }
}
