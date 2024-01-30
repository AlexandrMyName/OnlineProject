using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;


namespace Cryptograph
{

    public static class FileMetaData
    {

        public static string NewChunckFileXmlName;
        public static string NewChunckFileJsonName;
        public static string NewChunckFolderName;


        public static string Path
        {

            get
            {

#if UNITY_EDITOR
                return Application.dataPath.Replace("/Assets", "/Meshes");

#else

            return Path.Combine(Application.persistentDataPath, "Meshes");

#endif
            }
            set
            {

            }
        }
    }
}