using Core.Models;
using Photon.Pun.Demo.Procedural;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Unity.Burst.CompilerServices;
using UnityEngine;


namespace Core.Generation
{

    public class Generator
    {

        //Afonin A.I
        [SerializeField] private Material _chuncksMaterial;
        [SerializeField] private Material _waterMaterial;
        private Transform _playerTransform;
        private Transform _worldParrent;
        private int _radiusGenerate = 5;
        private Vector2Int _currentPlayerChunck;

        private ConcurrentQueue<MeshData> _meshDataQueue = new();

        private Camera _camera;
        private WorldChunckObjects _worldObjects;
        private MonoBehaviour _initializer;
        private TextureDataConfig _textureConfig;
        private Vector3Int BoundsChunck;
        private GameObject _loadObj;


        public Generator(Transform playerTransform, Material blocksMaterial,Material waterMaterial, Transform worldParrent, MonoBehaviour initializer, TextureDataConfig textureConfig)
        {

            _chuncksMaterial = blocksMaterial;
            _waterMaterial = waterMaterial;
            _playerTransform = playerTransform;
            _worldParrent = worldParrent;
            _initializer = initializer;
            _camera = Camera.main;
            _textureConfig = textureConfig;
        }


        public void WorldSetUp(WorldChunckObjects objects, bool isGenerateOne = true, int radiusGeneration = 5, GameObject loadObj = null)
        {
          
            _worldObjects = objects;
            _radiusGenerate = radiusGeneration;
           
            _initializer.StartCoroutine(SetPlayerPosition());
            _loadObj = loadObj;
            if (isGenerateOne) Generate();
         
        
        }
         

        public void RunProccedureGenerator()
        {

            Vector3Int worldPos = Vector3Int.FloorToInt(_playerTransform.position / WorldGeneration.Scale);
            Vector2Int playerChunck = _worldObjects.GetChunckContaineBlock(worldPos);

            if (playerChunck != _currentPlayerChunck)
            {
                _currentPlayerChunck = playerChunck;
                Generate();
            }
            if (_meshDataQueue.Count > 0)
            {
                Debug.LogWarning("C");
            }
            TryDequeueMeshData();
        }


        public void RunEditGeneration()
        {
            TryDequeueMeshData();
        }


        private void TryDequeueMeshData()
        {
            if (_meshDataQueue.TryDequeue(out MeshData meshData))
            {
                Debug.LogWarning("<color=green> ������������� ��������� ������</color>");
                GameObject generationObject = new("Gen");
                GameObject generationWaterObject = new("WaterGen");

                generationObject.transform.SetParent(_worldParrent, false);
                generationWaterObject.transform.SetParent(_worldParrent, false);

                generationObject.transform.position = meshData.WorldPositionStay;
                generationWaterObject.transform.position = meshData.WorldPositionStay;

                Mesh mesh = new Mesh();
                MeshFilter filter = generationObject.AddComponent<MeshFilter>();
                MeshRenderer render = generationObject.AddComponent<MeshRenderer>();
                MeshCollider collider = generationObject.AddComponent<MeshCollider>();

                Mesh waterMesh = new Mesh();
                MeshFilter waterFilter = generationWaterObject.AddComponent<MeshFilter>();
                MeshRenderer waterRender = generationWaterObject.AddComponent<MeshRenderer>();



                render.material = _chuncksMaterial;
                filter.mesh = mesh;
                mesh.vertices = meshData.Verticals.ToArray();
                mesh.triangles = meshData.Triangles.ToArray();
                mesh.uv = meshData.Uvs.ToArray();

                mesh.RecalculateNormals();
                mesh.RecalculateBounds();
                mesh.Optimize();

                waterRender.material = _waterMaterial;
                waterFilter.mesh = waterMesh;
                waterMesh.vertices = meshData.WaterVerticals.ToArray();
                waterMesh.triangles = meshData.WaterTriangles.ToArray();
                waterMesh.uv = meshData.WaterUvs.ToArray();

                waterMesh.RecalculateNormals();
                waterMesh.RecalculateBounds();
                waterMesh.Optimize();


                collider.sharedMesh = mesh;
                _worldObjects.Chuncks.Add(meshData.ChunckPosition, generationObject);
                //�������� water ������ � ��� (���������)

                if (_loadObj != null) _loadObj.SetActive(false);
            }
        }

