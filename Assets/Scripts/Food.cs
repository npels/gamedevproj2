using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : MonoBehaviour {

    public int DNA;
    public int heal;
    public bool randomSpawn = false;

    public float maxDistance = 70f;
    
    public void Eat(bool isPlayer) {
        if (isPlayer) {
            GameManager.instance.ChangeDNA(DNA);
            GameManager.instance.player.GetComponent<Creature>().Heal(heal);
        }
        if (randomSpawn) {
            GameManager.instance.numRandomFood--;
        }
        Destroy(gameObject);
    }

    private void Update() {
        if (Vector3.Distance(transform.position, GameManager.instance.player.transform.position) > maxDistance) {
            if (randomSpawn) {
                GameManager.instance.numRandomFood--;
            }
            Destroy(gameObject);
        }
    }
}
