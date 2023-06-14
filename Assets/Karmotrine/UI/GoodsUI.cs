using TMPro;
using UnityEngine;

public class GoodsUI<T> : MonoBehaviour
{
    [SerializeField] private CustomVariable<T> Variable;
    [SerializeField] private TextMeshProUGUI text;

    private void OnEnable()
    {
        UpdateText();
    }

    public void UpdateText()
    {
        text.text = Variable.RuntimeValue.ToString();
    }
}