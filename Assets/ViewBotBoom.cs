using UnityEngine;

public class ViewBotBoom : MonoBehaviour
{
    [SerializeField] private bool isDrop = false;
    [SerializeField] private IntVariable viewer;
    [SerializeField] private int amount;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            viewer.RuntimeValue += amount;

            if (isDrop)
            {
                ObjectManager.Instance.PopObject("AnimatedText", transform.position + Vector3.up).GetComponent<AnimatedText>().SetText($"½ÃÃ»ÀÚ {amount}", Color.white);
            }
        }
    }
}
