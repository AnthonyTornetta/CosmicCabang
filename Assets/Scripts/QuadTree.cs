using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class QuadTree
{
	private QuadTree[,] _children;
	private GameObject _gameObject;

	private bool[,] _blocks;

	private readonly int _size;

	private readonly Vector2 _location;
	private readonly Transform _parent;

	public QuadTree(int size, Vector2 location, Transform parent)
	{
		_size = size;
		_blocks = new bool[size, size];
		_location = location;
		_parent = parent;
	}

	public void Init()
	{
		if (IsFull())
		{
			_gameObject = new GameObject
			{
				name = "Voxel Collider",
				transform =
				{
					parent = _parent,
					localPosition = _location
				}
			};

			BoxCollider2D boxCollider2D = _gameObject.AddComponent<BoxCollider2D>();
			boxCollider2D.size = new Vector2(_size, _size);
		}
		else
		{
			Divide();

			foreach (var child in _children)
			{
				child.Init();
			}
		}
	}

	public bool IsFull()
	{
		foreach (var block in _blocks)
		{
			if (!block)
				return false;
		}

		return true;
	}

	public bool Check()
	{
		if (_children == null)
		{
			if (IsFull())
			{
				return true;
			}

			Divide();
			return false;
		}
		else
		{
			foreach (var child in _children)
			{
				if (!child.Check())
					return false;
			}

			Join();

			return true;
		}
	}

	void Divide()
	{
		if (_size == 1)
			return;

		_children = new QuadTree[2, 2];

		int size2 = _size / 2;

		for (int y = 0; y < 2; y++)
		for (int x = 0; x < 2; x++)
			_children[y, x] = new QuadTree(size2, _location + new Vector2(x * size2, y * size2), _parent);

		for (int y = 0; y < _size; y++)
		{
			for (int x = 0; x < _size; x++)
			{
				int xx = x % (size2), yy = y % (size2);

				_children[y / (size2), x / (size2)]._blocks[yy, xx] = _blocks[y, x];
			}
		}

		_blocks = null;

		foreach (var child in _children)
		{
			child.Check();
		}
	}

	void Join()
	{
		_blocks = new bool[_size, _size];

		for (int y = 0; y < _size; y++)
		{
			for (int x = 0; x < _size; x++)
			{
				int xx = x % (_size / 2), yy = y % (_size / 2);

				_blocks[y, x] = _children[y / (_size / 2), x / (_size / 2)]._blocks[yy, xx];
			}
		}

		_children = null;
	}

	public void SetBlock(int x, int y, bool b)
	{
		if (_children == null)
		{
			_blocks[y, x] = b;
		}
		else
		{
			int cx = 0, cy = 0;

			if (x > _size / 2)
			{
				cx = 1;
			}

			if (y > _size / 2)
			{
				cy = 1;
			}

			_children[cy, cx].SetBlock(x % (_size / 2), y % (_size / 2), b);
		}
	}
}