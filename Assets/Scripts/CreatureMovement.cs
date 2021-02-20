using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureMovement : MonoBehaviour
{
    #region Physical_locations
    public float flat_movespeed;
    public float flat_turnspeed;
    public float maxPlayerDistance = 50;

    Vector2 currDirection;
    Vector2 targetDirection;
    double startTurn;
    double turnTime;
    bool forward;
    #endregion

    #region Physics_components
    Rigidbody2D CreatureRB;
    #endregion

    #region Creature_type
    public bool isPlayer;
    public bool runsTowards;
    private GameObject targetCreature;
    #endregion

    void Start()
    {
        CreatureRB = GetComponent<Rigidbody2D>();
        currDirection = new Vector2(0, 1);
        targetDirection = new Vector2(1, 1);
        turnTime = 1;
        forward = false;
        if (!isPlayer)
        {
            targetCreature = GameManager.instance.GetClosestCreature(gameObject);
        }
        StartCoroutine(AggroCheck());

    }
    void FixedUpdate()
    {
        if (!isPlayer && targetCreature == null) return;

        if (!isPlayer && Vector3.Distance(transform.position, targetCreature.transform.position) > maxPlayerDistance) {
            GameManager.instance.creatures.Remove(gameObject);
            Destroy(gameObject);
            return;
        }

        if (GameManager.instance.inEditor) return;

        Vector2 temp = transform.rotation * new Vector2(0, 1);


        if (NearlyEqual(temp.x, targetDirection.x, .2) && NearlyEqual(temp.y, targetDirection.y, .2) && forward && isPlayer)
        {
            CreatureRB.angularVelocity = 0f;
            currDirection = transform.rotation * new Vector2(0, 1);
            if (Vector2.Dot(CreatureRB.velocity, currDirection) < GetMoveSpeed()) {
                CreatureRB.AddForce(currDirection * GetMoveSpeed());
            }
            
        }
        if (isPlayer && Input.GetMouseButton(0))
        {
            forward = true;
            currDirection = transform.rotation * new Vector2(0, 1);
            startTurn = Time.time;
            if (Vector2.Dot(CreatureRB.velocity, currDirection) < GetMoveSpeed() / 4) {
                CreatureRB.AddForce(currDirection * GetMoveSpeed() / 4);
            }

            Vector3 position = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            targetDirection = position - transform.position;
            targetDirection.Normalize();
            turnTime = Vector2.Angle(targetDirection, currDirection) / GetTurnSpeed();
            if (Vector2.SignedAngle(targetDirection, currDirection) > 0 && CreatureRB.angularVelocity > -GetTurnSpeed() / 5)
            {
                CreatureRB.AddTorque(-GetTurnSpeed() / 20);
            } 
            else if (CreatureRB.angularVelocity < GetTurnSpeed() / 5)
            {
                CreatureRB.AddTorque(GetTurnSpeed() / 20);
            }
        }
        else if (isPlayer && Input.GetMouseButton(1))
        {
            currDirection = transform.rotation * new Vector2(0, 1);
            CreatureRB.angularVelocity = 0f;
            CreatureRB.velocity = Vector2.zero;
            turnTime = 1;
            forward = false;
        } 

        if (!isPlayer && NearlyEqual(temp.x, targetDirection.x, .2) && NearlyEqual(temp.y, targetDirection.y, .2))
        {
            CreatureRB.angularVelocity = 0f;
            currDirection = transform.rotation * new Vector2(0, 1);
            if (Vector2.Dot(CreatureRB.velocity, currDirection) < GetMoveSpeed())
            {
                CreatureRB.AddForce(currDirection * GetMoveSpeed());
            }
        }
        if (!isPlayer)
        {
            forward = true;
            currDirection = transform.rotation * new Vector2(0, 1);
            startTurn = Time.time;
            if (Vector2.Dot(CreatureRB.velocity, currDirection) < GetMoveSpeed() / 4) {
                CreatureRB.AddForce(currDirection * GetMoveSpeed() / 4);
            }
            targetDirection = targetCreature.transform.position - transform.position;
            if (!runsTowards)
            {
                targetDirection *= -1;
            }
            targetDirection.Normalize();
            turnTime = Vector2.Angle(targetDirection, currDirection) / GetTurnSpeed();
            if (Vector2.SignedAngle(targetDirection, currDirection) > 0 && CreatureRB.angularVelocity > -GetTurnSpeed() / 5)
            {
                CreatureRB.AddTorque(-GetTurnSpeed() / 20);
            }
            else if (CreatureRB.angularVelocity < GetTurnSpeed() / 5)
            {
                CreatureRB.AddTorque(GetTurnSpeed() / 20);
            }
        }
    }
    private static bool NearlyEqual(float a, float b, double epsilon)
    {
        const float MinNormal = 2.2250738585072014E-308f;
        float absA = Mathf.Abs(a);
        float absB = Mathf.Abs(b);
        float diff = Mathf.Abs(a - b);

        if (a.Equals(b))
        { // shortcut, handles infinities
            return true;
        }
        else if (a == 0 || b == 0 || absA + absB < MinNormal)
        {
            // a or b is zero or both are extremely close to it
            // relative error is less meaningful here
            return diff < (epsilon * MinNormal);
        }
        else
        { // use relative error
            return diff / (absA + absB) < epsilon;
        }
    }

    private IEnumerator AggroCheck() {
        while (true) {
            yield return new WaitForSeconds(3);
            targetCreature = GameManager.instance.GetClosestCreature(gameObject);
        }
    }

    private float GetMoveSpeed() {
        if (GetComponentInChildren<Creature>()) {
            return flat_movespeed + GetComponentInChildren<Creature>().moveSpeedBonus;
        } else {
            return flat_movespeed;
        }
        
    }

    private float GetTurnSpeed() {
        if (GetComponentInChildren<Creature>()) {
            return flat_turnspeed + GetComponentInChildren<Creature>().turnSpeedBonus * 1.5f;
        } else {
            return flat_turnspeed;
        }
    }
}
