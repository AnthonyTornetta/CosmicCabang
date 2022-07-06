using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class Planet : MonoBehaviour, IHasGravity
{
	// Start is called before the first frame update

	public int blocksRadius;
	public Material voxelMaterial;

	public const float SCALE = 0.125f;

	public float scale = SCALE;

	private HasChunks _chunks;
	
	public float Mass { get; private set; }

	public float Radius => blocksRadius * scale;
	
	// private QuadTree _quadTree;

	private void Start()
	{
		_chunks = this.AddComponent<HasChunks>();

		int chunksAmt = (int)Math.Ceiling(blocksRadius * 2 / (double)Chunk.WIDTH);

		_chunks.Init(chunksAmt, chunksAmt, scale);

		GenerateMesh();
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.green;
		Gizmos.DrawWireSphere(transform.position, blocksRadius * scale);
	}

	public bool WithinBlocks(int x, int y)
	{
		return x >= -blocksRadius && x < blocksRadius
		                          && y >= -blocksRadius && y < blocksRadius;
	}

	public void Explode(int x, int y, int radius)
	{
		for (int dy = -radius; dy <= radius; dy++)
		{
			for (int dx = -radius; dx <= radius; dx++)
			{
				if (dx * dx + dy * dy > radius * radius)
					continue;

				int xx = x + dx;
				int yy = y + dy;

				if (WithinBlocks(xx, yy))
				{
					SetBlock(xx, yy, false);
				}
			}
		}
	}

	public void SetBlock(int x, int y, bool b)
	{
		bool prev = _chunks.HasBlock(x, y);

		if (prev == b)
			return;

		if (!b)
			Mass -= 20;
		else
			Mass += 20;

		_chunks.SetBlock(x, y, b);
		// _quadTree.SetBlock(x, y, b);
	}

	public bool HasBlock(int x, int y)
	{
		return _chunks.HasBlock(x, y);
	}

	private void GenerateMesh()
	{
		for (int y = -blocksRadius; y < blocksRadius; y++)
		{
			for (int x = -blocksRadius; x < blocksRadius; x++)
			{
				if (x * x + y * y < blocksRadius * blocksRadius)
					//SetBlock(x, y, UnityEngine.Random.Range(0, 10) < 8);
					SetBlock(x, y, true);
			}
		}

		foreach (var chunk in _chunks)
		{
			chunk.GenerateMesh(transform, voxelMaterial);
		}
	}

	// Update is called once per frame
	private void Update()
	{
		using var enumer = _chunks.DirtyChunksEnumerator();

		while (enumer.MoveNext())
		{
			if (enumer.Current == null)
				throw new Exception("Null not allowed");

			enumer.Current.ReRender();
		}

		_chunks.ClearDirtyChunks();
	}
}