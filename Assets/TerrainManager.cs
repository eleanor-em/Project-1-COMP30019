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
    /// Maximum height of the terrain.
    /// </summary>
    public int maxHeight;
    /// <summary>
    /// The shader to use.
    /// </summary>
    public Shader shader;
    /// <summary>
    /// The scaling factor for the map.
    /// </summary>
    public float mapScale;
    public float snowHeight;
    public float dirtHeight;
    public float sandHeight;

    // Size of the map
    private int max;
    // Actual map as an array
    private float[,] map;

	// Use this for initialization
	void Start () {
        // Create the map
        int size = (int)Mathf.Pow(2, detail) + 1;
        max = size - 1;
        map = new float[size, size];

        // Set some initial seed values. Let's start by making each corner halfway up
        map[0, 0] = maxHeight / 2;
        map[0, max] = maxHeight / 2;
        map[max, 0] = maxHeight / 2;
        map[max, max] = maxHeight / 2;

        Divide(max);
        gameObject.AddComponent<MeshFilter>().mesh = CreateMesh();

        Material material = gameObject.AddComponent<MeshRenderer>().material;
        material.shader = shader;
        material.SetFloat("_maxHeight", maxHeight * 2);
        material.SetFloat("_snowHeight", snowHeight);
        material.SetFloat("_dirtHeight", dirtHeight);
        material.SetFloat("_sandHeight", sandHeight);
    }

    // Returns the value of the array at x and y, or -1 if it's out of bounds
    private float Get(int x, int y) {
        if (x < 0 || x > max || y < 0 || y > max) {
            return -1;
        }
        return map[x, y];
    }

    // Recursive function to subdivide the map
    private void Divide(int size) {
        float scale = roughness * size;
        // check if we've divided as far as we can
        if (size / 2 < 1) {
            return;
        }
        for (int y = size / 2; y < max; y += size) {
            for (int x = size / 2; x < max; x += size) {
                Square(x, y, size / 2, Random.value * scale * 2 - scale);
            }
        }
        for (int y = 0; y <= max; y += size / 2) {
            for (int x = (y + size / 2) % size; x <= max; x += size) {
                Diamond(x, y, size / 2, Random.value * scale * 2 - scale);
            }
        }
        Divide(size / 2);
    }

    // Returns the average of four values, checking validity
    private float Average(float a, float b, float c, float d) {
        int count = 0;
        float sum = 0;
        if (a != -1) { sum += a; count++; }
        if (b != -1) { sum += b; count++; }
        if (c != -1) { sum += c; count++; }
        if (d != -1) { sum += d; count++; }
        return sum / count;
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
        // Iterate over all quads in the map
        for (int i = 0; i < max - 1; ++i) {
            for (int j = 0; j < max - 1; ++j) {
                AppendQuad(vertices, i, j);
            }
        }

        Mesh mesh = new Mesh();
        mesh.name = "Terrain";
        mesh.vertices = vertices.ToArray();
        // Linear mapping of triangles
        mesh.triangles = Enumerable.Range(0, vertices.Count).ToArray();
        return mesh;
    }

    // Appends a quad described by the given coordinates to the given list of vertices
    private void AppendQuad(List<Vector3> vertices, int x, int z) {
        Vector3 offset = new Vector3(-max / 2, 0, -max / 2) * mapScale;
        // First triangle
        vertices.Add(new Vector3(x, map[x, z] / mapScale, z) * mapScale
                     + offset);                                                           // Bottom left
        vertices.Add(new Vector3(x, map[x, z + 1] / mapScale, z + 1) * mapScale
                     + offset);                                                           // Top left
        vertices.Add(new Vector3(x + 1, map[x + 1, z] / mapScale, z) * mapScale
                     + offset);                                                           // Bottom right
        // Second triangle
        vertices.Add(new Vector3(x + 1, map[x + 1, z] / mapScale, z) * mapScale
                     + offset);                                                           // Bottom right
        vertices.Add(new Vector3(x, map[x, z + 1] / mapScale, z + 1) * mapScale
                     + offset);                                                           // Top left
        vertices.Add(new Vector3(x + 1, map[x + 1, z + 1] / mapScale, z + 1) * mapScale
                     + offset);                                                           // Top right
    }

    // Update is called once per frame
    void Update () {
	
	}
}
