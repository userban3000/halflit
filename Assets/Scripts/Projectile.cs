using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {
    
    public float lifetime;
    public float speed;
    public float damage;

    public event System.Action OnImpact;

    protected virtual void Update() {
         transform.Translate ( Vector3.up * speed * Time.deltaTime );
    }

    protected virtual void Start() {
        Invoke("Expire", lifetime);
    }

    protected virtual void Expire() {
        Remove();
    }

    protected virtual void Remove() {
        GameObject.Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D other) {
        GameObject hitObject = other.gameObject;

        if ( OnImpact != null ) 
            OnImpact();

        DamageableEntity hitEntity = hitObject.GetComponent<DamageableEntity>();
        if ( hitEntity != null ) {
            hitEntity.TakeDamage(damage);
        }

        Remove();
    }

}
