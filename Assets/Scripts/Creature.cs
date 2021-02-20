using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Creature : MonoBehaviour {

    [HideInInspector]
    public int numSegments;

    [HideInInspector]
    public List<CreatureBase> segments;

    [HideInInspector]
    public List<CreaturePart> parts;

    public CreatureInfo creatureInfo;

    public GameObject basePrefab;

    public float maxHealth;

    public float damageCooldown = 0.5f;

    public bool premadeCreature;

    public GameObject foodPrefab;

    public int numFoodDrops = 3;

    //[HideInInspector]
    public float health;

    [HideInInspector]
    public int numEyes;

    [HideInInspector]
    public int numPoison;

    [HideInInspector]
    public float moveSpeedBonus;

    [HideInInspector]
    public float turnSpeedBonus;

    private float damageTimer;

    private void Start() {
        health = maxHealth;
        damageTimer = 0;

        if (premadeCreature) {
            foreach (PartBase part in GetComponentsInChildren<PartBase>()) {
                part.SetupPart();
            }
            return;
        }

        GameObject segmentObject;
        CreatureBase creatureBase;
        CreatureBase prevBase = null;

        foreach (float segmentSize in creatureInfo.segmentSizes) {
            segmentObject = Instantiate(basePrefab, transform);
            creatureBase = segmentObject.GetComponent<CreatureBase>();

            if (prevBase != null) {
                creatureBase.prevBase = prevBase;
                prevBase.nextBase = creatureBase;
            }

            float scaleDelta = segmentSize - segmentObject.transform.localScale.y;
            creatureBase.UpdateScale(scaleDelta);

            prevBase = creatureBase;

            segments.Add(creatureBase);
        }

        foreach (CreatureInfo.PartInfo partInfo in creatureInfo.parts) {
            GameObject partObj = Instantiate(partInfo.partPrefab, transform);
            CreaturePart creaturePart = partObj.GetComponent<CreaturePart>();
            if (creaturePart == null) {
                Debug.LogError("Missing CreaturePart script on " + partObj.name);
            }
            creaturePart.radialPosition = partInfo.radialPosition;
            creaturePart.transform.localScale = Vector3.one * partInfo.scale;
            segments[partInfo.segmentIndex].parts.Add(creaturePart);
            creaturePart.UpdatePositionOnCreatureBase(segments[partInfo.segmentIndex]);
        }
    }

    private void Update() {
        if (damageTimer > 0) {
            damageTimer -= Time.deltaTime;
        }
    }

    public void ChangeColor(float hue, float saturation, float value) {
        foreach (CreatureBase segment in segments) {
            segment.GetComponentInChildren<SpriteRenderer>().color = Color.HSVToRGB(hue, saturation, value);
        }
    }

    public Color GetColor() {
        return GetComponentInChildren<SpriteRenderer>().color;
    }

    public virtual void Heal(float amount) {
        health += amount;
        if (health > maxHealth) health = maxHealth;
    }

    public virtual void TakeDamage(float amount) {
        if (damageTimer > 0) return;
        health -= amount;
        damageTimer = damageCooldown;
        if (health <= 0) {
            health = 0;
            Die();
        }
    }

    public virtual void Die() {
        for (int i = 0; i < numFoodDrops; i++) {
            Vector3 offset = Vector3.zero;
            offset.x += Random.Range(-1, 1);
            offset.y += Random.Range(-1, 1);
            offset.z += Random.Range(-1, 1);
            Instantiate(foodPrefab, transform.position + offset, Quaternion.identity);
        }
        GameManager.instance.creatures.Remove(gameObject);
        Destroy(gameObject);
    }

    public void CheckParts() {
        numEyes = 0;
        numPoison = 0;
        foreach (CreaturePart part in parts) {
            if (part.GetComponentInChildren<Eye>()) {
                numEyes++;
            }
            if (part.GetComponentInChildren<Poison>()) {
                numPoison++;
            }
        }
    }

    public void ChangeTurningSpeed(float amount) {
        turnSpeedBonus += amount;
    }

    public void ChangeMoveSpeed(float amount) {
        moveSpeedBonus += amount;
    }

    [System.Serializable]
    public class CreatureInfo {

        public List<float> segmentSizes;

        public List<PartInfo> parts;

        [System.Serializable]
        public class PartInfo {
            public GameObject partPrefab;
            public int segmentIndex;
            public float radialPosition;
            public float scale;
        }
    }
}
