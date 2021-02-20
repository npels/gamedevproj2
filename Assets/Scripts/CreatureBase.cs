using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureBase : MonoBehaviour {

    public float radius;

    [HideInInspector]
    public List<CreaturePart> parts;

    [HideInInspector]
    public CreatureBase nextBase;

    [HideInInspector]
    public CreatureBase prevBase;

    private static float scaleChangeMultiplier = 0.1f;

    private static float minSize = 0.4f;
    private static float maxSize = 1.5f;

    private static float baseAttachDistance = 0.2f;

    private void Awake() {
        if (parts != null) {
            parts = new List<CreaturePart>();
        }
    }

    private void Start() {
        if (transform.GetSiblingIndex() == 0) {
            prevBase = null;
        } else {
            Transform prevTransform = transform.parent.GetChild(transform.GetSiblingIndex() - 1);
            prevBase = prevTransform.GetComponent<CreatureBase>();
        }

        if (transform.parent.childCount > transform.GetSiblingIndex() + 1) {
            Transform nextTransform = transform.parent.GetChild(transform.GetSiblingIndex() + 1);
            nextBase = nextTransform.GetComponent<CreatureBase>();
        }
    }

    private void OnMouseOver() {
        if (Input.mouseScrollDelta.y != 0 && GameManager.instance.inEditor) {
            UpdateScale(Input.mouseScrollDelta.y);
        }
    }

    public void UpdateScale(float scaleDelta) {
        Vector3 prevScale = transform.localScale;
        transform.localScale = prevScale + Vector3.one * scaleDelta * scaleChangeMultiplier;
        if (transform.localScale.y < minSize) {
            transform.localScale = Vector3.one * minSize;
        } else if (transform.localScale.y > maxSize) {
            transform.localScale = Vector3.one * maxSize;
        }
        radius = radius / prevScale.y * transform.localScale.y;

        foreach (CreaturePart part in parts) {
            part.UpdatePositionOnCreatureBase(this);
        }

        UpdatePosition();
    }

    public void UpdatePosition() {
        if (prevBase != null) {
            Vector3 newPos = new Vector3(0, prevBase.transform.localPosition.y - prevBase.radius - radius + baseAttachDistance, 0);
            transform.localPosition = newPos;

            foreach (CreaturePart part in parts) {
                part.UpdatePositionOnCreatureBase(this);
            }
        }

        if (nextBase != null) {
            nextBase.UpdatePosition();
        }
    }
}
