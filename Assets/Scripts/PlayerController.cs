using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Déplacement horizontal")]
    [Space]
    [SerializeField] private float maxVelocityX;
    [SerializeField] private float acceleration;
    [SerializeField] private float deceleration;
    [SerializeField] private float sprintMaxVelocityX;
    [SerializeField] private float sprintAcceleration;
    [SerializeField] private bool allowTurnBoost = true;
    [SerializeField] private float turnBoostFactor;
    [SerializeField] private float turnBoostDuration;
    [SerializeField] private bool allowDash = true;
    [SerializeField] private float dashSpeed;
    [SerializeField] private float dashDuration;
    [SerializeField] private float dashCooldown;
    [SerializeField] private bool canDashWithoutDirection = true;
    [SerializeField] private AnimationCurve speedOnSlope;
    
    [Header("Saut")]
    [Space]
    [SerializeField] private float gravity = 20;
    [SerializeField] private int airJumpCount = 1;
    [SerializeField] private float maxVelocityY;
    [SerializeField] private float jumpVelocity;
    [SerializeField] private float airJumpVelocity = 5;
    [SerializeField] private Vector2 wallJumpVelocity;
    [SerializeField] private float wallJumpDuration;
    [SerializeField] private float onWallDelay;
    [SerializeField] private float jumpReleaseMultiplier = 2f;
    [SerializeField] private float bufferJumpMaxAllowedTime = .2f;
    [SerializeField] private bool airControl = true;
    [SerializeField] private bool airSprintControl = true;
    [SerializeField] private bool fixedAirJumpHeight = true;
    [SerializeField] private float coyoteTimeThreshold = 0.1f;
    [SerializeField] private bool wallsRefreshWallJump = false;
    
    [Header("Collisions verticales et horizontales")]
    [Space]
    [SerializeField] private BoxCollider2D groundCheck;
    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private LayerMask whatIsWall;
    [SerializeField] private BoxCollider2D wallCheck;
    [SerializeField] private float maxFallingSpeedOnWalls = 2f;
    [SerializeField] private float maxSlopeAngle = 45;
    [SerializeField] private float platformGoDownDirectionThreshold = -0.5f;
    [SerializeField] private float platformGoDownMinimumPressTime = .2f;
    
    [Header("Autre")]
    [Space]
    [SerializeField] private float groundAccelerationScale = 1;
    [SerializeField] private float groundDecelerationScale = 1;
    [SerializeField] private float iceAccelerationScale = 0.5f;
    [SerializeField] private float iceDecelerationScale = 0.5f;
    [SerializeField] private float airAccelerationScale = 0.5f;
    [SerializeField] private float airDecelerationScale = 0.5f;
    [SerializeField] private float trampolineBounceVelocity;
    
    [Header("Feedbacks")]
    [Space]
    [SerializeField] private GameObject doubleJumpParticles;
    [SerializeField] private ParticleSystem runParticles;
    [SerializeField] private ParticleSystem wallSlideParticles;
    [SerializeField] private TrailRenderer dashTrail;
    [SerializeField] Animator animator;
    [SerializeField] private AudioClip jumpSound;
    [SerializeField] private AudioClip trampolineSound;
    [SerializeField] private AudioClip landSound;
    [SerializeField] private AudioClip dashSound;

    private Vector2 velocity = Vector2.zero;
    private Vector2 moveDirection;
    
    private BoxCollider2D boxCollider;
    private Vector2 groundPosition;
    private Vector2 lastPosition;
    private int groundObjectId;
    private bool grounded = false;
    private bool onWall = false;
    private bool isWallOnTheRight = true;
    private bool wasGroundedLastFrame = false;
    
    private int currentAirJumpCount;
    private bool jump = false;
    private bool jumpButtonHeld;
    private bool jumpCancellable = false;
    private float bufferedJumpTime = 0f;

    private float onWallDelayStopTime = 0f;
    private float wallJumpStopTime = 0f;
    
    private bool sprinting = false;
    
    private float turnBoostStopTime = 0f;
    
    private float platformGoDownTimePressed = 0f;
    
    private float dashDirection = 0f;
    private float dashStopTime = 0f;
    private float dashCooldownStopTime = 0f;

    private bool hasBounced = false;
    
    
    private float timeSinceLeftGround = 0;
    private bool jumpWithCoyoteTime = false;
    private float slopeAngle = 0;

    private AudioSource audioSource;
    private void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        audioSource = GetComponent<AudioSource>();
        currentAirJumpCount = airJumpCount;
    }

    private void FixedUpdate()
    {
        lastPosition = transform.position;
        bool onTrampoline = false;
        bool onIce = false;
        GroundCheck(ref onTrampoline, ref onIce);
        if (!grounded)
            timeSinceLeftGround += Time.fixedDeltaTime;
        if (velocity.y <= 0)
            jumpCancellable = false;
        if (onTrampoline)
        {
            if (FeedbackController.Instance.SoundEffects)
                audioSource.PlayOneShot(trampolineSound);
            velocity.y = trampolineBounceVelocity;
            hasBounced = true;
            dashStopTime = 0;
            if (FeedbackController.Instance.DeformPlayerEffect)
                animator.SetTrigger("jump");
        }
        else if (!IsDashing())
        {
            if (jump)
            {
                if (FeedbackController.Instance.SoundEffects)
                    audioSource.PlayOneShot(jumpSound);
                if (FeedbackController.Instance.DeformPlayerEffect)
                    animator.SetTrigger("jump");
                if (grounded || !fixedAirJumpHeight)
                    jumpCancellable = true;
                jump = false;
                if ((onWall || isOnWallDelayActive()) && !grounded)
                {
                    velocity = wallJumpVelocity;
                    wallJumpStopTime = Time.time + wallJumpDuration;
                    if (isWallOnTheRight)
                        velocity.x = -velocity.x;
                }
                else
                {
                    velocity.y = grounded ? jumpVelocity : airJumpVelocity;
                }

                if ((!grounded || Mathf.Abs(slopeAngle) > maxSlopeAngle) && !onWall && !isOnWallDelayActive() && !jumpWithCoyoteTime && FeedbackController.Instance.EmitDoubleJumpEffect)
                    Instantiate(doubleJumpParticles, groundCheck.transform.position, Quaternion.identity);
                jumpWithCoyoteTime = false;
                onWallDelayStopTime = Time.time;
            }
            if (velocity.y > 0 && !jumpButtonHeld && jumpCancellable)
            {
                velocity.y /= jumpReleaseMultiplier;
                jumpCancellable = false;
            }
        }
        
        if (grounded && moveDirection.y < platformGoDownDirectionThreshold)
            platformGoDownTimePressed += Time.deltaTime;
        else
            platformGoDownTimePressed = 0f;

        float accelerationScale;
        float decelerationScale;
        if (grounded)
        {
            if (onIce)
            {
                accelerationScale = iceAccelerationScale;
                decelerationScale = iceDecelerationScale;
            }
            else
            {
                accelerationScale = groundAccelerationScale;
                decelerationScale = groundDecelerationScale;
            }
        }
        else
        {
            accelerationScale = airAccelerationScale;
            decelerationScale = airDecelerationScale;
        }

        float accelerationSpeed = (sprinting ? sprintAcceleration : acceleration) * accelerationScale;
        float decelerationSpeed = deceleration * decelerationScale;
        float maxSpeedX = sprinting ? sprintMaxVelocityX : maxVelocityX;
        if ((airControl || grounded) && !IsWallJumpActive())
            velocity.x += moveDirection.x * accelerationSpeed * Time.fixedDeltaTime;

        if (IsTurnBoostActive() && (airControl || grounded))
            velocity.x += moveDirection.x * Mathf.Abs(velocity.x) * turnBoostFactor * Time.fixedDeltaTime;

        if (!IsDashing() && velocity.x != 0 && (grounded || !FeedbackController.Instance.EmitRunEffectOnGroundOnly) &&
            ((IsTurnBoostActive() && FeedbackController.Instance.EmitRunEffectOnTurnBoost) || 
            (sprinting && FeedbackController.Instance.EmitRunEffect)))
        {
            if (!runParticles.isPlaying)
                runParticles.Play();
        }
        else if (runParticles.isPlaying)
            runParticles.Stop();

        WallCheck();
        if (!grounded || moveDirection.y < platformGoDownDirectionThreshold || Mathf.Abs(slopeAngle) > Mathf.Abs(maxSlopeAngle))
            velocity.y -= gravity * Time.fixedDeltaTime;

        if (onWall && !IsDashing() && !grounded)
        {
            velocity.y = Mathf.Clamp(velocity.y, -maxFallingSpeedOnWalls, maxVelocityY);
            
            if(!wallSlideParticles.isPlaying && FeedbackController.Instance.EmitWallSlideEffect)
                wallSlideParticles.Play();
        }
        else
        {
            wallSlideParticles.Stop();
            velocity.y = Mathf.Clamp(velocity.y, -maxVelocityY, hasBounced ? velocity.y : maxVelocityY);
        }

        if (moveDirection.x == 0 || (!grounded && !airControl))
        {
            if (Mathf.Abs(velocity.x) < decelerationSpeed * Time.fixedDeltaTime)
                velocity.x = 0;
            else
                velocity.x -= velocity.x > 0 ? decelerationSpeed * Time.fixedDeltaTime : -decelerationSpeed * Time.fixedDeltaTime;
        }

        if (IsDashing())
        {
            velocity.x = dashDirection * dashSpeed;
            velocity.y = 0;
            dashTrail.emitting = FeedbackController.Instance.EmitDashEffect;
        }
        else
        {
            float slopeSpeedModifier = 1;
            if (velocity.x * slopeAngle > 0)
                if (Mathf.Abs(slopeAngle) > Mathf.Abs(maxSlopeAngle))
                    slopeSpeedModifier = 0;
                else
                    slopeSpeedModifier = speedOnSlope.Evaluate(Mathf.Abs(slopeAngle / maxSlopeAngle));
            velocity.x = Mathf.Clamp(velocity.x, -maxSpeedX * slopeSpeedModifier, maxSpeedX * slopeSpeedModifier);
            dashTrail.emitting = false;
        }

        transform.position += (Vector3)(velocity * Time.fixedDeltaTime);
    }

    private void GroundCheck(ref bool onTrampoline, ref bool onIce)
    {
        bool wasGrounded = grounded;
        grounded = false;
        slopeAngle = 0;
        if (velocity.y > 0)
        {
            groundObjectId = GetInstanceID();
            return;
        }
        
        Collider2D[] colliders = Physics2D.OverlapBoxAll(groundCheck.transform.position, groundCheck.size, 0, whatIsGround);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject != gameObject)
            {
                onTrampoline = colliders[i].CompareTag("Trampoline") || onTrampoline;
                onIce = colliders[i].CompareTag("Ice");

                if(colliders[i].CompareTag("Trampoline") && FeedbackController.Instance.TrampolineBounceEffect)
                    colliders[i].GetComponent<Trampoline>().Animate();
                
                grounded = true;
                hasBounced = false;

                SpriteRenderer colliderSprite = colliders[i].GetComponent<SpriteRenderer>();
                ParticleSystem.MainModule particle = runParticles.main;
                particle.startColor = colliderSprite != null ? colliderSprite.color : Color.white;

                if (!wasGrounded)
                    GamepadVibrations.Instance.OnLand();

                if (colliders[i].CompareTag("Slope"))
                {
                    slopeAngle = colliders[i].transform.rotation.eulerAngles.z;
                    if (slopeAngle > 90)
                        slopeAngle -= 360;
                }
                if (Time.time < bufferedJumpTime)
                    Jump();
                if (groundObjectId == colliders[i].GetInstanceID())
                    transform.Translate(colliders[i].transform.position - (Vector3)groundPosition);
                groundObjectId = colliders[i].GetInstanceID();
                groundPosition = colliders[i].transform.position;
                if (Mathf.Abs(slopeAngle) < maxSlopeAngle)
                    currentAirJumpCount = 0;
            }
        }
        if (!grounded)
            groundObjectId = GetInstanceID();
        else
            timeSinceLeftGround = 0;
    }

    private void WallCheck()
    {
        onWall = false;
        if (moveDirection.x == 0 || moveDirection.x * velocity.x < 0)
            return;

        Collider2D[] colliders = Physics2D.OverlapBoxAll(wallCheck.transform.position, wallCheck.size, 0, whatIsWall);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject != gameObject)
            {
                onWall = true;
                isWallOnTheRight = transform.localScale.x > 0;
                if (!IsDashing())
                    onWallDelayStopTime = Time.time + onWallDelay;
                if (Time.time < bufferedJumpTime)
                    jump = true;
            }
        }
        if (wallsRefreshWallJump && onWall)
            currentAirJumpCount = 0;
    }

    private void Jump()
    {
        if ((grounded || onWall || (timeSinceLeftGround < coyoteTimeThreshold && velocity.y <= 0) || isOnWallDelayActive()) && Mathf.Abs(slopeAngle) < maxSlopeAngle)
        {
            if (timeSinceLeftGround < coyoteTimeThreshold && velocity.y <= 0)
                jumpWithCoyoteTime = true;
            jump = true;
            timeSinceLeftGround = coyoteTimeThreshold;
        }
        else {
            if (currentAirJumpCount < airJumpCount)
            {
                jump = true;
                currentAirJumpCount += 1;
            }
            else
                bufferedJumpTime = Time.time + bufferJumpMaxAllowedTime;
        }
    }

    public void SetJumpButtonValue(bool value)
    {
        jumpButtonHeld = value;
    }

    private void LateUpdate()
    {
        Collider2D[] hits = Physics2D.OverlapBoxAll(transform.position, boxCollider.size, 0, whatIsGround);
        foreach (Collider2D hit in hits)
        { 
            if (hit == boxCollider)
                continue;
            ColliderDistance2D colliderDistance = hit.Distance(boxCollider);
            if (colliderDistance.isOverlapped)
            {
                if (hit.gameObject.layer == LayerMask.NameToLayer("Plateform"))
                {
                    if ((!grounded && lastPosition.y - transform.position.y + boxCollider.bounds.min.y <
                        hit.bounds.max.y) || velocity.y > 0 ||
                        platformGoDownTimePressed >= platformGoDownMinimumPressTime)
                        continue;
                }
                Vector2 translation = colliderDistance.pointA - colliderDistance.pointB;
                if (hit.gameObject.layer == LayerMask.NameToLayer("Slope"))
                {
                    if (Mathf.Abs(slopeAngle) < maxSlopeAngle)
                    {
                        translation.y = Mathf.Abs(slopeAngle) < 0.01 ? translation.y : Mathf.Abs(translation.magnitude / Mathf.Cos(Mathf.Deg2Rad*slopeAngle));
                        translation.x = 0;
                    }
                    else
                    {
                        translation.x = Mathf.Abs(translation.y) < 0.01 ? translation.x : Mathf.Sign(translation.x) * Mathf.Abs(translation.magnitude / Mathf.Sin(Mathf.Deg2Rad * slopeAngle));
                        translation.y = 0;
                    }
                }
                transform.Translate(translation);
                if (Mathf.Abs(translation.x) >= 0.01f)
                {
                    velocity.x = 0;
                }
                if (Mathf.Abs(translation.y) >= 0.01f)
                {
                    velocity.y = 0;
                }
                if (!wasGroundedLastFrame && Mathf.Abs(slopeAngle) < maxSlopeAngle && grounded && FeedbackController.Instance.DeformPlayerEffect)
                {
                    animator.SetTrigger("land");
                    if (FeedbackController.Instance.SoundEffects)
                        audioSource.PlayOneShot(landSound);
                }
            }
        }
        wasGroundedLastFrame = grounded;
    }

    public void Move(Vector2 dir, bool jump, bool sprint, bool dash)
    {
        if (jump)
            Jump();
        
        if((dir.x > 0 && transform.localScale.x < 0) || (dir.x < 0 && transform.localScale.x > 0))
            Flip();

        if (allowTurnBoost && ((velocity.x < 0 && dir.x > 0) || (velocity.x > 0 && dir.x < 0)))
            turnBoostStopTime = Time.time + turnBoostDuration;

        moveDirection = dir;
        sprinting = sprint && (sprinting || airSprintControl || (!airSprintControl && grounded));

        if (allowDash && CanDash() && dash && (dir.x != 0 || canDashWithoutDirection))
        {
            if (FeedbackController.Instance.SoundEffects)
                audioSource.PlayOneShot(dashSound);
            if (dir.x != 0)
                dashDirection = dir.x > 0 ? 1 : -1;
            else
                dashDirection = Mathf.Sign(transform.localScale.x);
            dashStopTime = Time.time + dashDuration;
            dashCooldownStopTime = Time.time + dashCooldown;
        }
    }
    
    private bool IsTurnBoostActive()
    {
        return Time.time < turnBoostStopTime;
    }
    
    private bool IsDashing()
    {
        return Time.time < dashStopTime;
    }
    
    private bool IsWallJumpActive()
    {
        return Time.time < wallJumpStopTime;
    }

    private bool isOnWallDelayActive()
    {
        return Time.time < onWallDelayStopTime;
    }
    
    private bool CanDash()
    {
        return Time.time >= dashCooldownStopTime;
    }

    private void Flip()
    {
        Vector3 scale = transform.localScale;
        transform.localScale = new Vector3(scale.x * -1, scale.y, scale.z);
    }

    private void OnDisable()
    {
        runParticles.Stop();
        wallSlideParticles.Stop();
    }
}
