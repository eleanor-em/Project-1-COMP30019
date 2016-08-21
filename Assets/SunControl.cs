using UnityEngine;
using System;
using UnityEngine.UI;

public class SunControl : MonoBehaviour {
    public float rotationSpeed;
    public GameObject terrain;
    public Text timeOfDay;
	
	void Update () {
        float mapSize = terrain.GetComponent<TerrainManager>().Size;
        transform.RotateAround(new Vector3(mapSize / 2, 0, mapSize / 2),
                               Vector3.forward, rotationSpeed * Time.deltaTime);

        // calculate time of day, for funsies; 0 degree rotation is 06:00
        float adjustedRot = transform.rotation.eulerAngles.z + 90;
        if (adjustedRot > 360) {
            adjustedRot -= 360;
        }
        int hours = (int)(adjustedRot / 15);
        int minutes = (int)((adjustedRot - hours * 15) * 4);
        timeOfDay.text = String.Format("{0}:{1}", hours.ToString("D2"), minutes.ToString("D2"));
	}
}
