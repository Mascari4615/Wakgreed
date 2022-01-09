using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetSlot : MonoBehaviour
{
    [SerializeField] private SpecialThing specialThing;

    private void Awake()
    {
        GetComponent<Slot>().SetSlot(specialThing);
    }
}
