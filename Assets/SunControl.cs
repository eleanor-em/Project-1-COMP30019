using UnityEngine;
using System.Collections;

public class SunControl : MonoBehaviour {
    public float rotationSpeed;
    public GameObject terrain;
	
	// Update is called once per frame
	void Update () {
        float mapSize = terrain.GetComponent<TerrainManager>().Size;
        transform.RotateAround(new Vector3(mapSize / 2, 0, mapSize / 2),
                               Vector3.forward, rotationSpeed * Time.deltaTime);
	}
}
