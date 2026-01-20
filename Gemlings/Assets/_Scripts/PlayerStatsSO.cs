using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New PlayerStats", menuName = "Stats/New PlayerStats")]
public class PlayerStatsSO : ScriptableObject
{
    [Header("Stats")]
    public float activeDamage;
    public float autoDamagePerSecond;
    public int money;
    public List<int> unlockedGemIndexes = new();
    public int selectedLingIndex = 0;
}
