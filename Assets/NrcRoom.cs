using System.Collections.Generic;
using UnityEngine;

public class NrcRoom : Room
{
    [SerializeField] private List<NPC> nrcs;

    public override void Enter()
    { 
        if (IsVisited == false)
        {
            NPC random = null;
            for (int i = 0; i < nrcs.Count; i++)
            {
                if (DataManager.Instance.CurGameData.rescuedNPC[nrcs[i].ID] == false)
                {
                    random = nrcs[i];
                    break;
                }
            }

            if (random == null)
            {
                Probability<string> probability = new();
                probability.Add("CommonChest", 70);
                probability.Add("UncommonChest", 35);
                probability.Add("LegendaryChest", 5);
                ObjectManager.Instance.PopObject(probability.Get(), transform.position);
            }
            else
            {
                random.gameObject.SetActive(true);
            }
        }

        base.Enter();
    }
}
