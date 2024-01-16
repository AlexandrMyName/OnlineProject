using System.Collections.Generic;
using Core.Models;
using UnityEngine;


namespace Core.Generation
{

    public class ChunckRenderer
    {
        private List<Vector3> _verticals;
        private List<int> _triangles;

        private int _waterHeight;

        private List<Vector3> _waterVerticals;
        private List<int> _waterTriangles;

        public Dictionary<Vector3Int,Vector3> NormalsStairs = new Dictionary<Vector3Int, Vector3>();

        private TextureRenderer _textureRender;
        private ChunckData _data;
        private bool _isGenerated;


        public ChunckRenderer(TextureDataConfig textureConfig, int waterHeight)
        {
            _textureRender = new TextureRenderer(textureConfig);
            _waterHeight = waterHeight;


        }


        public MeshData Generate(ChunckData chunckData)
        {

            CreateChunck(chunckData);
            MeshData meshData = new();
            meshData.ChunckPosition = chunckData.ChunckPosition;
            meshData.SetTriangleBufferData(_triangles);
            meshData.SetVertexBufferData(_verticals);
            meshData.SetUVSBufferData(_textureRender.GetUVs());
            meshData.SetWorldPosBufferData(_data.WorldPosition);

            meshData.WaterTriangles = _waterTriangles;
            meshData.WaterVerticals = _waterVerticals;
            meshData.WaterUvs = _textureRender.GetWaterUVs();
            _isGenerated = true;
            return meshData;
        }


        public MeshData SetBlock(Vector3 normal )
        {

            if (_data == null) return null;

            
            Generate(normal);
            MeshData meshData = new();
            meshData.SetTriangleBufferData(_triangles);
            meshData.SetVertexBufferData(_verticals);
            meshData.SetUVSBufferData(_textureRender.GetUVs());
            meshData.SetWorldPosBufferData(_data.WorldPosition);

            meshData.WaterTriangles = _waterTriangles;
            meshData.WaterVerticals = _waterVerticals;
            meshData.WaterUvs = _textureRender.GetWaterUVs();


            return meshData;

        }


        public void ClearNormalData(Vector3 blockPosition)
        {

            Vector3Int blockPos = Vector3Int.FloorToInt(blockPosition);

            if (NormalsStairs.ContainsKey(blockPos)){
                NormalsStairs.Remove(blockPos);
            }
        }


        private void CreateChunck(ChunckData data)
        {

            _data = data;

            Generate(Vector3.zero);


        }


        private void Generate(Vector3 normal)
        {

            Init();
            SetWaterBlocks();

            for (int y = 0; y < WorldGeneration.Height; y++)
            {
                for (int x = 0; x < WorldGeneration.Width; x++)
                {
                    for (int z = 0; z < WorldGeneration.Width; z++)
                    {
                        CreateBlock(x, y, z, normal);
                    }
                }
            }
            _data.Render = this;
        }


        private void Init()
        {

            _textureRender.Dispose();
            _waterTriangles ??= new List<int>();
            _waterVerticals ??= new List<Vector3>();
            _verticals ??= new List<Vector3>();
            _triangles ??= new List<int>();
            _verticals.Clear();
            _triangles.Clear();
            _waterVerticals.Clear();
            _waterTriangles.Clear();
             
            
        }


