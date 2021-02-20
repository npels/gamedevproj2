using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boost : PartBase {

    public float moveSpeedBonus = 1;

    public override void SetupPart() {
        GetComponentInParent<Creature>().ChangeMoveSpeed(moveSpeedBonus);
    }

    public override void RemovePart() {
        GetComponentInParent<Creature>().ChangeMoveSpeed(-moveSpeedBonus);
    }
}
