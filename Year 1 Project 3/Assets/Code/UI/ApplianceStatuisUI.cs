using System;
using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Linq;

public class ApplianceStatusUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI statusText;
    private List<Appliance> _appliances = new List<Appliance>();
    private InteractionManager _interactionManager;

    [Obsolete("Obsolete")]
    private void Start()
    {
        _interactionManager = InteractionManager.Instance;
        if (_interactionManager != null) _interactionManager.OnInteraction += HandleInteraction;
        _appliances.AddRange(FindObjectsOfType<Appliance>());
        UpdateStatusText();
    }

    private void OnDestroy()
    {
        if (_interactionManager != null) _interactionManager.OnInteraction -= HandleInteraction;
    }

    private void HandleInteraction(GameObject obj)
    {
        var appliance = obj.GetComponent<Appliance>();
        if (appliance != null)
        {
            appliance.Toggle();
            UpdateStatusText();
        }
    }

    private void UpdateStatusText()
    {
        int totalCount = _appliances.Count;
        int onCount = _appliances.Count(app => app.IsOn);
        int offCount = totalCount - onCount;
        statusText.text = $"Go turn off all appliances {offCount}/{totalCount}";
    }
}