using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;



public enum ObjectType {COYOTE_MALE, COYOTE_FEMALE, DEER_MALE, DEER_FEMALE, TREE}

[System.Serializable]
public class SpawnObject
{
    public ObjectType ObjectType;
    public PrefabRef spawnObject;
    public string[] NonPlacableTextures;
    public int Count;
}

public class SpawnManager : MonoBehaviour
{
    public SpawnObject[] SpawnObject;

    public LayerMask NonPlacable;

    public float xTerrainMaxPos;
    public float zTerrainMaxPos;

    








    void Start()
    {
        xTerrainMaxPos = TerrainController.instance.xTerrainMaxPos;
        zTerrainMaxPos = TerrainController.instance.zTerrainMaxPos;


        SpawnObject[0].Count = PlayerPrefs.GetInt("CoyoteMale");
        SpawnObject[1].Count = PlayerPrefs.GetInt("CoyoteFemale");
        SpawnObject[2].Count = PlayerPrefs.GetInt("DeerMale");
        SpawnObject[3].Count = PlayerPrefs.GetInt("DeerFemale");
        SpawnObject[4].Count = PlayerPrefs.GetInt("Tree");
        

        for (int i = 0; i < SpawnObject.Length; i++)
        {
            for (int x = 0; x < SpawnObject[i].Count; x++)
            {
                StartCoroutine(Spawn(SpawnObject[i]));
            }
        }

        //return back
        // PlayerPrefs.SetInt("CoyoteMale", 0);
        // PlayerPrefs.SetInt("CoyoteFemale", 0);
        // PlayerPrefs.SetInt("DeerMale", 0);
        // PlayerPrefs.SetInt("DeerFemale", 0);
        // PlayerPrefs.SetInt("Tree", 0);



        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator Spawn(SpawnObject SpawnableObject)
    {
        bool ObjectSpawned = false;

        Vector3 euler = transform.eulerAngles;
        euler.y = Random.Range(0f, 360f);
        
        GameObject newObject =  Instantiate(SpawnableObject.spawnObject.prefab, Vector3.zero, Quaternion.Euler(euler));
        if(newObject.GetComponent<NavMeshAgent>())
        {
            newObject.GetComponent<NavMeshAgent>().enabled = false;
        }
        

        if(SpawnableObject.ObjectType == ObjectType.TREE)
        {
            TreeController newTreeController = newObject.GetComponent<TreeController>();
            newTreeController.enabled = false;
        }

        while(!ObjectSpawned)
        {
            yield return new WaitForSeconds(0.05f);
            Vector3 RandomPos = new Vector3(ReturnRandom(0, xTerrainMaxPos), 0, ReturnRandom(0, zTerrainMaxPos));
            Vector3 RandomPosNew = new Vector3(RandomPos.x, Terrain.activeTerrain.SampleHeight(RandomPos), RandomPos.z);

            if(!isColliding(RandomPosNew, SpawnableObject.spawnObject.prefab) && !IsInForbiddenTexture(RandomPosNew, SpawnableObject.NonPlacableTextures) && IsOnNavMesh(RandomPosNew))
            {
                ObjectSpawned = true;
                newObject.transform.position = RandomPosNew;

                if(newObject.GetComponent<NavMeshAgent>())
                {
                    newObject.GetComponent<NavMeshAgent>().enabled = true;
                }

                if(SpawnableObject.ObjectType == ObjectType.TREE)
                {
                    TreeController newTreeController = newObject.GetComponent<TreeController>();
                    newTreeController.enabled = true;
                }
            }

        }
        yield break;
    }

    public bool IsOnNavMesh(Vector3 Position)
    {
        NavMeshHit hit;
        return NavMesh.SamplePosition(Position, out hit, 1.0f, 3);
        
    }
    

    public bool isColliding(Vector3 randomWorldPointInRadius, GameObject _newTree)
    {
        SphereCollider Collider = _newTree.GetComponent<SphereCollider>();
        Collider.isTrigger = true;
        Vector3 SphereCenter = randomWorldPointInRadius + Collider.center;

        if (!Physics.CheckSphere(SphereCenter, Collider.radius, NonPlacable, QueryTriggerInteraction.Ignore))
        {
            //there was no collision
            Collider.isTrigger = false;
            return false;
           
        }
        else
        {
            Collider.isTrigger = false;
            return true;
            //was collision
        }
    }


    public bool IsInForbiddenTexture(Vector3 _randomWorldPointInRadius, string[] NonPlacableTextures)
    {
        int surfaceIndex = GetMainTexture(_randomWorldPointInRadius);
        string textureName = TerrainController.instance.terrainData.splatPrototypes[surfaceIndex].texture.name;
        for (int i = 0; i < NonPlacableTextures.Length; i++)
        {
            if(textureName == NonPlacableTextures[i])
            {
                //Debug.Log("Is in forbidden texture");
                return true;
            }
            
        }
        
        return false;
    }

    public float ReturnRandom(float Min, float Max)
    {
        return Random.Range(Min, Max);
    }






    //Get TerrainTexture
    private float[] GetTextureMix(Vector3 WorldPos){
         // returns an array containing the relative mix of textures
         // on the main terrain at this world position.
  
         // The number of values in the array will equal the number
         // of textures added to the terrain.
         
         // calculate which splat map cell the worldPos falls within (ignoring y)
         int mapX = (int)(((WorldPos.x - TerrainController.instance.terrainPos.x) / TerrainController.instance.terrainData.size.x) * TerrainController.instance.terrainData.alphamapWidth);
         int mapZ = (int)(((WorldPos.z - TerrainController.instance.terrainPos.z) / TerrainController.instance.terrainData.size.z) * TerrainController.instance.terrainData.alphamapHeight);
         
         // get the splat data for this cell as a 1x1xN 3d array (where N = number of textures)
         float[,,] splatmapData = TerrainController.instance.terrainData.GetAlphamaps( mapX, mapZ, 1, 1 );
         
         // extract the 3D array data to a 1D array:
         float[] cellMix = new float[ splatmapData.GetUpperBound(2) + 1 ];
         
         for(int n=0; n<cellMix.Length; n++){
             cellMix[n] = splatmapData[ 0, 0, n ];
         }
         return cellMix;
     }
     
    private int GetMainTexture(Vector3 WorldPos){
         // returns the zero-based index of the most dominant texture
         // on the main terrain at this world position.
         float[] mix = GetTextureMix(WorldPos);
         
         float maxMix = 0;
         int maxIndex = 0;
         
         // loop through each mix value and find the maximum
         for(int n=0; n<mix.Length; n++){
             if ( mix[n] > maxMix ){
                  maxIndex = n;
                  maxMix = mix[n];
                }
         }
         return maxIndex;
     }
}
