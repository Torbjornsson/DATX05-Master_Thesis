using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RubiksCubeFace : MonoBehaviour
{
    public RotateRubiks main;
    // public RubiksBoxScript[] boxes;
    [Space]
    public RubiksBoxScript boxTopLeft;
    public RubiksBoxScript boxTopRight;
    public RubiksBoxScript boxBottomLeft;
    public RubiksBoxScript boxBottomRight;

    public bool solutionFound {get; private set;}

    // Start is called before the first frame update
    void Start()
    {
        // if (boxes.Length != 4)
        //     Debug.LogError(gameObject.name+": Did not find 4 different RubiksBoxScript!");
        
        // for (int i = 0; i < boxes.Length; i++) {
        //     if (!boxes[i])
        //         Debug.LogError(gameObject.name+": One of the assigned RubiksBoxScript was not found!");
        // }
        
        if (!boxTopLeft)
            Debug.LogError(gameObject.name+": Top Left RubiksBoxScript was not found!");
        if (!boxTopRight)
            Debug.LogError(gameObject.name+": Top Right RubiksBoxScript was not found!");
        if (!boxBottomLeft)
            Debug.LogError(gameObject.name+": Bottom Left RubiksBoxScript was not found!");
        if (!boxBottomRight)
            Debug.LogError(gameObject.name+": Bottom Right RubiksBoxScript was not found!");
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void CheckSolution() {
        // Debug.Log("Checking solution in face "+gameObject.name+"... "+FacesToString());
        // First rotation
        if (boxTopLeft.GetActiveSymbol() == main.solutionTopLeft && boxTopRight.GetActiveSymbol() == main.solutionTopRight
                && boxBottomLeft.GetActiveSymbol() == main.solutionBottomLeft && boxBottomRight.GetActiveSymbol() == main.solutionBottomRight) {
            solutionFound = true;

        // Second rotation
        } else if (boxTopRight.GetActiveSymbol() == main.solutionTopLeft && boxBottomRight.GetActiveSymbol() == main.solutionTopRight
                && boxTopLeft.GetActiveSymbol() == main.solutionBottomLeft && boxBottomLeft.GetActiveSymbol() == main.solutionBottomRight) {
            solutionFound = true;
            
        // Third rotation
        } else if (boxBottomRight.GetActiveSymbol() == main.solutionTopLeft && boxBottomLeft.GetActiveSymbol() == main.solutionTopRight
                && boxTopRight.GetActiveSymbol() == main.solutionBottomLeft && boxTopLeft.GetActiveSymbol() == main.solutionBottomRight) {
            solutionFound = true;
            
        // Fourth rotation
        } else if (boxBottomLeft.GetActiveSymbol() == main.solutionTopLeft && boxTopLeft.GetActiveSymbol() == main.solutionTopRight
                && boxBottomRight.GetActiveSymbol() == main.solutionBottomLeft && boxTopRight.GetActiveSymbol() == main.solutionBottomRight) {
            solutionFound = true;
        
        // No luck
        } else {
            solutionFound = false;
        }
    }

    public string FacesToString() {
        return "Top left: "+boxTopLeft.GetActiveSymbol() + ", Top right " + boxTopRight.GetActiveSymbol()
        + ", Bottom left " + boxBottomLeft.GetActiveSymbol() + ", Bottom right " + boxBottomRight.GetActiveSymbol();
    }
}
