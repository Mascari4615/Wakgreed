using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NrcRoom : Room
{
    [SerializeField] private List<NPC> nrcs;

    private void Awake()
    {
        NPC random;
        do
        {
            random = nrcs[Random.Range(0, nrcs.Count)];
            nrcs.Remove(random);
        } while (DataManager.Instance.CurGameData.rescuedNPC[random.ID]);
        random.gameObject.SetActive(true);
    }
}
