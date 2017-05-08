using UnityEngine;
using System.Collections;

public class CubeAnimation : MonoBehaviour {

	void Update()
	{
		transform.Rotate(0F,30*Time.deltaTime,0F);
	}

}
