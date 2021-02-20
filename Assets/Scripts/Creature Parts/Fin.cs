using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fin : PartBase {

    public float turnSpeedBonus = 40;

    public override void SetupPart() {
        GetComponentInParent<Creature>().ChangeTurningSpeed(turnSpeedBonus);
    }

    public override void RemovePart() {
        GetComponentInParent<Creature>().ChangeTurningSpeed(-turnSpeedBonus);
    }
}
