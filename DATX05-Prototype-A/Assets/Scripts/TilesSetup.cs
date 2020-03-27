using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TilesSetup : MonoBehaviour
{
    private GameObject[] tiles;

    private List<Vector3> positions = new List<Vector3>();
    // Start is called before the first frame update
    void Start()
    {
        tiles = GameObject.FindGameObjectsWithTag("Tile");

        ScrambleTiles(tiles);

        ScrambleTiles(tiles);
    }

    private Vector3 GetRandomPosition(List<Vector3> positions)
    {
        int index = -1;
        Vector3 newPos = Vector3.zero;
        index = Random.Range(0, positions.Count - 1);
        newPos = positions[index];

        positions.RemoveAt(index);
        return newPos;
    }

    private void ScrambleTiles(GameObject[] tiles)
    {
        for (int i = 0; i < tiles.Length; i++)
        {
            positions.Add(tiles[i].transform.position);
        }

        for (int i = 0; i < tiles.Length; i++)
        {
            tiles[i].transform.position = GetRandomPosition(positions);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
