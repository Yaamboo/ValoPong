using UnityEngine;
using System.Collections;

public class ChangeColour : MonoBehaviour {
	
    public int lightId;
	
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
        light.color = renderer.material.color;
	}
}