        private void CreateBlock(int x, int y, int z, Vector3 normal)
        {

            Vector3Int blockPos = new Vector3Int(x, y, z);
            BlockType type = GetBlockAtPosition(blockPos);

            if (GetBlockAtPosition(blockPos) == 0) return; //?
            
            if (type != BlockType.WoodenStaircase)
            {
                if (type != BlockType.Water && type != BlockType.Glass_White)
                {
                    if ((GetBlockAtPosition(blockPos + Vector3Int.right) == 0) ||
                        (GetBlockAtPosition(blockPos + Vector3Int.right) == BlockType.Water) ||
                        (GetBlockAtPosition(blockPos + Vector3Int.right) == BlockType.WoodenStaircase) ||
                        (GetBlockAtPosition(blockPos + Vector3Int.right) == BlockType.Glass_White)) CreateRightSide(blockPos, type);

                    if ((GetBlockAtPosition(blockPos + Vector3Int.left) == 0) ||
                        (GetBlockAtPosition(blockPos + Vector3Int.left) == BlockType.Water) ||
                        (GetBlockAtPosition(blockPos + Vector3Int.left) == BlockType.WoodenStaircase) ||
                        (GetBlockAtPosition(blockPos + Vector3Int.left) == BlockType.Glass_White)) CreateLeftSide(blockPos, type);

                    if ((GetBlockAtPosition(blockPos + Vector3Int.forward) == 0) ||
                        (GetBlockAtPosition(blockPos + Vector3Int.forward) == BlockType.Water) ||
                        (GetBlockAtPosition(blockPos + Vector3Int.forward) == BlockType.WoodenStaircase) ||
                        (GetBlockAtPosition(blockPos + Vector3Int.forward) == BlockType.Glass_White)) CreateFrontSide(blockPos, type);

                    if ((GetBlockAtPosition(blockPos + Vector3Int.back) == 0) ||
                        (GetBlockAtPosition(blockPos + Vector3Int.back) == BlockType.Water) ||
                        (GetBlockAtPosition(blockPos + Vector3Int.back) == BlockType.WoodenStaircase) ||
                        (GetBlockAtPosition(blockPos + Vector3Int.back) == BlockType.Glass_White)) CreateBackSide(blockPos, type);


                    if ((GetBlockAtPosition(blockPos + Vector3Int.up) == 0) ||
                        (GetBlockAtPosition(blockPos + Vector3Int.up) == BlockType.Water) ||
                        (GetBlockAtPosition(blockPos + Vector3Int.up) == BlockType.WoodenStaircase) ||
                        (GetBlockAtPosition(blockPos + Vector3Int.up) == BlockType.Glass_White)) CreateTopSide(blockPos, type);

                    if ((GetBlockAtPosition(blockPos + Vector3Int.down) == 0) ||
                        (GetBlockAtPosition(blockPos + Vector3Int.down) == BlockType.Water) ||
                        (GetBlockAtPosition(blockPos + Vector3Int.down) == BlockType.WoodenStaircase) ||
                        (GetBlockAtPosition(blockPos + Vector3Int.down) == BlockType.Glass_White)) CreateDownSide(blockPos, type);
                }
                else if  (type == BlockType.Water || type == BlockType.Glass_White)
                {
                    //Out
                    if (GetBlockAtPosition(blockPos + Vector3Int.right) == 0)  CreateRightSide(blockPos, type);
                    if (GetBlockAtPosition(blockPos + Vector3Int.left) == 0)  CreateLeftSide(blockPos, type);
                    if (GetBlockAtPosition(blockPos + Vector3Int.forward) == 0)  CreateFrontSide(blockPos, type);
                    if (GetBlockAtPosition(blockPos + Vector3Int.back) == 0) CreateBackSide(blockPos, type);
                    if (GetBlockAtPosition(blockPos + Vector3Int.up) == 0) CreateTopSide(blockPos, type);
                    if (GetBlockAtPosition(blockPos + Vector3Int.down) == 0) CreateDownSide(blockPos, type);


                    //In
                    if (GetBlockAtPosition(blockPos + Vector3Int.right) != BlockType.Water) CreateINRightSide(blockPos, type);
                    if (GetBlockAtPosition(blockPos + Vector3Int.left) != BlockType.Water) CreateINLeftSide(blockPos, type);
                    if (GetBlockAtPosition(blockPos + Vector3Int.forward) != BlockType.Water) CreateINFrontSide(blockPos, type);
                    if (GetBlockAtPosition(blockPos + Vector3Int.back) != BlockType.Water) CreateINBackSide(blockPos, type);
                    if (GetBlockAtPosition(blockPos + Vector3Int.up) != BlockType.Water) CreateINTopSide(blockPos, type);
                    if (GetBlockAtPosition(blockPos + Vector3Int.down) != BlockType.Water) CreateINDownSide(blockPos, type);
                }
            }
            else
            {
                if(type == BlockType.WoodenStaircase)
                    CreateStair(blockPos, type, normal);
            }

        }


