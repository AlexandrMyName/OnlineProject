using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor;
using System.Threading.Tasks;
using System.Threading;
using System;
using Object = System.Object;
using Utilities;



namespace Editor
{
    public class AtlasPacker : EditorWindow
    {

        private int blockSize = 16;
        private int atlasSizeInBlocks = 16;
        private int atlasSize;

        private Object[] rawTextures;
        private List<Texture2D> _sortedTexture = new List<Texture2D>();
        private Texture2D atlas;

        private bool _taskRunning = false;
        private CancellationTokenSource _cts;
        private float _progress = 0f;


        private float Progress
        {
            set
            {
                _progress = value;

                Repaint();
            }
        }



        [MenuItem("SkyCraft/Atlas packer")]
        public static void ShowWindow() => GetCustomWindow(true).Show();
        private static AtlasPacker GetCustomWindow(bool focus) => GetWindow<AtlasPacker>("Task packer", focus);


        private void ProgressBar()
        {
            var size = position.size;
            var fullRect = GUILayoutUtility.GetRect(size.x, 30);
            var completedRect = new Rect(fullRect.x, fullRect.y, fullRect.width * _progress, fullRect.height);

            EditorGUI.DrawRect(fullRect, Color.black);
            EditorGUI.DrawRect(completedRect, Color.Lerp(Color.red, Color.green, _progress));
            EditorGUI.LabelField(fullRect, $"{_progress * 100}%", EditorStyles.centeredGreyMiniLabel);
        }



        private void OnGUI()
        {
            GUILayout.Label("Texture Atlas Packer", EditorStyles.boldLabel);

            //if (_taskRunning)
                ProgressBar();

            CustomDispatcher.Instance.InvokePending();

            blockSize = EditorGUILayout.IntField("Block size", blockSize);
            atlasSizeInBlocks = EditorGUILayout.IntField("Atlas size (in blocks)", atlasSizeInBlocks);
          
            GUILayout.Label(atlas);

            if (_taskRunning)
            {
                if (GUILayout.Button("Cansel Texture Task"))
                {
                    _cts.Cancel();

                    OnTaskFinishedOrCanceled(null);
                    atlas = new Texture2D(atlasSize, atlasSize);
                }
            }
            else
            {
                atlasSize = blockSize * atlasSizeInBlocks;

                if (GUILayout.Button("Load Texture"))
                {

                    rawTextures = new Object[atlasSize];
                    Debug.Log(rawTextures.Length + " objects");
                    LoadTexture();
                }

                if (_sortedTexture.Count > 0 )
                {
                    if (GUILayout.Button("Pack Texture"))
                    {

                        _progress = 0f;
                        _taskRunning = true;

                        _cts = new CancellationTokenSource();
                        var token = _cts.Token;
                        var context = SynchronizationContext.Current;


                        Task.Run(() => PackAtlas(context, token, atlasSize, atlasSizeInBlocks, blockSize, _sortedTexture), token)
                           .ContinueWith(
                               t => { HandleTaskException(t); },
                               token,
                               TaskContinuationOptions.OnlyOnFaulted,
                               TaskScheduler.FromCurrentSynchronizationContext()
                       );

                    }
                }

            }
             
        }



        private static void HandleTaskException(Task task)
        {
            if (task.IsFaulted)
            {
                Exception ex = task.Exception;
                while (ex is AggregateException && ex.InnerException != null)
                    ex = ex.InnerException;
                EditorUtility.DisplayDialog("Task chain terminated", $"Exception: {ex.Message}", "Ok");
            }
        }



        private void LoadTexture()
        {
            _sortedTexture.Clear();
            rawTextures = Resources.LoadAll("AtlasPacker", typeof(Texture2D));

            int index = 0;

            foreach (Object texture in rawTextures)
            {

                Texture2D t = (Texture2D)texture;
                if (t.width == blockSize && t.height == blockSize)
                    _sortedTexture.Add(t);
                index++;
            }

            Debug.Log($"Atlas packer: sorted textures |{_sortedTexture.Count} count|");
        }


        private void OnTaskFinishedOrCanceled(Color[] pixels)
        {
            _taskRunning = false;
            _cts.Dispose();
            _cts = null;
            Debug.ClearDeveloperConsole();
            if (pixels == null)
            {
                Debug.LogError("Task error!");
            }
            else
            {
                Debug.LogWarning($"Getted pixels with : {pixels.Length} objects");
                atlas = new Texture2D(atlasSize, atlasSize);
                atlas.SetPixels(pixels);
                atlas.Apply();

                SaveAtlasToPNG(atlas);
            }
        }


        private void SaveAtlasToPNG(Texture2D atlas)
        {
            byte[] bytes = atlas.EncodeToPNG();

            File.WriteAllBytes(Application.dataPath + "/dataPacker.png", bytes);
        }


        private async static void PackAtlas(SynchronizationContext context, CancellationToken token, int atlasSize, int atlasSizeInBlocks, int blockSize, List<Texture2D> sortedTexture)
        {

            Color[] pixels = new Color[atlasSize * atlasSize];
            Debug.Log($"Task started at {Thread.CurrentThread.ManagedThreadId}");
            token.ThrowIfCancellationRequested();


            for (int x = 0; x < atlasSize; x++)
            {
                if (x > 0)
                    context.Post(_ => GetCustomWindow(true).Progress = (float)x / atlasSize, null);

                for (int y = 0; y < atlasSize; y++)
                {

                    int currentBlockX = x / blockSize;
                    int currentBlockY = y / blockSize;

                    int currentIndex = currentBlockY * atlasSizeInBlocks + currentBlockX;
                    int currentPixelX = x - (currentBlockX * blockSize);
                    int currentPixelY = y - (currentBlockY * blockSize);




                    Color pixel = new Color(0f, 0f, 0f, 0f);

                    if (currentIndex < sortedTexture.Count)
                    {

                        pixel = await Task.Run(() =>
                        {
                            bool val = false;
                            Color refreshedPixel = new(0, 0, 0, 0);
                            token.ThrowIfCancellationRequested();

                            CustomDispatcher.Instance.Invoke(() =>
                            {

                                refreshedPixel = sortedTexture[currentIndex].GetPixel(currentPixelX, blockSize - currentPixelY - 1);
                                val = true;
                            });
                            while (val == false)
                            {

                            }
                            return refreshedPixel;
                        }, token);

                        pixels[(atlasSize - y - 1) * atlasSize + x] = pixel;

                    }
                    else
                        pixels[(atlasSize - y - 1) * atlasSize + x] = pixel;

                }
            }
            Debug.Log($"Task done at {Thread.CurrentThread.ManagedThreadId}");
            context.Post(_ => GetCustomWindow(true).OnTaskFinishedOrCanceled(pixels), null);

        }


    }



}