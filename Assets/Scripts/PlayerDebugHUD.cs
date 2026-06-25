using UnityEngine;

[RequireComponent(typeof(Player))]
public class PlayerDebugHUD : MonoBehaviour
{
    [Header("Display")]
    [SerializeField] private bool showDebug = false;
    // [SerializeField] private KeyCode toggleKey = KeyCode.F1;
    [SerializeField] private int fontSize = 10;
    [SerializeField] private Vector2 screenOffset = new Vector2(10f, 10f);
    [SerializeField] private Vector2 panelSize = new Vector2(460f, 820f);

    private Player player;
    private PlayerOneWayPlatformDrop oneWayPlatformDrop;
    private MovingPlatformPassengerMover[] movingPlatformMovers;
    private GUIStyle labelStyle;
    private GUIStyle boxStyle;

    private void Awake()
    {
        player = GetComponent<Player>();
        oneWayPlatformDrop = GetComponent<PlayerOneWayPlatformDrop>();
        RefreshMovingPlatformMovers();
    }

    private void Update()
    {
        // if (Input.GetKeyDown(toggleKey))
        // {
        //     showDebug = !showDebug;
        // }
    }

    private void OnGUI()
    {
        if (!showDebug || player == null)
        {
            return;
        }

        EnsureStyles();

        Rect panelRect = new Rect(screenOffset.x, screenOffset.y, panelSize.x, panelSize.y);
        GUI.Box(panelRect, GUIContent.none, boxStyle);

        GUILayout.BeginArea(new Rect(
            panelRect.x + 10f,
            panelRect.y + 8f,
            panelRect.width - 20f,
            panelRect.height - 16f
        ));

        DrawLine("PLAYER DEBUG", true);
        GUILayout.Space(6f);

        DrawLine($"Current State:  {player.CurrentStateName}");
        DrawLine($"Previous State: {player.PreviousStateName}");
        GUILayout.Space(6f);
        
        // DrawMovementDebug();
        
        // DrawMovingPlatformDebug();

        DrawLine("Collision", true);
        DrawLine($"Grounded / Ground Detected: {player.groundDetected}");
        DrawLine($"Wall Detected:              {player.wallDetected}");
        DrawLine($"Wall Slide Surface:         {player.wallSlideSurfaceDetected}");
        GUILayout.Space(6f);

        
        GUILayout.Space(6f);

        DrawLine("Ground Hit Debug", true);
        DrawLine($"Hit Detected:       {player.debugGroundHitDetected}");
        DrawLine($"Hit Object:         {player.debugGroundHitName}");
        DrawLine($"Is Moving Platform: {player.debugGroundHitIsMovingPlatform}");
        DrawLine($"Moving Platform:    {player.debugGroundHitMovingPlatformName}");
        DrawLine($"Is One Way:         {player.debugGroundHitIsOneWayPlatform}");
        DrawLine($"One Way Valid:      {player.debugOneWayGroundHitValid}");
        // DrawLine($"Player GroundCheck position Y:      {player.debugGroundCheckY:0.000}");
        DrawLine($"Player BoxCast Bottom Y:      {player.debugGroundCastBottomY:0.000}");
        DrawLine($"Platform Top Y:     {player.debugPlatformTopY:0.000}");
        DrawLine($"Distance To Top:    {player.debugDistanceToPlatformTop:0.000}");
        DrawLine($"Top Tolerance:      {player.debugOneWayPlatformTopTolerance:0.000}");
        DrawLine($"Max Landing Dist:   {player.debugOneWayPlatformMaxLandingDistance:0.000}");
        GUILayout.Space(6f);

        DrawLine("One Way Platform", true);

        if (oneWayPlatformDrop == null)
        {
            DrawLine("Component: Missing");
        }
        else
        {
            DrawLine($"On Platform:        {oneWayPlatformDrop.HasCurrentPlatform}");
            DrawLine($"Platform:           {oneWayPlatformDrop.CurrentPlatformName}");
            // DrawLine($"Drop Input:         {oneWayPlatformDrop.DropInputDetected}");
            // DrawLine($"Dropped This Frame: {player.droppedThroughOneWayPlatformThisFrame}");
            // DrawLine($"Ignore Ground:      {player.shouldIgnoreGroundAfterOneWayDrop}");
            // DrawLine($"Ignore Ground Left: {player.ignoreGroundAfterOneWayDropTimeLeft:0.000}");
            // DrawLine($"Ignoring Collision: {oneWayPlatformDrop.IsIgnoringPlatformCollision}");
        }

        GUILayout.EndArea();
    }

    private void DrawMovementDebug()
    {
        DrawLine("Movement", true);
        DrawLine($"Velocity: X {player.rb.linearVelocity.x:0.00} / Y {player.rb.linearVelocity.y:0.00}");
        DrawLine($"Position: X {player.transform.position.x:0.00} / Y {player.transform.position.y:0.00}");
        DrawLine($"Facing Direction: {player.facingDirection}");
        GUILayout.Space(6f);
    }

