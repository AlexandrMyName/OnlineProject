using Core.Models;
using System;
using System.Collections.Generic;
using UnityEngine;
using static TextureDataConfig;


public class TextureRenderer  : IDisposable
{
   
    float width;
    float height;

    float widthWater;
    float heightWater;


    private TextureDataConfig _config;
    private List<Vector2> _uvs;
    private List<Vector2> _waterUVs;
    public TextureRenderer(TextureDataConfig config)
    {
        width = config.TableWidth;
        height = config.TableHeight;
 

        widthWater = config.TableWaterWidth;
        heightWater = config.TableWaterHeight;
        _config = config;
        _uvs = new List<Vector2>();
        _waterUVs = new List<Vector2>();
    }
    public List<Vector2> GetUVs() => _uvs;

    public void SetUVs(List<Vector2> uvs) => _uvs = uvs;
    public List<Vector2> GetWaterUVs() => _waterUVs;

    public void AddTexture(BlockType block,  SideData sideType)
    {

        List<Vector2> uvs = block == BlockType.Water ? _waterUVs : _uvs;

        Vector2 textureUV =  GetTexture(block, sideType);

        float x0 = textureUV.x - 1 > 0 ? (textureUV.x - 1) / (block == BlockType.Water ? widthWater : width) : 0.0f;
        float x1 = textureUV.x > 0 ? textureUV.x / (block == BlockType.Water ? widthWater : width) : 1f;

        float y0 = textureUV.y - 1 > 0 ? (textureUV.y - 1) / (block == BlockType.Water ? heightWater : height) : 0.0f;
        float y1 = textureUV.y > 0 ? textureUV.y / (block == BlockType.Water ? heightWater : height) : 1f;

        if (sideType == SideData.Left || sideType == SideData.Front || sideType == SideData.Top || sideType == SideData.Down)
        {
            uvs.Add(new Vector2(x0, y0));
            uvs.Add(new Vector2(x1, y0));
            uvs.Add(new Vector2(x0, y1));
            uvs.Add(new Vector2(x1, y1));
        }
        else
        {
            uvs.Add(new Vector2(x0, y0));
            uvs.Add(new Vector2(x0, y1));
            uvs.Add(new Vector2(x1, y0));
            uvs.Add(new Vector2(x1, y1));
        }
 
    }
   
    private Vector2 GetTexture(BlockType block,  SideData side = SideData.Right)
    {
        Vector2 textureUV = Vector2.zero;
        TextureConfig config = _config.Configs.Find(conf => conf.BlockType == block);


        if(block != BlockType.Air)
            switch (side)
            {
                case SideData.Right:

                    textureUV = config.RightSide;
                    break;

                case SideData.Left:

                    textureUV = config.LeftSide;
                    break;

                case SideData.Top:

                    textureUV = config.TopSide;
                    break;

                case SideData.Down:
                    textureUV = config.BottomSide;

                    break;

                case SideData.Front:

                    textureUV = config.FrontSide;
                    break;

                case SideData.Back:

                    textureUV = config.BackSide;
                    break;
            }

      
        return textureUV;
    }

    public void Dispose()
    {
        _uvs ??= new();
        _uvs.Clear();
    }
}
