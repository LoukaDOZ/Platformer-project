using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageSource : MonoBehaviour
{
    [SerializeField] private int damage = 1;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag != "Player")
            return;
        HitManager hitManager = collision.GetComponent<HitManager>();
        if (hitManager)
            hitManager.Hit(damage);
    }
}
