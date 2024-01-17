using Core.Models;
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(IconsConfig),menuName = "Configs/" + nameof(IconsConfig))]
public class IconsConfigs : ScriptableObject
{
    [field: SerializeField] public List<IconsConfig> Configs { get; set; }
  


}

[Serializable]
public class IconsConfig
{
    public Sprite Icon;
    public BlockType Type;
}
