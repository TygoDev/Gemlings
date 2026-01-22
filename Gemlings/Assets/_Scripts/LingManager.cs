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
                // Only update UI/state, persistent lists are already populated from PlayerStats
                UnlockGemling(lingOptions[i]);
            }
        }

        // Ensure selected index is valid before selecting
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

        // If locked, attempt purchase; abort if purchase failed
        if (!selectedLing.unlocked)
        {
            if (!TryBuyGemling(selectedLing))
                return;
        }

        // Defensive bounds check for gemlings list
        int index = selectedLing.LingIndex;
        if (index < 0 || index >= gemlings.Count) return;

        // Ensure persistent state contains this unlocked index (covers any edge cases)
        EnsurePersistedUnlocked(index);

        // Activate the chosen gemling and deactivate others
        for (int i = 0; i < gemlings.Count; i++)
        {
            gemlings[i].SetActive(i == index);
        }

        var stats = PlayerStats.Instance;
        stats.selectedLingIndex = index;
        stats.SaveGame();
    }

    // Attempts to buy; returns true when unlocked (either bought now or already unlocked)
    private bool TryBuyGemling(LingOption selectedLing)
    {
        if (selectedLing == null) return false;

        var stats = PlayerStats.Instance;
        if (stats.GetMoney() < selectedLing.cost) return false;

        stats.UpdateMoney(-selectedLing.cost);

        // Update UI/state and persist the unlock
        UnlockGemling(selectedLing);
        AddToUnlockedIndexes(selectedLing.LingIndex);

        return true;
    }

    // Update UI/state for an unlocked LingOption (does not persist PlayerStats)
    private void UnlockGemling(LingOption lingOption)
    {
        if (lingOption == null) return;
        if (lingOption.unlocked) return;

        lingOption.unlocked = true;
        if (lingOption.costText != null)
            lingOption.costText.text = "unlocked!";
    }

    // Persist unlocked index to PlayerStats and save once
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

    // Ensure persistence matches the in-memory unlocked flag
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