        private void SetWaterBlocks() {

            if (!_isGenerated)
            {
                for (int y = 0; y < WorldGeneration.Height; y++)
                {
                    for (int x = 0; x < WorldGeneration.Width; x++)
                    {
                        for (int z = 0; z < WorldGeneration.Width; z++)
                        {
                            if (y < _waterHeight && GetBlockAtPosition(new Vector3Int(x,y,z)) == BlockType.Air)
                            {
                                _data.Blocks[x, y, z] = BlockType.Water;
                               
                            }
                        }
                    }
                }

            }
        }


        private void CreateStair(Vector3Int blockPos, BlockType type, Vector3 normal)
        {
            Vector3 Normal = Vector3.zero;

            if (NormalsStairs.TryGetValue(blockPos,out Vector3 enter))
            {
                Normal = enter; 
            }
            else
            {
                NormalsStairs.Add(blockPos, normal);
                Normal = normal;
            }


            if (Normal == Vector3.back || Normal == Vector3.up || Normal == Vector3.down)
            {
                //first cube withOut offSet
                CreateRightSide(blockPos, type, 0, -.5f);
                CreateLeftSide(blockPos, type, 0, -.5f);
                CreateFrontSide(blockPos, type, 0, -.5f);
                CreateBackSide(blockPos, type, 0, -.5f);
                CreateTopSide(blockPos, type, 0, -.5f);
                CreateDownSide(blockPos, type, 0, -.5f);

                //second cube - with offSet
                CreateRightSide(blockPos, type, 0, 0f, 0f, 0, .5f, .5f);
                CreateLeftSide(blockPos, type, 0, 0f, 0f, 0, .5f, .5f);
                CreateFrontSide(blockPos, type, 0, 0f, 0f, 0, .5f, .5f);
                CreateBackSide(blockPos, type, 0, 0f, 0f, 0, .5f, .5f);
                CreateTopSide(blockPos, type, 0, 0f, 0f, 0, .5f, .5f);
                CreateDownSide(blockPos, type, 0, 0f, 0f, 0, .5f, .5f);

            }

            else if (Normal == Vector3.forward)
            {
                //first cube withOut offSet
                CreateRightSide(blockPos, type, 0, -.5f);
                CreateLeftSide(blockPos, type, 0, -.5f);
                CreateFrontSide(blockPos, type, 0, -.5f);
                CreateBackSide(blockPos, type, 0, -.5f);
                CreateTopSide(blockPos, type, 0, -.5f);
                CreateDownSide(blockPos, type, 0, -.5f);

                //second cube - with offSet
                CreateRightSide(blockPos, type, 0, 0f, -.5f, 0, .5f, 0);
                CreateLeftSide(blockPos, type, 0, 0f, -.5f, 0, .5f, 0);
                CreateFrontSide(blockPos, type, 0, 0f, -.5f, 0, .5f, 0);
                CreateBackSide(blockPos, type, 0, 0f, -.5f, 0, .5f, 0);
                CreateTopSide(blockPos, type, 0, 0f, -.5f, 0, .5f, 0);
                CreateDownSide(blockPos, type, 0, 0f, -.5f, 0, .5f, 0);

            }

            else if (Normal == Vector3.right)
            {
                //first cube withOut offSet
                CreateRightSide(blockPos, type, 0, -.5f);
                CreateLeftSide(blockPos, type, 0, -.5f);
                CreateFrontSide(blockPos, type, 0, -.5f);
                CreateBackSide(blockPos, type, 0, -.5f);
                CreateTopSide(blockPos, type, 0, -.5f);
                CreateDownSide(blockPos, type, 0, -.5f);

                //second cube - with offSet
                CreateRightSide(blockPos, type, -.5f, 0f, 0f, 0, .5f, 0);
                CreateLeftSide(blockPos, type, -.5f, 0f, 0f, 0, .5f, 0);
                CreateFrontSide(blockPos, type, -.5f, 0f, 0f, 0, .5f, 0);
                CreateBackSide(blockPos, type, -.5f, 0f, 0f, 0, .5f, 0);
                CreateTopSide(blockPos, type, -.5f, 0f, 0f, 0, .5f, 0);
                CreateDownSide(blockPos, type, -.5f, 0f, 0f, 0, .5f, 0);

            }
            else if (Normal == Vector3.left)
            {
                //first cube withOut offSet
                CreateRightSide(blockPos, type, 0, -.5f);
                CreateLeftSide(blockPos, type, 0, -.5f);
                CreateFrontSide(blockPos, type, 0, -.5f);
                CreateBackSide(blockPos, type, 0, -.5f);
                CreateTopSide(blockPos, type, 0, -.5f);
                CreateDownSide(blockPos, type, 0, -.5f);

                //second cube - with offSet
                CreateRightSide(blockPos, type, 0, 0f, 0f, 0.5f, .5f, 0);
                CreateLeftSide(blockPos, type, 0, 0f, 0f, 0.5f, .5f, 0);
                CreateFrontSide(blockPos, type, 0, 0f, 0f, 0.5f, .5f, 0);
                CreateBackSide(blockPos, type, 0, 0f, 0f, 0.5f, .5f, 0);
                CreateTopSide(blockPos, type, 0, 0f, 0f, 0.5f, .5f, 0);
                CreateDownSide(blockPos, type, 0, 0f, 0f, 0.5f, .5f, 0);

            }
        }
        

