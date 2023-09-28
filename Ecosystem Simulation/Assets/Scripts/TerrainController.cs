using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainController : MonoBehaviour
{
    private float terrainWidth;
    private float terrainLength;

    private float xTerrainPos;
    private float zTerrainPos;

    public float xTerrainMaxPos;
    public float zTerrainMaxPos;
    public Terrain terrain;
    public TerrainData terrainData;
    public static TerrainController instance;
    public Vector3 terrainPos;
    

    void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }

        terrain = GetComponent<Terrain>();
        terrainData = terrain.terrainData;
        terrainPos = terrain.transform.position;
        
         //Get terrain size
        terrainWidth = terrain.terrainData.size.x;
        terrainLength = terrain.terrainData.size.z;

        //Get terrain position
        xTerrainPos = terrain.transform.position.x;
        zTerrainPos = terrain.transform.position.z;

        xTerrainMaxPos = xTerrainPos + terrainWidth;
        zTerrainMaxPos = zTerrainPos + terrainLength;
    }
    void Start()
    {
        


    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
