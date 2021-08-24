using UnityEngine;

[CreateAssetMenu(fileName = "TotalAD", menuName = "Variable/TotalAD")]
public class TotalAD : ScriptableObject
{
    [SerializeField] private IntVariable T_AD;
    [SerializeField] private IntVariable I_AD;
    [SerializeField] private IntVariable I_ADper;
    [SerializeField] private IntVariable M_AD;
    [SerializeField] private IntVariable M_ADper;

    public int GetTotalDamage()
    {
        return (T_AD.RuntimeValue + I_AD.RuntimeValue + M_AD.RuntimeValue)
         * ((100 + I_ADper.RuntimeValue + M_ADper.RuntimeValue) / 100); 
    }
}