    private void DrawMovingPlatformDebug()
    {
        MovingPlatformPassengerMover attachedPlatform = GetAttachedMovingPlatform();
        MovingPlatformPassengerMover groundHitPlatform = GetGroundHitMovingPlatform();
        MovingPlatformPassengerMover debugPlatform = attachedPlatform != null ? attachedPlatform : groundHitPlatform;

        DrawLine("Moving Platform", true);
        DrawLine($"Player On Moving Platform:       {player.debugGroundHitIsMovingPlatform}");
        DrawLine($"Ground Hit Moving Platform:      {player.debugGroundHitMovingPlatformName}");
        DrawLine($"Player Attached To Platform:     {attachedPlatform != null}");

        if (debugPlatform == null)
        {
            DrawLine("Attached Platform:               None");
            DrawLine("Platform Delta:                  X 0.00 / Y 0.00");
            DrawLine("Applied Passenger Delta:         X 0.00 / Y 0.00");
            DrawLine("Platform Passenger Count:        0");
            return;
        }

        Vector2 platformDelta = debugPlatform.LastPlatformDelta;
        Vector2 appliedPassengerDelta = debugPlatform.LastAppliedPassengerDelta;

        DrawLine($"Debug Platform:                  {debugPlatform.PlatformName}");
        DrawLine($"Attached Platform:               {(attachedPlatform != null ? attachedPlatform.PlatformName : "None")}");
        DrawLine($"Platform Delta:                  X {platformDelta.x:0.000} / Y {platformDelta.y:0.000}");
        DrawLine($"Applied Passenger Delta:         X {appliedPassengerDelta.x:0.000} / Y {appliedPassengerDelta.y:0.000}");
        DrawLine($"Platform Passenger Count:        {debugPlatform.PassengerCount}");
        DrawLine($"Last Attach Reject Reason:       {debugPlatform.LastAttachRejectReason}");
        DrawLine($"Last Detach Reason:              {debugPlatform.LastDetachReason}");
        DrawLine($"Last Contact Normal Y:           {debugPlatform.LastContactNormalY:0.000}");
        DrawLine($"Last Had Top Contact:            {debugPlatform.LastHadTopContact}");
        DrawLine($"Last Bounds Top Check:           {debugPlatform.LastPassedBoundsTopCheck}");
        DrawLine($"Last Attached Passenger:         {debugPlatform.LastAttachedPassengerName}");
        DrawLine($"Last Detached Passenger:         {debugPlatform.LastDetachedPassengerName}");
    }
    
    private MovingPlatformPassengerMover GetGroundHitMovingPlatform()
    {
        if (movingPlatformMovers == null || movingPlatformMovers.Length == 0)
        {
            RefreshMovingPlatformMovers();
        }

        for (int i = 0; i < movingPlatformMovers.Length; i++)
        {
            MovingPlatformPassengerMover mover = movingPlatformMovers[i];

            if (mover == null)
            {
                continue;
            }

            if (mover.PlatformName == player.debugGroundHitMovingPlatformName)
            {
                return mover;
            }
        }

        return null;
    }

    private MovingPlatformPassengerMover GetAttachedMovingPlatform()
    {
        if (player == null || player.rb == null)
        {
            return null;
        }

        if (movingPlatformMovers == null || movingPlatformMovers.Length == 0)
        {
            RefreshMovingPlatformMovers();
        }

        for (int i = 0; i < movingPlatformMovers.Length; i++)
        {
            MovingPlatformPassengerMover mover = movingPlatformMovers[i];

            if (mover == null)
            {
                continue;
            }

            if (mover.HasPassenger(player.rb))
            {
                return mover;
            }
        }

        return null;
    }

    private void RefreshMovingPlatformMovers()
    {
        movingPlatformMovers = FindObjectsByType<MovingPlatformPassengerMover>(
            FindObjectsInactive.Include,
            FindObjectsSortMode.None
        );
    }

    private void DrawLine(string text, bool header = false)
    {
        if (header)
        {
            GUILayout.Label($"<b>{text}</b>", labelStyle);
            return;
        }

        GUILayout.Label(text, labelStyle);
    }

    private void EnsureStyles()
    {
        if (labelStyle == null)
        {
            labelStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = fontSize,
                richText = true,
                normal =
                {
                    textColor = Color.white
                }
            };
        }

        if (boxStyle == null)
        {
            Texture2D background = new Texture2D(1, 1);
            background.SetPixel(0, 0, new Color(0f, 0f, 0f, 0.75f));
            background.Apply();

            boxStyle = new GUIStyle(GUI.skin.box)
            {
                normal =
                {
                    background = background
                }
            };
        }
    }
}