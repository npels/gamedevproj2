using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureBasePart : CreaturePart {

    private static float singlePartThreshold = 0.1f;

    public override void CheckForCreatureBase() {
        CreatureBase closestBase = null;
        float closestBaseDistance = float.MaxValue;

        foreach (CreatureBase creatureBase in GameManager.instance.player.GetComponentsInChildren<CreatureBase>()) {
            if (!creatureBase.enabled) continue;
            float baseDistance = Vector3.Distance(creatureBase.transform.position, transform.position);

            if (closestBase == null || baseDistance < closestBaseDistance) {
                closestBase = creatureBase;
                closestBaseDistance = baseDistance;
                _closestBaseDistance = baseDistance;
            }
        }

        if (closestBase == null) return;

        if (closestBaseDistance > closestBase.radius) {
            inValidLocation = false;
            if (twinPart != null) {
                closestBase.parts.Remove(twinPart);
                Destroy(twinPart.gameObject);
            }
            return;
        }

        inValidLocation = true;

        UpdatePositionOnCreatureBase(closestBase);
    }

    public override void UpdatePositionOnCreatureBase(CreatureBase creatureBase, bool updateTwin = true) {
        if ((transform.localPosition.x > 0 && transform.localPosition.x < singlePartThreshold) || (transform.localPosition.x < 0 && -transform.localPosition.x < singlePartThreshold)) {
            Vector3 position = transform.position;
            position.x = 0f;
            transform.position = position;
            if (twinPart != null) {
                currentBase.parts.Remove(twinPart);
                Destroy(twinPart.gameObject);
                twinPart = null;
            }
        }

        transform.parent = creatureBase.transform.parent;

        Vector3 scale = transform.localScale;
        scale.x = (transform.localPosition.x < 0) ? -Mathf.Abs(scale.x) : Mathf.Abs(scale.x);
        transform.localScale = scale;

        if (creatureBase != currentBase) {
            if (currentBase != null) {
                currentBase.parts.Remove(this);
            }
            currentBase = creatureBase;
            currentBase.parts.Add(this);
        }

        if (twinPart == null && transform.localPosition.x != 0) {
            twinPart = Instantiate(gameObject, transform.parent).GetComponent<CreaturePart>();
            twinPart.twinPart = this;
        }

        if (twinPart != null && updateTwin) {
            Vector3 position = transform.position;
            position.x = -transform.position.x;
            twinPart.transform.position = position;
            twinPart.UpdatePositionOnCreatureBase(creatureBase, false);
        }
    }
}
