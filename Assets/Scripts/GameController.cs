using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class GameController : MonoBehaviour
{
	public Dictionary<GameObject, Planet> HaveGravities { get; private set; }

	public GameObject gun;

	[SerializeField] private GameObject activePlayer;

	public GameObject ActivePlayer
	{
		get { return activePlayer; }
	}

	// Start is called before the first frame update
	void Start()
	{
		HaveGravities = new Dictionary<GameObject, Planet>();

		foreach (var obj in GameObject.FindGameObjectsWithTag("HasGravity"))
		{
			var planet = obj.GetComponent<Planet>();
			if (planet != null)
				HaveGravities.Add(obj, planet);
		}
	}

	// Update is called once per frame
	void FixedUpdate()
	{
		if (Input.GetMouseButtonDown(0) && ActivePlayer != null)
		{
			Meeple meeple = ActivePlayer.GetComponent<Meeple>();
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