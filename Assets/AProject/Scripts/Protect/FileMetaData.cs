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

            return System.IO.Path.Combine(Application.persistentDataPath, "Meshes");

#endif
            }
            set
            {

            }
        }


        public static MetaData CreateMeta(string fileXmlMesh, string fileJsonName, string folderName)
        {
            
            return new MetaData(fileXmlMesh,fileJsonName,folderName);
        }
    }

    public struct MetaData
    {

        public string FileXmlName;
        public string FileJsonName;
        public string FolderName;


        public MetaData(string fileXmlMesh,string fileJsonName, string folderName)
        {
            FileXmlName = fileXmlMesh;

            FileJsonName = fileJsonName;
            FolderName = folderName;
        }
    }
}