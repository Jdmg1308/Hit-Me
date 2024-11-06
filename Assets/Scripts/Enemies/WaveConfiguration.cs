using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WaveConfiguration", menuName = "ScriptableObjects/WaveConfiguration", order = 1)]
public class WaveConfiguration : ScriptableObject
{
    public List<EnemyComposition> waveConfigurations = new List<EnemyComposition>();
}

[System.Serializable]
public class EnemyComposition
{
    public int BasicCount;
    public int RangedCount;
    public int HeavyCount;

    // Calculated sum based on counts
    public int Sum => BasicCount + RangedCount + HeavyCount;
}
