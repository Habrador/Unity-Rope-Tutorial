using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Copypasta
{
    public class Materials
    {
        private static readonly Material matWhite;
        private static readonly Material matRed;
        private static readonly Material matBlue;
        private static readonly Material matYellow;
        private static readonly Material matBlack;
        private static readonly Material matGreen;
        private static readonly Material matOrange;
        private static readonly Material matPink;



        public enum ColorOptions
        {
            White, Red, Blue, Yellow, Black, Green, Orange, Pink
        }



        static Materials()
        {
            matWhite = GetBaseMaterial(Color.white);

            matRed = GetBaseMaterial(Color.red);

            matBlue = GetBaseMaterial(Color.blue);

            matYellow = GetBaseMaterial(Color.yellow);

            matBlack = GetBaseMaterial(Color.black);

            matGreen = GetBaseMaterial(Color.green);

            matOrange = GetBaseMaterial(new Color32(255, 125, 0, 255));

            matPink = GetBaseMaterial(Color.magenta);
        }



        private static Material GetBaseMaterial(Color color)
        {
            //Material baseMaterial = new(Shader.Find("Unlit/Color"));

            Material baseMaterial = new(Shader.Find("Universal Render Pipeline/Unlit"));

            baseMaterial.color = color;

            return baseMaterial;
        }



        public static Material GetMaterial(ColorOptions color)
        {
            return color switch
            {
                (ColorOptions.Red) => matRed,
                (ColorOptions.Blue) => matBlue,
                (ColorOptions.Yellow) => matYellow,
                (ColorOptions.White) => matWhite,
                (ColorOptions.Black) => matBlack,
                (ColorOptions.Green) => matGreen,
                (ColorOptions.Orange) => matOrange,
                (ColorOptions.Pink) => matPink,
                _ => matWhite,
            };
        }
    }
}