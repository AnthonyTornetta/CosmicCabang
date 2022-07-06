using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

public class GameController : MonoBehaviour
{
	public Dictionary<GameObject, IHasGravity> HaveGravities { get; private set; }

	public GameObject gun;

	public GameObject planetPrefab;

	[SerializeField] private GameObject activePlayer;

	public GameObject ActivePlayer
	{
		get { return activePlayer; }
	}

	// Start is called before the first frame update
	void Start()
	{
		for (int i = 0; i < 10; i++)
		{
			PlanetFactory.CreatePlanet(Random.Range(30, 50), new Vector2(Random.Range(-40.0f, 40.0f), Random.Range(-40.0f, 40.0f)), planetPrefab);
		}
		
		HaveGravities = new Dictionary<GameObject, IHasGravity>();

		foreach (var obj in GameObject.FindGameObjectsWithTag("HasGravity"))
		{
			var gravity = obj.GetComponent<IHasGravity>();
			if (gravity != null)
				HaveGravities.Add(obj, gravity);
		}
	}

	// Update is called once per frame
	void FixedUpdate()
	{
		if (Input.GetMouseButtonDown(0) && ActivePlayer != null)
		{
			var meeple = ActivePlayer.GetComponent<Meeple>();
			if (meeple.HasItem())
				meeple.HeldItem.GetComponent<Item>().Fire(ActivePlayer);
		}
	}

	public void CreateExplosion(Vector2 position, float power)
	{
		Collider2D[] hits = Physics2D.OverlapCircleAll(position, power * Planet.SCALE);

		List<Planet> planets = new List<Planet>();

		foreach (var hit in hits)
		{
			var planet = hit.gameObject.GetComponentInParent<Planet>();

			if (planet == null)
				continue;
			if (planets.Contains(planet))
				continue;

			planets.Add(planet);
		}

		foreach (var planet in planets)
		{
			var go = planet.gameObject;
			var pos = position - (Vector2)go.transform.position;
			planet.Explode((int)(pos.x / planet.scale), (int)(pos.y / planet.scale), (int)Math.Floor(power));
		}
	}
}