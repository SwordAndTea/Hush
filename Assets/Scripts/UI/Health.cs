using UnityEngine;
using UnityEngine.UIElements;

public class Health : MonoBehaviour
{
    [SerializeField] private int health = 5;

    private int currentHealth;
    private VisualElement healthContainer;

    private void Start()
    {
        UIDocument uiDocument = GetComponent<UIDocument>();
        if (uiDocument == null)
        {
            Debug.LogError("Health requires a UIDocument component.");
            return;
        }

        healthContainer = uiDocument.rootVisualElement.Q<VisualElement>("HealthContainer");
        if (healthContainer == null)
        {
            Debug.LogError("HealthContainer was not found in Health.uxml.");
            return;
        }

        currentHealth = Mathf.Max(0, health);
        DrawHealthIcons();
    }

    public void UpdateHealth(int newHealth)
    {
        Debug.Log("Updating health: " + newHealth);
        currentHealth = Mathf.Max(0, newHealth);
        if (healthContainer == null)
            return;
        DrawHealthIcons();
    }

    private void DrawHealthIcons()
    {
        Debug.Log("Drawing health icons: " + currentHealth);
        if (healthContainer == null)
            return;

        healthContainer.Clear();

        for (int i = 0; i < currentHealth; i++)
        {
            VisualElement healthIcon = new VisualElement();
            healthIcon.AddToClassList("health-icon");
            healthContainer.Add(healthIcon);
        }
    }
}
