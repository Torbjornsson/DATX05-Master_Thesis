using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMaster : MonoBehaviour
{
    public static GameMaster instance {get; private set;}

    public bool goalCriteriaSatisfied = false;

    // Start is called before the first frame update
    void Start()
    {

    }

    void Awake() {
        instance = this;

        var objects = GameObject.FindObjectsOfType<GameMaster>();
        if (objects.Length > 1 || !objects[0].Equals(instance))
            Debug.LogError("Game Master Script: Too many scripts active!");
    }

    // Update is called once per frame
    void Update()
    {
        // if (goalCriteriaSatisfied)
        //     Debug.Log("WON THE GAME!!!!");
    }
}