        #region Sides OUT
        private void CreateRightSide(Vector3Int blockPos, BlockType type, float XScale = 0, float  YScale = 0, float ZScale = 0 , float XOffSet = 0, float YOffSet = 0, float ZOffSet = 0)
        {
            List<Vector3> verticals = type == BlockType.Water ? _waterVerticals : _verticals;
             

            _textureRender.AddTexture(type, SideData.Right);

            verticals.Add((new Vector3(1 + XScale, 0 + YOffSet, 0 + ZOffSet) + blockPos) * WorldGeneration.Scale);
            verticals.Add((new Vector3(1 + XScale, 1 + YScale, 0 + ZOffSet) + blockPos) * WorldGeneration.Scale);
            verticals.Add((new Vector3(1 + XScale, 0 + YOffSet, 1 + ZScale) + blockPos) * WorldGeneration.Scale);
            verticals.Add((new Vector3(1 + XScale, 1 + YScale, 1 + ZScale) + blockPos) * WorldGeneration.Scale);
            AddLastSquereVerticals(type);

        }


        private void CreateLeftSide(Vector3Int blockPos, BlockType type, float XScale = 0, float YScale = 0, float ZScale = 0, float XOffSet = 0, float YOffSet = 0, float ZOffSet = 0)
        {
            List<Vector3> verticals = type == BlockType.Water ? _waterVerticals : _verticals;
            

            _textureRender.AddTexture(type, SideData.Left);

            verticals.Add((new Vector3(0 + XOffSet, 0 + YOffSet, 0 + ZOffSet) + blockPos) * WorldGeneration.Scale);
            verticals.Add((new Vector3(0 + XOffSet, 0 + YOffSet, 1 + ZScale) + blockPos) * WorldGeneration.Scale);
            verticals.Add((new Vector3(0 + XOffSet, 1 + YScale, 0 + ZOffSet) + blockPos) * WorldGeneration.Scale);
            verticals.Add((new Vector3(0 + XOffSet, 1 + YScale, 1 + ZScale) + blockPos) * WorldGeneration.Scale);
            AddLastSquereVerticals(type);
        }


