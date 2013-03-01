using UnityEngine;
using System.Collections;

public class ChangeColour : MonoBehaviour {
	
	//public Color colour;
    public int lightId;
	
	// Use this for initialization
	void Start () {
		//renderer.material.color = colour;
	}
	
	// Update is called once per frame
	void Update () {
        light.color = renderer.material.color;
		//renderer.material.color  = new Color(Random.value, Random.value, Random.value);
        //Color c = new Color(renderer.material.color.r + 0.01f, renderer.material.color.g + 0.01f, renderer.material.color.b + 0.01f);
        //renderer.material.color = c;
	}
}
