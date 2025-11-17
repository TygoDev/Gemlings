using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Minigame : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float speed = 2.5f;
    [SerializeField] private float greenDamage = 20f;
    [SerializeField] private float yellowDamage = 5f;
    [SerializeField] private float redDamage = 10f;
    [SerializeField] private float activeDamage;
    [SerializeField] private float autoDamagePerSecond;
    [SerializeField] private float timerDuration = 10f;

    [Header("References")]
    public GemSO gem;
    [SerializeField] private Slider healthSlider;
    [SerializeField] private TMP_Text healthText;
    [SerializeField] private Slider timerSlider;
    [SerializeField] private InputActionReference clickAction;
    [SerializeField] private RectTransform greenZone;
    [SerializeField] private RectTransform yellowZone;
    [SerializeField] private RectTransform redZone;

    private Slider bounceSlider;
    private RectTransform handle;
    private float nodeHealth;
    private float timer;
    private float autoDamageTimer;
    private float t;

    private bool initialized;

    private void Awake()
    {
        bounceSlider = GetComponent<Slider>();
        handle = bounceSlider.handleRect;

        // CHANGE THIS: EVENT SYSTEM WHEN THE SHOP IS DONE TO UPDATE STATS
        PlayerStatsSO stats = PlayerStats.Instance.GetPlayerStats();

        activeDamage = stats.activeDamage;
        autoDamagePerSecond = stats.autoDamagePerSecond;
    }

    private void OnEnable()
    {
        // Subscribe input only once
        if (clickAction != null)
            clickAction.action.performed += OnAttack;

        UpdateDamageValues();
        ResetMinigame();
    }

    private void OnDisable()
    {
        if (clickAction != null)
            clickAction.action.performed -= OnAttack;
    }

    private void Update()
    {
        UpdateBounce();
        UpdateTimer();
        ApplyAutoDamage();
    }

    private void UpdateDamageValues()
    {
        redDamage = activeDamage / 2;
        yellowDamage = activeDamage / 4;
        greenDamage = activeDamage;
    }

    // --------------------------
    // Reset / Initialization
    // --------------------------
    private void ResetMinigame()
    {
        if (gem == null)
        {
            Debug.LogWarning("Minigame has no Gem assigned!");
            return;
        }

        t = 0f;
        nodeHealth = gem.durability;
        timer = timerDuration;

        healthSlider.maxValue = gem.durability;
        healthSlider.value = nodeHealth;
        UpdateHealthText();

        if (timerSlider != null)
        {
            timerSlider.maxValue = timerDuration;
            timerSlider.value = timerDuration;
        }

        initialized = true;
    }

    // --------------------------
    // Core Game Loop
    // --------------------------
    private void UpdateBounce()
    {
        if (!initialized || bounceSlider == null) return;

        t += Time.deltaTime * speed;
        bounceSlider.value = (Mathf.Sin(t) + 1f) / 2f;
    }

    private void UpdateTimer()
    {
        if (!initialized || timerSlider == null) return;

        timer -= Time.deltaTime;
        timerSlider.value = Mathf.Clamp(timer, 0f, timerDuration);

        if (timer <= 0f)
            EndMinigame(false);
    }

    private void ApplyAutoDamage()
    {
        if (!initialized) return;

        autoDamageTimer += Time.deltaTime;
        if (autoDamageTimer >= 1f)
        {
            autoDamageTimer = 0f;
            nodeHealth -= autoDamagePerSecond; // Apply automatic damage each second
            healthSlider.value = nodeHealth;
            UpdateHealthText();

            if (nodeHealth <= 0f)
                EndMinigame(true);
        }
    }


    private void EndMinigame(bool success)
    {
        if (success)
        {
            Inventory.Instance.AddGem(gem);
        }
        else
        {
            Debug.Log("Minigame Failed!");
        }

        gameObject.SetActive(false);
    }

    // --------------------------
    // Player Interaction
    // --------------------------
    private void OnAttack(InputAction.CallbackContext context)
    {
        if (!initialized || bounceSlider == null || handle == null) return;

        if (IsOverlapping(handle, greenZone))
            nodeHealth -= greenDamage;
        else if (IsOverlapping(handle, yellowZone))
            nodeHealth -= yellowDamage;
        else if (IsOverlapping(handle, redZone))
            nodeHealth += redDamage;

        healthSlider.value = nodeHealth;
        UpdateHealthText();

        if (nodeHealth <= 0f)
            EndMinigame(true);
    }

    // --------------------------
    // Utility
    // --------------------------
    private bool IsOverlapping(RectTransform a, RectTransform b)
    {
        if (a == null || b == null) return false;
        return GetWorldRect(a).Overlaps(GetWorldRect(b));
    }

    private Rect GetWorldRect(RectTransform rt)
    {
        Vector3[] corners = new Vector3[4];
        rt.GetWorldCorners(corners);
        return new Rect(corners[0], corners[2] - corners[0]);
    }

    private void UpdateHealthText()
    {
        if (healthText != null)
            healthText.text = $"{Mathf.Clamp(nodeHealth, 0, gem.durability)}/{gem.durability}";
    }
}
