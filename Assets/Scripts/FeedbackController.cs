using System;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UI;
using Toggle = UnityEngine.UI.Toggle;

public class FeedbackController : MonoBehaviour
{
    [field: Header("Toggles")]
    [field: Space]
    [SerializeField] private Toggle allToggle;
    [SerializeField] private Toggle runEffectToggle;
    [SerializeField] private Toggle runEffectOnTurnBoostToggle;
    [SerializeField] private Toggle runEffectOnGroundOnlyToggle;
    [SerializeField] private Toggle doubleJumpEffectToggle;
    [SerializeField] private Toggle wallSlideEffectToggle;
    [SerializeField] private Toggle dashEffectToggle;
    [SerializeField] private Toggle deformPlayerEffectToggle;
    [SerializeField] private Toggle trampolineBounceEffectToggle;
    [SerializeField] private Toggle hitEffectToggle;
    [SerializeField] private Toggle dieEffectToggle;
    [SerializeField] private Toggle vibrationsEffectToggle;
    [SerializeField] private Toggle cameraShakeEffectToggle;
    [SerializeField] private Toggle soundEffectsToggle;

    [field: Header("Pre-selected")]
    [field: Space]
    [SerializeField] private bool runEffect = true;
    [SerializeField] private bool runEffectOnTurnBoost = false;
    [SerializeField] private bool runEffectOnGroundOnly = true;
    [SerializeField] private bool doubleJumpEffect = true;
    [SerializeField] private bool wallSlideEffect = true;
    [SerializeField] private bool dashEffect = true;
    [SerializeField] private bool deformPlayerEffect = true;
    [SerializeField] private bool trampolineBounceEffect = true;
    [SerializeField] private bool hitEffect = true;
    [SerializeField] private bool dieEffect = true;
    [SerializeField] private bool vibrationsEffect = true;
    [SerializeField] private bool cameraShakeEffect = true;
    [SerializeField] private bool soundEffects = true;

    public bool EmitRunEffect
    {
        get => runEffectToggle.isOn;
        private set {}
    }

    public bool EmitRunEffectOnTurnBoost
    {
        get => runEffectOnTurnBoostToggle.isOn;
        private set {}
    }
    public bool EmitRunEffectOnGroundOnly
    {
        get => runEffectOnGroundOnlyToggle.isOn;
        private set {}
    }
    public bool EmitDoubleJumpEffect
    {
        get => doubleJumpEffectToggle.isOn;
        private set {}
    }
    public bool EmitWallSlideEffect
    {
        get => wallSlideEffectToggle.isOn;
        private set {}
    }
    public bool EmitDashEffect
    {
        get => dashEffectToggle.isOn;
        private set { }
    }

    public bool DeformPlayerEffect
    {
        get => deformPlayerEffectToggle.isOn;
        private set { }
    }

    public bool TrampolineBounceEffect
    {
        get => trampolineBounceEffectToggle.isOn;
        private set { }
    }

    public bool HitEffect
    {
        get => hitEffectToggle.isOn;
        private set { }
    }

    public bool DieEffect
    {
        get => dieEffectToggle.isOn;
        private set { }
    }

    public bool VibrationsEffect
    {
        get => vibrationsEffectToggle.isOn;
        private set { }
    }

    public bool CameraShakeEffect
    {
        get => cameraShakeEffectToggle.isOn;
        private set { }
    }

    public bool SoundEffects
    {
        get => soundEffectsToggle.isOn;
        private set { }
    }
    public static FeedbackController Instance;
    private bool allowEmitAll = false;

    private void Awake()
    {
        Debug.Assert(Instance == null);
        Instance = this;
        Invoke("InitToggles", .05f);
    }

