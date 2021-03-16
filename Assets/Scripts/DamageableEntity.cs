using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageableEntity : MonoBehaviour {
    
    public float startingHealth;
    protected float health;

    public event System.Action OnDeath;
    public event System.Action OnHealthChanged;

    protected virtual void Start() {
        health = startingHealth;
    }

    public virtual float GetHealth() {
        return health;
    }

    public virtual void TakeDamage(float damage) {
        health -= damage;
        if ( OnHealthChanged != null ) {
            OnHealthChanged();
        }
        if ( health <= 0 ) {
            Die();
        }
    }

    protected virtual void Die() {
        if ( OnDeath != null ) {
            OnDeath();
        }
        GameObject.Destroy(gameObject);
    }

}
