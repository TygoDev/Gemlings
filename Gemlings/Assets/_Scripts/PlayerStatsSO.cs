using UnityEngine;

[CreateAssetMenu(fileName = "New PlayerStats", menuName = "Stats/New PlayerStats")]
public class PlayerStatsSO : ScriptableObject
{
    [Header("Stats")]
    public float activeDamage;
    public float autoDamagePerSecond;
    public int money;
}
