using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TreeController : MonoBehaviour
{
    [HideInInspector] public float TreeHeight = 2;
    [SerializeField] private Transform TreeTopPart;
    [SerializeField] private Transform TreeRoot;
    [SerializeField] private PrefabRef PrefabRef;

    [Header("Tree Spawn Radius")]
    [Range(10, 100)]
    [SerializeField] private float NewTreeMinSpawnRadius;
    [Range(10, 100)]
    [SerializeField] private float NewTreeMaxSpawnRadius;

    
    [Header("Tree Spawn Timers")]
    [SerializeField] private float minSpawnTime = 115;
    [SerializeField] private float maxSpawnTime = 200;

    private float NewTreeSpawntime;
    private int TreesToSpawn;
    private float SpawnTimer;
    private bool SpawnMore;

    [Header("Tree Spawn Chances")]
    [SerializeField] private float OneTreeSpawnChance = 0.7f;
    [SerializeField] private float TwoTreesSpawnChance = 0.23f;
    [SerializeField] private float ThreeTreesSpawnChance = 0.12f;

    [SerializeField] private LayerMask NonPlacable;
    
    private float xTerrainMaxPos;
    private float zTerrainMaxPos;

    [SerializeField] private string[] NonBuildTextureLayers;

    [SerializeField] private float MutationPercent = 0.35f;

    public List<GameObject> leafesList = new List<GameObject>();
    [SerializeField] private float _leafShrinkDuration;
    [HideInInspector] public bool _isBeingEaten;
    

    [SerializeField] private Color[] TreeColors;
    [SerializeField] private Color[] LeafColors;

    private TreeCanvasManager TreeCanvasManager;

    private Vector3 startScale = new Vector3(0.2f, 0.2f, 0.2f);
    private Vector3 endScale = new Vector3(1, 1, 1);
    private float ShrinkDuration = 1.0f;
    
    void Start()
    {
        TreeCanvasManager = GetComponentInChildren<TreeCanvasManager>();
        NewTreeSpawntime = Random.Range((float)minSpawnTime, (float)maxSpawnTime); 
        
        xTerrainMaxPos = TerrainController.instance.xTerrainMaxPos;
        zTerrainMaxPos = TerrainController.instance.zTerrainMaxPos;

        //Set Random Color of a tree
        SkinnedMeshRenderer TreeMaterial = TreeRoot.GetComponent<SkinnedMeshRenderer>();
        TreeMaterial.material.color = TreeColors[Random.Range(0, TreeColors.Length)]; 

        //set random leaf color of a tree
        foreach (GameObject leaf in leafesList)
        {
            leaf.GetComponent<MeshRenderer>().material.color = LeafColors[Random.Range(0, TreeColors.Length)]; 
        }

        //Set Random Height of a tree
        float mutationThisTime = Random.Range(-MutationPercent, MutationPercent);
        float newYvalue = TreeTopPart.transform.localPosition.y + (TreeTopPart.transform.localPosition.y * mutationThisTime);
        
        TreeTopPart.transform.localPosition = new Vector3(TreeTopPart.transform.localPosition.x, newYvalue, TreeTopPart.transform.localPosition.z);

        //Set Tree height value
        TreeHeight = TreeHeight + (TreeHeight * mutationThisTime);
        if(TreeCanvasManager)
        {
            TreeCanvasManager.HeightText.SetText("Height - " + TreeHeight.ToString("f2") +"m");
        }
        
        //Add tree data to the GameController
        GameController.instance.AddTreeToOverall();
        GameController.instance.DailyBornTrees++;
        GameController.instance.InsertIndividualTreeData(TreeHeight, this.gameObject);

        // Makes tree grow when just spawns on the scene
        StartCoroutine(Grow(this.gameObject));

        float randomTreeCountValue = Random.value;
    }

    
    private void Update()
    {
        if(SpawnTimer > NewTreeSpawntime && SpawnMore && TreesToSpawn > 0)
        {
            TreesToSpawn--;
            SpawnTimer = 0;
            StartCoroutine(SpawnTree());
        }
        else
        {
            SpawnTimer += Time.deltaTime;
        }
    }

    public IEnumerator SpawnTree()
    {
        bool treeIsSpawned = false;

        //get random rotation for a new tree
        Vector3 euler = transform.eulerAngles;
        euler.y = Random.Range(0f, 360f);
        
        GameObject newTree =  Instantiate(PrefabRef.prefab, Vector3.zero, Quaternion.Euler(euler));

        TreeController newTreeController = newTree.GetComponent<TreeController>();

        //set actual tree height to parent so that it mutates on start
        newTreeController.TreeTopPart.transform.localPosition = new Vector3(newTreeController.TreeTopPart.transform.localPosition.x, TreeTopPart.transform.localPosition.y, newTreeController.TreeTopPart.transform.localPosition.z);
        newTreeController.TreeHeight = TreeHeight;
        newTreeController.enabled = false;

        while(!treeIsSpawned)
        {
            yield return new WaitForSeconds(0.2f);

            //Get random point in the radius from the parent tree taking into account the radius and the height of the terrain on that point
            Vector3 randomLocalPointInRadius = Random.insideUnitCircle * NewTreeMaxSpawnRadius;
            Vector3 randomWorldPointInRadius = transform.TransformPoint(randomLocalPointInRadius.x, 0, randomLocalPointInRadius.y);
            var yPos = Terrain.activeTerrain.SampleHeight(randomWorldPointInRadius);

            randomWorldPointInRadius = new Vector3(randomWorldPointInRadius.x, yPos, randomWorldPointInRadius.z);
            
            //Check if the possition in the radius is actually on our terrain,
            //Check if it's colliding with another excisting tree
            //Check if the tree is not on a forbidden texture of the terrain
            if(IsInTerrain(randomWorldPointInRadius) && !isColliding(randomWorldPointInRadius, newTree) && !IsInForbiddenTexture(randomWorldPointInRadius) && IsOnNavMesh(randomWorldPointInRadius))
            {
                //Put tree on terrain
                newTree.transform.position = randomWorldPointInRadius;
                newTreeController.enabled = true;
                treeIsSpawned = true;  
            }
        }
        yield break; 
    }

    public bool IsOnNavMesh(Vector3 Position)
    {
        NavMeshHit hit;
        return NavMesh.SamplePosition(Position, out hit, 1.0f, 3);
        
    }

    public bool IsInForbiddenTexture(Vector3 _randomWorldPointInRadius)
    {
        int surfaceIndex = GetMainTexture(_randomWorldPointInRadius);
        string textureName = TerrainController.instance.terrainData.splatPrototypes[surfaceIndex].texture.name;
        for (int i = 0; i < NonBuildTextureLayers.Length; i++)
        {
            if(textureName == NonBuildTextureLayers[i])
            {
                return true;
            }  
        }
        return false;
    }

    
    public bool isColliding(Vector3 randomWorldPointInRadius, GameObject _newTree)
    {
        SphereCollider TreeCollider = _newTree.GetComponent<SphereCollider>();
        TreeCollider.isTrigger = true;
        Vector3 SphereCenter = randomWorldPointInRadius + TreeCollider.center;

        if (!Physics.CheckSphere(SphereCenter, TreeCollider.radius, NonPlacable, QueryTriggerInteraction.Ignore))
        {
            //No collision detected
            TreeCollider.isTrigger = false;
            return false;
           
        }
        else
        {
            //Collision detected
            TreeCollider.isTrigger = false;
            return true;
            
        }
    }

    public bool IsInTerrain(Vector3 _randomPointInRadius)
    {
        if(_randomPointInRadius.x > 0 && _randomPointInRadius.x < xTerrainMaxPos && _randomPointInRadius.z > 0 && _randomPointInRadius.z < zTerrainMaxPos)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

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

    public void BeEaten()
    {   
        _isBeingEaten = true;
        int RandomLeafIndex = Random.Range(0, leafesList.Count);
        GameObject randomLeaf = leafesList[RandomLeafIndex];
        StartCoroutine(ShrinkLeaf(randomLeaf));
        leafesList.RemoveAt(RandomLeafIndex);
    }

    public IEnumerator ShrinkLeaf(GameObject Leaf)
    {
        Vector3 startScale = Leaf.transform.localScale;

        for (float t = 0; t < 1; t += Time.deltaTime / _leafShrinkDuration)
        {
            Leaf.transform.localScale = Vector3.Lerp(startScale, Vector3.zero, t);
            yield return null;
        }

        _isBeingEaten = false;

        Destroy(Leaf);

        if(leafesList.Count <= 0)
        {
            OnTreeEaten();
        }  
    }

    public void OnTreeEaten()
    {
        GameController.instance.RemoveIndividualTreeData(this.gameObject);
        GameController.instance.RemoveTreeFromOverall();
        GameController.instance.DailyDiedTrees++;
        Destroy(gameObject);
    }

    public IEnumerator Grow(GameObject newTree)
    {
        float startTime = Time.time;
        while (Time.time < startTime + ShrinkDuration)
        {
            float timeFraction = (Time.time - startTime) / ShrinkDuration;
            newTree.transform.localScale = Vector3.Lerp(startScale, endScale, timeFraction);
            yield return null;
        }
        newTree.transform.localScale = endScale;
    }
}
