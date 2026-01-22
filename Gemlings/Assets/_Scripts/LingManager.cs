using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class LingManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> gemlings = new List<GameObject>();
    [SerializeField] private List <LingOption> lingOptions = new List<LingOption>();

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

        SelectGemling(lingOptions[PlayerStats.Instance.selectedLingIndex]);
    }

    public void SelectGemling(LingOption selectedLing)
    {
        if (!selectedLing.unlocked)
        {
            BuyGemling(selectedLing);
            return;
        }

        if (!PlayerStats.Instance.unlockedLingIndexes.Contains(selectedLing.LingIndex))
        {
            PlayerStats.Instance.unlockedLingIndexes.Add(selectedLing.LingIndex);
            PlayerStats.Instance.GetPlayerStats().unlockedGemIndexes.Add(selectedLing.LingIndex);
            PlayerStats.Instance.SaveGame();
        }
            

        foreach (GameObject gemling in gemlings)
        {
            gemling.SetActive(false);
        }

        gemlings[selectedLing.LingIndex].SetActive(true);
        PlayerStats.Instance.selectedLingIndex = selectedLing.LingIndex;
        PlayerStats.Instance.SaveGame();
    }

    public void BuyGemling(LingOption selectedLing)
    {
        if (PlayerStats.Instance.GetMoney() >= selectedLing.cost)
        {
            PlayerStats.Instance.UpdateMoney(-selectedLing.cost);
            UnlockGemling(selectedLing);
            SelectGemling(selectedLing);
        }
    }

    private void UnlockGemling(LingOption lingOption)
    {
        lingOption.unlocked = true;
        lingOption.costText.text = "unlocked!";
    }
}
