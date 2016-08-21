using UnityEngine;
using System.Collections;

public class CameraControl : MonoBehaviour {
    public float speed;
    public float angularSpeed;
    public GameObject terrainObject;

    private float gap = 10f;

    void Start() {
        // Lock the cursor to allow mouse look
        Cursor.lockState = CursorLockMode.Locked;
    }

	// Update is called once per frame
	void Update () {
        // Movement
	    if (Input.GetKey(KeyCode.W)) {
            transform.position += transform.rotation * new Vector3(0, 0, speed) * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.S)) {
            transform.position += transform.rotation * new Vector3(0, 0, -speed) * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.A)) {
            transform.position += transform.rotation * new Vector3(-speed, 0, 0) * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.D)) {
            transform.position += transform.rotation * new Vector3(speed, 0, 0) * Time.deltaTime;
        }
        Quaternion delta = Quaternion.identity;
        // Roll
        if (Input.GetKey(KeyCode.Q)) {
            delta *= Quaternion.AngleAxis(angularSpeed * Time.deltaTime, Vector3.forward);
        }
        if (Input.GetKey(KeyCode.E)) {
            delta *= Quaternion.AngleAxis(-angularSpeed * Time.deltaTime, Vector3.forward);
        }
        // Pitch
        float pitch = Input.GetAxis("Mouse Y");
        delta *= Quaternion.AngleAxis(angularSpeed * Time.deltaTime * pitch,
                                      -Vector3.right);
        // Yaw
        float yaw = Input.GetAxis("Mouse X");
        delta *= Quaternion.AngleAxis(angularSpeed * Time.deltaTime * yaw,
                                                   Vector3.up);
        transform.rotation *= delta;

        // Keep camera inside terrain
        bound();
    }

    private void bound() {
        TerrainManager terrain = terrainObject.GetComponent<TerrainManager>();
        // Keep the camera above the terrain
        float h = terrain.Get(transform.position.x, transform.position.z) + gap;
        Vector3 newPos = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        if (transform.position.y < h) {
            newPos.y = h;
        }
        // Keep the camera inside the terrain's bounds
        if (newPos.x < -terrain.Size / 2 + gap) {
            newPos.x = -terrain.Size / 2 + gap;
        }
        if (newPos.z < -terrain.Size / 2 + gap) {
            newPos.z = -terrain.Size / 2 + gap;
        }
        if (newPos.x > terrain.Size / 2 - gap) {
            newPos.x = terrain.Size / 2 - gap;
        }
        if (newPos.z > terrain.Size / 2 - gap) {
            newPos.z = terrain.Size / 2 - gap;
        }
        transform.position = newPos;
    }
}
