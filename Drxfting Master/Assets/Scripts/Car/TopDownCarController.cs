
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopDownCarController : MonoBehaviour
{
    [Header("Car settings")]
    public float driftFactor = 0.95f;
    public float accelerationFactor = 30.0f;
    public float turnFactor = 3.5f;
    public float maxSpeed = 15;

    [Header("Drift Effects")]
    public TrailRenderer leftWheelTrail;
    public TrailRenderer rightWheelTrail;
    public ParticleSystem leftWheelSmoke;
    public ParticleSystem rightWheelSmoke;
    public float driftThreshold = 2.0f;
    public Color driftTrailColor = Color.black;

    [Header("Wheels")]
    public Transform frontLeftWheel;
    public Transform frontRightWheel;
    public float maxWheelTurnAngle = 30f;

    [Header("Sprites")]
    public SpriteRenderer carSpriteRenderer;
    public SpriteRenderer carShadowRenderer;

    [Header("Jumping")]
    public AnimationCurve jumpCurve;
    public ParticleSystem landingParticleSystem;

    //Local variables
    float accelerationInput = 0;
    float steeringInput = 0;

    float rotationAngle = 0;

    float velocityVsUp = 0;

    bool isJumping = false;

    //Components
    Rigidbody2D carRigidbody2D;
    Collider2D carCollider;
    CarSFXHandler carSfxHandler;

    //Awake is called when the script instance is being loaded.
    void Awake()
    {
        carRigidbody2D = GetComponent<Rigidbody2D>();
        carCollider = GetComponentInChildren<Collider2D>();
        carSfxHandler = GetComponent<CarSFXHandler>();

        if (leftWheelTrail != null)
            leftWheelTrail.emitting = false;
        if (rightWheelTrail != null)
            rightWheelTrail.emitting = false;

        if (leftWheelTrail != null)
            leftWheelTrail.startColor = driftTrailColor;
        if (rightWheelTrail != null)
            rightWheelTrail.startColor = driftTrailColor;
    }

    void Start()
    {
        rotationAngle = transform.rotation.eulerAngles.z;
    }

    //Frame-rate independent for physics calculations.
    void FixedUpdate()
    {
        if (GameManager.instance.GetGameState() == GameStates.countDown)
            return;

        ApplyEngineForce();
        KillOrthogonalVelocity();
        ApplySteering();
        UpdateWheelVisuals();
        UpdateDriftEffects();
    }

    void ApplyEngineForce()
    {
        //Don't let the player brake while in the air, but we still allow some drag so it can be slowed slightly. 
        if (isJumping && accelerationInput < 0)
            accelerationInput = 0;

        //Apply drag if there is no accelerationInput so the car stops when the player lets go of the accelerator
        if (accelerationInput == 0)
            carRigidbody2D.drag = Mathf.Lerp(carRigidbody2D.drag, 3.0f, Time.fixedDeltaTime * 3);
        else carRigidbody2D.drag = 0;

        //Caculate how much "forward" we are going in terms of the direction of our velocity
        velocityVsUp = Vector2.Dot(transform.up, carRigidbody2D.velocity);

        //Limit so we cannot go faster than the max speed in the "forward" direction
        if (velocityVsUp > maxSpeed && accelerationInput > 0)
            return;

        //Limit so we cannot go faster than the 50% of max speed in the "reverse" direction
        if (velocityVsUp < -maxSpeed * 0.5f && accelerationInput < 0)
            return;

        //Limit so we cannot go faster in any direction while accelerating
        if (carRigidbody2D.velocity.sqrMagnitude > maxSpeed * maxSpeed && accelerationInput > 0 && !isJumping)
            return;

        //Create a force for the engine
        Vector2 engineForceVector = transform.up * accelerationInput * accelerationFactor;

        //Apply force and pushes the car forward
        carRigidbody2D.AddForce(engineForceVector, ForceMode2D.Force);
    }

    void ApplySteering()
    {
        //Limit the cars ability to turn when moving slowly
        float minSpeedBeforeAllowTurningFactor = (carRigidbody2D.velocity.magnitude / 2);
        minSpeedBeforeAllowTurningFactor = Mathf.Clamp01(minSpeedBeforeAllowTurningFactor);

        //Update the rotation angle based on input
        rotationAngle -= steeringInput * turnFactor * minSpeedBeforeAllowTurningFactor;

        //Apply steering by rotating the car object
        carRigidbody2D.MoveRotation(rotationAngle);
    }

    void KillOrthogonalVelocity()
    {
        //Get forward and right velocity of the car
        Vector2 forwardVelocity = transform.up * Vector2.Dot(carRigidbody2D.velocity, transform.up);
        Vector2 rightVelocity = transform.right * Vector2.Dot(carRigidbody2D.velocity, transform.right);

        //Kill the orthogonal velocity (side velocity) based on how much the car should drift. 
        carRigidbody2D.velocity = forwardVelocity + rightVelocity * driftFactor;
    }

    float GetLateralVelocity()
    {
        //Returns how how fast the car is moving sideways. 
        return Vector2.Dot(transform.right, carRigidbody2D.velocity);
    }

    public bool IsTireScreeching(out float lateralVelocity, out bool isBraking)
    {
        lateralVelocity = GetLateralVelocity();
        isBraking = false;

        if (isJumping)
            return false;

        if (accelerationInput < 0 && velocityVsUp > 0)
        {
            isBraking = true;
            return true;
        }

        if (Mathf.Abs(GetLateralVelocity()) > 2.0f)
            return true;

        if (Mathf.Abs(steeringInput) > 0.8f && velocityVsUp > maxSpeed * 0.5f)
            return true;

        return false;
    }

    void UpdateDriftEffects()
    {
        float lateralVelocity = Mathf.Abs(GetLateralVelocity());
        bool isDrifting = lateralVelocity > driftThreshold;
        bool isBraking = accelerationInput < 0 && velocityVsUp > 0;
        bool isSharpTurning = Mathf.Abs(steeringInput) > 0.8f && velocityVsUp > maxSpeed * 0.5f;
        bool shouldShowEffects = isDrifting || isBraking || isSharpTurning;

        if (leftWheelTrail != null)
            leftWheelTrail.emitting = shouldShowEffects;
        if (rightWheelTrail != null)
            rightWheelTrail.emitting = shouldShowEffects;

        if (shouldShowEffects)
        {
            float effectIntensity = Mathf.Clamp01(lateralVelocity / 6.0f);

            if (leftWheelSmoke != null)
            {
                if (!leftWheelSmoke.isPlaying)
                    leftWheelSmoke.Play();
                var mainModule = leftWheelSmoke.main;
                mainModule.startSizeMultiplier = 0.7f + effectIntensity * 0.8f;
            }

            if (rightWheelSmoke != null)
            {
                if (!rightWheelSmoke.isPlaying)
                    rightWheelSmoke.Play();
                var mainModule = rightWheelSmoke.main;
                mainModule.startSizeMultiplier = 0.7f + effectIntensity * 0.8f;
            }
        }
        else
        {
            if (leftWheelSmoke != null && leftWheelSmoke.isPlaying)
                leftWheelSmoke.Stop();
            if (rightWheelSmoke != null && rightWheelSmoke.isPlaying)
                rightWheelSmoke.Stop();
        }
    }

    void UpdateWheelVisuals()
    {
        if (frontLeftWheel != null && frontRightWheel != null)
        {
            float wheelAngle = steeringInput * maxWheelTurnAngle;
            frontLeftWheel.localRotation = Quaternion.Euler(0, 0, wheelAngle);
            frontRightWheel.localRotation = Quaternion.Euler(0, 0, wheelAngle);

            float lateralVelocity = Mathf.Abs(GetLateralVelocity());
            bool isBraking = accelerationInput < 0 && velocityVsUp > 0;

            if (lateralVelocity > driftThreshold || isBraking)
            {
                float driftExtraAngle = Mathf.Sign(steeringInput) * 30f;
                frontLeftWheel.localRotation = Quaternion.Euler(0, 0, wheelAngle + driftExtraAngle);
                frontRightWheel.localRotation = Quaternion.Euler(0, 0, wheelAngle + driftExtraAngle);

                float scaleFactor = 1.0f + (lateralVelocity - driftThreshold) * 0.025f;
                if (isBraking) scaleFactor += 0.05f;
                scaleFactor = Mathf.Clamp(scaleFactor, 1.0f, 1.25f);

                frontLeftWheel.localScale = new Vector3(scaleFactor, scaleFactor, 1);
                frontRightWheel.localScale = new Vector3(scaleFactor, scaleFactor, 1);

                if (lateralVelocity > driftThreshold * 1.5f || isBraking)
                {
                    float shakeAmount = 0.03f * Mathf.Sin(Time.time * 40);
                    Vector3 basePos = frontLeftWheel.localPosition;
                    frontLeftWheel.localPosition = new Vector3(basePos.x, basePos.y + shakeAmount, basePos.z);
                    basePos = frontRightWheel.localPosition;
                    frontRightWheel.localPosition = new Vector3(basePos.x, basePos.y - shakeAmount, basePos.z);
                }
            }
            else
            {
                frontLeftWheel.localScale = Vector3.one;
                frontRightWheel.localScale = Vector3.one;
                Vector3 leftBasePos = frontLeftWheel.localPosition;
                Vector3 rightBasePos = frontRightWheel.localPosition;
                frontLeftWheel.localPosition = new Vector3(leftBasePos.x, 0, leftBasePos.z);
                frontRightWheel.localPosition = new Vector3(rightBasePos.x, 0, rightBasePos.z);
            }
        }
    }

    public void SetInputVector(Vector2 inputVector)
    {
        steeringInput = inputVector.x;
        accelerationInput = inputVector.y;
    }

    public float GetVelocityMagnitude()
    {
        return carRigidbody2D.velocity.magnitude;
    }

    public void Jump(float jumpHeightScale, float jumpPushScale, int carColliderLayerBeforeJump)
    {
        if (!isJumping)
            StartCoroutine(JumpCo(jumpHeightScale, jumpPushScale, carColliderLayerBeforeJump));
    }

    private IEnumerator JumpCo(float jumpHeightScale, float jumpPushScale, int carColliderLayerBeforeJump)
    {
        isJumping = true;

        float jumpStartTime = Time.time;
        float jumpDuration = carRigidbody2D.velocity.magnitude * 0.05f;
        jumpDuration = Mathf.Clamp(jumpDuration, 0.3f, 0.6f);

        jumpHeightScale = jumpHeightScale * carRigidbody2D.velocity.magnitude * 0.01f;
        jumpHeightScale = Mathf.Clamp(jumpHeightScale, 0.1f, 0.3f);

        carCollider.gameObject.layer = LayerMask.NameToLayer("ObjectFlying");
        carSfxHandler.PlayJumpSfx();

        carSpriteRenderer.sortingLayerName = "Flying";
        carShadowRenderer.sortingLayerName = "Flying";

        carRigidbody2D.AddForce(carRigidbody2D.velocity.normalized * jumpPushScale, ForceMode2D.Impulse);

        while (isJumping)
        {
            float jumpCompletedPercentage = (Time.time - jumpStartTime) / jumpDuration;
            jumpCompletedPercentage = Mathf.Clamp01(jumpCompletedPercentage);

            carSpriteRenderer.transform.localScale = Vector3.one + Vector3.one * jumpCurve.Evaluate(jumpCompletedPercentage) * jumpHeightScale;
            carShadowRenderer.transform.localScale = carSpriteRenderer.transform.localScale * 0.75f;
            carShadowRenderer.transform.localPosition = new Vector3(1, -1, 0.0f) * 3 * jumpCurve.Evaluate(jumpCompletedPercentage) * jumpHeightScale;

            if (jumpCompletedPercentage == 1.0f)
                break;

            yield return null;
        }

        carCollider.enabled = false;

        ContactFilter2D contactFilter2D = new ContactFilter2D();
        contactFilter2D.useTriggers = false;
        contactFilter2D.SetLayerMask(Physics2D.GetLayerCollisionMask(carColliderLayerBeforeJump));

        Collider2D[] hitResults = new Collider2D[3];
        int numberOfHitObjects = Physics2D.OverlapCircle(transform.position, 1.5f, contactFilter2D, hitResults);

        carCollider.enabled = true;

        carSpriteRenderer.transform.localScale = Vector3.one;
        carShadowRenderer.transform.localPosition = Vector3.zero;
        carShadowRenderer.transform.localScale = carSpriteRenderer.transform.localScale;
        carCollider.gameObject.layer = carColliderLayerBeforeJump;
        carSpriteRenderer.sortingLayerName = "Default";
        carShadowRenderer.sortingLayerName = "Default";

        if (numberOfHitObjects > 0)
        {
            transform.position += Vector3.up * 0.2f;
        }

        if (jumpHeightScale > 0.2f)
        {
            landingParticleSystem.Play();
            carSfxHandler.PlayLandingSfx();
        }

        isJumping = false;
    }

    public bool IsJumping()
    {
        return isJumping;
    }

    //Detect Jump trigger
    void OnTriggerEnter2D(Collider2D collider2d)
    {
        if (collider2d.CompareTag("Jump"))
        {
            //Get the jump data from the jump
            JumpData jumpData = collider2d.GetComponent<JumpData>();
            Jump(jumpData.jumpHeightScale, jumpData.jumpPushScale, carCollider.gameObject.layer);
        }
    }
}