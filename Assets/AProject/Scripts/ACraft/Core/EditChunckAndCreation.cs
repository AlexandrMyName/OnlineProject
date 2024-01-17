using Core.Crafting;
using Core.Generation;
using Core.Models;
using UnityEngine;


namespace TadWhat.ACraft.Constructor
{

    public class EditChunckAndCreation : MonoBehaviour
    {
         
        [SerializeField] private Material _matChunck;
        [SerializeField] private Material _matWater;
        [SerializeField] private GameObject _loadObj;
        [SerializeField] private TextureDataConfig _textureConfig; 
        [SerializeField] private Transform _playerRoot;

        private WorldChunckObjects _wco;
        private Generator _gen;
        private CraftEditMode _craft;
        private bool _isInitEditor;


        private void Start()
        {

            _wco = new WorldChunckObjects();
            _gen = new Generator(_playerRoot.transform, _matChunck, _matWater, this.transform, this, _textureConfig);


            _gen.WorldSetUp(_wco, false, 0,_loadObj);

           
             
        }


        public void InitEditor(int width,int height)
        {

            _gen.GenerateOneEmptyChunck(new Vector3Int(width, height, width));
            _craft = new CraftEditMode(_wco, _matChunck, _playerRoot);
            _craft.InstallingBlocksSetUp(true);
            _isInitEditor = true;
        }


        private void Update()
        {
            if (!_isInitEditor) return;

            _gen.RunEditGeneration();
            _craft.RunCraftingBlocks();
        }
    }
}