        private void CreateFrontSide(Vector3Int blockPos, BlockType type, float XScale= 0, float YScale = 0, float ZScale = 0, float XOffSet = 0, float YOffSet = 0, float ZOffSet = 0)
        {
            List<Vector3> verticals = type == BlockType.Water ? _waterVerticals : _verticals;
            
            _textureRender.AddTexture(type, SideData.Front);

            verticals.Add((new Vector3(0 + XOffSet, 0 + YOffSet, 1 + ZScale) + blockPos) * WorldGeneration.Scale);
            verticals.Add((new Vector3(1 + XScale, 0 + YOffSet, 1 + ZScale) + blockPos) * WorldGeneration.Scale);
            verticals.Add((new Vector3(0 + XOffSet, 1 + YScale, 1 + ZScale) + blockPos) * WorldGeneration.Scale);
            verticals.Add((new Vector3(1 + XScale, 1 + YScale, 1 + ZScale) + blockPos) * WorldGeneration.Scale);
            AddLastSquereVerticals(type);

        }


        private void CreateBackSide(Vector3Int blockPos, BlockType type, float XScale = 0, float YScale = 0, float ZScale = 0, float XOffSet = 0, float YOffSet = 0, float ZOffSet = 0)
        {
            List<Vector3> verticals = type == BlockType.Water ? _waterVerticals : _verticals;
            
            _textureRender.AddTexture(type, SideData.Back);

            verticals.Add((new Vector3(0 + XOffSet, 0 + YOffSet, 0 + ZOffSet) + blockPos) * WorldGeneration.Scale);
            verticals.Add((new Vector3(0 + XOffSet, 1 + YScale, 0 + ZOffSet) + blockPos) * WorldGeneration.Scale);
            verticals.Add((new Vector3(1 + XScale, 0 + YOffSet, 0 + ZOffSet) + blockPos) * WorldGeneration.Scale);
            verticals.Add((new Vector3(1 + XScale, 1 + YScale, 0 + ZOffSet) + blockPos) * WorldGeneration.Scale);
            AddLastSquereVerticals(type);
        }


        private void CreateTopSide(Vector3Int blockPos, BlockType type, float XScale = 0, float YScale = 0, float ZScale = 0, float XOffSet = 0, float YOffSet = 0, float ZOffSet = 0)
        {
            List<Vector3> verticals = type == BlockType.Water ? _waterVerticals : _verticals;
            
            _textureRender.AddTexture(type, SideData.Top);

            verticals.Add((new Vector3(0 + XOffSet, 1 + YScale, 0 + ZOffSet) + blockPos) * WorldGeneration.Scale);
            verticals.Add((new Vector3(0 + XOffSet, 1 + YScale, 1 + ZScale) + blockPos) * WorldGeneration.Scale);
            verticals.Add((new Vector3(1 + XScale, 1 + YScale, 0 + ZOffSet) + blockPos) * WorldGeneration.Scale);
            verticals.Add((new Vector3(1 + XScale, 1 + YScale, 1 + ZScale) + blockPos) * WorldGeneration.Scale);
            AddLastSquereVerticals(type);
        }


        private void CreateDownSide(Vector3Int blockPos, BlockType type , float XScale = 0, float YScale = 0, float ZScale = 0, float XOffSet = 0, float YOffSet = 0, float ZOffSet = 0)
        {
            List<Vector3> verticals = type == BlockType.Water ? _waterVerticals : _verticals;
            

            _textureRender.AddTexture(type, SideData.Down);

            verticals.Add((new Vector3(0 + XOffSet, 0 + YOffSet, 0 + ZOffSet) + blockPos) * WorldGeneration.Scale);
            verticals.Add((new Vector3(1 + XScale, 0 + YOffSet, 0 + ZOffSet) + blockPos) * WorldGeneration.Scale);
            verticals.Add((new Vector3(0 + XOffSet, 0 + YOffSet, 1 + ZScale) + blockPos) * WorldGeneration.Scale);
            verticals.Add((new Vector3(1 + XScale, 0 + YOffSet, 1 + ZScale) + blockPos) * WorldGeneration.Scale);
            AddLastSquereVerticals(type);
        }
        #endregion


