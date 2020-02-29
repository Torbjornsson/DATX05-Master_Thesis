using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotHatchSet_Rotate : MonoBehaviour
{
    public enum Axis {
        X, Y, Z
    }

    public static GameObject FindGameObjectInChildWithTag (GameObject parent, string tag) {
        Transform t = parent.transform;
        for (int i = 0; i < t.childCount; i++)
            if (t.GetChild(i).gameObject.tag == tag) return t.GetChild(i).gameObject;
        return null;
    }

    public WinningSlot winningSlot;
    [Space]
    public GameObject[] hatches;
    // public Axis[] hatchTurnAxes;
    public Vector3[] hatchTurnAxes;
    public float[] hatchTurnSpeed;
    public float[] openAngle;

    private GameObject[] hatchPivotPoints;
    private bool open = false;
    private bool moving = false;
    // private float[] startingAngle;
    private float[] currentAngle;

    // private float debugDelay = 0;

    // Start is called before the first frame update
    void Start()
    {
        if (hatches.Length <= 0)
            Debug.LogError("Slot Hatch Set: No hatch was found!");
        if (hatchTurnAxes.Length != hatches.Length)
            Debug.LogError("Slot Hatch Set: No (or the wrong amount of) hatch turn axes found!");
        if (hatchTurnSpeed.Length != hatches.Length)
            Debug.LogError("Slot Hatch Set: No (or the wrong amount of) hatch turn speed found!");
        if (openAngle.Length != hatches.Length)
            Debug.LogError("Slot Hatch Set: No (or the wrong amount of) open angle found!");

        hatchPivotPoints = new GameObject[hatches.Length];
        currentAngle = new float[hatches.Length];

        for (int i = 0; i < hatches.Length; i++) {
            var go = hatches[i];

            if (!go) {
                Debug.LogError("Slot Hatch Set: One of the assigned hatches was not defined!");
                continue;
            }
            
            currentAngle[i] = 0;
            var pivotPoint = FindGameObjectInChildWithTag(go, "HatchPivotPoint");

            if (!pivotPoint) {
                Debug.LogError("Slot hatch Set: Slot Hatch: Pivot Point was not found!");
                continue;
            }

            hatchPivotPoints[i] = pivotPoint;
        }

        if (!winningSlot) Debug.LogError("Slot Hatch Set: Winning Slot was not found!");

        // winningSlot.active = false;
        SetWinningSlotActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        // debugDelay += Time.deltaTime;
        // if (!open && debugDelay > 5) OpenHatches();
        // else if (open && debugDelay > 10) {
        //     CloseHatches();
        //     debugDelay = 0;
        // }

        if (!moving) return;

        var done = new bool[hatches.Length];

        for (int i = 0; i < hatches.Length; i++) {
            if (done[i]) continue;

            var hatch = hatches[i];
            var axis = hatchTurnAxes[i];
            var pivot = hatchPivotPoints[i];
            var angle = currentAngle[i];
            var turnSpeed = hatchTurnSpeed[i] * Time.deltaTime;
            var targetAngle = (open ? openAngle[i] : 0);

            if (angle < targetAngle - turnSpeed)
                angle += turnSpeed;
            else if (angle > targetAngle + turnSpeed)
                angle -= turnSpeed;
            else {
                angle = targetAngle;
                done[i] = true;
            }
            var deltaAngle = angle - currentAngle[i];
            currentAngle[i] += deltaAngle;

            hatch.transform.RotateAround(pivot.transform.position, axis, deltaAngle);
        }

        var allDone = true;
        foreach (bool d in done) {
            allDone &= d;
        }
        if (allDone) {
            moving = false;
            if (open) SetWinningSlotActive(true);//winningSlot.active = true;
        }
    }

    public void OpenHatches() {
        open = true;
        moving = true;
    }

    public void CloseHatches() {
        open = false;
        moving = true;
        // winningSlot.active = false;
        SetWinningSlotActive(false);
    }

    public void SetWinningSlotActive(bool active) {
        winningSlot.active = active;
        GameMaster.instance.goalCriteriaSatisfied = active;
    }
}
