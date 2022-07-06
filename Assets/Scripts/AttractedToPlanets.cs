using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttractedToPlanets : MonoBehaviour
{
	// Start is called before the first frame update
	private GameController _gameController;
	private Rigidbody2D _body;

	private const float G = 0.01f;

	private void Start()
	{
		_gameController = GameObject.FindWithTag("GameController").GetComponent<GameController>();
		_body = GetComponent<Rigidbody2D>();
	}

	private void FixedUpdate()
	{
		foreach (var pair in _gameController.HaveGravities)
		{
			Vector3 diff = pair.Key.transform.position - transform.position;

			// Prevents stuff going crazy once it's inside
			float ratio = 1 - (pair.Value.Radius - diff.magnitude) /
				(pair.Value.Radius);
			if (ratio > 1)
				ratio = 1;

			if (ratio == 0)
				continue;

			float force = _body.mass * ratio * ratio * pair.Value.Mass * G / diff.sqrMagnitude;
			_body.AddForce(force * diff.normalized);
		}
	}
}