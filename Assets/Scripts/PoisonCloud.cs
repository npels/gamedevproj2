using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonCloud : MonoBehaviour {

    public float damageInterval;
    public float duration;
    public float scaleIncrease;
    public float damage;

    private float damageTimer;
    private float lifetime;

    private List<Creature> creaturesDamaged;

    private void Start() {
        lifetime = 0;
        damageTimer = damageInterval;
        creaturesDamaged = new List<Creature>();
    }

    private void Update() {
        lifetime += Time.deltaTime;
        if (lifetime >= duration) Destroy(gameObject);

        transform.localScale += Vector3.one * Time.deltaTime * scaleIncrease;

        Color color = GetComponent<SpriteRenderer>().color;
        color.a = 1 - lifetime / duration;
        GetComponent<SpriteRenderer>().color = color;

        if (damageTimer < -0.5f) {
            damageTimer = damageInterval;
        }
        damageTimer -= Time.deltaTime;
        
    }

    private void OnTriggerStay2D(Collider2D collision) {
        if (damageTimer <= 0) {
            if (collision.GetComponentInParent<Creature>()) {
                Creature target = collision.GetComponentInParent<Creature>();
                if (creaturesDamaged.Contains(target) || target.numPoison > 0) return;
                creaturesDamaged.Add(target);
                target.TakeDamage(damage);
            }
        }
    }
}
