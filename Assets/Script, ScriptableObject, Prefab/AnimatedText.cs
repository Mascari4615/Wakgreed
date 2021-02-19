using UnityEngine;
using TMPro;

public enum TextType
{
    Normal,
    Critical
}

public class AnimatedText : ObjectWithDuration
{
    private TextMeshPro textMesh = null;
    private float moveSpeed = 3;
    private float finalPositionX;
    private float finalPositionY;

    protected override void Awake()
    {
        base.Awake();
        textMesh = GetComponent<TextMeshPro>();
    }
    
    protected override void OnEnable()
    {
        // Debug.Log("OnEnable");
        base.OnEnable();
        transform.position += new Vector3(Random.Range(-0.5f, 0.6f), Random.Range(-0.5f, 0.6f), 0);
        
        finalPositionY = transform.position.y + 1.5f;
        finalPositionX = transform.position.x + 1f;
    }

    private void FixedUpdate()
    {
        transform.position = new Vector3(Mathf.Lerp(transform.position.x, finalPositionX, moveSpeed * Time.deltaTime), Mathf.Lerp(transform.position.y, finalPositionY, moveSpeed * Time.deltaTime * 2), -5);    
    }

    public void SetText(string text, TextType damageType)
    {
        textMesh.text = text;

        if (damageType.Equals(TextType.Normal))
        {
            textMesh.color = Color.white;
        }
        else if (damageType.Equals(TextType.Critical))
        {
            textMesh.color = Color.yellow;
        }
    }
}
