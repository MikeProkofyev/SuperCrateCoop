using UnityEngine;
using System.Collections;

public class PlayerAnimationStates : MonoBehaviour {

	public float speed = 0f;
	public bool ground = false;
	public float vSpeed = 0f;
	public bool shooting = false;

	Animator anim;

	void Start() {
//		anim = GetComponent<Animator> ();
	}

}
