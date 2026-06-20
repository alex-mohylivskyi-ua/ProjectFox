using UnityEngine;

[RequireComponent(typeof(Player))]
public class PlayerDebugHUD : MonoBehaviour
{
    [Header("Display")]
    [SerializeField] private bool showDebug = false;
    [SerializeField] private KeyCode toggleKey = KeyCode.F1;
    [SerializeField] private int fontSize = 10;
    [SerializeField] private Vector2 screenOffset = new Vector2(10f, 10f);
    [SerializeField] private Vector2 panelSize = new Vector2(460f, 620f);

    private Player player;
    private PlayerOneWayPlatformDrop oneWayPlatformDrop;
    private GUIStyle labelStyle;
    private GUIStyle boxStyle;

    private void Awake()
    {
        player = GetComponent<Player>();
        oneWayPlatformDrop = GetComponent<PlayerOneWayPlatformDrop>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            showDebug = !showDebug;
        }
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

        // DrawLine("Input", true);
        // DrawLine($"Move: X {player.moveInput.x:0.00} / Y {player.moveInput.y:0.00}");
        // DrawLine($"Jump Pressed:  {player.inputReader.jumpPressed}");
        // DrawLine($"Jump Held:     {player.inputReader.jumpHeld}");
        // DrawLine($"Attack Pressed:{player.inputReader.attackPressed}");
        // DrawLine($"Dash Pressed:  {player.inputReader.dashPressed}");
        // GUILayout.Space(6f);
        
        

        // DrawLine("Movement", true);
        // DrawLine($"Velocity: X {player.rb.linearVelocity.x:0.00} / Y {player.rb.linearVelocity.y:0.00}");
        // DrawLine($"Facing Direction: {player.facingDirection}");
        // GUILayout.Space(6f);

        DrawLine("Collision", true);
        DrawLine($"Ground Detected:           {player.groundDetected}");
        DrawLine($"Wall:               {player.wallDetected}");
        DrawLine($"Wall Slide Surface: {player.wallSlideSurfaceDetected}");
        GUILayout.Space(6f);
        
        DrawLine("Ground Hit Debug", true);
        DrawLine($"Hit Detected:       {player.debugGroundHitDetected}");
        DrawLine($"Hit Object:         {player.debugGroundHitName}");
        DrawLine($"Is One Way:         {player.debugGroundHitIsOneWayPlatform}");
        DrawLine($"One Way Valid:      {player.debugOneWayGroundHitValid}");
        DrawLine($"GroundCheck Y:      {player.debugGroundCheckY:0.000}");
        DrawLine($"Cast Bottom Y:      {player.debugGroundCastBottomY:0.000}");
        DrawLine($"Platform Top Y:     {player.debugPlatformTopY:0.000}");
        DrawLine($"Distance To Top:    {player.debugDistanceToPlatformTop:0.000}");
        DrawLine($"Top Tolerance:      {player.debugOneWayPlatformTopTolerance:0.000}");
        DrawLine($"Max Landing Dist:   {player.debugOneWayPlatformMaxLandingDistance:0.000}");
        GUILayout.Space(6f);

        // DrawLine("Jump", true);
        // DrawLine($"Jump Buffered:          {player.jumpBuffered}");
        // DrawLine($"Jump Buffer Left:       {player.jumpBufferTimeLeft:0.000}");
        // DrawLine($"Coyote Available:       {player.canUseCoyoteJump}");
        // DrawLine($"Coyote Left:            {player.coyoteTimeLeft:0.000}");
        // DrawLine($"Buffered Jump Released: {player.bufferedJumpReleased}");
        // GUILayout.Space(6f);

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