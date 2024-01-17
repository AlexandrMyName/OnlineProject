using UnityEngine;
using Core.Crafting;
using Core.Models;
using Core.Generation;
using System.Collections.Generic;


namespace Core
{
    public class World : MonoBehaviour
    {
        [SerializeField] private Transform _playerTransform;
        [SerializeField] private Material _material;
        [SerializeField] private Material _waterMaterial;
        [SerializeField] private TextureDataConfig _textureConfig;
        [SerializeField] private IconsConfigs _iconsConfigs;
        [SerializeField] private InventoryBarView _inventoryBar_View;

        private Generator _generator;
       // private Craft _craftSystem;
        private InventoryBar _inventoryBar;
        private bool _initialized;
 

        private void Awake() => WorldGeneration.SetRandomSid();


        public void StartWorld(Transform playerTransform, InventoryBarView viewInventoryBar)
        {
            
            _inventoryBar_View = viewInventoryBar;
            _playerTransform = playerTransform;
            _inventoryBar = new();
            _inventoryBar.LoadDataBar(LoadInventoryBar(), _inventoryBar_View, _iconsConfigs);


            WorldChunckObjects worldChunckObjects = new WorldChunckObjects();

            _generator = new Generator(_playerTransform, _material, _waterMaterial, this.transform, this ,_textureConfig);

           // _craftSystem = new Craft(worldChunckObjects, _material,_playerTransform);

            _generator.WorldSetUp(worldChunckObjects, true);
          //  _craftSystem.InstallingBlocksSetUp(true);
            _initialized = true;
        }
        private List<BlockType> LoadInventoryBar()
        {
            //Init Save (not reliase!)
            List<BlockType> blocks = new List<BlockType>() { 
                BlockType.CraftBlock,
                BlockType.WoodLight, 
                BlockType.WoodenStaircase, 
                BlockType.Bake, 
                BlockType.Break, 
                BlockType.Grass, 
                BlockType.Stone ,
                BlockType.WoodenChest,
                BlockType.Glass_White,
                
            };

            return blocks;

        }

        private void Update()
        {
           
            if (!_initialized) return;
          
            _inventoryBar.RunInventoryButtons();
          //  _generator.RunGenerator();
           // _craftSystem.RunCraftingBlocks();
        }


    }
}
