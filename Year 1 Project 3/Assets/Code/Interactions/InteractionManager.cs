using System;
using UnityEngine;
using UnityEngine.Serialization;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class InteractionManager : MonoBehaviour
{
    public static InteractionManager Instance { get; private set; }
    public event Action<GameObject> OnInteraction;

    [FormerlySerializedAs("_boltSlider")]
    [SerializeField] private Slider boltSlider;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private GameObject promptUI;
    [SerializeField] private TextMeshProUGUI keyText;
    [SerializeField] private TextMeshProUGUI actionText;
    [SerializeField, Tooltip("Max distance at which you can interact")]
    private float interactDistance = 3.5f;
    [SerializeField] private KeyCode interactKey = KeyCode.E;

    private IInteractable _currentInteractable;
    private GameObject _currentInteractableObject;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (promptUI == null || playerCamera == null)
        {
            Debug.LogError("InteractionManager: Assign all references in the inspector!");
            enabled = false;
            return;
        }
        promptUI.SetActive(false);
        keyText.text = interactKey.ToString();
    }

    private void Update()
    {
        CheckForInteractable();
        if (_currentInteractable != null && Input.GetKeyDown(interactKey))
        {
            _currentInteractable.OnInteract();
            OnInteraction?.Invoke(_currentInteractableObject);
            promptUI.SetActive(false);
            _currentInteractable = null;
            _currentInteractableObject = null;
            boltSlider.value -= 10;
            if (boltSlider.value <= 0) SceneManager.LoadSceneAsync(3);
        }
    }

    private void CheckForInteractable()
    {
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance))
        {
            float actualDist = Vector3.Distance(playerCamera.transform.position, hit.collider.transform.position);
            if (actualDist <= interactDistance)
            {
                var invis = hit.collider.GetComponent<InvisibilityInteractable>();
                if (invis != null)
                {
                    _currentInteractable = invis;
                    _currentInteractableObject = hit.collider.gameObject;
                    promptUI.SetActive(true);
                    return;
                }
            }
        }

        if (_currentInteractable != null)
        {
            _currentInteractable = null;
            _currentInteractableObject = null;
            promptUI.SetActive(false);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (playerCamera != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(playerCamera.transform.position, interactDistance);
        }
    }
}