        #region Sides IN


        private void CreateINRightSide(Vector3Int blockPos, BlockType type, float XScale = 0, float YScale = 0, float ZScale = 0, float XOffSet = 0, float YOffSet = 0, float ZOffSet = 0)
        {
            List<Vector3> verticals = type == BlockType.Water ? _waterVerticals : _verticals;


            _textureRender.AddTexture(type, SideData.Right);

            verticals.Add((new Vector3(1 + XScale, 0 + YOffSet, 0 + ZOffSet) + blockPos) * WorldGeneration.Scale);
             
            verticals.Add((new Vector3(1 + XScale, 0 + YOffSet, 1 + ZScale) + blockPos) * WorldGeneration.Scale);
            verticals.Add((new Vector3(1 + XScale, 1 + YScale, 0 + ZOffSet) + blockPos) * WorldGeneration.Scale);
            verticals.Add((new Vector3(1 + XScale, 1 + YScale, 1 + ZScale) + blockPos) * WorldGeneration.Scale);
            AddLastSquereVerticals(type);

        }


        private void CreateINLeftSide(Vector3Int blockPos, BlockType type, float XScale = 0, float YScale = 0, float ZScale = 0, float XOffSet = 0, float YOffSet = 0, float ZOffSet = 0)
        {
            List<Vector3> verticals = type == BlockType.Water ? _waterVerticals : _verticals;


            _textureRender.AddTexture(type, SideData.Left);

            verticals.Add((new Vector3(0 + XOffSet, 0 + YOffSet, 0 + ZOffSet) + blockPos) * WorldGeneration.Scale);
            
            verticals.Add((new Vector3(0 + XOffSet, 1 + YScale, 0 + ZOffSet) + blockPos) * WorldGeneration.Scale);
            verticals.Add((new Vector3(0 + XOffSet, 0 + YOffSet, 1 + ZScale) + blockPos) * WorldGeneration.Scale);
            verticals.Add((new Vector3(0 + XOffSet, 1 + YScale, 1 + ZScale) + blockPos) * WorldGeneration.Scale);
            AddLastSquereVerticals(type);
        }


        private void CreateINFrontSide(Vector3Int blockPos, BlockType type, float XScale = 0, float YScale = 0, float ZScale = 0, float XOffSet = 0, float YOffSet = 0, float ZOffSet = 0)
        {
            List<Vector3> verticals = type == BlockType.Water ? _waterVerticals : _verticals;

            _textureRender.AddTexture(type, SideData.Front);

            verticals.Add((new Vector3(0 + XOffSet, 0 + YOffSet, 1 + ZScale) + blockPos) * WorldGeneration.Scale);
            
            verticals.Add((new Vector3(0 + XOffSet, 1 + YScale, 1 + ZScale) + blockPos) * WorldGeneration.Scale);
            verticals.Add((new Vector3(1 + XScale, 0 + YOffSet, 1 + ZScale) + blockPos) * WorldGeneration.Scale);
            verticals.Add((new Vector3(1 + XScale, 1 + YScale, 1 + ZScale) + blockPos) * WorldGeneration.Scale);
            AddLastSquereVerticals(type);

        }


