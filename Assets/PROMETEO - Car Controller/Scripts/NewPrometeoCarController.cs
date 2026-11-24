using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class NewPrometeoCarController : MonoBehaviour
{
    // CAR SETUP
    [Header("CAR SETUP")]
    [Range(20, 190)]
    public int maxSpeed = 90;
    [Range(10, 120)]
    public int maxReverseSpeed = 45;
    [Range(1, 10)]
    public int accelerationMultiplier = 2;
    [Space(10)]
    [Range(10, 45)]
    public int maxSteeringAngle = 27;
    [Range(0.1f, 1f)]
    public float steeringSpeed = 0.5f;
    [Space(10)]
    [Range(100, 3000)]
    public int brakeForce = 1000;
    [Range(0f, 35f)]
    [Tooltip("Desaceleración asistida (m/s²) del freno normal al 100% de presión del gatillo.")]
    public float normalBrakeDeceleration = 14f;
    [Range(1, 10)]
    public int decelerationMultiplier = 2;
    [Range(1, 10)]
    public int handbrakeDriftMultiplier = 5;
    [Space(10)]
    public Vector3 bodyMassCenter;

    // HANDBRAKE & DRIFT SETUP
    [Header("HANDBRAKE & DRIFT")]
    [Range(0, 1200)]
    [Tooltip("Frenado suave en ruedas traseras al derrapar en giro sin acelerar.")]
    public int handbrakeDriftBrakeForce = 200;
    [Range(0f, 15f)]
    [Tooltip("Resistencia suave (m/s²) al derrapar en curva para evitar que el auto acelere mientras el freno de mano está activo.")]
    public float handbrakeDriftDrag = 3f;
    [Range(0f, 1f)]
    [Tooltip("Porcentaje de torque del motor usado para sostener la velocidad durante un derrape en curva.")]
    public float driftThrottleFactor = 0.75f;
    [Range(0f, 1f)]
    [Tooltip("Proporción de pérdida de agarre lateral en ruedas delanteras respecto a traseras.")]
    public float frontDriftSlipRatio = 0.25f;
    [Space(10)]
    [Range(500, 4000)]
    [Tooltip("Fuerza de frenado al accionar solo el freno de mano en trayecto recto.")]
    public int handbrakeStraightBrakeForce = 1800;
    [Range(0f, 50f)]
    [Tooltip("Desaceleración (m/s²) al accionar el freno de mano yendo recto (anula el acelerador).")]
    public float handbrakeStraightDeceleration = 24f;
    [Space(10)]
    [Range(1000, 6000)]
    [Tooltip("Fuerza de frenado en las 4 ruedas al combinar Freno de Mano + Freno normal.")]
    public int handbrakeEmergencyForce = 3500;
    [Range(0f, 60f)]
    [Tooltip("Desaceleración máxima (m/s²) al combinar Freno de Mano + Freno normal.")]
    public float handbrakeEmergencyDeceleration = 38f;

    // WHEELS
    [Header("WHEELS")]
    public GameObject frontLeftMesh;
    public WheelCollider frontLeftCollider;
    [Space(10)]
    public GameObject frontRightMesh;
    public WheelCollider frontRightCollider;
    [Space(10)]
    public GameObject rearLeftMesh;
    public WheelCollider rearLeftCollider;
    [Space(10)]
    public GameObject rearRightMesh;
    public WheelCollider rearRightCollider;

    // PARTICLE SYSTEMS
    [Header("EFFECTS")]
    public bool useEffects = false;
    public ParticleSystem RLWParticleSystem;
    public ParticleSystem RRWParticleSystem;
    [Space(10)]
    public TrailRenderer RLWTireSkid;
    public TrailRenderer RRWTireSkid;

    // SPEED TEXT (UI)
    [Header("UI")]
    public bool useUI = false;
    public Text carSpeedText;

    // SOUNDS
    [Header("SOUNDS")]
    public bool useSounds = false;
    public AudioSource carEngineSound;
    public AudioSource tireScreechSound;
    float initialCarEngineSoundPitch;

    // NEW INPUT SYSTEM (INPUT ACTION REFERENCES)
    [Header("NEW INPUT SYSTEM")]
    [Tooltip("Acción Value (Axis / Float de 0 a 1) para acelerar (ej. Right Trigger / tecla W).")]
    public InputActionReference throttleAction;
    [Tooltip("Acción Value (Axis / Float de 0 a 1) para frenar y reversa (ej. Left Trigger / tecla S).")]
    public InputActionReference brakeReverseAction;
    [Tooltip("Acción Value (Axis / Float de -1 a 1) para dirección (ej. Left Stick X / 1D Axis A-D).")]
    public InputActionReference steeringAction;
    [Tooltip("Acción Button para el freno de mano (ej. Space / Button South).")]
    public InputActionReference handbrakeAction;

    // TOUCH CONTROLS
    [Header("TOUCH CONTROLS")]
    public bool useTouchControls = false;
    public GameObject throttleButton;
    PrometeoTouchInput throttlePTI;
    public GameObject reverseButton;
    PrometeoTouchInput reversePTI;
    public GameObject turnRightButton;
    PrometeoTouchInput turnRightPTI;
    public GameObject turnLeftButton;
    PrometeoTouchInput turnLeftPTI;
    public GameObject handbrakeButton;
    PrometeoTouchInput handbrakePTI;

    // CAR DATA
    [HideInInspector]
    public float carSpeed;
    [HideInInspector]
    public bool isDrifting;
    [HideInInspector]
    public bool isTractionLocked;

    // PRIVATE VARIABLES
    Rigidbody carRigidbody;
    float realCarSpeedKmh;
    float driftSpeedCapKmh;
    float steeringAxis;
    float throttleAxis;
    float driftingAxis;
    float localVelocityZ;
    float localVelocityX;
    bool deceleratingCar;
    bool wasHandbraking;
    bool touchControlsSetup = false;

    WheelFrictionCurve FLwheelFriction;
    float FLWextremumSlip;
    WheelFrictionCurve FRwheelFriction;
    float FRWextremumSlip;
    WheelFrictionCurve RLwheelFriction;
    float RLWextremumSlip;
    WheelFrictionCurve RRwheelFriction;
    float RRWextremumSlip;

    private void OnEnable()
    {
        throttleAction?.action?.Enable();
        brakeReverseAction?.action?.Enable();
        steeringAction?.action?.Enable();
        handbrakeAction?.action?.Enable();
    }

    private void OnDisable()
    {
        throttleAction?.action?.Disable();
        brakeReverseAction?.action?.Disable();
        steeringAction?.action?.Disable();
        handbrakeAction?.action?.Disable();
    }

    void Start()
    {
        carRigidbody = GetComponent<Rigidbody>();
        carRigidbody.centerOfMass = bodyMassCenter;

        FLwheelFriction = CopyFriction(frontLeftCollider.sidewaysFriction, out FLWextremumSlip);
        FRwheelFriction = CopyFriction(frontRightCollider.sidewaysFriction, out FRWextremumSlip);
        RLwheelFriction = CopyFriction(rearLeftCollider.sidewaysFriction, out RLWextremumSlip);
        RRwheelFriction = CopyFriction(rearRightCollider.sidewaysFriction, out RRWextremumSlip);

        if (carEngineSound != null)
        {
            initialCarEngineSoundPitch = carEngineSound.pitch;
        }

        if (useUI)
        {
            InvokeRepeating(nameof(CarSpeedUI), 0f, 0.1f);
        }
        else if (carSpeedText != null)
        {
            carSpeedText.text = "0";
        }

        if (useSounds)
        {
            InvokeRepeating(nameof(CarSounds), 0f, 0.1f);
        }
        else
        {
            if (carEngineSound != null) carEngineSound.Stop();
            if (tireScreechSound != null) tireScreechSound.Stop();
        }

        if (!useEffects)
        {
            StopEffects();
        }

        if (useTouchControls)
        {
            if (throttleButton != null && reverseButton != null &&
                turnRightButton != null && turnLeftButton != null && handbrakeButton != null)
            {
                throttlePTI = throttleButton.GetComponent<PrometeoTouchInput>();
                reversePTI = reverseButton.GetComponent<PrometeoTouchInput>();
                turnLeftPTI = turnLeftButton.GetComponent<PrometeoTouchInput>();
                turnRightPTI = turnRightButton.GetComponent<PrometeoTouchInput>();
                handbrakePTI = handbrakeButton.GetComponent<PrometeoTouchInput>();
                touchControlsSetup = true;
            }
            else
            {
                Debug.LogWarning("Touch controls are not completely set up. Assign all scene buttons in PrometeoCarController.");
            }
        }
    }

    void Update()
    {
        // CAR DATA
        carSpeed = (2f * Mathf.PI * frontLeftCollider.radius * frontLeftCollider.rpm * 60f) / 1000f;
        realCarSpeedKmh = carRigidbody.linearVelocity.magnitude * 3.6f;
        localVelocityX = transform.InverseTransformDirection(carRigidbody.linearVelocity).x;
        localVelocityZ = transform.InverseTransformDirection(carRigidbody.linearVelocity).z;

        // CAR PHYSICS
        if (useTouchControls && touchControlsSetup)
        {
            ProcessTouchControls();
        }
        else
        {
            ProcessInputSystemControls();
        }

        AnimateWheelMeshes();
    }

    private void ProcessInputSystemControls()
    {
        float throttleAmt = Mathf.Clamp01(throttleAction?.action?.ReadValue<float>() ?? 0f);
        float brakeRevAmt = Mathf.Clamp01(brakeReverseAction?.action?.ReadValue<float>() ?? 0f);
        float steerAmt = Mathf.Clamp(steeringAction?.action?.ReadValue<float>() ?? 0f, -1f, 1f);
        bool isHandbraking = handbrakeAction?.action != null && handbrakeAction.action.IsPressed();
        bool handbrakeReleased = handbrakeAction?.action != null && handbrakeAction.action.WasReleasedThisFrame();

        if (isHandbraking && !wasHandbraking)
        {
            driftSpeedCapKmh = realCarSpeedKmh;
        }
        wasHandbraking = isHandbraking;

        bool hasThrottle = throttleAmt > 0.02f;
        bool hasBrakeOrReverse = brakeRevAmt > 0.02f;

        if (hasThrottle || hasBrakeOrReverse || isHandbraking)
        {
            CancelInvoke(nameof(DecelerateCar));
            deceleratingCar = false;
        }

        // 1. Freno de mano + Freno normal -> Frenado de emergencia máximo
        if (isHandbraking && hasBrakeOrReverse)
        {
            EmergencyHandbrake(brakeRevAmt);
        }
        // 2. Solo Freno de mano -> Frena en recta (incluso acelerando) y derrapa controlado en curva
        else if (isHandbraking)
        {
            DriftHandbrake(hasThrottle ? throttleAmt : 0f, steerAmt);
        }
        // 3. Freno normal avanzando
        else if (hasBrakeOrReverse && localVelocityZ > 1f)
        {
            BrakesAnalog(brakeRevAmt);
        }
        // 4. Aceleración normal
        else if (hasThrottle)
        {
            GoForwardAnalog(throttleAmt);
        }
        // 5. Reversa normal
        else if (hasBrakeOrReverse)
        {
            GoReverseAnalog(brakeRevAmt);
        }
        // 6. Sin pedales activos
        else
        {
            ThrottleOff();
            SetBrakeTorque(0f);
        }

        if (handbrakeReleased)
        {
            RecoverTraction();
        }

        // Dirección analógica
        if (Mathf.Abs(steerAmt) > 0.05f)
        {
            SteerAnalog(steerAmt);
        }
        else if (steeringAxis != 0f)
        {
            ResetSteeringAngle();
        }

        // Desaceleración progresiva al soltar todos los controles
        if (!hasThrottle && !hasBrakeOrReverse && !isHandbraking && !deceleratingCar)
        {
            InvokeRepeating(nameof(DecelerateCar), 0f, 0.1f);
            deceleratingCar = true;
        }
    }

    private void ProcessTouchControls()
    {
        float touchSteer = 0f;
        if (turnLeftPTI.buttonPressed) touchSteer = -1f;
        else if (turnRightPTI.buttonPressed) touchSteer = 1f;

        bool isHandbraking = handbrakePTI.buttonPressed;
        if (isHandbraking && !wasHandbraking)
        {
            driftSpeedCapKmh = realCarSpeedKmh;
        }
        wasHandbraking = isHandbraking;

        if (isHandbraking)
        {
            CancelInvoke(nameof(DecelerateCar));
            deceleratingCar = false;

            if (reversePTI.buttonPressed)
            {
                EmergencyHandbrake(1f);
            }
            else
            {
                DriftHandbrake(throttlePTI.buttonPressed ? 1f : 0f, touchSteer);
            }
        }
        else
        {
            if (isTractionLocked || driftingAxis > 0f)
            {
                RecoverTraction();
            }

            if (throttlePTI.buttonPressed) { CancelInvoke(nameof(DecelerateCar)); deceleratingCar = false; GoForwardAnalog(1f); }
            else if (reversePTI.buttonPressed) { CancelInvoke(nameof(DecelerateCar)); deceleratingCar = false; GoReverseAnalog(1f); }
            else { ThrottleOff(); SetBrakeTorque(0f); }
        }

        if (Mathf.Abs(touchSteer) > 0f) { SteerAnalog(touchSteer); }
        else if (steeringAxis != 0f) { ResetSteeringAngle(); }

        if (!reversePTI.buttonPressed && !throttlePTI.buttonPressed && !isHandbraking && !deceleratingCar)
        {
            InvokeRepeating(nameof(DecelerateCar), 0f, 0.1f);
            deceleratingCar = true;
        }
    }

    // UI & SOUNDS
    public void CarSpeedUI()
    {
        if (!useUI || carSpeedText == null) return;
        try
        {
            carSpeedText.text = Mathf.RoundToInt(realCarSpeedKmh).ToString();
        }
        catch (Exception ex) { Debug.LogWarning(ex); }
    }

    public void CarSounds()
    {
        if (useSounds)
        {
            try
            {
                if (carEngineSound != null)
                {
                    carEngineSound.pitch = initialCarEngineSoundPitch + (Mathf.Abs(carRigidbody.linearVelocity.magnitude) / 25f);
                }

                if (tireScreechSound != null)
                {
                    if (isDrifting || (isTractionLocked && realCarSpeedKmh > 5f))
                    {
                        if (!tireScreechSound.isPlaying) tireScreechSound.Play();
                    }
                    else
                    {
                        tireScreechSound.Stop();
                    }
                }
            }
            catch (Exception ex) { Debug.LogWarning(ex); }
        }
        else
        {
            if (carEngineSound != null && carEngineSound.isPlaying) carEngineSound.Stop();
            if (tireScreechSound != null && tireScreechSound.isPlaying) tireScreechSound.Stop();
        }
    }

    // STEERING METHODS
    public void SteerAnalog(float targetDirection)
    {
        targetDirection = Mathf.Clamp(targetDirection, -1f, 1f);
        steeringAxis = Mathf.MoveTowards(steeringAxis, targetDirection, Time.deltaTime * 10f * steeringSpeed);
        ApplySteeringAngle();
    }

    public void TurnLeft() => SteerAnalog(-1f);
    public void TurnRight() => SteerAnalog(1f);

    public void ResetSteeringAngle()
    {
        steeringAxis = Mathf.MoveTowards(steeringAxis, 0f, Time.deltaTime * 10f * steeringSpeed);
        if (Mathf.Abs(frontLeftCollider.steerAngle) < 1f && Mathf.Abs(steeringAxis) < 0.05f)
        {
            steeringAxis = 0f;
        }
        ApplySteeringAngle();
    }

    private void ApplySteeringAngle()
    {
        float steeringAngle = steeringAxis * maxSteeringAngle;
        frontLeftCollider.steerAngle = Mathf.Lerp(frontLeftCollider.steerAngle, steeringAngle, steeringSpeed);
        frontRightCollider.steerAngle = Mathf.Lerp(frontRightCollider.steerAngle, steeringAngle, steeringSpeed);
    }

    void AnimateWheelMeshes()
    {
        try
        {
            SyncWheel(frontLeftCollider, frontLeftMesh);
            SyncWheel(frontRightCollider, frontRightMesh);
            SyncWheel(rearLeftCollider, rearLeftMesh);
            SyncWheel(rearRightCollider, rearRightMesh);
        }
        catch (Exception ex) { Debug.LogWarning(ex); }
    }

    private void SyncWheel(WheelCollider col, GameObject mesh)
    {
        if (col == null || mesh == null) return;
        col.GetWorldPose(out Vector3 pos, out Quaternion rot);
        mesh.transform.position = pos;
        mesh.transform.rotation = rot;
    }

    // ENGINE & BRAKING METHODS
    public void GoForwardAnalog(float pressure)
    {
        pressure = Mathf.Clamp01(pressure);
        UpdateDriftState();

        throttleAxis = Mathf.MoveTowards(throttleAxis, pressure, Time.deltaTime * 3f);

        if (localVelocityZ < -1f)
        {
            BrakesAnalog(pressure);
        }
        else if (Mathf.RoundToInt(carSpeed) < maxSpeed * pressure)
        {
            SetBrakeTorque(0f);
            SetMotorTorque((accelerationMultiplier * 50f) * throttleAxis);
        }
        else
        {
            SetBrakeTorque(0f);
            SetMotorTorque(0f);
        }
    }

    public void GoReverseAnalog(float pressure)
    {
        pressure = Mathf.Clamp01(pressure);
        UpdateDriftState();

        throttleAxis = Mathf.MoveTowards(throttleAxis, -pressure, Time.deltaTime * 3f);

        if (localVelocityZ > 1f)
        {
            BrakesAnalog(pressure);
        }
        else if (Mathf.Abs(Mathf.RoundToInt(carSpeed)) < maxReverseSpeed * pressure)
        {
            SetBrakeTorque(0f);
            SetMotorTorque((accelerationMultiplier * 50f) * throttleAxis);
        }
        else
        {
            SetBrakeTorque(0f);
            SetMotorTorque(0f);
        }
    }

    public void BrakesAnalog(float pressure)
    {
        pressure = Mathf.Clamp01(pressure);
        UpdateDriftState();
        SetMotorTorque(0f);
        SetBrakeTorque(brakeForce * pressure);
        ApplyHorizontalDeceleration(normalBrakeDeceleration * pressure);
    }

    public void GoForward() => GoForwardAnalog(1f);
    public void GoReverse() => GoReverseAnalog(1f);
    public void Brakes() => BrakesAnalog(1f);
    public void Handbrake() => DriftHandbrake(0f, 0f);

    public void ThrottleOff()
    {
        SetMotorTorque(0f);
    }

    public void DecelerateCar()
    {
        UpdateDriftState();

        if (throttleAxis != 0f)
        {
            throttleAxis = Mathf.MoveTowards(throttleAxis, 0f, Time.deltaTime * 10f);
            if (Mathf.Abs(throttleAxis) < 0.15f) throttleAxis = 0f;
        }

        carRigidbody.linearVelocity *= 1f / (1f + (0.025f * decelerationMultiplier));
        SetMotorTorque(0f);

        if (carRigidbody.linearVelocity.magnitude < 0.25f)
        {
            carRigidbody.linearVelocity = Vector3.zero;
            CancelInvoke(nameof(DecelerateCar));
        }
    }

    // Freno de mano adaptativo: frena firme en recta (incluso acelerando) y sostiene derrapes en curva sin acelerar
    public void DriftHandbrake(float throttlePressure, float steerInput)
    {
        CancelInvoke(nameof(RecoverTraction));
        throttlePressure = Mathf.Clamp01(throttlePressure);

        // 0 = trayecto recto, 1 = girando activamente o deslizando de costado
        float steerMagnitude = Mathf.Max(Mathf.Abs(steerInput), Mathf.Abs(steeringAxis));
        float lateralSlipFactor = Mathf.InverseLerp(2.5f, 6f, Mathf.Abs(localVelocityX));
        float driftIntent = Mathf.Clamp01(Mathf.InverseLerp(0.15f, 0.7f, steerMagnitude) + lateralSlipFactor);

        // 1. Trayecto recto: el freno de mano siempre corta el motor y frena
        if (driftIntent < 0.2f)
        {
            throttleAxis = Mathf.MoveTowards(throttleAxis, 0f, Time.deltaTime * 8f);
            SetMotorTorque(0f);

            frontLeftCollider.brakeTorque = handbrakeStraightBrakeForce * 0.6f;
            frontRightCollider.brakeTorque = handbrakeStraightBrakeForce * 0.6f;
            rearLeftCollider.brakeTorque = handbrakeStraightBrakeForce;
            rearRightCollider.brakeTorque = handbrakeStraightBrakeForce;

            ApplyHorizontalDeceleration(handbrakeStraightDeceleration);
            driftSpeedCapKmh = Mathf.Min(driftSpeedCapKmh, realCarSpeedKmh);
        }
        // 2. Girando + acelerando con impulso previo: sostiene el derrape sin superar la velocidad de entrada
        else if (throttlePressure > 0.02f && localVelocityZ >= -1f && driftSpeedCapKmh > 8f)
        {
            SetBrakeTorque(0f);

            float targetDriftSpeed = Mathf.Min(driftSpeedCapKmh * 0.92f, maxSpeed * throttlePressure);
            if (realCarSpeedKmh < targetDriftSpeed)
            {
                throttleAxis = Mathf.MoveTowards(throttleAxis, throttlePressure, Time.deltaTime * 3f);
                float torque = (accelerationMultiplier * 50f) * throttleAxis * driftThrottleFactor * driftIntent;
                SetMotorTorque(torque);
            }
            else
            {
                SetMotorTorque(0f);
                ApplyHorizontalDeceleration(handbrakeDriftDrag);
            }
        }
        // 3. Girando sin acelerador (o detenido con el freno de mano puesto)
        else
        {
            throttleAxis = Mathf.MoveTowards(throttleAxis, 0f, Time.deltaTime * 6f);
            SetMotorTorque(0f);

            float frontBrake = Mathf.Lerp(handbrakeStraightBrakeForce * 0.6f, 0f, driftIntent);
            float rearBrake = Mathf.Lerp(handbrakeStraightBrakeForce, handbrakeDriftBrakeForce, driftIntent);

            frontLeftCollider.brakeTorque = frontBrake;
            frontRightCollider.brakeTorque = frontBrake;
            rearLeftCollider.brakeTorque = rearBrake;
            rearRightCollider.brakeTorque = rearBrake;

            float decel = Mathf.Lerp(handbrakeStraightDeceleration, handbrakeDriftDrag, driftIntent);
            ApplyHorizontalDeceleration(decel);
            driftSpeedCapKmh = Mathf.Min(driftSpeedCapKmh, realCarSpeedKmh);
        }

        // Reducir agarre lateral solo en proporción al giro para evitar coletazos involuntarios en línea recta
        driftingAxis = Mathf.Min(1f, driftingAxis + Time.deltaTime * 1.5f);
        float minAxis = 1f / handbrakeDriftMultiplier;
        if (driftingAxis < minAxis)
        {
            driftingAxis = minAxis;
        }

        float rearMultiplier = Mathf.Lerp(1f, Mathf.Max(1f, handbrakeDriftMultiplier * driftingAxis), driftIntent);
        float frontMultiplier = Mathf.Lerp(1f, rearMultiplier, frontDriftSlipRatio);
        ApplyDriftFriction(rearMultiplier, frontMultiplier);

        isDrifting = Mathf.Abs(localVelocityX) > 2.5f && realCarSpeedKmh > 5f;
        isTractionLocked = true;
        DriftCarPS();
    }

    // Freno de mano + Freno normal -> Detención máxima inmediata
    public void EmergencyHandbrake(float brakePressure)
    {
        CancelInvoke(nameof(RecoverTraction));
        brakePressure = Mathf.Clamp01(brakePressure);

        throttleAxis = 0f;
        SetMotorTorque(0f);

        float effectiveFactor = Mathf.Lerp(0.6f, 1f, brakePressure);
        SetBrakeTorque(handbrakeEmergencyForce * effectiveFactor);

        driftingAxis = 0f;
        ApplyDriftFriction(1f, 1f);

        ApplyHorizontalDeceleration(handbrakeEmergencyDeceleration * effectiveFactor);
        driftSpeedCapKmh = Mathf.Min(driftSpeedCapKmh, realCarSpeedKmh);

        isDrifting = Mathf.Abs(localVelocityX) > 2.5f && realCarSpeedKmh > 5f;
        isTractionLocked = true;
        DriftCarPS();
    }

    public void RecoverTraction()
    {
        isTractionLocked = false;
        driftingAxis = Mathf.Max(0f, driftingAxis - (Time.deltaTime / 1.5f));
        float rearMultiplier = handbrakeDriftMultiplier * driftingAxis;

        if (rearMultiplier > 1f)
        {
            float frontMultiplier = Mathf.Lerp(1f, rearMultiplier, frontDriftSlipRatio);
            ApplyDriftFriction(rearMultiplier, frontMultiplier);
            Invoke(nameof(RecoverTraction), Time.deltaTime);
        }
        else
        {
            ApplyDriftFriction(1f, 1f);
            driftingAxis = 0f;
        }
    }

    public void DriftCarPS()
    {
        if (!useEffects)
        {
            StopEffects();
            return;
        }

        try
        {
            if (RLWParticleSystem != null && RRWParticleSystem != null)
            {
                if (isDrifting) { RLWParticleSystem.Play(); RRWParticleSystem.Play(); }
                else { RLWParticleSystem.Stop(); RRWParticleSystem.Stop(); }
            }
        }
        catch (Exception ex) { Debug.LogWarning(ex); }

        try
        {
            if (RLWTireSkid != null && RRWTireSkid != null)
            {
                bool emitSkid = (isTractionLocked || Mathf.Abs(localVelocityX) > 5f) && realCarSpeedKmh > 5f && IsAnyWheelGrounded();
                RLWTireSkid.emitting = emitSkid;
                RRWTireSkid.emitting = emitSkid;
            }
        }
        catch (Exception ex) { Debug.LogWarning(ex); }
    }

    // HELPERS
    private void ApplyHorizontalDeceleration(float deceleration)
    {
        if (deceleration <= 0f || !IsAnyWheelGrounded()) return;

        Vector3 horizontalVelocity = new Vector3(carRigidbody.linearVelocity.x, 0f, carRigidbody.linearVelocity.z);
        Vector3 slowedHorizontal = Vector3.MoveTowards(
            horizontalVelocity,
            Vector3.zero,
            deceleration * Time.deltaTime
        );

        if (slowedHorizontal.sqrMagnitude < 0.04f)
        {
            slowedHorizontal = Vector3.zero;
        }

        carRigidbody.linearVelocity = new Vector3(slowedHorizontal.x, carRigidbody.linearVelocity.y, slowedHorizontal.z);
    }

    private bool IsAnyWheelGrounded()
    {
        return frontLeftCollider.isGrounded ||
               frontRightCollider.isGrounded ||
               rearLeftCollider.isGrounded ||
               rearRightCollider.isGrounded;
    }

    private void UpdateDriftState()
    {
        isDrifting = Mathf.Abs(localVelocityX) > 2.5f && realCarSpeedKmh > 5f;
        DriftCarPS();
    }

    private void SetMotorTorque(float torque)
    {
        frontLeftCollider.motorTorque = torque;
        frontRightCollider.motorTorque = torque;
        rearLeftCollider.motorTorque = torque;
        rearRightCollider.motorTorque = torque;
    }

    private void SetBrakeTorque(float torque)
    {
        frontLeftCollider.brakeTorque = torque;
        frontRightCollider.brakeTorque = torque;
        rearLeftCollider.brakeTorque = torque;
        rearRightCollider.brakeTorque = torque;
    }

    private void ApplyDriftFriction(float rearMultiplier, float frontMultiplier)
    {
        FLwheelFriction.extremumSlip = FLWextremumSlip * frontMultiplier;
        frontLeftCollider.sidewaysFriction = FLwheelFriction;

        FRwheelFriction.extremumSlip = FRWextremumSlip * frontMultiplier;
        frontRightCollider.sidewaysFriction = FRwheelFriction;

        RLwheelFriction.extremumSlip = RLWextremumSlip * rearMultiplier;
        rearLeftCollider.sidewaysFriction = RLwheelFriction;

        RRwheelFriction.extremumSlip = RRWextremumSlip * rearMultiplier;
        rearRightCollider.sidewaysFriction = RRwheelFriction;
    }

    private WheelFrictionCurve CopyFriction(WheelFrictionCurve source, out float extremumSlip)
    {
        extremumSlip = source.extremumSlip;
        return new WheelFrictionCurve
        {
            extremumSlip = source.extremumSlip,
            extremumValue = source.extremumValue,
            asymptoteSlip = source.asymptoteSlip,
            asymptoteValue = source.asymptoteValue,
            stiffness = source.stiffness
        };
    }

    private void StopEffects()
    {
        if (RLWParticleSystem != null) RLWParticleSystem.Stop();
        if (RRWParticleSystem != null) RRWParticleSystem.Stop();
        if (RLWTireSkid != null) RLWTireSkid.emitting = false;
        if (RRWTireSkid != null) RRWTireSkid.emitting = false;
    }
}