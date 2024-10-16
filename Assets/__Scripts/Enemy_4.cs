using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EnemyShield))]
public class Enemy_4 : Enemy {

    [Header("Inscribed")]
    public float duration = 4f;

    private EnemyShield[] allShields;
    private EnemyShield thisShield;
    private Vector3 p0, p1;
    private float timeStart;

    void Start() {
        allShields = GetComponentsInChildren<EnemyShield>();
        thisShield = GetComponent<EnemyShield>();

        p0 = p1 = pos;
        InitMovement();
    }

    void InitMovement() {
        p0 = p1;
        float widMinRad = bndCheck.camWidth - bndCheck.radius;
        float hgtMinRad = bndCheck.camHeight - bndCheck.radius;
        p1.x = Random.Range(-widMinRad, widMinRad);
        p1.y = Random.Range(-hgtMinRad, hgtMinRad);

        if (p0.x * p1.x > 0 && p0.y * p1.y > 0) {
            if (Mathf.Abs(p0.x) > Mathf.Abs(p0.y)) {
                p1.x *= -1;
            } else {
                p1.y *= -1;
            }
        }

        timeStart = Time.time;
    }

    public override void Move() {
        float u = (Time.time - timeStart) / duration;

        if (u >= 1) {
            InitMovement();
            u = 0;
        }

        u -= 0.15f * Mathf.Sin(u * Mathf.PI * 2);
        pos = (1 - u) * p0 + u * p1;
    }

    void OnCollisionEnter(Collision coll) {
        GameObject otherGO = coll.gameObject;

        ProjectileHero p = otherGO.GetComponent<ProjectileHero>();

        if (p != null) {
            Destroy(otherGO);

            if (bndCheck.isOnScreen) {
                GameObject goHit = coll.contacts[0].thisCollider.gameObject;

                if (goHit == otherGO) {
                    goHit = coll.contacts[0].otherCollider.gameObject;
                }

                float damage = Main.GET_WEAPON_DEFINITION(p.type).damageOnHit;

                bool shieldFound = false;
                foreach (EnemyShield shield in allShields) {
                    if (shield.gameObject == goHit) {
                        shield.TakeDamage(damage);
                        shieldFound = true;
                        break;
                    }
                }

                if (!shieldFound) {
                    thisShield.TakeDamage(damage);
                }

                if (thisShield.isActive) {
                    return;
                }

                if (!calledShipDestroyed) {
                    Main.SHIP_DESTROYED(this);
                    calledShipDestroyed = true;
                }

                Destroy(this.gameObject);
            }
        } else {
            print("Enemy hit by non-ProjectileHero: " + otherGO.name);
        }
    }
}
