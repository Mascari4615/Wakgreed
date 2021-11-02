using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Probability<T>
{
    private readonly List<ProbabilityElement> probabilityList = new();

    private class ProbabilityElement
    {
        public readonly T Target;
        public readonly float Probability;
 
        public ProbabilityElement(T target, float probability)
        {
            Target = target;
            Probability = probability;
        }
    }
 
    public void Add(T target, float probability)
    {
        probabilityList.Add(new ProbabilityElement(target, probability));
    }
 
    public T Get()
    {
        float totalProbability = probabilityList.Sum(t => t.Probability);

        float pick = Random.value * totalProbability;
        foreach (ProbabilityElement t in probabilityList)
        {
            if (pick < t.Probability)
                return t.Target;
            pick -= t.Probability;
        }
        
        /*float sum = 0;
        foreach (ProbabilityElement t in probabilityList)
        {
            sum += t.probability;
            if (pick >= sum)
                return t.target;
        }*/
        
        return default;
    }
}