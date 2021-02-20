using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Creature {

    public override void Heal(float amount) {
        base.Heal(amount);
        GameManager.instance.ChangeHealthSlider(health / maxHealth);
    }

    public override void TakeDamage(float amount) {
        base.TakeDamage(amount);
        GameManager.instance.ChangeHealthSlider(health / maxHealth);
    }

    public override void Die() {
        GameManager.instance.GoToRespawnScreen();
        gameObject.SetActive(false);
    }
}
