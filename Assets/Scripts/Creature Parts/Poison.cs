using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Poison : PartBase {

    public GameObject poisonCloudPrefab;
    public float poisonSpawnTime = 2;
    public Vector3 poisonSpawnOffset = Vector3.up;

    private float timer;
    private bool poisonEnabled = false;

    public override void SetupPart() {
        poisonEnabled = true;
        timer = poisonSpawnTime;
    }

    public override void RemovePart() {
        poisonEnabled = false;
    }

    public void SpawnPoison() {
        Instantiate(poisonCloudPrefab, transform.position + transform.rotation * poisonSpawnOffset, transform.rotation);
    }

    private void Update() {
        if (poisonEnabled && !GameManager.instance.inEditor) {
            timer -= Time.deltaTime;
            if (timer <= 0) {
                SpawnPoison();
                timer = poisonSpawnTime;
            }
        }
    }
}
