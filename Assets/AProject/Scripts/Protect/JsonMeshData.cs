using System;
using UnityEngine;


namespace Cryptograph
{

    [Serializable]
    public class JsonMeshData
    {

        public BlockSerializable[] Blocks;

        public NormalStairBlockSerializable[] NormalStairs;
    }

    
    [Serializable]
    public struct BlockSerializable
    {

        public Vector3IntSerializable Position;

        public int ValueBlock;


        public static implicit operator XmlBlocks(BlockSerializable value)
        {

            XmlBlocks xmlBlocks = new XmlBlocks();

            xmlBlocks.X_blockKey = value.Position.X_pos;
            xmlBlocks.Y_blockKey = value.Position.Y_pos;
            xmlBlocks.Z_blockKey = value.Position.Z_pos;
            xmlBlocks.Value = value.ValueBlock;

            return xmlBlocks;
        }

        public static implicit operator BlockSerializable(XmlBlocks value)
        {

            var blockSerializable = new BlockSerializable();
            blockSerializable.Position = new Vector3Int(value.X_blockKey, value.Y_blockKey, value.Z_blockKey);
            return blockSerializable;
        }
    }


    [Serializable]
    public struct NormalStairBlockSerializable
    {

        public Vector3IntSerializable Position;

        public Vector3Serializable Normal;


        public static implicit operator XmlNormalStairs(NormalStairBlockSerializable value)
        {

            XmlNormalStairs xmlNormal = new XmlNormalStairs();
            xmlNormal.Position = value.Position;
            xmlNormal.Normal = value.Normal;

            return xmlNormal;
        }

        public static implicit operator NormalStairBlockSerializable(XmlNormalStairs value)
        {
            var normalSerializable = new NormalStairBlockSerializable();
            normalSerializable.Position = value.Position;
            normalSerializable.Normal = value.Normal;
            return normalSerializable;
        }


    }

    [Serializable]
    public struct Vector3Serializable
    {

        public float X_pos;
        public float Y_pos;
        public float Z_pos;


        private Vector3Serializable(float xValue, float yValue, float zValue)
        {
            X_pos = xValue;
            Y_pos = yValue;
            Z_pos = zValue;
        }
        public static implicit operator Vector3(Vector3Serializable value)
        {
            return new Vector3(value.X_pos, value.Y_pos, value.Z_pos);
        }

        public static implicit operator Vector3Serializable(Vector3 value)
        {
            return new Vector3Serializable(value.x,value.y,value.z);
        }
    }

    [Serializable]
    public struct Vector3IntSerializable
    {

        public int X_pos;
        public int Y_pos;
        public int Z_pos;


        private Vector3IntSerializable(int xValue, int yValue, int zValue)
        {
            X_pos = xValue;
            Y_pos = yValue;
            Z_pos = zValue;
        }
        public static implicit operator Vector3Int(Vector3IntSerializable value)
        {
            return new Vector3Int(value.X_pos, value.Y_pos, value.Z_pos);
        }

        public static implicit operator Vector3IntSerializable(Vector3Int value)
        {
            return new Vector3IntSerializable(value.x, value.y, value.z);
        }
    }
}