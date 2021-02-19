using UnityEngine;
using UnityEngine.UI;

public class NyangText : MonoBehaviour
{
    [SerializeField] private IntVariable Nyang;
    [SerializeField] private Text textField;

    private void Awake()
    {
        SetNyangText();
    }
    
    public void SetNyangText()
    {
        textField.text = Nyang.RuntimeValue.ToString();
    }
}
