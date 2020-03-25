using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionTestReset : MonoBehaviour
{
    public bool reset = false;

    GameObject[] cubes;
    Vector3[] startPositions;

    // Start is called before the first frame update
    void Start()
    {
        var cubeCount = transform.childCount;
        cubes = new GameObject[cubeCount];
        startPositions = new Vector3[cubeCount];
        for(int i = 0; i < cubeCount; i++) {
            cubes[i] = transform.GetChild(i).gameObject;
            startPositions[i] = cubes[i].transform.position;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (reset) {
            ResetCubes();
            reset = false;
        }
    }

    public void ResetCubes() {
        for (int i = 0; i < cubes.Length; i++) {
            cubes[i].transform.position = startPositions[i];
            cubes[i].transform.localRotation = Quaternion.Euler(Vector3.zero);
        }
    }
}
