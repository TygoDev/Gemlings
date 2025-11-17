using System;
using System.Collections.Generic;

[System.Serializable]
public class PlayerSaveData
{
    public float activeDamagePerSecond;
    public float autoDamagePerSecond;
    public int money;

    public List<GemSaveData> inventoryGems = new();
}

[Serializable]
public class GemSaveData
{
    public int baseGemID;      // references GemSO.id
    public int adjective;
    public int trueValue;
    public float weight;
    public float durability;
}