using Core.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[CreateAssetMenu(fileName = nameof(TextureDataConfig), menuName = "Configs/" + nameof(TextureDataConfig))]
public class TextureDataConfig : ScriptableObject
{
    public List<TextureConfig> Configs;

    public float TableWidth;
    public float TableHeight;

    public int TableWaterWidth;
    public int TableWaterHeight;

    [Serializable]
    public class TextureConfig
    {
        public BlockType BlockType;

        public Vector2 LeftSide;
        public Vector2 RightSide;

        public Vector2 FrontSide;
        public Vector2 BackSide;

        public Vector2 TopSide;
        public Vector2 BottomSide;



    }

    
}