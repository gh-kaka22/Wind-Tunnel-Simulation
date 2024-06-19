using System; 
using System.Collections; 
using System.Collections.Generic; 
using Unity.Mathematics; 
using UnityEditor; 
using UnityEditor.PackageManager; 
using UnityEngine; 


public class NewBehaviourScript : MonoBehaviour 
{ 
    // Start is called before the first frame update 

    public int xdim, ydim, zdim; 
    public int tunnleWidthX, tunnleDepthZ, tunnleHightY; 
    private Cell[,,] grid; 
    public bool[,,] barriers; 
    // public Material[] cellMaterials; 
    //public Texture2D cellTexture; 
    public Material redMaterial; 
    public Material blueMaterial; 
    public Material blackMaterial; 
    //public LatticeBoltzmann_copy lattic; 
    public NewLatticeScript latticeScript;
    bool isBariers =  false;
    public Color[] color_map; 
 
 
    public void setColorMaps(float vis) 
    { 
        color_map = new Color[6]; 
        // color_map[0] = Color(0, 0, 0, 0.1f); 
        color_map[0] = new Color(0, 0, 1, vis); 
        color_map[1] = new Color(0, 1, 1, vis); 
        color_map[2] = new Color(0, 1, 0, vis); 
        color_map[3] = new Color(1, 1, 0, vis); 
        color_map[4] = new Color(1, 0.65f, 0, 0.01f); 
        color_map[5] = new Color(1, 0, 0, 0.01f); 
    }
    private int evaluateColor(double d) 
    { 
        if(d > 0.9) return 5; 
        if(d > 0.75) return 4; 
        if(d > 0.6) return 3; 
        if(d > 0.45) return 2; 
        if(d > 0.3) return 1; 
        if(d > 0.15) return 0; 
        return 0; 
    } 
 
    double NormalizeSpeed(double speed, double minSpeed, double maxSpeed) 
    { 
        return (speed - minSpeed) / (maxSpeed - minSpeed); 
    }


 

    // void OnDrawGizmos()
    // {
    //     Fan ac = FindObjectOfType<Fan>(); 
    //     List<Vector3> vertices = ac.getVertices();
    //     Gizmos.color = Color.red;
    //     foreach (Vector3 vertex in vertices)
    //     {
    //         Gizmos.DrawSphere(vertex, 0.1f); // Draw a small sphere at each vertex
    //     }
    // }
 public void convertVerticesToBarriers() 
    { 
        //AccessFanMesh ac = new AccessFanMesh(); 
        Fan ac = FindObjectOfType<Fan>(); 
        List<Vector3> vert = ac.getVertices();
 
        for(int i = 0; i < vert.Count; i++){ 
            Vector3 v = vert[i]; 
            int xx = (int)(v.x + xdim / 2); 
            int yy = (int)(v.y + ydim / 2); 
            int zz = (int)(v.z + zdim / 2);
            //Debug.Log((int)v.x + "   " +(int) v.y + "   " + (int)v.z); 
 
            barriers[xx, yy-10, zz] = true; 
        } 
    }
    private void InitializeGrid() 
    { 
        xdim = 80; 
        ydim = 20; 
        zdim = 20; 
 
        tunnleWidthX = 80; 
        tunnleHightY = 20; 
        tunnleDepthZ = 20;
 
        InitBarriers(); 
 
        // lattic = new LatticeBoltzmann_copy();
        // latticeScript = gameObject.GetComponent<NewLatticeScript>();
        latticeScript = new NewLatticeScript();
        latticeScript.InitWithBarriers(xdim, ydim, zdim, barriers);
        // lattic.InitFluid(xdim, ydim, zdim);
 
        // Initialize each cell in the grid 
        for (int x = 0; x < xdim; x++) 
        { 
            for (int y = 0; y < ydim; y++) 
            { 
                for (int z = 0; z < zdim; z++) 
                { 
                    grid[x, y, z] = new Cell();
                    grid[x, y, z].material = new Material(blackMaterial);
                    // if(barriers[x,y,z])
                    //     latticeScript.zeroSite(x,y,z);
                } 
            } 
        }
        
    } 
 
    public void InitBarriers()  
    {  
        barriers = new bool[xdim, ydim, zdim];
        isBariers = true;
        for (int x = 0; x < xdim; x++)  
        {  
            for (int y = 0; y < ydim; y++)  
            {  
                for (int z = 0; z < zdim; z++)  
                {  
                    // int xx = (xdim/2), yy = (ydim/2), zz = (zdim/2); 
                    // int al = 2; 
                    // if(x<50 && x > 30 && y>7.5 && y <12.5) 
                    //     barriers[x, y, z] = true; 
                    // else 
                        barriers[x, y, z] = false; 
                }  
            }  
        }  
    }

