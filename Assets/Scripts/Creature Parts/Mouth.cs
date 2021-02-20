using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mouth : PartBase {

    public float damageAmount = 2;

    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.collider.GetComponent<Food>()) {
            collision.collider.GetComponent<Food>().Eat(GetComponentInParent<Creature>().gameObject == GameManager.instance.player);
        } else if (collision.collider.GetComponent<CreatureBase>()) {
            if (collision.collider.GetComponentInParent<Creature>() != GetComponentInParent<Creature>()) {
                collision.collider.GetComponentInParent<Creature>().TakeDamage(damageAmount);
            }
        }
    }
}
