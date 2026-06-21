using UnityEngine;

public class MovingPlatformButton : MonoBehaviour
{
    public enum ButtonAction
    {
        Activate,
        Toggle,
        Stop
    }

    [Header("Target")]
    [SerializeField] private MovingPlatform targetPlatform;

    [Header("Activation")]
    [SerializeField] private LayerMask activatorLayers;
    [SerializeField] private ButtonAction buttonAction = ButtonAction.Activate;
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
        if (activateOnlyOnce && wasActivated)
        {
            return;
        }

        if (!LayerIsActivator(other.gameObject.layer))
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