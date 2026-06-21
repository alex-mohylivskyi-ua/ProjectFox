using UnityEngine;

public class MovingPlatformButton : MonoBehaviour, IInteractable
{
    public enum ButtonAction
    {
        Activate,
        Toggle,
        Stop
    }

    public enum ActivationType
    {
        OnTouch,
        OnInteract,
        OnTouchOrInteract
    }

    [Header("Target")]
    [SerializeField] private MovingPlatform targetPlatform;

    [Header("Activation")]
    [SerializeField] private LayerMask activatorLayers;
    [SerializeField] private ButtonAction buttonAction = ButtonAction.Activate;
    [SerializeField] private ActivationType activationType = ActivationType.OnTouch;
    [SerializeField] private bool activateOnlyOnce;

    private bool wasActivated;

    private void Awake()
    {
        if (targetPlatform == null)
        {
            targetPlatform = GetComponentInParent<MovingPlatform>();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!LayerIsActivator(other.gameObject.layer))
        {
            return;
        }

        if (activationType == ActivationType.OnTouch || activationType == ActivationType.OnTouchOrInteract)
        {
            TryUseButton();
        }
    }

    public void Interact(Player player)
    {
        if (activationType != ActivationType.OnInteract && activationType != ActivationType.OnTouchOrInteract)
        {
            return;
        }

        TryUseButton();
    }

    private void TryUseButton()
    {
        if (activateOnlyOnce && wasActivated)
        {
            return;
        }

        if (targetPlatform == null)
        {
            return;
        }

        UseButton();
        wasActivated = true;
    }

    private void UseButton()
    {
        switch (buttonAction)
        {
            case ButtonAction.Activate:
                targetPlatform.Activate();
                break;

            case ButtonAction.Toggle:
                targetPlatform.Toggle();
                break;

            case ButtonAction.Stop:
                targetPlatform.Stop();
                break;
        }
    }

    private bool LayerIsActivator(int layer)
    {
        return (activatorLayers.value & (1 << layer)) != 0;
    }
}