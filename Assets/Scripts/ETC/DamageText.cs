using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamageText : MonoBehaviour
{
    [SerializeField] private TextMeshPro textMesh = null;
    [HideInInspector] public string text = "ERROR";
    [HideInInspector] public string type = "";
    [SerializeField] private float speed = 0;
    private float finalPositionX = 0f;
    private float finalPositionY = 0f;
    private float t = 0f;

    void OnEnable()
    {
        transform.position += new Vector3(Random.Range(-0.5f, 0.6f), Random.Range(-0.5f, 0.6f), 0);
        textMesh.text = text;
        finalPositionY = transform.position.y + 1.5f;
        finalPositionX = transform.position.x + 1f;

        switch (type)
        {
            case "" :
                textMesh.color = Color.white;
            break;

            case "Critical" :
                textMesh.color = Color.yellow;
            break;            
            
            default:

            break;
        }
    }

    void Update()
    {
        transform.position = new Vector3(Mathf.Lerp(transform.position.x, finalPositionX, speed * Time.deltaTime), Mathf.Lerp(transform.position.y, finalPositionY, speed * Time.deltaTime * 2), -5);

        t += Time.deltaTime;
        if (t >= 1.4f)
        {
            ObjectManager.Instance.InsertQueue(PoolType.AnimatedText, gameObject);
            t = 0;
        }       
    }
}
