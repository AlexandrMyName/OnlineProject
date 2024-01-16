using Core.Generation;
using System;
using UnityEngine;

namespace Core.Models
{
   
    public class ChunckData
    {
        public BlockType[,,] Blocks;
        public Vector3 WorldPosition;
        public Vector2Int ChunckPosition;
        public ChunckRenderer Render { get; set; }
        public ChunckData(BlockType[,,] blocks, Vector3 worldPosition)
        {
            Blocks = blocks;
            WorldPosition = worldPosition;
        }
        public ChunckData() { } 
    }
}