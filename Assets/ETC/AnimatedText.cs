using UnityEngine;
using TMPro;

public enum TextType
{
    Normal,
    Critical,
    Heal
}

public class AnimatedText : MonoBehaviour
{
    private TextMeshPro textMesh;
    private const float MoveSpeed = 3;
    private float finalPositionX;
    private float finalPositionY;

    private void Awake()
    {
        textMesh = GetComponent<TextMeshPro>();
    }

    private void OnEnable()
    {
        transform.position += new Vector3(Random.Range(-0.5f, 0.6f), Random.Range(-0.5f, 0.6f), 0);
        
        finalPositionY = transform.position.y + 1.5f;
        finalPositionX = transform.position.x + 1f;
    }

    private void FixedUpdate()
    {
        transform.position = new Vector3(Mathf.Lerp(transform.position.x, finalPositionX, MoveSpeed * Time.deltaTime), Mathf.Lerp(transform.position.y, finalPositionY, MoveSpeed * Time.deltaTime * 2), -5);    
    }

    public void SetText(string text, TextType damageType)
    {
        textMesh.text = text;
        textMesh.color = damageType switch
        {
            TextType.Normal => Color.white,
            TextType.Critical => Color.yellow,
            TextType.Heal => Color.green,
            _ => textMesh.color
        };
    }
}
