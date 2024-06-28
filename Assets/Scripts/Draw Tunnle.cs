using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class NewBehaviourScript : MonoBehaviour
{
    // Start is called before the first frame update 

    public int xdim, ydim, zdim;
    public int mxdim = 60, mydim = 30, mzdim = 30;
    public int tunnleWidthX, tunnleDepthZ, tunnleHightY;
    private Cell[,,] grid;
    public bool[,,] barriers;
    public Material redMaterial, blueMaterial, blackMaterial;
    public NewLatticeScript latticeScript;
    bool isBariers = false;
    public Slider change;
    public Color[] color_map;
    [SerializeField] private InputField speedInputField;
    [SerializeField] private InputField tunnelx;
    [SerializeField] private InputField tunnelz;
    [SerializeField] private InputField tunnely;
    [SerializeField] private InputField sizex;
    [SerializeField] private InputField sizez;
    [SerializeField] private InputField sizey;
    [SerializeField] private Dropdown viscosityDropdown;
    [SerializeField] private Dropdown particlesDropdown;
    [SerializeField] private Dropdown coloringDropdown;
    public Button applyButton;

    public Button resetButton;
    public bool isSimulationActive = false;

    private List<GameObject> gridObjects = new List<GameObject>(); // Add this line

    // Tracer objects
    public GameObject tracerPrefab;
    private List<GameObject> tracers = new List<GameObject>();
    private List<Vector3> tracersPos = new List<Vector3>();
    public Tunnle tunnelScript;
    public Balloon balloon;
    public int particlesType = 0;
    public int coloringType = 0;


    // Applies the viscosity setting based on the dropdown selection


    public void ApplyAllSettings()
    {
        ApplyTunnelX(tunnelx.text);
        ApplyTunnelY(tunnely.text);
        ApplyTunnelz(tunnelz.text);

        ApplySpeedSetting(speedInputField.text);
        ApplyViscositySetting();
    }

    public void ApplyTunnelX(string sizex)
    {
        int number;
        bool success = int.TryParse(sizex, out number);

        if (success)
        {
            // Debug.Log("Converted number: " + number);
            number = math.min(math.max(number, 1), 5);
        }
        else
        {
            // Debug.LogError("Failed to convert string to number.");
            number = 4;
        }
        tunnleWidthX = number * 10;
        xdim = tunnleWidthX;
        tunnelScript.UpdateSize(new Vector3(1, 1, number));
    }

    public void ApplyTunnelY(string sizey)
    {
        int number;
        bool success = int.TryParse(sizey, out number);

        if (success)
        {
            // Debug.Log("Converted number: " + number);
            number = math.min(math.max(number, 1), 5);
        }
        else
        {
            // Debug.LogError("Failed to convert string to number.");
            number = 2;
        }
        tunnleHightY = number * 10;
        ydim = tunnleHightY;
        tunnelScript.UpdateSize(new Vector3(1, number, 1));
    }

    public void ApplyTunnelz(string sizez)
    {
        int number;
        bool success = int.TryParse(sizez, out number);

        if (success)
        {
            // Debug.Log("Converted number: " + number);
            number = math.min(math.max(number, 1), 5);
        }
        else
        {
            // Debug.LogError("Failed to convert string to number.");
            number = 2;
        }
        tunnleDepthZ = number * 10;
        zdim = tunnleDepthZ;
        tunnelScript.UpdateSize(new Vector3(number, 1, 1));
    }


    public void ApplyViscositySetting()
    {
        string selectedOption = viscosityDropdown.options[viscosityDropdown.value].text;
        switch (selectedOption)
        {
            case "O2":
                latticeScript.initViscosity = 0.0171f;
                break;
            case "CO2":
                latticeScript.initViscosity = 0.0146f;
                break;
            case "H2":
                latticeScript.initViscosity = 0.0089f;
                break;
            default:
                break;
        }
    }

    public void ApplyParticleSetting()
    {
        string selectedOption = particlesDropdown.options[particlesDropdown.value].text;
        switch (selectedOption)
        {
            case "No Particles":
                particlesType = 0;
                break;
            case "Particles":
                particlesType = 1;
                break;
            case "Flow Lines":
                particlesType = 2;
                break;
            default:
                break;
        }
    }

    public void ApllyColoring()
    {
        string selectedOption = coloringDropdown.options[coloringDropdown.value].text;
        switch (selectedOption)
        {
            case "No Particles":
                coloringType = 0;
                break;
            case "Particles":
                coloringType = 1;
                break;
            default:
                break;
        }
    }

    // Applies the speed setting
    public void ApplySpeedSetting(string speedText)
    {
        if (latticeScript != null)
        {
            if (float.TryParse(speedText, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out float speed))
            {
                eraseBarriers();

                latticeScript.initSpeed = speed;
                for (int x = 0; x < xdim; x++)
                {
                    for (int y = 0; y < ydim; y++)
                    {
                        for (int z = 0; z < zdim; z++)
                        {
                            latticeScript.zeroSite(x, y, z);
                            latticeScript.setEqual(x, y, z, speed, 0, 0, 1f);
                        }
                    }
                }
                // additional code
            }
            else
            {
                eraseBarriers();

                latticeScript.initSpeed = 0.1;
                for (int x = 0; x < xdim; x++)
                {
                    for (int y = 0; y < ydim; y++)
                    {
                        for (int z = 0; z < zdim; z++)
                        {
                            latticeScript.zeroSite(x, y, z);
                            latticeScript.setEqual(x, y, z, speed, 0, 0, 1f);
                        }
                    }
                }
            }

        }

    }







    public void setColorMaps(float vis)
    {
        color_map = new Color[6];
        // color_map[0] = Color(0, 0, 0, 0.1f); 
        color_map[0] = new Color(0, 0, 1, vis);
        color_map[1] = new Color(0, 1, 1, vis);
        color_map[2] = new Color(0, 1, 0, 0.07f);
        color_map[3] = new Color(1, 1, 0, 0.07f);
        color_map[4] = new Color(1, 0.65f, 0, 0.07f);
        color_map[5] = new Color(1, 0, 0, 0.07f);
    }
    private int evaluateColor(double d)
    {
        if (d > 0.9) return 5;
        if (d > 0.75) return 4;
        if (d > 0.6) return 3;
        if (d > 0.45) return 2;
        if (d > 0.3) return 1;
        if (d > 0.15) return 0;
        return 0;
    }

    double NormalizeSpeed(double speed, double minSpeed, double maxSpeed)
    {
        return (speed - minSpeed) / (maxSpeed - minSpeed);
    }
    double NormalizeCurl(double curl, double minCurl, double maxCurl)
    {
        return (curl - minCurl) / (maxCurl - minCurl);
    }




    public void convertVerticesToBarriers()
    {
        // Clear existing barriers
        eraseBarriers();

        // Find all Fan objects in the scene
        Fan[] fans = FindObjectsOfType<Fan>();

        // Define the boundaries for barrier placement
        int xMin = 0;
        int xMax = xdim;
        int yMin = 10; // Since you're subtracting 10, the effective minimum is 10.
        int yMax = ydim + 10; // Compensate for the 10 subtracted.
        int zMin = 0;
        int zMax = zdim;

        // Iterate through each Fan object
        foreach (Fan fan in fans)
        {
            // Get the vertices from the current Fan
            List<Vector3> vertices = fan.GetVertices();

            // Iterate through each vertex of the current Fan
            foreach (Vector3 vertex in vertices)
            {
                int xx = (int)(vertex.x + xdim / 2);
                int yy = (int)(vertex.y + ydim / 2);
                int zz = (int)(vertex.z + zdim / 2);

                // Check if the vertex is within the bounds
                if (xx >= xMin && xx < xMax && yy >= yMin && yy < yMax && zz >= zMin && zz < zMax)
                {
                    // Adjust yy by subtracting 10 to place the barrier correctly
                    barriers[xx, yy - 10, zz] = true;
                }
            }
        }
    }

    private void InitializeGrid()
    {
        // xdim = 40;
        // ydim = 20;
        // zdim = 20;

        // tunnleWidthX = 40;
        // tunnleHightY = 20;
        // tunnleDepthZ = 20;

        InitBarriers();

        // lattic = new LatticeBoltzmann_copy();
        // latticeScript = gameObject.GetComponent<NewLatticeScript>();
        // latticeScript = new NewLatticeScript();
        latticeScript.InitWithBarriers(xdim, ydim, zdim, barriers, mxdim, mydim, mzdim);
        // lattic.InitFluid(xdim, ydim, zdim);

        // Initialize each cell in the grid 
        for (int x = 0; x < mxdim; x++)
        {
            for (int y = 0; y < mydim; y++)
            {
                for (int z = 0; z < mzdim; z++)
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
        // Debug.Log(xdim + "   " + ydim + "   " + zdim);
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

                    gridObjects.Add(cube);


                    // Apply texture to cube 
                    Renderer renderer = cube.GetComponent<Renderer>();
                    renderer.material = grid[x, y, z].material;
                }
            }
        }
    }

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
                    if (z < 1 || z == zdim - 1) continue;
                    // Debug.Log("speed"+ latticeScript.speed2[x, y, z]+ "On cell" + x + "  " + y + "  " + z);

                    if (barriers[x, y, z])
                    {
                        // If the cell is a barrier, set its color to black  
                        grid[x, y, z].material.color = new Color(0, 0, 0, 0f); // black barrier 

                        double speed = math.sqrt(latticeScript.speed2[x, y, z]);

                        float speedf = (float)speed;
                        // Debug.Log("barSpeed  "+speedf);
                    }
                    else
                    {
                        if (coloringType == 0)
                            viewSpeed(x, y, z);
                        else
                            viewCurl(x, y, z);
                    }
                }
            }
        }
    }


    void viewSpeed(int x, int y, int z)
    {
        double speed = math.sqrt(latticeScript.speed2[x, y, z]);

        double norm = NormalizeSpeed(speed, latticeScript.min_speed, latticeScript.max_speed);
        int color_idx = evaluateColor(norm);
        grid[x, y, z].material.color = color_map[color_idx];
    }

    void viewCurl(int x, int y, int z)
    {
        double cur = latticeScript.curl[x, y, z];
        cur = NormalizeCurl(cur, latticeScript.min_curl, latticeScript.max_curl);
        int color_idx = evaluateColor(cur);
        grid[x, y, z].material.color = color_map[color_idx];
    }

    void Start()
    {
        grid = new Cell[mxdim, mydim, mzdim];
        barriers = new bool[mxdim, mydim, mzdim];
        latticeScript.InitializeArrays(mxdim, mydim, mzdim);
        tunnelScript = FindObjectOfType<Tunnle>();
        balloon = FindObjectOfType<Balloon>();

        //cellMaterials = new Material[1];  
        setColorMaps(0.1f);
        applyButton.onClick.AddListener(ApplyAllSettings);
        applyButton.onClick.AddListener(ToggleSimulation);
        resetButton.onClick.AddListener(resetSimulation); // Add this line
        //latticeScript.InitBarriers(barriers); 
    }

    private int frames = 0;
    private WindTunnelDistortion distoration;
    void Update()
    {

        // lattic.Collide(); 
        // lattic.Stream();

        // Debug.Log(latticeScript.speed2[13,13,10]);
        frames++;


        if (frames % change.value == 0)
        {
            if (isSimulationActive)
            {
                latticeScript.simulate();
                UpdateColors();
                convertVerticesToBarriers();
                // UpdateTracerArrowsLine();
                UpdateTracers();
                latticeScript.InitBarriers(barriers);
                HandleCollisionsAndDeformations();

                // balloon.ApplyDistortion(new Vector3((float)latticeScript.fx, (float)latticeScript.fy, (float)latticeScript.fz));
            }

        }
    }

    private List<Fan> fans = new List<Fan>();

    private void HandleCollisionsAndDeformations()
    {
        foreach (Fan fan in fans)
        {
            WindTunnelDistortion jellyfier = fan.GetComponent<WindTunnelDistortion>();
            if (jellyfier != null)
            {
                ApplyLBMVelocitiesToJellyfier(fan, jellyfier);
            }
        }
    }

    private void ApplyLBMVelocitiesToJellyfier(Fan fan, WindTunnelDistortion jellyfier)
    {
        MeshFilter[] meshFilters = fan.GetComponentsInChildren<MeshFilter>();
        foreach (MeshFilter meshFilter in meshFilters)
        {
            if (meshFilter != null && meshFilter.sharedMesh != null)
            {
                Mesh mesh = meshFilter.sharedMesh;
                Vector3[] vertices = mesh.vertices;
                Vector3[] velocities = new Vector3[vertices.Length];

                for (int i = 0; i < vertices.Length; i++)
                {
                    Vector3 worldVertex = meshFilter.transform.TransformPoint(vertices[i]);
                    int x = Mathf.Clamp((int)((worldVertex.x + tunnleWidthX / 2) / tunnleWidthX * xdim), 0, xdim - 1);
                    int y = Mathf.Clamp((int)(worldVertex.y / tunnleHightY * ydim), 0, ydim - 1);
                    int z = Mathf.Clamp((int)((worldVertex.z + tunnleDepthZ / 2) / tunnleDepthZ * zdim), 0, zdim - 1);

                    velocities[i] = new Vector3(
                        (float)latticeScript.ux[x, y, z],
                        (float)latticeScript.uy[x, y, z],
                        (float)latticeScript.uz[x, y, z]
                    );
                }

                jellyfier.ApplyVelocity(velocities);
            }
        }
    }



    public void resetSimulation()
    {
        isSimulationActive = false;
    }

    public void berforeStart()
    {
        ClearGridObjects();
        //isSimulationActive = false;
        InitBarriers();
        // latticeScript.ResetLattice();
        ClearTracers();
        InitializeGrid();
        VisualizeGrid();


    }

    private void ClearGridObjects()
    {
        foreach (var obj in gridObjects)
        {
            Destroy(obj);
        }
        gridObjects.Clear();
    }


    private void ClearTracers()
    {
        foreach (var tracer in tracers)
        {
            Destroy(tracer);
        }
        tracers.Clear();
    }

    public void ToggleSimulation()
    {
        isSimulationActive = !isSimulationActive;

        if (isSimulationActive)
        {
            berforeStart();
            InitializeGrid();
            InitBarriers();
            InitTracers();
            if (particlesType != 2) VisualizeGrid();
        }
    }

    public GameObject tracer;
    private void InitializeTracers()
    {
        for (int i = 0; i < 1000; i++) // Create 100 tracers for example
        {
            Vector3 position = new Vector3(
                UnityEngine.Random.Range(-tunnleWidthX / 2f, tunnleWidthX / 2f),
                UnityEngine.Random.Range(0, tunnleHightY),
                UnityEngine.Random.Range(-tunnleDepthZ / 2f, tunnleDepthZ / 2f)
            );

            tracer = Instantiate(tracerPrefab, position, Quaternion.identity);
            tracers.Add(tracer);
        }
    }



    private void UpdateTracerArrows()
    {
        foreach (var tracerArrow in tracers)
        {
            Vector3 position = tracerArrow.transform.position;
            int x = Mathf.Clamp((int)((position.x + tunnleWidthX / 2) / tunnleWidthX * xdim), 0, xdim - 1);
            int y = Mathf.Clamp((int)(position.y / tunnleHightY * ydim), 0, ydim - 1);
            int z = Mathf.Clamp((int)((position.z + tunnleDepthZ / 2) / tunnleDepthZ * zdim), 0, zdim - 1);

            Vector3 velocity = new Vector3(
                (float)latticeScript.ux[x, y, z],
                (float)latticeScript.uy[x, y, z],
                (float)latticeScript.uz[x, y, z]
            );

            tracerArrow.transform.position += velocity * 2;

            // Check if the tracer has reached the end of the tunnel
            if (tracerArrow.transform.position.x > tunnleWidthX / 2)
            {
                // Reset the tracer to the start
                tracerArrow.transform.position = new Vector3(-tunnleWidthX / 2, position.y, position.z);
            }

            if (velocity != Vector3.zero)
            {
                tracerArrow.transform.rotation = Quaternion.LookRotation(velocity);
            }
        }
    }


    private void InitializeTracersLine()
    {
        float starty = (tunnleHightY / 2) - 1, endy = (tunnleHightY / 2) + 1;
        float startz = -1, endz = 1, step = 0.2f;
        for (float y = starty; y < endy; y += step)
        {
            for (float z = startz; z < endz; z += step)
            {
                Vector3 position = new Vector3(-tunnleWidthX / 2, y, z);
                tracersPos.Add(position);

                GameObject tracer = Instantiate(tracerPrefab, position, Quaternion.identity);
                tracer.GetComponent<LineRenderer>().positionCount = 1;
                tracer.GetComponent<LineRenderer>().SetPosition(0, position);
                tracers.Add(tracer);
            }
        }
    }

    private void UpdateTracerArrowsLine()
    {
        int idx = 0;
        foreach (var tracerArrow in tracers)
        {
            Vector3 position = tracerArrow.transform.position;
            int x = Mathf.Clamp((int)((position.x + tunnleWidthX / 2) / tunnleWidthX * xdim), 0, xdim - 1);
            int y = Mathf.Clamp((int)(position.y / tunnleHightY * ydim), 0, ydim - 1);
            int z = Mathf.Clamp((int)((position.z + tunnleDepthZ / 2) / tunnleDepthZ * zdim), 0, zdim - 1);

            Vector3 velocity = new Vector3(
                (float)latticeScript.ux[x, y, z],
                (float)latticeScript.uy[x, y, z],
                (float)latticeScript.uz[x, y, z]
            );

            tracerArrow.transform.position += velocity * 10;

            // Update the LineRenderer with the new position
            LineRenderer lineRenderer = tracerArrow.GetComponent<LineRenderer>();
            lineRenderer.positionCount++;
            lineRenderer.SetPosition(lineRenderer.positionCount - 1, tracerArrow.transform.position);

            // Check if the tracer has reached the end of the tunnel
            if (tracerArrow.transform.position.x > tunnleWidthX / 2)
            {
                // Reset the tracer to the start
                tracerArrow.transform.position = tracersPos[idx];

                // Clear the LineRenderer
                lineRenderer.positionCount = 1;
                lineRenderer.SetPosition(0, tracerArrow.transform.position);
            }

            if (velocity != Vector3.zero)
            {
                tracerArrow.transform.rotation = Quaternion.LookRotation(velocity);
            }

            idx++;
        }
    }

    public void UpdateTracers()
    {
        if (particlesType == 0)
        {
            return;
        }
        else if (particlesType == 1)
        {
            UpdateTracerArrows();
        }
        else
        {
            UpdateTracerArrowsLine();
        }
    }

    public void InitTracers()
    {
        if (particlesType == 0)
        {
            return;
        }
        else if (particlesType == 1)
        {
            InitializeTracers();
        }
        else
        {
            InitializeTracersLine();
        }
    }

}