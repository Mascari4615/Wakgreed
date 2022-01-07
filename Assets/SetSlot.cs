using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetSlot : MonoBehaviour
{
    [SerializeField] private Mastery Mastery;

    private void Awake()
    {
        GetComponent<Slot>().SetSlot(Mastery);
    }
}
