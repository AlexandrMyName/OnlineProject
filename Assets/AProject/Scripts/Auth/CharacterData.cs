using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace TadWhat.Auth
{

    public static class CharacterData
    {


        public static CharacterType Type { get; private set; }


        public static void SetCharacter(CharacterType type)
        {
            Type = type;
        } 

    }

    public enum CharacterType : byte
    {
        Steve = 0,
        Modern_steve = 0,

    }
}