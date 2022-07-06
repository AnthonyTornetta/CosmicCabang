using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetFactory : MonoBehaviour
{
	public static void CreatePlanet(int radius, Vector2 position, GameObject planetPrefab)
	{
		var go = Instantiate(planetPrefab, position, Quaternion.identity);
		var planet = go.GetComponent<Planet>();
		planet.blocksRadius = radius;
	}
}