using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
	// public GameObject Shooter { get; set; } // The gun that shot this

	private void OnCollisionEnter2D(Collision2D collision)
	{
		GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>()
			.CreateExplosion(transform.position, 10);
		Destroy(gameObject);
	}
}