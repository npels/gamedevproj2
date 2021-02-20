using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawn : MonoBehaviour {
    public float spawnrate;
    public float spawnDistance;
    public int maxCreatures;
    public List<MonsterSpawnInfo> monsterSpawns;

    private GameObject player;
    private float timer;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player(Clone)");
        timer = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (timer >= 1 / spawnrate)
        {
            timer = 0;
            if (GameManager.instance.creatures.Count >= maxCreatures) return;
            bool spawnedMonster = false;
            while (!spawnedMonster) {
                MonsterSpawnInfo info = monsterSpawns[Random.Range(0, monsterSpawns.Count)];
                if (info.minimumDnaCollectedToSpawn > GameManager.totalCollectedDNA) continue;
                Vector3 spawnLoc = Quaternion.Euler(0, 0, Random.Range(0f, 360f)) * Vector3.right * spawnDistance;
                spawnLoc += player.transform.position;
                spawnLoc.z = 10;
                GameObject monster = Instantiate(info.monsterPrefab, spawnLoc, Quaternion.identity);
                GameManager.instance.creatures.Add(monster);
                monster.GetComponent<Creature>().CheckParts();
                spawnedMonster = true;
            }
        }
        else
        {
            timer += Time.deltaTime;
        }
    }

    [System.Serializable]
    public class MonsterSpawnInfo {
        public GameObject monsterPrefab;
        public int minimumDnaCollectedToSpawn;
    }
}
