using UnityEngine;
using TMPro;

public enum DamageType
{
    Normal,
    Critical
}

public class DamageText : ObjectWithDuration
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
        Debug.Log("Onenable");
        base.OnEnable();
        transform.position += new Vector3(Random.Range(-0.5f, 0.6f), Random.Range(-0.5f, 0.6f), 0);
        
        finalPositionY = transform.position.y + 1.5f;
        finalPositionX = transform.position.x + 1f;
    }

    private void FixedUpdate()
    {
        transform.position = new Vector3(Mathf.Lerp(transform.position.x, finalPositionX, moveSpeed * Time.deltaTime), Mathf.Lerp(transform.position.y, finalPositionY, moveSpeed * Time.deltaTime * 2), -5);    
    }

    public void SetText(string text, DamageType damageType)
    {
        textMesh.text = text;

        if (damageType.Equals(DamageType.Normal))
        {
            textMesh.color = Color.white;
        }
        else if (damageType.Equals(DamageType.Critical))
        {
            textMesh.color = Color.yellow;
        }
    }
}
