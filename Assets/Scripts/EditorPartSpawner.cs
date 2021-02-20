using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorPartSpawner : MonoBehaviour {

    public GameObject partPrefab;

    private GameObject currentPart;

    public Vector3 partOffset;

    private void Start() {
        MakeNewPart();
    }

    private void Update() {
        if (currentPart == null || currentPart.GetComponent<CreaturePart>().onBase) {
            MakeNewPart();
        }
    }

    private void MakeNewPart() {
        currentPart = Instantiate(partPrefab);
        Vector3 position = transform.position;
        position += partOffset;
        currentPart.transform.position = position;
        currentPart.GetComponent<CreaturePart>().onBase = false;
    }

}
