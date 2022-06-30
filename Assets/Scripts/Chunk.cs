using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.U2D.Path;
using UnityEngine;

//  ae

public class Chunk
{
    public const int WIDTH = 16, HEIGHT = 16;

    private readonly bool[,] _blocks = new bool[HEIGHT,WIDTH];

    private readonly float _scale;

    private readonly Vector3 _position;

    private GameObject _gameObject;
    
    public Chunk(float scale, Vector3 position)
    {
        this._scale = scale;
        this._position = position;
    }

    public void SetBlock(int x, int y, bool b)
    {
        _blocks[y, x] = b;
    }

    public bool HasBlock(int x, int y)
    {
        return _blocks[y, x];
    }

    public void ReRender()
    {
        GenerateMesh(null, null);
    }
    
    public void GenerateMesh(Transform parent, Material voxelMaterial)
    {
        bool wasNull = !_gameObject;
        if (wasNull)
        {
            _gameObject = new GameObject
            {
                name = "Voxel Chunk",
                transform =
                {
                    parent = parent,
                    localPosition = Vector3.zero
                }
            };
        }

        var mesh = new Mesh();
        
        var vertices = new List<Vector3>();
        var indices = new List<int>();
        var uvs = new List<Vector2>();
        var normals = new List<Vector3>();

        float xSize = 1 * _scale;
        float ySize = 1 * _scale;

        float hxSize = xSize * .5f;
        float hySize = ySize * .5f;

        int maxIndex = 0;

        for (int y = 0; y < HEIGHT; y++)
        {
            for (int x = 0; x < WIDTH; x++)
            {
                if (!HasBlock(x, y))
                    continue;
                
                var upperLeft = (new Vector3(x - hxSize, y - hySize, 0) + _position) * _scale;
                var index = maxIndex;
                
                uvs.Add(Vector2.zero);
                uvs.Add(Vector3.right);
                uvs.Add(Vector3.down);
                uvs.Add(Vector3.down + Vector3.right);
                
                vertices.Add(upperLeft);
                vertices.Add(upperLeft + Vector3.right * xSize);
                vertices.Add(upperLeft + Vector3.down * ySize);
                vertices.Add(upperLeft + Vector3.down * ySize + Vector3.right * xSize);
                
                normals.Add(Vector3.back);
                normals.Add(Vector3.back);
                normals.Add(Vector3.back);
                normals.Add(Vector3.back);
                
                indices.Add(index);
                indices.Add(index + 1);
                indices.Add(index + 2);
                
                indices.Add(index + 3);
                indices.Add(index + 2);
                indices.Add(index + 1);

                maxIndex += 4;
            }
        }

        mesh.SetVertices(vertices);
        mesh.SetTriangles(indices, 0);
        mesh.SetUVs(0, uvs);
        mesh.SetNormals(normals);

        if (wasNull)
        {
            var meshFilter = _gameObject.AddComponent<MeshFilter>();
            var meshRenderer = _gameObject.AddComponent<MeshRenderer>();
            var body = _gameObject.AddComponent<Rigidbody2D>();
            body.bodyType = RigidbodyType2D.Static;
            
            var compositeCollider2D = _gameObject.AddComponent<CompositeCollider2D>();
            compositeCollider2D.generationType = CompositeCollider2D.GenerationType.Manual;
            compositeCollider2D.vertexDistance = 0.005f;

            meshRenderer.material = voxelMaterial;

            meshFilter.mesh = mesh;

            GenerateColliders(compositeCollider2D);
        }
        else
        {
            var filter = _gameObject.GetComponent<MeshFilter>();
            
            Object.Destroy(filter.mesh);
            filter.mesh = mesh;

            var comps = _gameObject.GetComponents<BoxCollider2D>();

            foreach(var comp in comps)
                Object.Destroy(comp);
            
            GenerateColliders(_gameObject.GetComponent<CompositeCollider2D>());
        }
    }

    bool IsFull()
    {
        for (int y = 0; y < HEIGHT; y++)
        {
            for (int x = 0; x < WIDTH; x++)
            {
                if (!HasBlock(x, y))
                    return false;
            }
        }

        return true;
    }

    void GenerateColliders(CompositeCollider2D compositeCollider2D)
    {
        // A quick optimization so full chunks are faster
        if (IsFull())
        {
            var collider = _gameObject.AddComponent<BoxCollider2D>();
            collider.offset = new Vector2( WIDTH / 2.0f * _scale + _position.x * _scale, -_scale + HEIGHT / 2.0f * _scale + _position.y * _scale);
            collider.size = new Vector2(_scale * WIDTH, _scale * HEIGHT);
            collider.usedByComposite = true;
        }
        else
        {
            for (int y = 0; y < HEIGHT; y++)
            {
                for (int x = 0; x < WIDTH; x++)
                {
                    if(!HasBlock(x, y))
                        continue;
                    
                    var collider = _gameObject.AddComponent<BoxCollider2D>();
                    collider.offset = new Vector2(_scale / 2 + x * _scale + _position.x * _scale,
                        -_scale / 2 + y * _scale + _position.y * _scale);
                    collider.size = new Vector2(_scale, _scale);
                    collider.usedByComposite = true;
                }
            }
        }

        compositeCollider2D.GenerateGeometry();
    }
}
