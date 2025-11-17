using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("References")]
    [SerializeField] private GameObject minigameObject;
    [SerializeField] private List<GemSO> gemPool; // All possible gems

    [Header("Timing")]
    [SerializeField] private float minTriggerTime = 5f;
    [SerializeField] private float maxTriggerTime = 15f;

    private State currentState;
    private Coroutine triggerRoutine;

    private enum State
    {
        Idle,
        WaitingForMinigame,
        PlayingMinigame
    }

    private void Awake()
    {
        // Singleton setup
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        ChangeState(State.WaitingForMinigame);
    }

    // --------------------------
    // State Management
    // --------------------------
    private void ChangeState(State newState)
    {
        if (currentState == newState) return;

        currentState = newState;
        StopAllCoroutines();

        switch (currentState)
        {
            case State.WaitingForMinigame:
                triggerRoutine = StartCoroutine(RandomMinigameRoutine());
                break;

            case State.PlayingMinigame:
                StartMinigame();
                break;

            case State.Idle:
                // Reserved for future pause/menu logic
                break;
        }
    }

    // --------------------------
    // Minigame Logic
    // --------------------------
    private IEnumerator RandomMinigameRoutine()
    {
        float waitTime = Random.Range(minTriggerTime, maxTriggerTime);
        yield return new WaitForSeconds(waitTime);

        if (currentState == State.WaitingForMinigame)
            ChangeState(State.PlayingMinigame);
    }

    private void StartMinigame()
    {
        if (minigameObject == null)
        {
            Debug.LogWarning("Minigame reference missing in GameManager!");
            ChangeState(State.WaitingForMinigame);
            return;
        }

        // Pick and configure gem before starting
        GemSO randomGem = GenerateRandomGemInstance();
        if (randomGem == null)
        {
            Debug.LogWarning("No valid gem found in pool!");
            ChangeState(State.WaitingForMinigame);
            return;
        }

        // Assign gem to minigame
        Minigame minigame = minigameObject.GetComponent<Minigame>();
        minigame.gem = randomGem;

        // Activate and monitor
        minigameObject.SetActive(true);
        StartCoroutine(WaitForMinigameEnd(minigame));
    }

    private IEnumerator WaitForMinigameEnd(Minigame minigame)
    {
        while (minigame.gameObject.activeSelf)
        {
            yield return null;
        }

        ChangeState(State.WaitingForMinigame);
    }

    // --------------------------
    // Gem Generation
    // --------------------------
    private GemSO GenerateRandomGemInstance()
    {
        if (gemPool == null || gemPool.Count == 0)
            return null;

        // Weighted random pick by rarity
        GemSO chosen = GetWeightedRandomGem();

        // Create a *runtime instance* so we can modify stats without affecting the original asset
        GemSO instance = ScriptableObject.Instantiate(chosen);

        // Randomize weight (for example, 50%–200% of original)
        float weightMultiplier = Random.Range(0.5f, 2f);
        instance.weight *= weightMultiplier;

        // Calculate true value: base value * weight
        instance.trueValue = Mathf.RoundToInt(instance.baseValue * instance.weight);

        // Optional: adjust durability slightly for variation
        instance.durability = Mathf.RoundToInt(instance.durability * Random.Range(0.9f, 1.1f));

        return instance;
    }

    private GemSO GetWeightedRandomGem()
    {
        float totalWeight = 0f;
        foreach (var gem in gemPool)
        {
            totalWeight += Mathf.Max((float)gem.rarityLevel, 0.001f); // avoid 0 weights
        }

        float randomValue = Random.Range(0, totalWeight);
        float cumulative = 0f;

        foreach (var gem in gemPool)
        {
            cumulative += (float)gem.rarityLevel;
            if (randomValue <= cumulative)
                return gem;
        }

        // Fallback (should not happen)
        return gemPool[Random.Range(0, gemPool.Count)];
    }

    public GemSO GetGemByID(int id)
    {
        return gemPool.FirstOrDefault(g => g.id == id);
    }

    // --------------------------
    // Public API
    // --------------------------
    public void ForceTriggerMinigame()
    {
        if (currentState != State.PlayingMinigame)
            ChangeState(State.PlayingMinigame);
    }

    public void StopTriggering()
    {
        ChangeState(State.Idle);
    }
}
