using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Player))]
public class PlayerInteraction : MonoBehaviour
{
    [Header("Detection")]
    [SerializeField] private LayerMask interactableLayers;

    private readonly List<IInteractable> interactablesInRange = new List<IInteractable>();
    private Player player;

    private void Awake()
    {
        player = GetComponent<Player>();
    }

    public void HandleInteraction()
    {
        if (player == null || player.inputReader == null)
        {
            return;
        }

        if (!player.inputReader.interactPressed)
        {
            return;
        }

        TryInteract();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!LayerIsInteractable(other.gameObject.layer))
        {
            return;
        }

        IInteractable interactable = other.GetComponentInParent<IInteractable>();

        if (interactable == null)
        {
            interactable = other.GetComponent<IInteractable>();
        }

        if (interactable == null)
        {
            return;
        }

        if (!interactablesInRange.Contains(interactable))
        {
            interactablesInRange.Add(interactable);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!LayerIsInteractable(other.gameObject.layer))
        {
            return;
        }

        IInteractable interactable = other.GetComponentInParent<IInteractable>();

        if (interactable == null)
        {
            interactable = other.GetComponent<IInteractable>();
        }

        if (interactable == null)
        {
            return;
        }

        interactablesInRange.Remove(interactable);
    }

    private void TryInteract()
    {
        RemoveMissingInteractables();

        if (interactablesInRange.Count == 0)
        {
            return;
        }

        IInteractable interactable = interactablesInRange[interactablesInRange.Count - 1];
        interactable.Interact(player);
    }

    private void RemoveMissingInteractables()
    {
        for (int i = interactablesInRange.Count - 1; i >= 0; i--)
        {
            if (interactablesInRange[i] == null)
            {
                interactablesInRange.RemoveAt(i);
            }
        }
    }

    private bool LayerIsInteractable(int layer)
    {
        return (interactableLayers.value & (1 << layer)) != 0;
    }
}