        private void CreateINBackSide(Vector3Int blockPos, BlockType type, float XScale = 0, float YScale = 0, float ZScale = 0, float XOffSet = 0, float YOffSet = 0, float ZOffSet = 0)
        {
            List<Vector3> verticals = type == BlockType.Water ? _waterVerticals : _verticals;

            _textureRender.AddTexture(type, SideData.Back);

            verticals.Add((new Vector3(0 + XOffSet, 0 + YOffSet, 0 + ZOffSet) + blockPos) * WorldGeneration.Scale);
            
            verticals.Add((new Vector3(1 + XScale, 0 + YOffSet, 0 + ZOffSet) + blockPos) * WorldGeneration.Scale);
            verticals.Add((new Vector3(0 + XOffSet, 1 + YScale, 0 + ZOffSet) + blockPos) * WorldGeneration.Scale);
            verticals.Add((new Vector3(1 + XScale, 1 + YScale, 0 + ZOffSet) + blockPos) * WorldGeneration.Scale);
            AddLastSquereVerticals(type);
        }


        private void CreateINTopSide(Vector3Int blockPos, BlockType type, float XScale = 0, float YScale = 0, float ZScale = 0, float XOffSet = 0, float YOffSet = 0, float ZOffSet = 0)
        {
            List<Vector3> verticals = type == BlockType.Water ? _waterVerticals : _verticals;

            _textureRender.AddTexture(type, SideData.Top);

            verticals.Add((new Vector3(0 + XOffSet, 1 + YScale, 0 + ZOffSet) + blockPos) * WorldGeneration.Scale);
             
            verticals.Add((new Vector3(1 + XScale, 1 + YScale, 0 + ZOffSet) + blockPos) * WorldGeneration.Scale);
            verticals.Add((new Vector3(0 + XOffSet, 1 + YScale, 1 + ZScale) + blockPos) * WorldGeneration.Scale);
            verticals.Add((new Vector3(1 + XScale, 1 + YScale, 1 + ZScale) + blockPos) * WorldGeneration.Scale);
            AddLastSquereVerticals(type);
        }


        private void CreateINDownSide(Vector3Int blockPos, BlockType type, float XScale = 0, float YScale = 0, float ZScale = 0, float XOffSet = 0, float YOffSet = 0, float ZOffSet = 0)
        {
            List<Vector3> verticals = type == BlockType.Water ? _waterVerticals : _verticals;


            _textureRender.AddTexture(type, SideData.Down);

            verticals.Add((new Vector3(0 + XOffSet, 0 + YOffSet, 0 + ZOffSet) + blockPos) * WorldGeneration.Scale);
            
            verticals.Add((new Vector3(0 + XOffSet, 0 + YOffSet, 1 + ZScale) + blockPos) * WorldGeneration.Scale);
            verticals.Add((new Vector3(1 + XScale, 0 + YOffSet, 0 + ZOffSet) + blockPos) * WorldGeneration.Scale);
            verticals.Add((new Vector3(1 + XScale, 0 + YOffSet, 1 + ZScale) + blockPos) * WorldGeneration.Scale);
            AddLastSquereVerticals(type);
        }
        #endregion


        private BlockType GetBlockAtPosition(Vector3Int blockPos)
        {
            if (blockPos.x >= 0 && blockPos.y >= 0 && blockPos.z >= 0
                && blockPos.x < WorldGeneration.Width && blockPos.y < WorldGeneration.Height && blockPos.z < WorldGeneration.Width
                 )
            {

                return _data.Blocks[blockPos.x, blockPos.y, blockPos.z];
            }
            else
            {
                return BlockType.Air;
            }
        }


        private void AddLastSquereVerticals(BlockType type)
        {

            List<int> triangles = type == BlockType.Water ? _waterTriangles : _triangles;
            List<Vector3> verticals = type == BlockType.Water ? _waterVerticals : _verticals;


            triangles.Add(verticals.Count - 4);
            triangles.Add(verticals.Count - 3);
            triangles.Add(verticals.Count - 2);

            triangles.Add(verticals.Count - 3);
            triangles.Add(verticals.Count - 1);
            triangles.Add(verticals.Count - 2);
        }

    }
}