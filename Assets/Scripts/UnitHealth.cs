using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UnitHealth : MonoBehaviour
{
    [SerializeField] private UnityEvent damageEvent;
    [SerializeField] private UnityEvent deathEvent;
    [SerializeField] private int maxHealth;
    [SerializeField] private bool screenShake = false;
    [SerializeField] private GameObject deathParticles;
    [SerializeField] private AudioClip hitSound;
    [SerializeField] private AudioClip deathSound;
    private AudioSource playerAudioSource;
    private int currentHealth;
    private bool alive = true;
    private MyCamera cam;

    private void Start()
    {
        cam = FindObjectOfType<MyCamera>();
        playerAudioSource = GetComponent<AudioSource>();
    }
    private void OnEnable()
    {
        alive = true;
        currentHealth = maxHealth;
    }

    public void TakeDamage(int value)
    {
        if (!alive)
            return;
        currentHealth -= value;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        damageEvent.Invoke();
        
        if (screenShake)
            cam.StartShake(0.1f);
        if (currentHealth <= 0)
        {
            alive = false;
            if (deathParticles && FeedbackController.Instance.DieEffect)
                Instantiate(deathParticles, transform.position, transform.rotation);
            GamepadVibrations.Instance.OnDie();
            deathEvent.Invoke();
            if (FeedbackController.Instance.SoundEffects)
                playerAudioSource.PlayOneShot(deathSound);
        }
        else
        {
            if (FeedbackController.Instance.SoundEffects)
                playerAudioSource.PlayOneShot(hitSound);
        }
    }


    public bool isAlive()
    {
        return alive;
    }



    public void SetMaxHealth(int value)
    {
        maxHealth = value;
        currentHealth = value;
    }
}
