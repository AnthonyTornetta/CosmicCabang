using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HasChunks : MonoBehaviour
{
    private int _chunksX, _chunksY;
    private float _scale;
    private Chunk[,] _chunks;

    private readonly List<Chunk> _dirtyChunks = new List<Chunk>();

    public void Init(int chunksX, int chunksY, float scale)
    {
        this._chunksX = chunksX;
        this._chunksY = chunksY;
        this._scale = scale;

        _chunks = new Chunk[chunksY, chunksX];
        
        for (int y = 0; y < chunksY; y++)
        {
            for (int x = 0; x < chunksX; x++)
            {
                float chunksXOffset = -Chunk.WIDTH * chunksX / 2.0f;
                float chunksYOffset = -Chunk.HEIGHT * chunksY / 2.0f;

                _chunks[y, x] = new Chunk(scale,
                    new Vector3(chunksXOffset + x * Chunk.WIDTH, chunksYOffset + y * Chunk.HEIGHT, 0));
            }
        }
    }
    
    public void SetBlock(int x, int y, bool b)
    {
        int xx = x + (Chunk.WIDTH * _chunksX / 2);
        int yy = y + (Chunk.HEIGHT * _chunksY / 2);

        var chunk = _chunks[yy / Chunk.HEIGHT, xx / Chunk.WIDTH];
        
        if(!_dirtyChunks.Contains(chunk))
            _dirtyChunks.Add(chunk);

        chunk.SetBlock(xx % Chunk.WIDTH, yy % Chunk.HEIGHT, b);
    }

    public bool HasBlock(int x, int y)
    {
        int xx = x + (Chunk.WIDTH * _chunksX / 2);
        int yy = y + (Chunk.HEIGHT * _chunksY / 2);

        return _chunks[yy / Chunk.HEIGHT, xx / Chunk.WIDTH].HasBlock(xx % Chunk.WIDTH, yy % Chunk.HEIGHT);
    }
    
    public IEnumerator<Chunk> DirtyChunksEnumerator()
    {
        return _dirtyChunks.GetEnumerator();
    }
    
    public IEnumerator<Chunk> GetEnumerator()
    {
        return _chunks.Cast<Chunk>().GetEnumerator();
    }

    public void ClearDirtyChunks()
    {
        _dirtyChunks.Clear();
    }
}
