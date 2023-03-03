using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class GamepadVibrations : MonoBehaviour
{
    [SerializeField] [Min(0)] private float onHitVibrationIntensity;
    [SerializeField] [Min(0)] private float onHitVibrationDuration;
    [SerializeField] [Min(0)] private float onDieVibrationIntensity;
    [SerializeField] [Min(0)] private float onDieVibrationDuration;
    [SerializeField] [Min(0)] private float onLandVibrationIntensity;
    [SerializeField] [Min(0)] private float onLandVibrationDuration;
    public static GamepadVibrations Instance;

    private bool onHitVibrate = false;
    private bool onDieVibrate = false;

    private void Awake()
    {
        Debug.Assert(Instance == null);
        Instance = this;
    }

    public void OnHit()
    {
        if (!onDieVibrate)
        {
            onHitVibrate = true;
            StartCoroutine(GamepadVibrationCo(onHitVibrationIntensity, onHitVibrationDuration));
        }
    }

    public void OnDie()
    {
        onDieVibrate = true;
        StartCoroutine(GamepadVibrationCo(onDieVibrationIntensity, onDieVibrationDuration));
    }

    public void OnLand()
    {
        if(!onHitVibrate && !onDieVibrate)
            StartCoroutine(GamepadVibrationCo(onLandVibrationIntensity, onLandVibrationDuration));
    }

    private IEnumerator GamepadVibrationCo(float intensity, float duration)
    {
        if (Gamepad.current != null && FeedbackController.Instance.VibrationsEffect)
        {
            Gamepad.current.SetMotorSpeeds(intensity, intensity);
            yield return new WaitForSeconds(duration);
            Gamepad.current.SetMotorSpeeds(0f, 0f);
        }

        onHitVibrate = false;
        onDieVibrate = false;
        yield return null;
    }
}