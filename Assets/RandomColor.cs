using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomColor : MonoBehaviour
{
    [SerializeField] private SpriteRenderer SpriteRenderer;

    private void OnEnable()
    {
        SpriteRenderer.color = Random.ColorHSV();
    }
}
