using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class LingManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> gemlings = new List<GameObject>();
    [SerializeField] private List<LingOption> lingOptions = new List<LingOption>();

    private List<int> unlockedIndexes = new List<int>();

    private void Start()
    {
        unlockedIndexes = PlayerStats.Instance.unlockedLingIndexes;

        for (int i = 0; i < lingOptions.Count; i++)
        {
            if (unlockedIndexes.Contains(lingOptions[i].LingIndex))
            {
                UnlockGemling(lingOptions[i]);
            }
        }

        var selectedIndex = PlayerStats.Instance.selectedLingIndex;
        if (selectedIndex >= 0 && selectedIndex < lingOptions.Count)
        {
            SelectGemling(lingOptions[selectedIndex]);
        }
        else if (lingOptions.Count > 0)
        {
            SelectGemling(lingOptions[0]);
        }
    }

    public void SelectGemling(LingOption selectedLing)
    {
        if (selectedLing == null) return;

        if (!selectedLing.unlocked)
        {
            if (!TryBuyGemling(selectedLing))
                return;
        }

        int index = selectedLing.LingIndex;
        if (index < 0 || index >= gemlings.Count) return;

        EnsurePersistedUnlocked(index);

        for (int i = 0; i < gemlings.Count; i++)
        {
            gemlings[i].SetActive(i == index);
        }

        var stats = PlayerStats.Instance;
        stats.selectedLingIndex = index;
        stats.SaveGame();
    }

    private bool TryBuyGemling(LingOption selectedLing)
    {
        if (selectedLing == null) return false;

        var stats = PlayerStats.Instance;
        if (stats.GetMoney() < selectedLing.cost) return false;

        stats.UpdateMoney(-selectedLing.cost);

        UnlockGemling(selectedLing);
        AddToUnlockedIndexes(selectedLing.LingIndex);

        return true;
    }

    private void UnlockGemling(LingOption lingOption)
    {
        if (lingOption == null) return;
        if (lingOption.unlocked) return;

        lingOption.unlocked = true;
        if (lingOption.costText != null)
            lingOption.costText.text = "unlocked!";
    }

    private void AddToUnlockedIndexes(int index)
    {
        var stats = PlayerStats.Instance;
        var saveData = stats.GetPlayerStats();

        if (!stats.unlockedLingIndexes.Contains(index))
            stats.unlockedLingIndexes.Add(index);

        if (!saveData.unlockedGemIndexes.Contains(index))
            saveData.unlockedGemIndexes.Add(index);

        stats.SaveGame();
    }

    private void EnsurePersistedUnlocked(int index)
    {
        var stats = PlayerStats.Instance;
        var saveData = stats.GetPlayerStats();

        bool changed = false;
        if (!stats.unlockedLingIndexes.Contains(index))
        {
            stats.unlockedLingIndexes.Add(index);
            changed = true;
        }

        if (!saveData.unlockedGemIndexes.Contains(index))
        {
            saveData.unlockedGemIndexes.Add(index);
            changed = true;
        }

        if (changed)
            stats.SaveGame();
    }
}
