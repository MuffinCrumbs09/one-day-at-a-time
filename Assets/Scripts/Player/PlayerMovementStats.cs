using UnityEngine;
[CreateAssetMenu(menuName = "Player Movement")]
public class PlayerMovementStats : ScriptableObject
{
    [Header("Walk")]
    [Range(1f, 100f)] public float maxWalkSpeed = 12.5f;
    [Range(.025f, 50f)] public float groundAcel = 5f;
    [Range(.025f, 50f)] public float groundDecel = 20f;
    [Range(.025f, 50f)] public float airAcel = 5f;
    [Range(.025f, 50f)] public float airDecel = 5f;

    [Header("Run")]
    [Range(1f, 100f)] public float maxRunSpeed = 20f;

    [Header("Ground/Collision Check")]
    public LayerMask groundLayer;
    public float groundDetectionRayLength = 0.02f;
    public float headDetectionRayLength = 0.02f;
    [Range(0f, 1f)] public float headWidth = 0.75f;

    [Header("Jump")]
    public float jumpHeight = 6.5f;
    [Range(1f, 1.1f)] public float jumpHeightCompensationFactor = 1.054f;
    public float timeTillJumpApex = 0.35f;
    [Range(0.01f, 5f)] public float gravityOnReleaseMultipler = 2f;
    public float maxFallSpeed = 28f;
    [Range(1, 5)] public int maxJumps = 2;

    [Header("Jump Cut")]
    [Range(0.02f, 0.3f)] public float timeForUpCancel = 0.027f;

    [Header("Jump Apex")]
    [Range(0.5f, 1f)] public float apexThreshold = 0.97f;
    [Range(0.01f, 1f)] public float apexHangTime = 0.75f;

    [Header("Jump Buffer")]
    [Range(0f, 1f)] public float jumpBufferTime = 0.125f;

    [Header("Jump Coyote Time")]
    [Range(0f, 1f)] public float jumpCoyoteTime = 0.1f;

    [Header("JumpVisualisation Tool")]
    public bool showWalkJumpArc = false;
    public bool showRunJumpArc = false;
    public bool stopOnCollision = true;
    public bool drawRight = true;
    [Range(5, 100)] public int arcResolution = 20;
    [Range(0, 500)] public int visualisationSteps = 90;

    public float gravity { get; private set; }
    public float intialJumpVelocity { get; private set; }
    public float adjustedJumpHeight { get; private set; }

    private void OnValidate()
    {
        CalculateValues(); 
    }

    private void OnEnable()
    {
        CalculateValues();
    }

    private void CalculateValues()
    {
        adjustedJumpHeight = jumpHeight * jumpHeightCompensationFactor;
        gravity = -(2f * adjustedJumpHeight) / Mathf.Pow(timeTillJumpApex, 2f);
        intialJumpVelocity = Mathf.Abs(gravity) * timeTillJumpApex;
    }
}
