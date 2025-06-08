using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemyPersonality
{
    public float aggressionWeight = 1f;
    public float coverWeight = 1f;
    public float clusteringPenalty = 1f;
    public float randomnessWeight = 1f;

    public EnemyPersonality()
    {

    }

    public EnemyPersonality(float aggression, float cover, float clustering, float randomness)
    {
        aggressionWeight = aggression;
        coverWeight = cover;
        clusteringPenalty = clustering;
        randomnessWeight = randomness;
    }
}