using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CollectionLog : MonoBehaviour
{
    [SerializeField] private GameObject collectionLogItemPrefab = null;
    [SerializeField] private GameObject collectionLogContent = null;
    private List<CollectionLogItem> collectionLogItems = new List<CollectionLogItem>();

    private void Start()
    {
        PopulateList();
    }

    public void PopulateList()
    {
        foreach (Transform child in collectionLogContent.transform)
            Destroy(child.gameObject);

        collectionLogItems.Clear();

        var sortedGems = GameManager.Instance
            .GetGemSOs()
            .OrderBy(gem => gem.rarityLevel);

        var unlockedCollection = Inventory.Instance.GetCollection();

        foreach (var gem in sortedGems)
        {
            GameObject newLogItem =
                Instantiate(collectionLogItemPrefab, collectionLogContent.transform);

            CollectionLogItem logItem =
                newLogItem.GetComponent<CollectionLogItem>();

            GemSO unlockedVersion = unlockedCollection
                .FirstOrDefault(g => g.id == gem.id);

            if (unlockedVersion != null)
            {
                logItem.gem = unlockedVersion;
                logItem.Unlock(true);
            }
            else
            {
                logItem.gem = gem;
                logItem.Unlock(false);
            }

            collectionLogItems.Add(logItem);
        }
    }


}
