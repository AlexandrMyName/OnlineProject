using Core.Models;
using System;
using System.Collections.Generic;
using UnityEngine;


namespace Cryptograph
{
    [Serializable]
    public class XmlMesh 
    {

        public List<Vector3> Verticals { get; set; }
        public List<int> Triangles { get; set; }
        public List<Vector2> Uvs { get; set; }
        public Vector3 WorldPositionStay { get; set; }
        public Vector2Int ChunckPosition { get; set; }

        public List<XmlNormalStairs> XmlNormalStairs { get; set; }
 
        public List<XmlBlocks> Blocks { get; set; }  
    }

    [Serializable]
    public class XmlNormalStairs
    {
        public Vector3Int Position { get; set; }
        public Vector3 Normal { get; set; }

    }

    [Serializable]
    public class XmlBlocks
    {

        public int X_blockKey { get; set; }
        public int Y_blockKey { get; set; }
        public int Z_blockKey { get; set; }
        public int Value { get; set; }
    }
}
