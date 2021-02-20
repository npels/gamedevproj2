using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodSpawner : MonoBehaviour {
    public float spawnrate;
    public float minSpawnDistance;
    public float maxSpawnDistance;
    public int maxFood;
    public GameObject foodPrefab;


    private GameObject player;
    private float timer;

    // Start is called before the first frame update
    void Start() {
        player = GameObject.Find("Player(Clone)");
        timer = 0;
    }

    void Update() {
        if (timer >= 1 / spawnrate) {
            timer = 0;
            if (GameManager.instance.numRandomFood >= maxFood) return;
            Vector3 spawnLoc = Quaternion.Euler(0, 0, Random.Range(0f, 360f)) * Vector3.right * Random.Range(minSpawnDistance, maxSpawnDistance);
            spawnLoc += player.transform.position;
            spawnLoc.z = 10;
            GameManager.instance.numRandomFood++;
            Instantiate(foodPrefab, spawnLoc, Quaternion.identity);
        } else {
            timer += Time.deltaTime;
        }
    }
}
