using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacterController : MonoBehaviour
{
    // public Camera PersCamera;
    // public Camera OrthCamera;
    public Camera[] PlayerCameras;
    // private int CamNum = 0;
    // private Camera PlayerCamera;
    // public AudioSource AudioSource;
    public float GravityDownForce = 20f;
    public LayerMask GroundCheckLayers = -1;
    public float GroundCheckDistance = 0.05f;
    public float MaxSpeedOnGround = 10f;
    public float MovementSharpnessOnGround = 15;
    public float MaxSpeedCrouchedRatio = 0.5f;
    public float MaxSpeedInAir = 10f;
    public float AccelerationSpeedInAir = 25f;
    public float SprintSpeedModifier = 2f;
    public float KillHeight = -50f;
    public float RotationSpeed = 200f;
    public float AimingRotationMultiplier = 0.4f;
    public float JumpForce = 9f;
    public float CameraHeightRatio = 0.9f;
    public float CapsuleHeightStanding = 1.8f;
    public float CapsuleHeightCrouching = 5.9f;
    public float CrouchingSharpness = 10f;
    public float FootstepSfxFrequency = 1f;
    public float FootstepSfxFrequencyWhileSprinting = 1f;
    // public AudioClip FootstepSfx;
    // public AudioClip FallDamageSfx;
    // public bool RecievesFallDamage;
    public float MinSpeedForFallDamage = 10f;
    public float MaxSpeedForFallDamage = 30f;
    public float FallDamageAtMinSpeed = 10f;
    public float FallDamageAtMaxSpeed = 50f;
    // public UnityAction<bool> OnStanceChanged;
    public Vector3 CharacterVelocity { get; set; }
    public bool IsGrounded { get; private set; }
    public bool HasJumpedThisFrame { get; private set; }
    // public bool IsDead { get; private set; }
    public bool IsCrouching { get; private set; }

    public float RotationMultiplier
    {
        get
        {
            // if (m_WeaponsManager.IsAiming)
            // {
            //     return AimingRotationMultiplier;
            // }

            return 1f;
        }
    }

    // Health m_Health;
    PlayerInputHandler m_InputHandler;
    CharacterController m_Controller;
    // PlayerWeaponsManager m_WeaponsManager;
    // Actor m_Actor;
    Vector3 m_GroundNormal;
    // Vector3 m_CharacterVelocity;
    Vector3 m_LatestImpactSpeed;
    float m_LastTimeJumped = 0f;
    float m_CameraVerticalAngle = 0f;
    // float m_FootstepDistanceCounter;
    float m_TargetCharacterHeight;

    const float k_JumpGroundingPreventionTime = 0.2f;
    const float k_GroundCheckDistanceInAir = 0.07f;

    // Start is called before the first frame update
    void Start()
    {
        m_Controller = GetComponent<CharacterController>();
        m_InputHandler = GetComponent<PlayerInputHandler>();

        m_TargetCharacterHeight = CapsuleHeightCrouching;
        UpdateCharacterHeight(true);

        // CamNum = 0;
        // PersCamera.enabled = true;
        // OrthCamera.enabled = false;
        // PlayerCamera = PersCamera;
        PlayerCameras[0].gameObject.SetActive(true);
        PlayerCameras[1].gameObject.SetActive(true);
        // print(PlayerCamera);
    }

    // Update is called once per frame
    void Update()
    {
        HasJumpedThisFrame = false;

        // bool wasGrounded = IsGrounded;
        GroundCheck();
        

        UpdateCharacterHeight(false);

        HandleCharacterMovement();
        // SetCrouchingState(false, true);
    }

    void GroundCheck()
    {
        // Make sure that the ground check distance while already in air is very small, to prevent suddenly snapping to ground
        float chosenGroundCheckDistance = 0.2f;
           // IsGrounded ? (m_Controller.skinWidth + GroundCheckDistance) : k_GroundCheckDistanceInAir;

        // reset values before the ground check
        IsGrounded = false;
        m_GroundNormal = Vector3.up;

        // only try to detect ground if it's been a short amount of time since last jump; otherwise we may snap to the ground instantly after we try jumping
        if (Time.time >= m_LastTimeJumped + k_JumpGroundingPreventionTime)
        {
            // if we're grounded, collect info about the ground normal with a downward capsule cast representing our character capsule
            if (Physics.CapsuleCast(GetCapsuleBottomHemisphere(), GetCapsuleTopHemisphere(m_Controller.height),
                m_Controller.radius, Vector3.down, out RaycastHit hit, chosenGroundCheckDistance, GroundCheckLayers,
                QueryTriggerInteraction.Ignore))
            {
                // storing the upward direction for the surface found
                m_GroundNormal = hit.normal;

                // Only consider this a valid ground hit if the ground normal goes in the same direction as the character up
                // and if the slope angle is lower than the character controller's limit
                if (Vector3.Dot(hit.normal, transform.up) > 0f &&
                    IsNormalUnderSlopeLimit(m_GroundNormal))
                {
                    IsGrounded = true;

                    // handle snapping to the ground
                    if (hit.distance > m_Controller.skinWidth)
                    {
                        m_Controller.Move(Vector3.down * hit.distance);
                    }
                }
            }
        }
    }
    void HandleCharacterMovement()
    {
        // horizontal character rotation
        {
            // rotate the transform with the input speed around its local Y axis
            transform.Rotate(
                new Vector3(0f, (m_InputHandler.GetLookInputsHorizontal() * RotationSpeed * RotationMultiplier),
                    0f), Space.Self);
        }

        // vertical camera rotation
        {
            // add vertical inputs to the camera's vertical angle
            m_CameraVerticalAngle += m_InputHandler.GetLookInputsVertical() * RotationSpeed * RotationMultiplier;

            // limit the camera's vertical angle to min/max
            m_CameraVerticalAngle = Mathf.Clamp(m_CameraVerticalAngle, -89f, 89f);

            // apply the vertical angle as a local rotation to the camera transform along its right axis (makes it pivot up and down)

            // PlayerCameras[CamNum].transform.localEulerAngles = new Vector3(m_CameraVerticalAngle, 0, 0);
            PlayerCameras[0].transform.localEulerAngles = new Vector3(m_CameraVerticalAngle, 0, 0);
            PlayerCameras[1].transform.localEulerAngles = new Vector3(m_CameraVerticalAngle, 0, 0);
        }

        // bool isSprinting = false;
        float speedModifier = 1f;

        // converts move input to a worldspace vector based on our character's transform orientation
        Vector3 worldspaceMoveInput = transform.TransformVector(m_InputHandler.GetMoveInput());

        // Debug.Log("IsGrounded???");
        // handle grounded movement
        if (IsGrounded)
        {
            // calculate the desired velocity from inputs, max speed, and current slope
            Vector3 targetVelocity = worldspaceMoveInput * MaxSpeedOnGround * speedModifier;
            // reduce speed if crouching by crouch speed ratio
            // targetVelocity = GetDirectionReorientedOnSlope(targetVelocity.normalized, m_GroundNormal) *
            //                     targetVelocity.magnitude;

            // smoothly interpolate between our current velocity and the target velocity based on acceleration speed
            CharacterVelocity = Vector3.Lerp(CharacterVelocity, targetVelocity,
                MovementSharpnessOnGround * Time.deltaTime);

            // jumping
            // Debug.Log("IsGrounded");

            if (IsGrounded && m_InputHandler.GetJumpInputDown())
            {
                // // force the crouch state to false
                // if (SetCrouchingState(false, false))
                // {
                // start by canceling out the vertical component of our velocity
                CharacterVelocity = new Vector3(CharacterVelocity.x, 0f, CharacterVelocity.z);

                // then, add the jumpSpeed value upwards
                CharacterVelocity += Vector3.up * JumpForce;

                // // play sound
                // AudioSource.PlayOneShot(JumpSfx);

                // remember last time we jumped because we need to prevent snapping to ground for a short time
                m_LastTimeJumped = Time.time;
                HasJumpedThisFrame = true;

                // Force grounding to false
                IsGrounded = false;
                m_GroundNormal = Vector3.up;
                // }
            }

            // footsteps sound
            // float chosenFootstepSfxFrequency =
            //     (isSprinting ? FootstepSfxFrequencyWhileSprinting : FootstepSfxFrequency);
            // if (m_FootstepDistanceCounter >= 1f / chosenFootstepSfxFrequency)
            // {
            //     m_FootstepDistanceCounter = 0f;
            //     AudioSource.PlayOneShot(FootstepSfx);
            // }

            // keep track of distance traveled for footsteps sound
            // m_FootstepDistanceCounter += CharacterVelocity.magnitude * Time.deltaTime;
        }
        // handle air movement
        else
        {
            // add air acceleration
            CharacterVelocity += worldspaceMoveInput * AccelerationSpeedInAir * Time.deltaTime;

            // limit air speed to a maximum, but only horizontally
            float verticalVelocity = CharacterVelocity.y;
            Vector3 horizontalVelocity = Vector3.ProjectOnPlane(CharacterVelocity, Vector3.up);
            horizontalVelocity = Vector3.ClampMagnitude(horizontalVelocity, MaxSpeedInAir * speedModifier);
            CharacterVelocity = horizontalVelocity + (Vector3.up * verticalVelocity);

            // apply the gravity to the velocity
            CharacterVelocity += Vector3.down * GravityDownForce * Time.deltaTime;
        }
        // }

        // apply the final calculated velocity value as a character movement
        Vector3 capsuleBottomBeforeMove = GetCapsuleBottomHemisphere();
        Vector3 capsuleTopBeforeMove = GetCapsuleTopHemisphere(m_Controller.height);
        m_Controller.Move(CharacterVelocity * Time.deltaTime);

        // detect obstructions to adjust velocity accordingly
        m_LatestImpactSpeed = Vector3.zero;
        if (Physics.CapsuleCast(capsuleBottomBeforeMove, capsuleTopBeforeMove, m_Controller.radius,
            CharacterVelocity.normalized, out RaycastHit hit, CharacterVelocity.magnitude * Time.deltaTime, -1,
            QueryTriggerInteraction.Ignore))
        {
            // We remember the last impact speed because the fall damage logic might need it
            m_LatestImpactSpeed = CharacterVelocity;

            CharacterVelocity = Vector3.ProjectOnPlane(CharacterVelocity, hit.normal);
        }
    }
    
    // Returns true if the slope angle represented by the given normal is under the slope angle limit of the character controller
    bool IsNormalUnderSlopeLimit(Vector3 normal)
    {
        return Vector3.Angle(transform.up, normal) <= m_Controller.slopeLimit;
    }

    // Gets the center point of the bottom hemisphere of the character controller capsule    
    Vector3 GetCapsuleBottomHemisphere()
    {
        return transform.position + (transform.up * m_Controller.radius);
    }

    // Gets the center point of the top hemisphere of the character controller capsule    
    Vector3 GetCapsuleTopHemisphere(float atHeight)
    {
        return transform.position + (transform.up * (atHeight - m_Controller.radius));
    }

    void UpdateCharacterHeight(bool force)
    {
        // Update height instantly
        if (force)
        {
            m_Controller.height = m_TargetCharacterHeight;
            m_Controller.center = Vector3.up * m_Controller.height * 0.5f;
            // PlayerCameras[CamNum].transform.localPosition = Vector3.up * m_TargetCharacterHeight * CameraHeightRatio;
            PlayerCameras[0].transform.localPosition = Vector3.up * m_TargetCharacterHeight * CameraHeightRatio;
            PlayerCameras[1].transform.localPosition = Vector3.up * m_TargetCharacterHeight * CameraHeightRatio;
            // m_Actor.AimPoint.transform.localPosition = m_Controller.center;
        }
        // Update smooth height
        else if (m_Controller.height != m_TargetCharacterHeight)
        {
            // resize the capsule and adjust camera position
            m_Controller.height = Mathf.Lerp(m_Controller.height, m_TargetCharacterHeight,
                CrouchingSharpness * Time.deltaTime);
            m_Controller.center = Vector3.up * m_Controller.height * 0.5f;
            // PlayerCameras[CamNum].transform.localPosition = Vector3.Lerp(PlayerCameras[CamNum].transform.localPosition,
            //     Vector3.up * m_TargetCharacterHeight * CameraHeightRatio, CrouchingSharpness * Time.deltaTime);
            PlayerCameras[0].transform.localPosition = Vector3.Lerp(PlayerCameras[0].transform.localPosition,
                Vector3.up * m_TargetCharacterHeight * CameraHeightRatio, CrouchingSharpness * Time.deltaTime);
            PlayerCameras[1].transform.localPosition = Vector3.Lerp(PlayerCameras[1].transform.localPosition,
                Vector3.up * m_TargetCharacterHeight * CameraHeightRatio, CrouchingSharpness * Time.deltaTime);
            // m_Actor.AimPoint.transform.localPosition = m_Controller.center;
        }
    }
}
