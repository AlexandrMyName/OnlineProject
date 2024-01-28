using Core.Crafting;
using Core.Generation;
using Core.Models;
using Cryptograph.Xml;
using Cryptograph;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Xml.Serialization;
using TadWhat.Auth;
using System;

namespace TadWhat.ACraft.Constructor
{

    public class EditChunckAndCreation : MonoBehaviour
    {
         
        [SerializeField] private Material _matChunck;
        [SerializeField] private Material _matWater;
        [SerializeField] private GameObject _loadObj, _saveObj;
        [SerializeField] private TextureDataConfig _textureConfig;
        [SerializeField] private IconsConfigs _iconsItemConfigs;
        [SerializeField] private Transform _playerRoot;

        [Space,Header("Инвентарь")]
        [SerializeField] private InventoryBarView _inventoryBarView;
        [SerializeField] private Inventory _inventoryPresenter;


        private WorldChunckObjects _wco;
        private Generator _gen;
        private CraftEditMode _craft;
        private InventoryBar _inventoryBar;
        private bool _isInitEditor;

       

        private void Start()
        {

            _wco = new WorldChunckObjects();
            _gen = new Generator(_playerRoot.transform, _matChunck, _matWater, this.transform, this, _textureConfig);
            _inventoryBar = new InventoryBar();

            _gen.WorldSetUp(_wco, false, 0,_loadObj, _saveObj);

            _inventoryPresenter.Init(_inventoryBar);
            _inventoryPresenter.InitCreativeInventory(new[]
            {
                BlockType.Grass,
                BlockType.Wood_Gray,
                BlockType.Wood,
                BlockType.White_Wool,
                BlockType.Bedrock,
                BlockType.Snow,
                BlockType.Boards,
                BlockType.WoodenLightBoards ,
                BlockType.Boards_Dark,
                BlockType.Modern_Boards,
                BlockType.Modern_Boards_Dark,
                BlockType.Modern_Boards_Light,
                BlockType.Modern_Boards_Light,
                BlockType.Stone,
                BlockType.Modern_Stone,
                BlockType.Modern_StoneTree,
                BlockType.CraftBlock,
                BlockType.TNT,

                  BlockType.WoodenDoorDOWN,
                BlockType.Red_Brick,
                BlockType.Bedrock,
                BlockType.Black_Wool,
                BlockType.BlueLight_Wool,
                BlockType.Blue_Wool,
                BlockType.Brown_Wool,
                BlockType.GreenLight_Wool,
                BlockType.Green_Wool,
            
            });
             
        }

        private List<BlockType> LoadInventoryBar()
        {
            //Init Save (not reliase!)
            List<BlockType> blocks = new List<BlockType>() {
                BlockType.Grass,
                BlockType.WoodenStaircase,
                BlockType.Glass_White,
                BlockType.Stone,
                BlockType.Leave,
                BlockType.WoodLight,
                BlockType.WoodenLightBoards ,
                BlockType.WoodenDoorDOWN,
                BlockType.Leave,

            };

            return blocks;

        }


        public XmlMesh LoadMeshFromXML(string name)
        {
             
             
            XmlMesh xmlMesh = new();

            var stream = new XmlSerializer(typeof(XmlMesh));

            string path = Path.Combine(Application.dataPath.Replace("/Assets", "/Meshes"), $"{name}");

            using (var s = new FileStream(path,  FileMode.Open ))
            {

               xmlMesh = (XmlMesh) stream.Deserialize(s);
            }
            return xmlMesh;

            
        }
         

        public void CreateNewChunck(int width, int height, string fileName)
        {

            _gen.GenerateOneEmptyChunck(new Vector3Int(width, height, width));

            _inventoryBar.LoadDataBar(LoadInventoryBar(), _inventoryBarView, _iconsItemConfigs);
            _inventoryBarView.gameObject.SetActive(true);
            _craft = new CraftEditMode(_wco, _matChunck, _playerRoot);
            _craft.InstallingBlocksSetUp(true);
            _isInitEditor = true;
        }


        public void LoadChuncks(LoadChuncksRequest request)
        {

            if (request.Chuncks.Count == 1)
            {
               
                XmlMesh xml = LoadMeshFromXML(request.Chuncks[0].FileName);

                _gen.GenerateExistXmlMesh(Vector3Int.one, xml);
            }
            else
            {
                foreach (var chunckRef in request.Chuncks)
                {

                    // if (chunckRef.MeshView == null) continue;

                    XmlMesh xml = LoadMeshFromXML(chunckRef.FileName);

                    _gen.GenerateExistXmlMesh(Vector3Int.FloorToInt(chunckRef.WorldPosition), xml);

                }
            }
            _inventoryBar.LoadDataBar(LoadInventoryBar(), _inventoryBarView, _iconsItemConfigs);
            _inventoryBarView.gameObject.SetActive(true);
            _craft = new CraftEditMode(_wco, _matChunck, _playerRoot);
            _craft.InstallingBlocksSetUp(true);
            _isInitEditor = true;
        }


        private void Update()
        {

            if (!_isInitEditor) return;

            _inventoryPresenter.RunInventory();
            _inventoryBar.RunInventoryButtons();
            _gen.RunEditGeneration();
            _craft.RunCraftingBlocks();
        }
    }
}