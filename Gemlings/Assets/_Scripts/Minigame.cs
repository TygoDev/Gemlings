using System.Collections;
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
    private PlayerStatsSO playerStats => PlayerStats.Instance.GetPlayerStats();

    private Slider bounceSlider;
    private RectTransform handle;
    private float nodeHealth;
    private float timer;
    private float autoDamageTimer;
    private float t;

    private bool initialized;

    private Vector2 greenOriginalPos;
    private Vector2 yellowOriginalPos;
    private Vector2 redOriginalPos;

    private void Awake()
    {
        bounceSlider = GetComponent<Slider>();
        handle = bounceSlider.handleRect;
    }

    private void OnEnable()
    {
        if (clickAction != null)
            clickAction.action.performed += OnAttack;

        // Save original zone positions
        greenOriginalPos = greenZone.anchoredPosition;
        yellowOriginalPos = yellowZone.anchoredPosition;
        redOriginalPos = redZone.anchoredPosition;

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
        redDamage = playerStats.activeDamage / 2;
        yellowDamage = playerStats.activeDamage / 4;
        greenDamage = playerStats.activeDamage;
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
            nodeHealth -= playerStats.autoDamagePerSecond; // Apply automatic damage each second
            healthSlider.value = nodeHealth;
            UpdateHealthText();

            if (nodeHealth <= 0f)
                EndMinigame(true);
        }
    }


    private void EndMinigame(bool success)
    {
        // Stop all shake coroutines immediately
        StopAllCoroutines();

        // Reset zone positions so nothing stays offset
        greenZone.anchoredPosition = greenOriginalPos;
        yellowZone.anchoredPosition = yellowOriginalPos;
        redZone.anchoredPosition = redOriginalPos;

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
        {
            nodeHealth -= greenDamage;
            StartCoroutine(ShakeZone(greenZone));
        }
        else if (IsOverlapping(handle, yellowZone))
        {
            nodeHealth -= yellowDamage;
            StartCoroutine(ShakeZone(yellowZone));
        }
        else if (IsOverlapping(handle, redZone))
        {
            nodeHealth += redDamage;
            StartCoroutine(ShakeZone(redZone));
        }

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

    private IEnumerator ShakeZone(RectTransform zone)
    {
        Vector2 basePos = zone.anchoredPosition;

        float duration = 0.2f;
        float strength = 2f;
        float t = 0f;

        while (t < duration && gameObject.activeInHierarchy)
        {
            t += Time.deltaTime;
            float offset = Random.Range(-strength, strength);
            zone.anchoredPosition = basePos + new Vector2(offset, offset);
            yield return null;
        }

        // Only restore if still active
        if (gameObject.activeInHierarchy)
            zone.anchoredPosition = basePos;
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
