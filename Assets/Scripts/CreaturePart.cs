using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreaturePart : MonoBehaviour {

    #region Creature Part Variables
    [HideInInspector]
    public float radialPosition;

    [HideInInspector]
    public CreaturePart twinPart;

    [HideInInspector]
    public CreatureBase currentBase;

    [HideInInspector]
    public float _closestBaseDistance;

    public int DNACost;
    #endregion

    #region Creature Editor Variables
    protected bool inValidLocation;

    protected static float scaleChangeMultiplier = 0.1f;

    protected static float minSize = 0.4f;
    protected static float maxSize = 2f;

    private static float singlePartThreshold = 10f;

    protected static float maxDistanceFromBase = 1.5f;

    [HideInInspector]
    public bool onBase = true;

    [HideInInspector]
    public bool canAfford = true;
    #endregion

    #region Unity Functions
    private void Update() {
        if (GameManager.instance.inEditor) {
            if (GameManager.currentDNA < DNACost && !onBase) {
                canAfford = false;
                transform.GetChild(0).GetComponent<SpriteRenderer>().color = Color.red;
            } else {
                canAfford = true;
                transform.GetChild(0).GetComponent<SpriteRenderer>().color = Color.white;
            }
        }
    }
    #endregion

    #region Mouse Interaction Functions
    private void OnMouseDrag() {
        if (GameManager.instance.inEditor && canAfford) {
            MoveToMouse();
            CheckForCreatureBase();
        }
    }

    private void OnMouseUp() {
        if (GameManager.instance.inEditor && canAfford) {
            if (!inValidLocation) {
                if (currentBase != null) {
                    currentBase.parts.Remove(this);
                }
                GetComponent<PartBase>().RemovePart();
                if (onBase) GameManager.instance.ChangeDNA(DNACost);
                Destroy(gameObject);
            } else {
                if (!onBase) {
                    GetComponent<PartBase>().SetupPart();
                    GameManager.instance.ChangeDNA(-DNACost);
                }
                onBase = true;
                if (twinPart != null) twinPart.onBase = true;
            }
        }
        
    }

    private void OnMouseOver() {
        if (Input.mouseScrollDelta.y != 0 && GameManager.instance.inEditor && onBase) {
            UpdateScale(Input.mouseScrollDelta.y);
            if (twinPart != null) {
                twinPart.UpdateScale(Input.mouseScrollDelta.y);
            }
        }
    }
    #endregion

    #region Creature Editor Functions
    private void MoveToMouse() {
        Vector3 position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        position.z = 0;
        transform.position = position;
        transform.rotation = Quaternion.Euler(Vector3.zero);
    }

    /* Find the closest CreatureBase and calculate the position along the base circle that this part lies. */
    public virtual void CheckForCreatureBase() {
        CreatureBase closestBase = null;
        float closestBaseDistance = float.MaxValue;

        foreach (CreatureBase creatureBase in GameManager.instance.player.GetComponentsInChildren<CreatureBase>()) {
            if (!creatureBase.enabled) continue;
            float baseDistance = Vector3.Distance(creatureBase.transform.position, transform.position) - creatureBase.radius;
            if (baseDistance < 0) {
                inValidLocation = false;
                if (twinPart != null) {
                    currentBase.parts.Remove(twinPart);
                    Destroy(twinPart.gameObject);
                }
                return;
            }

            if (closestBase == null || baseDistance < closestBaseDistance) {
                closestBase = creatureBase;
                closestBaseDistance = baseDistance;
                _closestBaseDistance = baseDistance;
            }
        }

        if (closestBase == null) return;

        if (closestBaseDistance > maxDistanceFromBase) {
            inValidLocation = false;
            if (twinPart != null) {
                closestBase.parts.Remove(twinPart);
                Destroy(twinPart.gameObject);
            }
            return;
        }

        inValidLocation = true;

        radialPosition = Vector3.SignedAngle(Vector3.up, closestBase.transform.position - transform.position, Vector3.forward) + 180;
        UpdatePositionOnCreatureBase(closestBase);
    }

    /* Update the position and rotation of this part relative to the given CreatureBase. */
    public virtual void UpdatePositionOnCreatureBase(CreatureBase creatureBase, bool updateTwin = true) {
        /* If the part is within a threshold of the very front of the creature, snap it to the front and destroy the part's twin. */
        if ((radialPosition >= 0 && radialPosition < singlePartThreshold) || (radialPosition > 0 && 360 - radialPosition < singlePartThreshold)) {
            radialPosition = 0;
            if (twinPart != null) {
                currentBase.parts.Remove(twinPart);
                twinPart.GetComponent<PartBase>().RemovePart();
                Destroy(twinPart.gameObject);
                twinPart = null;
            }
        }

        /* If the part is within a threshold of the very back of the creature, snap it to the back and destroy the part's twin. */
        if ((180 - radialPosition >= 0 && 180 - radialPosition < singlePartThreshold) || (radialPosition - 180 >= 0 && radialPosition - 180 < singlePartThreshold)) {
            radialPosition = 180;
            if (twinPart != null) {
                currentBase.parts.Remove(twinPart);
                twinPart.GetComponent<PartBase>().RemovePart();
                Destroy(twinPart.gameObject);
                twinPart = null;
            }
        }

        /* Calculate and update the position and rotation of the part. */
        transform.parent = creatureBase.transform.parent;
        float xPos = creatureBase.radius * Mathf.Cos(Mathf.Deg2Rad * radialPosition + Mathf.PI / 2f);
        float yPos = creatureBase.radius * Mathf.Sin(Mathf.Deg2Rad * radialPosition + Mathf.PI / 2f);
        transform.localPosition = creatureBase.transform.localPosition + new Vector3(xPos, yPos, 0);
        transform.rotation = Quaternion.Euler(0, 0, radialPosition);

        /* If the part is on the right side of the creature, mirror its sprite it along its x-axis. */
        Vector3 scale = transform.localScale;
        scale.x = (radialPosition > 180 || (radialPosition < 0 && radialPosition > -180)) ? -Mathf.Abs(scale.x) : Mathf.Abs(scale.x);
        transform.localScale = scale;

        /* Update the CreatureBase this is attached to. */
        if (creatureBase != currentBase) {
            if (currentBase != null) {
                currentBase.parts.Remove(this);
            }
            currentBase = creatureBase;
            currentBase.parts.Add(this);
        }

        /* If this part has no twin and needs one, create it. */
        if (twinPart == null && radialPosition != 0 && radialPosition != 180) {
            twinPart = Instantiate(gameObject, transform.parent).GetComponent<CreaturePart>();
            twinPart.twinPart = this;
        }

        /* Update this part's twin, if one exists and. Prevent the twin from making another recursive update by setting the updateTwin argument to false. */
        if (twinPart != null && updateTwin) {
            twinPart.radialPosition = -radialPosition;
            twinPart.UpdatePositionOnCreatureBase(creatureBase, false);
        }
    }

    /* Update the scale of this part within the bounds of the min and max size. */
    public void UpdateScale(float scaleDelta) {
        Vector3 newScale;
        if (transform.localScale.x > 0) {
            newScale = new Vector3(transform.localScale.x + scaleDelta * scaleChangeMultiplier, transform.localScale.y + scaleDelta * scaleChangeMultiplier, transform.localScale.z);
        } else {
            newScale = new Vector3(transform.localScale.x - scaleDelta * scaleChangeMultiplier, transform.localScale.y + scaleDelta * scaleChangeMultiplier, transform.localScale.z);
        }
        
        if (newScale.y < minSize) {
            if (transform.localScale.x > 0) {
                newScale = new Vector3(minSize, minSize, newScale.z);
            } else {
                newScale = new Vector3(-minSize, minSize, newScale.z);
            };
        } else if (newScale.y > maxSize) {
            if (transform.localScale.x > 0) {
                newScale = new Vector3(maxSize, maxSize, newScale.z);
            } else {
                newScale = new Vector3(-maxSize, maxSize, newScale.z);
            }
            
        }
        transform.localScale = newScale;
    }
    #endregion
}
