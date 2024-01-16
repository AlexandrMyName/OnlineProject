using Core.Generation;
using Core.Models;
using UnityEngine;


namespace Core.Crafting
{
 
    public class Craft
    {

        private Material _chuncksMaterial;
        private Camera _camera;
        private Transform _playerTransform;
        private WorldChunckObjects _worldObjects;
        private bool _useCrafting;
       

        public Craft(WorldChunckObjects worldObjects, Material chancksMaterial, Transform playerTransform)
        {
           
            _playerTransform = playerTransform;
            _chuncksMaterial = chancksMaterial;
            _worldObjects = worldObjects;
           
            _camera = Camera.main;
        }


        public void InstallingBlocksSetUp( bool isActivate = true) => _useCrafting = isActivate;


        public void RunCraftingBlocks()
        {
            if(!Input.GetMouseButtonDown(0) && !Input.GetMouseButtonDown(1)) return;
            CraftBlock();
        }

      
        private void CraftBlock()
            {
            if (!_useCrafting) return;
            bool destroy = Input.GetMouseButtonDown(0) ? true : false;

            Ray ray = _camera.ViewportPointToRay(new Vector3(0.5f, 0.5f));

            if (Physics.Raycast(ray, out var hit, 3f))
            {

                Vector3 centerPosition = destroy ? hit.point + hit.normal * -WorldGeneration.Scale / 2 
                    : hit.point + hit.normal * WorldGeneration.Scale / 2;// Global без учета 'scale'
                if ((Vector3.Distance(centerPosition, _playerTransform.position) < .3f) && !destroy)  return;

                Debug.LogWarning(hit.normal + "Normal");
                Vector3Int worldPosition = Vector3Int.FloorToInt(centerPosition / WorldGeneration.Scale); // Global с учетом 'scale'
                Vector2Int chunckPosition = _worldObjects.GetChunckContaineBlock(worldPosition); // ѕозици€ чанка в 2D - x, z
                
                GameObject chunckObject = _worldObjects.GetChunck(chunckPosition);

                if (chunckObject == null) return;
                if (_worldObjects.ChunckData.TryGetValue(chunckPosition, out var data))
                {
                    Vector3Int chunckOrigin = new Vector3Int(chunckPosition.x, 0, chunckPosition.y) * WorldGeneration.Width; //ѕеревод 2D Global в 3D Global
                    Vector3Int localCubePosition = worldPosition - chunckOrigin;// world (позици€ куба) - global chunck = локальные координаты куба

                    data.Blocks[localCubePosition.x, localCubePosition.y, localCubePosition.z] = destroy ? BlockType.Air : CurrentBlock.CurrentCraftingBlock;

                    
                        if (destroy) data.Render.ClearNormalData(localCubePosition);


                        MeshData meshData = data.Render.SetBlock(hit.normal);
                        MeshFilter filter = chunckObject.GetComponent<MeshFilter>();
                        MeshRenderer render = chunckObject.GetComponent<MeshRenderer>();
                        MeshCollider collider = chunckObject.GetComponent<MeshCollider>();
                        Mesh mesh = filter.sharedMesh;

                        mesh.Clear();
                        if (filter == null || render == null || collider == null) return;

                        render.material = _chuncksMaterial;
                        filter.mesh = mesh;
                        mesh.vertices = meshData.Verticals.ToArray();
                        mesh.triangles = meshData.Triangles.ToArray();
                        mesh.uv = meshData.Uvs.ToArray();

                        mesh.RecalculateNormals();
                        mesh.RecalculateBounds();
                        mesh.Optimize();

                        collider.sharedMesh = mesh;
                    
                }
            }
        }
        }
    }
