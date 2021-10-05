using System.Collections.Generic;
using UnityEngine;

public class Probability<T>
{
    private List<ProbabilityElement> probabilityList = new List<ProbabilityElement>();
 
    public class ProbabilityElement
    {
        public T target;
        public float probability;
 
        public ProbabilityElement(T target, float probability)
        {
            this.target = target;
            this.probability = probability;
        }
    }
 
    public void Add(T target, float probability)
    {
        probabilityList.Add(new ProbabilityElement(target, probability));
    }
 
    public T Get()
    {
        float totalProbability = 0;
        for (int i = 0; i < probabilityList.Count; i++)
            totalProbability += probabilityList[i].probability;
 
        float pick = Random.value * totalProbability;
        for (int i = 0; i < probabilityList.Count; i++)
        {
            if (pick < probabilityList[i].probability)
                return probabilityList[i].target;
            else
                pick -= probabilityList[i].probability;
        }
        /*
        float sum = 0;
        for (int i = 0; i < probabilityList.Count; i++)
        {
            sum += probabilityList[i].probability;
            if (pick >= sum)
                return probabilityList[i].target;
        }
        */      
        return default(T);
    }
}