using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Based on KAY B 14's Unity 3D interactive grass shader graph (high performance method) tutorial
 * Link: https://www.youtube.com/watch?v=zm7rKXEPa9M
 */

public class Grass : MonoBehaviour
{
    public Material[] materials;    // shader of materials must be "Shader Graphs/Grass"
    public Transform player;
    Vector3 position;
    Vector3 headPosition;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine("writeToMaterial");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator writeToMaterial()
    {
        while (true)
        {
            // long grass
            // position = player.transform.position + player.transform.up;
            // headPosition = player.transform.position + (player.transform.up * 2.5f);

            // short grass
            // position and headPosition dictate how much grass should be deformed based on
            // the grass's distance from the position and headPosition
            position = player.transform.position;
            headPosition = player.transform.position + (player.transform.up * 0.5f);
            int length = materials.Length;
            for (int i = 0; i < length; i++)
            {
                materials[i].SetVector("_PlayerPosition", position);
                materials[i].SetVector("_PlayerHeadPosition", headPosition);
            }

            yield return null;
        }
    }
}
