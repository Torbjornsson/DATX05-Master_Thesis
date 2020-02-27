using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMasterScript : MonoBehaviour
{
    public static GameMasterScript instance {get; private set;}

    public bool goalCriteriaSatisfied {get; set;}

    // Start is called before the first frame update
    void Start()
    {
        goalCriteriaSatisfied = false;
    }

    void Awake() {
        instance = this;

        var objects = GameObject.FindObjectsOfType<GameMasterScript>();
        if (objects.Length > 1 || !objects[0].Equals(instance))
            Debug.LogError("Game Master Script: Too many scripts active!");
    }

    // Update is called once per frame
    void Update()
    {
        if (goalCriteriaSatisfied)
            Debug.Log("WON THE GAME!!!!");
    }
}
