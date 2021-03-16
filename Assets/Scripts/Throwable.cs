using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Throwable : MonoBehaviour {

    [Header("Components")]
    private float t;
    private float airtime;
    public bool thrown;
    private Rigidbody2D rb;
    private float rbInitVel;
    
    [Header("Properties")]
    public float lifetime;
    public float airDrag;
    public float groundDrag;
    public bool zeroHealthCausesExpiration;
    public float health;
    public bool lit;

    public event System.Action OnBounce;
    public event System.Action OnExpire;

    protected virtual void Awake() {
        rb = GetComponent<Rigidbody2D>();

        t = airtime = 0;
        thrown = false;
        lit = false;
    
        rb.drag = airDrag;
    }

    protected virtual void Update() {
        if ( lit )
            t += Time.deltaTime;

        if ( thrown && rb.velocity.sqrMagnitude < rbInitVel / 3f && rb.drag < groundDrag ) {
            rb.drag = groundDrag;
        } 

        if ( t >= lifetime || health <= 0 && zeroHealthCausesExpiration ) {
            Expire();
        }

    }

    private void OnCollisionEnter2D(Collision2D other) {
        if ( OnBounce != null ) {
            OnBounce();
        }
    }

    public virtual void Ignite() {
        lit = true;
    }

    public virtual void Expire() {
        if ( OnExpire != null ) {
            OnExpire();
        }
        GameObject.Destroy(this.gameObject);
    }

    public virtual void Throw(Vector3 dir, float force) {
        rb.AddForce(dir.normalized * force, ForceMode2D.Impulse);
        thrown = true;
        rbInitVel = rb.velocity.sqrMagnitude;
    }

}
