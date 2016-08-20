using UnityEngine;
using System.Collections;

public class LightControl : MonoBehaviour {
    public GameObject sun;

	// Update is called once per frame
	void Update () {
        transform.LookAt(Vector3.zero);
	}
}