        private void Generate()
        => _initializer.StartCoroutine(RuntimeGenerationProccess());


        private IEnumerator SetPlayerPosition()
        {
            WaitForSeconds seconds = new WaitForSeconds(4);
            yield return seconds;
            _playerTransform.GetComponent<Rigidbody>().isKinematic = false;
        }


        private IEnumerator RuntimeGenerationProccess()
        {

            WaitForSeconds seconds = new WaitForSeconds(0.02f);

            for (int x = _currentPlayerChunck.x - _radiusGenerate; x < _currentPlayerChunck.x + _radiusGenerate; x++)
            {
                for (int z = _currentPlayerChunck.y - _radiusGenerate; z < _currentPlayerChunck.y + _radiusGenerate; z++)
                {
                    float xOffset = x * WorldGeneration.Width * WorldGeneration.Scale;
                    float zOffset = z * WorldGeneration.Width * WorldGeneration.Scale;

                    Vector2Int worldPosition = new Vector2Int(x, z);
                    
                    

                    if (_worldObjects.ChunckData.ContainsKey(worldPosition)) continue;

                    BlockType[,,] blocks = WorldGeneration.GetChunckTerrain(xOffset, zOffset, 5);
                     
                    ChunckData data = new ChunckData(blocks, new Vector3(xOffset, 0, zOffset));
                    _worldObjects.ChunckData.Add(worldPosition, data);
                    data.ChunckPosition = worldPosition;

                    InstantiateRenderStream(data);
                    yield return seconds;
                }
            }
        }

         
         
        public void GenerateOneEmptyChunck(Vector3Int worldSize)
        {

            ChunckData data = GetEmptyChunkData(worldSize);

            try
            {
                InstantiateRenderStream(data, true);
            }
            catch (Exception ex)
            {
                Debug.LogWarning("������ " + ex.Message);
            }

        }


        private ChunckData GetEmptyChunkData(Vector3Int worldSize)
        {

            if (_loadObj != null) _loadObj.SetActive(true);
            Vector2Int worldPosition = new Vector2Int(0, 0);
            BoundsChunck = worldSize;

            BlockType[,,] blocks = new BlockType[BoundsChunck.x, BoundsChunck.y, BoundsChunck.z];
            blocks[11, 0, 11] = BlockType.Grass;

            for (int i = 0; i < 20; i++)
            {
                blocks[11, i, 11] = BlockType.Grass;
            }


            ChunckData data = new ChunckData(blocks, new Vector3(worldPosition.x, 0, worldPosition.y));
            _worldObjects.ChunckData.Add(worldPosition, data);
            data.ChunckPosition = worldPosition;
            ChunckRenderer render = new(_textureConfig, 14);//Int - waterHeight
            Debug.Log("<color=red>����� ��������</color> 0.0.0.0");
            return data;
        }

        private void InstantiateRenderStream(ChunckData data,bool notGame = false)
        {
            Task.Factory.StartNew(() =>
            {
                ChunckRenderer render = new(_textureConfig, 14);//Int - waterHeight
                Debug.Log("<color=red>����� ��������</color> 0.0.0.0");

                MeshData meshData = null;

                if (notGame)
                {
                    meshData = MeshBuilder.GenerateMeshDataWithEditMode(render, data, BoundsChunck);
                }
                else
                {
                    meshData = MeshBuilder.GenerateMeshData(render, data);
                }
               

                _meshDataQueue.Enqueue(meshData);
            });
        }

    }

}