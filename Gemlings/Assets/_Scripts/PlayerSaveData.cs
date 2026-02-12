using System;
using System.Collections.Generic;

[System.Serializable]
public class PlayerSaveData
{
    public float activeDamage;
    public float autoDamagePerSecond;
    public int money;

    public int activeDamageLevel;
    public int autoDamageLevel;

    public List<int> unlockedGemIndexes = new();
    public int selectedLingIndex;
    public List<GemSaveData> inventoryGems = new();
    public List<GemSaveData> collectionGems = new();
}

[Serializable]
public class GemSaveData
{
    public int baseGemID;
    public int adjective;
    public int trueValue;
    public float weight;
    public float durability;
}