    private void InitToggles()
    {
        runEffectToggle.SetIsOnWithoutNotify(GetPref("runEffectToggle", runEffect));
        runEffectOnTurnBoostToggle.SetIsOnWithoutNotify(GetPref("runEffectOnTurnBoostToggle", runEffectOnTurnBoost));
        runEffectOnGroundOnlyToggle.SetIsOnWithoutNotify(GetPref("runEffectOnGroundOnlyToggle", runEffectOnGroundOnly));
        doubleJumpEffectToggle.SetIsOnWithoutNotify(GetPref("doubleJumpEffectToggle", doubleJumpEffect));
        wallSlideEffectToggle.SetIsOnWithoutNotify(GetPref("wallSlideEffectToggle", wallSlideEffect));
        dashEffectToggle.SetIsOnWithoutNotify(GetPref("dashEffectToggle", dashEffect));
        deformPlayerEffectToggle.SetIsOnWithoutNotify(GetPref("deformPlayerEffectToggle", deformPlayerEffect));
        trampolineBounceEffectToggle.SetIsOnWithoutNotify(GetPref("trampolineBounceEffectToggle", trampolineBounceEffect));
        hitEffectToggle.SetIsOnWithoutNotify(GetPref("hitEffectToggle", hitEffect));
        dieEffectToggle.SetIsOnWithoutNotify(GetPref("dieEffectToggle", dieEffect));
        vibrationsEffectToggle.SetIsOnWithoutNotify(GetPref("vibrationsEffectToggle", vibrationsEffect));
        cameraShakeEffectToggle.SetIsOnWithoutNotify(GetPref("cameraShakeEffectToggle", cameraShakeEffect));
        soundEffectsToggle.SetIsOnWithoutNotify(GetPref("soundEffectsToggle", soundEffects));
        UpdateAllToggle();
        allowEmitAll = true;
    }

    private void SaveToggles()
    {
        SetPref("runEffectToggle", runEffectToggle.isOn);
        SetPref("runEffectOnTurnBoostToggle", runEffectOnTurnBoostToggle.isOn);
        SetPref("runEffectOnGroundOnlyToggle", runEffectOnGroundOnlyToggle.isOn);
        SetPref("doubleJumpEffectToggle", doubleJumpEffectToggle.isOn);
        SetPref("wallSlideEffectToggle", wallSlideEffectToggle.isOn);
        SetPref("dashEffectToggle", dashEffectToggle.isOn);
        SetPref("deformPlayerEffectToggle", deformPlayerEffectToggle.isOn);
        SetPref("trampolineBounceEffectToggle", trampolineBounceEffectToggle.isOn);
        SetPref("hitEffectToggle", hitEffectToggle.isOn);
        SetPref("dieEffectToggle", dieEffectToggle.isOn);
        SetPref("vibrationsEffectToggle", vibrationsEffectToggle.isOn);
        SetPref("cameraShakeEffectToggle", cameraShakeEffectToggle.isOn);
        SetPref("soundEffectsToggle", soundEffectsToggle.isOn);
        PlayerPrefs.Save();
    }

    private bool GetPref(string key, bool placeholder)
    {
        return PlayerPrefs.HasKey(key) ? (PlayerPrefs.GetInt(key) > 0) : placeholder;
    }

    private void SetPref(string key, bool value)
    {
        PlayerPrefs.SetInt(key, value ? 1 : 0);
    }

    public void UpdateAllToggle()
    {
        allToggle.SetIsOnWithoutNotify(runEffectToggle.isOn &&
                                       runEffectOnTurnBoostToggle.isOn &&
                                       doubleJumpEffectToggle.isOn &&
                                       wallSlideEffectToggle.isOn &&
                                       dashEffectToggle.isOn &&
                                       runEffectOnGroundOnlyToggle.isOn &&
                                       deformPlayerEffectToggle.isOn &&
                                       trampolineBounceEffectToggle.isOn &&
                                       hitEffectToggle.isOn &&
                                       dieEffectToggle.isOn &&
                                       vibrationsEffectToggle.isOn &&
                                       cameraShakeEffectToggle.isOn &&
                                       soundEffectsToggle.isOn
                                       );
        SaveToggles();
    }

    public void OnEmitAll()
    {
        if (allowEmitAll)
        {
            runEffectToggle.SetIsOnWithoutNotify(allToggle.isOn);
            runEffectOnTurnBoostToggle.SetIsOnWithoutNotify(allToggle.isOn);
            runEffectOnGroundOnlyToggle.SetIsOnWithoutNotify(allToggle.isOn);
            doubleJumpEffectToggle.SetIsOnWithoutNotify(allToggle.isOn);
            wallSlideEffectToggle.SetIsOnWithoutNotify(allToggle.isOn);
            dashEffectToggle.SetIsOnWithoutNotify(allToggle.isOn);
            deformPlayerEffectToggle.SetIsOnWithoutNotify(allToggle.isOn);
            trampolineBounceEffectToggle.SetIsOnWithoutNotify(allToggle.isOn);
            hitEffectToggle.SetIsOnWithoutNotify(allToggle.isOn);
            dieEffectToggle.SetIsOnWithoutNotify(allToggle.isOn);
            vibrationsEffectToggle.SetIsOnWithoutNotify(allToggle.isOn);
            cameraShakeEffectToggle.SetIsOnWithoutNotify(allToggle.isOn);
            soundEffectsToggle.SetIsOnWithoutNotify(allToggle.isOn);
            SaveToggles();
        }
    }
}