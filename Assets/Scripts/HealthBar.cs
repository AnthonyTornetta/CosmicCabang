using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class HealthBar : MonoBehaviour
{
	private float _health;

	public float Health
	{
		get { return _health; }
		set
		{
			_health = value;

			var trans = healthBarForeground.transform;

			{
				float ratio = _health / maxHealth;

				var scale = trans.localScale;
				trans.localScale = new Vector3(initialScale * ratio, scale.y, scale.z);
			}

			{
				float diff = trans.localScale.x - initialScale;

				var localPos = trans.localPosition;

				trans.localPosition = new Vector3(diff / 2, localPos.y, localPos.z);
			}

			{
				float ratio = _health / maxHealth;
				float g = ratio > 0.5f ? 1.0f : 2 * ratio;
				float r = ratio < 0.5f ? 1.0f : 2 * (1 - ratio);
				healthBarForeground.GetComponent<SpriteRenderer>().color =
					new Color(r, g, 0);
			}

			ShowHealthBar();
		}
	}

	public float maxHealth;

	private float initialScale;
	private float lastShowTime = 0;

	private void Start()
	{
		initialScale = healthBarForeground.transform.localScale.x;

		HideHealthBar();

		_health = maxHealth;
	}

	public GameObject healthBarForeground;
	public GameObject healthBar;

	private void HideHealthBar()
	{
		healthBar.SetActive(false);
	}

	private void ShowHealthBar()
	{
		healthBar.SetActive(true);

		lastShowTime = Time.time;
	}

	private void FixedUpdate()
	{
		if (healthBar.activeSelf)
		{
			if (Time.time - 5 > lastShowTime)
			{
				HideHealthBar();
			}
		}

		lastShowTime = Time.time;
	}
}