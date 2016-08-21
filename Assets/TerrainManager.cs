using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class TerrainManager : MonoBehaviour {
    /// <summary>
    /// Detail level of the terrain.
    /// </summary>
    public int detail;
    /// <summary>
    /// How rough the fractal looks.
    /// </summary>
    public float roughness;
    /// <summary>
    /// Maximum height of the terrain; used only for shading.
    /// </summary>
    public float maxHeight;
    /// <summary>
    /// The shader to use.
    /// </summary>
    public Shader shader;
    /// <summary>
    /// The scaling factor for the map. Also included are percentages used to decide
    /// colours.
    /// </summary>
    public float mapScale;
    public float snowHeight;
    public float dirtHeight;
    public float sandHeight;

    /// <summary>
    /// Lighting options
    /// </summary>
    public float ambientIntensity;
    public float ambientAlbedo;
    public float diffuseIntensity;
    public float diffuseAlbedo;
    public float attenuationFactor;
    public float specularity;
    public float specularAlbedo;

    /// <summary>
    /// Sun and camera objects for lighting.
    /// </summary>
    public GameObject sun;
    public GameObject cam;

    // Size of the map
    private int max;
    public float Size { get { return max * mapScale; } }
    // Actual map as an array
    private float[,] map;

    // Used for dynamic camera and sun movement
    private Material material;

    // Use this for initialization
    void Start () {
        // Create the map
        int size = (int)Mathf.Pow(2, detail) + 1;
        max = size - 1;
        map = new float[size, size];

        // Set some initial seed values. Let's start by making each corner halfway up
        map[0, 0] = 0;//Random.value * maxHeight;
        map[0, max] = 0; //Random.value * maxHeight;
        map[max, 0] = 0; //Random.value * maxHeight;
        map[max, max] = 0; //Random.value * maxHeight;

        // Start the recursion
        Divide(max);
        // Generate the mesh
        Mesh mesh = CreateMesh();
        mesh.RecalculateNormals();
        gameObject.AddComponent<MeshFilter>().mesh = mesh;
        // Create the material
        material = gameObject.AddComponent<MeshRenderer>().material;
        // Set up shader
        material.shader = shader;
        material.SetFloat("_maxHeight", maxHeight);
        material.SetFloat("_snowHeight", snowHeight);
        material.SetFloat("_dirtHeight", dirtHeight);
        material.SetFloat("_sandHeight", sandHeight);
    }

    // Returns actual height at the given place
    public float Get(float x, float z) {
        int xx = (int)(x / mapScale);
        int yy = (int)(z / mapScale);
        if (xx < 0) xx = 0;
        if (xx > max) xx = max;
        if (yy < 0) yy = 0;
        if (yy > max) yy = max;
        return map[xx, yy];
    }

    // Returns the value of the array at x and y, with wrapping
    private float Get(int x, int y) {
        // Wrap around sides of array to prevent bad access; also gives a nice symmetry
        if (x < 0) x += max;
        if (x > max) x -= max;
        if (y < 0) y += max;
        if (y > max) y -= max;
        return map[x, y];
    }

    // Recursive function to subdivide the map
    // based on http://www.playfuljs.com/realistic-terrain-in-130-lines/
    private void Divide(int size) {
        // scale based on size -- early choices matter more than later ones
        float scale = roughness * size;
        // check if we've divided as far as we can
        if (size / 2 < 1) {
            return;
        }
        // loop over squares
        for (int y = size / 2; y < max; y += size) {
            for (int x = size / 2; x < max; x += size) {
                // create a square with +/- scale offset
                Square(x, y, size / 2, Random.value * scale * 2 - scale);
            }
        }
        // loop over diamonds
        for (int y = 0; y <= max; y += size / 2) {
            for (int x = (y + size / 2) % size; x <= max; x += size) {
                // create a diamond with the given offset
                Diamond(x, y, size / 2, Random.value * scale * 2 - scale);
            }
        }
        Divide(size / 2);
    }

    // Returns the average of four values
    private float Average(float a, float b, float c, float d) {
        return (a + b + c + d) * 0.25f;
    }

    // Generates a square average at the given point, of the given size, with an offset value
    private void Square(int x, int y, int size, float offset) {
        map[x, y] = Average(Get(x - size, y - size),
                            Get(x + size, y - size),
                            Get(x + size, y + size),
                            Get(x - size, y + size))
                    + offset;
    }
    // Generates a diamond average at the given point, of the given size, with an offset value
    private void Diamond(int x, int y, int size, float offset) {
        map[x, y] = Average(Get(x, y - size),
                            Get(x + size, y),
                            Get(x, y + size),
                            Get(x - size, y))
                    + offset;
    }

    // Create the mesh from the map
    private Mesh CreateMesh() {
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        // Iterate over all quads in the map
        for (int i = 0; i < max - 1; ++i) {
            for (int j = 0; j < max - 1; ++j) {
                AppendQuad(vertices, triangles, i, j);
            }
        }

        Mesh mesh = new Mesh();
        mesh.name = "Terrain";
        mesh.vertices = vertices.ToArray();
        // Linear mapping of triangles
        mesh.triangles = triangles.ToArray();
        return mesh;
    }

    // Appends a quad described by the given coordinates to the given list of vertices
    private void AppendQuad(List<Vector3> vertices, List<int> triangles, int x, int z) {
        Vector3 offset = transform.position;
        int n = vertices.Count;
        // First triangle
        vertices.Add(new Vector3(x, map[x, z] / mapScale, z) * mapScale
                     + offset);                                                           // Bottom left
        triangles.Add(n);
        vertices.Add(new Vector3(x, map[x, z + 1] / mapScale, z + 1) * mapScale
                     + offset);                                                           // Top left
        triangles.Add(n + 1);
        vertices.Add(new Vector3(x + 1, map[x + 1, z] / mapScale, z) * mapScale
                     + offset);                                                           // Bottom right
        triangles.Add(n + 2);
        // Second triangle
        vertices.Add(new Vector3(x + 1, map[x + 1, z + 1] / mapScale, z + 1) * mapScale
                     + offset);                                                           // Top right
        // Build up the triangle from previous triangles
        triangles.Add(n + 1);
        triangles.Add(n + 3);
        triangles.Add(n + 2);
    }

    void Update() {
        // Update shader settings
        material.SetVector("_sunPosition", sun.transform.position);
        material.SetVector("_camPosition", cam.transform.position);
        // These are included so that the settings can be played with
        // while the game is running
        material.SetFloat("_Ia", ambientIntensity);
        material.SetFloat("_Ka", ambientAlbedo);
        material.SetFloat("_Ip", diffuseIntensity);
        material.SetFloat("_Kd", diffuseAlbedo);
        material.SetFloat("_C", attenuationFactor);
        material.SetFloat("_n", specularity);
        material.SetFloat("_Ks", specularAlbedo);
    }
}
