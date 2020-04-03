using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMaster : MonoBehaviour
{
    public static GameMaster instance { get; private set; }

    public TutorialMaster tutorialMaster;
    public bool goalCriteriaSatisfied = false;
    public bool LastSlideReached = false;
    public bool hasWon { get; private set; }

    private bool isPaused = false;
    private Camera pauseCamera;
    private GameObject ovrCameraRig;

    // Start is called before the first frame update
    void Start()
    {
        if (!tutorialMaster)
            Debug.LogWarning("Game Master Script: WARNING! Could not find a Tutorial Master!");

        hasWon = false;

        pauseCamera = GameObject.Find("PauseCamera").GetComponent<Camera>();
        pauseCamera.gameObject.SetActive(false);
        ovrCameraRig = GameObject.Find("OVRCameraRig");
    }

    void Awake()
    {
        instance = this;

        var objects = GameObject.FindObjectsOfType<GameMaster>();
        if (objects.Length > 1 || !objects[0].Equals(instance))
            Debug.LogError("Game Master Script: Too many scripts active!");
    }

    // Update is called once per frame
    void Update()
    {
        if (!OVRManager.hasVrFocus && !isPaused)
        {
            PauseGame();
        }
        else if (OVRManager.hasVrFocus && isPaused)
        {
            UnPauseGame();
        }
        // if (goalCriteriaSatisfied)
        //     Debug.Log("WON THE GAME!!!!");
    }

    public void PuzzleWon()
    {
        hasWon = true;
    }

    private void PauseGame()
    {
        Debug.Log("Pausing game");
        isPaused = true;
        ovrCameraRig.GetComponent<OVRCameraRig>().disableEyeAnchorCameras = true;
        pauseCamera.gameObject.SetActive(true);
    }

    private void UnPauseGame()
    {   
        if (!ovrCameraRig)
            ovrCameraRig = GameObject.Find("OVRCameraRig");
            
        Debug.Log("Unpausing game");
        isPaused = false;
        ovrCameraRig.GetComponent<OVRCameraRig>().disableEyeAnchorCameras = false;
        pauseCamera.gameObject.SetActive(false);
    }
}
