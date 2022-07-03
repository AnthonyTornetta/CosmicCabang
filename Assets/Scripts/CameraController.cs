using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
	public GameController controller;

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Space))
		{
			transform.position = controller.ActivePlayer.transform.position;
		}
	}
}