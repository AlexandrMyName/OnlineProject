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
using TadWhat.ACraft.ChunckEditor;
using System.Threading.Tasks;

using UnityEngine.UI;
using System.Collections;

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

        [SerializeField] private ChunckEditorPauseView _pauseView;

        [SerializeField] private Slider _sliderLoadBar;

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
                BlockType.WoodLight,

                BlockType.Boards,
                BlockType.Boards_Dark,
                BlockType.WoodenLightBoards ,
                BlockType.Modern_Boards,
                BlockType.Modern_Boards_Dark,
                BlockType.Modern_Boards_Light,
                BlockType.Modern_Boards_Light,

                BlockType.WoodenStaircase,
                BlockType.Glass_White,

                BlockType.Leave,

#region Stones blocks
                BlockType.Red_Brick,
                BlockType.Stone,
                BlockType.Modern_Stone,
                BlockType.Modern_StoneTree,
                #endregion

#region Wools
                BlockType.White_Wool,
                BlockType.BlueLight_Wool,
                BlockType.Blue_Wool,
                BlockType.Brown_Wool,
                BlockType.GreenLight_Wool,
                BlockType.Green_Wool,
                BlockType.Black_Wool,
#endregion
                BlockType.Bedrock,
                BlockType.Snow,
                 
                BlockType.WoodenDoorDOWN,
                BlockType.CraftBlock,
                BlockType.TNT,


                BlockType.Grass_green, BlockType.Grass_green_02, BlockType.Grass_green_03, BlockType.Grass_green_04,

                BlockType.Flower, BlockType.Flower_02, BlockType.Flower_03,
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
                BlockType.CraftBlock,

            };

            return blocks;

        }


        public static XmlMesh LoadMeshFromXML(string name , string folder = "")
        {
             
             
            XmlMesh xmlMesh = new();

            var stream = new XmlSerializer(typeof(XmlMesh));

            string path = Path.Combine(Path.Combine(FileMetaData.Path, folder), $"{name}");

            using (var s = new FileStream(path,  FileMode.Open ))
            {

               xmlMesh = (XmlMesh) stream.Deserialize(s);
            }
            return xmlMesh;

            
        }
         

        public void CreateNewChunck(int width, int height, MetaData meta)
        {

            
            _gen.GenerateOneEmptyChunck(new Vector3Int(width, height, width), meta);

            _inventoryBar.LoadDataBar(LoadInventoryBar(), _inventoryBarView, _iconsItemConfigs);
            _inventoryBarView.gameObject.SetActive(true);
            _craft = new CraftEditMode(_wco, _matChunck, _playerRoot);
            _craft.InstallingBlocksSetUp(true);
            _pauseView.Init(_wco);
            _isInitEditor = true;
        }


        public void LoadChuncks(LoadChuncksRequest request)
        {

             
            if (request.Chuncks.Count == 1)
            {
                ChunckRequest chunk = request.Chuncks[0];
                XmlMesh xml = LoadMeshFromXML(chunk.FileName, chunk.FolderName);

                FileMetaData.NewChunckFileXmlName = chunk.FileName;
                FileMetaData.NewChunckFolderName = chunk.FolderName;
                FileMetaData.NewChunckFileJsonName = Path.ChangeExtension(chunk.FileName, ".json");

                AdminView.Width = xml.ChunckWidth;
                AdminView.Height = xml.ChunckHeight;


                var meta = FileMetaData.CreateMeta(chunk.FileName, Path.ChangeExtension(chunk.FileName, ".json"), chunk.FolderName);
               _gen.GenerateExistXmlMesh(Vector3Int.one, xml, meta);
            }
            else
            {

                _sliderLoadBar.maxValue = request.Chuncks.Count;
                _sliderLoadBar.value = 0;
                _sliderLoadBar.GetComponentInParent<Transform>().gameObject.SetActive(true);
                 
                StartCoroutine(GenerateAll(request.Chuncks));
            }

            _inventoryBar.LoadDataBar(LoadInventoryBar(), _inventoryBarView, _iconsItemConfigs);
            _inventoryBarView.gameObject.SetActive(true);
            _craft = new CraftEditMode(_wco, _matChunck, _playerRoot);
            _craft.InstallingBlocksSetUp(true);
            _pauseView.Init(_wco);
            _isInitEditor = true;
        }


        private IEnumerator GenerateAll(List<ChunckRequest> request)
        {

            int count = 0;
            

            yield return new WaitForSeconds(0.7f);

            foreach (var chunckRef in request)
            {
                 
                XmlMesh xml = LoadMeshFromXML(chunckRef.FileName, chunckRef.FolderName);
                MetaData meta = FileMetaData.CreateMeta(chunckRef.FileName, Path.ChangeExtension(chunckRef.FileName, ".json"), chunckRef.FolderName);
                


                _gen.GenerateExistXmlMesh(Vector3Int.FloorToInt(chunckRef.WorldPosition), xml, meta);

                count++;
                UpdateLoadBar(count);
                yield return new WaitForSeconds(.5f);
            }
           _sliderLoadBar.transform.parent.gameObject.SetActive(false);
        }


        private void UpdateLoadBar(int value)
        {
            _sliderLoadBar.value = value;
        }

        private void Update()
        {

            if (!_isInitEditor) return;

            _inventoryPresenter.RunInventory();
            _inventoryBar.RunInventoryButtons();
            _gen.RunEditGeneration();
            _craft.RunCraftingBlocks(); 
            _pauseView.UpdatePauseView();
        }
    }
}