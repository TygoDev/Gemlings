using UnityEngine;

[CreateAssetMenu(menuName = "Shop/Shop Item Data")]
public class ShopItemData : ScriptableObject
{
    [Header("General")]
    public StatType statType;
    public int maxLevel = 20;

    [Header("Cost Formula")]
    public int baseCost = 100;
    public float costMultiplier = 1.25f;

    [Header("Value Formula")]
    public int baseValue = 20;
    public int valuePerLevel = 5;
}
