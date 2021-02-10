using UnityEngine;
using UnityEngine.UI;

public class NyangText : MonoBehaviour
{
    [SerializeField] private IntVariable Nyang;
    [SerializeField] private Text textField;

    public void SetNyangText()
    {
        textField.text = Nyang.RuntimeValue.ToString();
    }
}
