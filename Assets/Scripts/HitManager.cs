using System.Collections;
using UnityEngine;

public class HitManager : MonoBehaviour
{
    private UnitHealth health;

    [SerializeField] private float invinsibilityTime = 0f;
    private bool isInvinsible = false;


    private void Start()
    {
        health = GetComponent<UnitHealth>();
    }

    public void Hit(int value)
    {
        if (isInvinsible)
            return;
        health.TakeDamage(value);
        if (value > 0 && gameObject.activeSelf)
        {
            StartCoroutine(SetInvisible(invinsibilityTime));
            GamepadVibrations.Instance.OnHit();
        }
    }

    public IEnumerator SetInvisible(float duration)
    {
        isInvinsible = true;
        yield return new WaitForSeconds(duration);
        isInvinsible = false;
    }
}
