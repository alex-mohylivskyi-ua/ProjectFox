using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInputManager))]
public class PlayerInputSpawnHandler : MonoBehaviour
{
    [Header("Spawn Points")]
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private bool useHandlerPositionIfNoSpawnPoint = true;

    [Header("Camera")]
    [SerializeField] private CameraTargetGroup2D cameraTargetGroup;
    [SerializeField] private bool registerPlayersToCamera = true;

    [Header("Debug")]
    [SerializeField] private bool logPlayerJoin = false;

    private PlayerInputManager playerInputManager;
    private readonly List<Player> joinedPlayers = new List<Player>();

    public IReadOnlyList<Player> JoinedPlayers => joinedPlayers;

    private void Awake()
    {
        playerInputManager = GetComponent<PlayerInputManager>();

        if (registerPlayersToCamera && cameraTargetGroup == null)
        {
            Debug.LogWarning($"{nameof(PlayerInputSpawnHandler)} on {name} has no CameraTargetGroup2D reference.", this);
        }
    }

    private void OnEnable()
    {
        Debug.Log($"{nameof(PlayerInputSpawnHandler)} on {name} enabled.", this);
        if (playerInputManager == null)
        {
            playerInputManager = GetComponent<PlayerInputManager>();
        }

        playerInputManager.onPlayerJoined += HandlePlayerJoined;
        playerInputManager.onPlayerLeft += HandlePlayerLeft;
    }

    private void OnDisable()
    {
        if (playerInputManager == null)
        {
            return;
        }

        playerInputManager.onPlayerJoined -= HandlePlayerJoined;
        playerInputManager.onPlayerLeft -= HandlePlayerLeft;
    }
    
    // Support for SendMessage notification behavior
    private void OnPlayerJoined(PlayerInput playerInput)
    {
        Debug.Log($"Player joined: {playerInput.name}, device: {playerInput.devices}");
        HandlePlayerJoined(playerInput);
    }

    // Support for SendMessage notification behavior
    private void OnPlayerLeft(PlayerInput playerInput)
    {
        HandlePlayerLeft(playerInput);
    }

    private void HandlePlayerJoined(PlayerInput playerInput)
    {
        // Debug.Log($"Player joined: {playerInput.name}, device: {playerInput.devices}, time: {Time.time}");
        if (playerInput == null)
        {
            return;
        }

        Player player = playerInput.GetComponent<Player>();

        if (player == null)
        {
            Debug.LogWarning($"{nameof(PlayerInputSpawnHandler)} received PlayerInput without Player component.", playerInput);
            return;
        }

        if (joinedPlayers.Contains(player))
        {
            return;
        }
        
        int playerIndex = joinedPlayers.Count;

        MovePlayerToSpawnPoint(player.transform, playerIndex);
        joinedPlayers.Add(player);
        RegisterPlayerToCamera(player);

        if (logPlayerJoin)
        {
            Debug.Log($"Player joined: {player.name}, index: {playerIndex}, device: {playerInput.devices}", player);
        }
    }

    private void HandlePlayerLeft(PlayerInput playerInput)
    {
        if (playerInput == null)
        {
            return;
        }

        Player player = playerInput.GetComponent<Player>();

        if (player == null)
        {
            return;
        }

        UnregisterPlayerFromCamera(player);
        joinedPlayers.Remove(player);
    }

    private void MovePlayerToSpawnPoint(Transform playerTransform, int playerIndex)
    {
        if (playerTransform == null)
        {
            return;
        }

        Transform spawnPoint = GetSpawnPoint(playerIndex);

        if (spawnPoint != null)
        {
            playerTransform.position = spawnPoint.position;
            playerTransform.rotation = spawnPoint.rotation;
            return;
        }

        if (useHandlerPositionIfNoSpawnPoint)
        {
            playerTransform.position = transform.position;
            playerTransform.rotation = Quaternion.identity;
        }
    }

    private Transform GetSpawnPoint(int playerIndex)
    {
        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            return null;
        }

        int spawnPointIndex = Mathf.Clamp(playerIndex, 0, spawnPoints.Length - 1);
        return spawnPoints[spawnPointIndex];
    }

    private void RegisterPlayerToCamera(Player player)
    {
        if (!registerPlayersToCamera)
        {
            return;
        }

        if (cameraTargetGroup == null || player == null)
        {
            return;
        }

        cameraTargetGroup.RegisterTarget(player.transform);
    }

    private void UnregisterPlayerFromCamera(Player player)
    {
        if (cameraTargetGroup == null || player == null)
        {
            return;
        }

        cameraTargetGroup.UnregisterTarget(player.transform);
    }

    private void OnDrawGizmos()
    {
        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, 0.2f);
            return;
        }

        Gizmos.color = Color.green;

        for (int i = 0; i < spawnPoints.Length; i++)
        {
            Transform spawnPoint = spawnPoints[i];

            if (spawnPoint == null)
            {
                continue;
            }

            Gizmos.DrawWireSphere(spawnPoint.position, 0.2f);
            Gizmos.DrawLine(spawnPoint.position, spawnPoint.position + spawnPoint.right * 0.35f);
        }
    }
}