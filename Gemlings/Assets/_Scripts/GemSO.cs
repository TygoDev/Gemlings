using UnityEngine;

[CreateAssetMenu(fileName = "New Gem", menuName = "Gems/New Gem")]
public class GemSO : ScriptableObject
{
    [Header("Static Info")]
    public int id;
    public string itemName;
    public Sprite icon;
    public Rarity rarityLevel;
    public int baseValue;

    [Header("Instance Info")]
    public Adjective adjective;
    public int trueValue;
    public float weight;
    public float durability;
}

public enum Adjective
{
    Dull = 50,
    None = 100,
    Shiny = 150,   
    Radiant = 160,
    Brilliant = 170,
    Pristine = 180,
    Flawless = 190,
    Lustrous = 250
}

public enum Rarity
{
    Abundant = 50,
    Uncommon = 40,
    Valuable = 30,
    Precious = 10,
    Mythic = 1
}