    public void eraseBarriers()
    {
        isBariers = false;
        for (int x = 0; x < xdim; x++)
        {
            for (int y = 0; y < ydim; y++)
            {
                for (int z = 0; z < zdim; z++)
                {
                    barriers[x, y, z] = false;
                }
            }
        }
    }
    private void VisualizeGrid() 
    { 
        float cubeX = tunnleWidthX / xdim; 
        float cubeY = tunnleHightY / ydim; 
        float cubeZ = tunnleDepthZ / zdim; 
 
        float startX = -tunnleWidthX / 2 + cubeX / 2; 
        float startY = cubeY / 2; 
        float startZ = -tunnleDepthZ / 2 + cubeZ / 2; 
 
        float endX = tunnleWidthX / 2; 
        float endY = tunnleHightY;

        float endZ = tunnleDepthZ / 2; 
 
        for (int x = 0; x < xdim; x++) 
        { 
            for (int y = 0; y < ydim; y++) 
            { 
                for (int z = 0; z < zdim; z++) 
                { 
                    float posX = (x * cubeX) - tunnleWidthX / 2 + cubeX / 2; 
                    float posY = (y * cubeY) + cubeY / 2;
                    float posZ = (z * cubeZ) - tunnleDepthZ / 2 + cubeZ / 2; 
 
                    GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube); 
                    cube.transform.position = new Vector3(posX, posY, posZ); 
 
                    // Apply texture to cube 
                    Renderer renderer = cube.GetComponent<Renderer>(); 
                    renderer.material = grid[x, y, z].material; 
                } 
            } 
        } 
    } 
 
    // private void UpdateColors() 
    // { 
    //     for (int x = 0; x < xdim; x++) 
    //     { 
    //         for (int y = 0; y < ydim; y++) 
    //         { 
    //             for (int z = 0; z < zdim; z++) 
    //             { 
    //                 // Get the current cell's material 
    //                 // Material cellMaterial = grid[x, y, z].material; 
    //                 // Debug.Log("speedFromPhysics  "+math.sqrt(latticeScript.speed2[x, y, z]));
    //
    //                 if (barriers[x, y, z]) 
    //                 { 
    //                     // If the cell is a barrier, set its color to black 
    //                      grid[x, y, z].material.color = new Color(0, 0, 1,0.4f); // Preserve alpha
    //                      double speed = math.sqrt(latticeScript.speed2[x, y, z]);
    //                     
    //                      float speedf = (float)speed * 10;
    //                      // Debug.Log("barSpeed  "+speedf);
    //                 } 
    //                 else 
    //                 { 
    //                     // Otherwise, change the color based on the speed\
    //                     double speed = math.sqrt(latticeScript.speed2[x, y, z]);
    //
    //                     float speedf = (float)speed * 10;
    //                     // Debug.Log("strSpeed  "+speedf);
    //                     if (speedf > 1)
    //                         grid[x, y, z].material.color = new Color(1.0f, 0, 0, speedf - 1);
    //                     else grid[x, y, z].material.color = new Color(1.0f, 0, 0, 0);
    //                     // else grid[x, y, z].material.color = new Color(1.0f, 0, 0, speedf - 0.7f);
    //                     // if(x == 3 && y == 3 && z == 3) Debug.Log("defSpeed  "+speedf);
    //                     // grid[x, y, z].material.color = new Color( 1.0f, 0, 0,    speedf);
    //                 } 
    //             } 
    //         } 
    //     } 
    // } 
    
    private void UpdateColors() 
    { 
        for (int x = 0; x < xdim; x++) 
        { 
            for (int y = 0; y < ydim; y++) 
            { 
                for (int z = 0; z < zdim; z++) 
                { 
                    // Get the current cell's material  
                    // Material cellMaterial = grid[x, y, z].material;  
                    if(z < 1 || z == zdim-1) continue; 
 
                    if (barriers[x, y, z]) 
                    { 
                        // If the cell is a barrier, set its color to black  
                         grid[x, y, z].material.color = new Color(1, 1, 1, 0.1f); // black barrier 
 
                        double speed = math.sqrt(latticeScript.speed2[x, y, z]); 
 
                        float speedf = (float)speed; 
                        // Debug.Log("barSpeed  "+speedf); 
                    } 
                    else 
                    { 
                        // // ############# Obada color viewing: 
                        double speed = math.sqrt(latticeScript.speed2[x, y, z]); 
                        // Debug.Log(norm);
                        // double speed2 = latticeScript.speed2[x, y, z]; 
 
                        //grid[x, y, z].material.color = new Color((float)speed2, 0, 0, 0.1f); 
                        double norm = NormalizeSpeed(speed, latticeScript.min_speed, latticeScript.max_speed); 
                        int color_idx = evaluateColor(norm); 
                        // Debug.Log(norm);
                        grid[x, y, z].material.color = color_map[color_idx]; 
                    } 
                } 
            } 
        } 
    }

 
    void Start()  
    {  
        grid = new Cell[200, 200, 200];  
        //cellMaterials = new Material[1];  
        setColorMaps(0.1f);
  
        InitializeGrid();  
        
 
        InitBarriers(); 
         
        VisualizeGrid(); 
        // convertVerticesToBarriers();
        latticeScript.InitBarriers(barriers);
        
        //latticeScript.InitBarriers(barriers); 
 
    }
    void Update() 
    { 
        // lattic.Collide(); 
        // lattic.Stream();
        latticeScript.simulate();
        UpdateColors(); 
        
        
        // if (Input.GetKeyDown(KeyCode.O)) 
        // { 
        //     if(isBariers)
        //     {
        //         eraseBarriers();
        //     } 
        //     else{ 
        //         InitBarriers(); 
        //         latticeScript.InitBarriers(barriers);
        //         // latticeScript.InitWithBarriers(xdim,ydim,zdim,barriers);
        //     } 
        // }
    } 

}