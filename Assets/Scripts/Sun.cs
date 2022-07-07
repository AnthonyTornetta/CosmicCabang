using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sun : MonoBehaviour, IHasGravity
{
	public float Mass { get; private set; }

	public float Radius { get; private set; }

	private void Start()
	{
		Radius = transform.localScale.x;
		Mass = (float)Math.PI * Radius * Radius * 300;
		print(Radius);
		print(Mass);
	}

	private void OnCollisionEnter2D(Collision2D col)
	{
		Destroy(col.gameObject);
		print("poof");
	}
}
