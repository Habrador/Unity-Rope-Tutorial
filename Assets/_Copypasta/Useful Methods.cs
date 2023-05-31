using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Copypasta
{
    public class UsefulMethods
    {
        //Remap value from range 1 to range 2
        public static float Remap(float value, MinMax range1, MinMax range2)
        {
            float remappedValue = range2.min + (value - range1.min) * ((range2.max - range2.min) / (range1.max - range1.min));

            return remappedValue;
        }



        //Clamp list indices
        //Will even work if index is larger/smaller than listSize, so can loop multiple times
        public static int ClampListIndex(int index, int listSize)
        {
            index = ((index % listSize) + listSize) % listSize;

            return index;
        }



        //Add value to average
        //http://www.bennadel.com/blog/1627-create-a-running-average-without-storing-individual-values.htm
        //count - how many values does the average consist of
        public static float AddValueToRunningAverage(float oldAverage, float valueToAdd, float count)
        {
            float newAverage = ((oldAverage * count) + valueToAdd) / (count + 1f);

            return newAverage;
        }



        //Take a screenshot
        //Path should be like "C:/Download/Temp/"
        public static void PrintScreen(string name, string path)
        {
            System.DateTime localDate = System.DateTime.Now;

            string dateTime = localDate.ToString();

            string year = dateTime.Substring(0, 4);
            string month = dateTime.Substring(5, 2);
            string day = dateTime.Substring(8, 2);
            string hours = dateTime.Substring(11, 2);
            string minutes = dateTime.Substring(14, 2);
            string seconds = dateTime.Substring(17, 2);

            //Has to be - and not _ or it wont sort the screenshots in correct order
            string timeStamp = $"{year}-{month}-{day}--{hours}-{minutes}-{seconds}";

            //Debug.Log(dateTime);
            //Debug.Log(timeStamp);

            string fileName = $"{name}_{timeStamp}.png";

            ScreenCapture.CaptureScreenshot(path + fileName);
        }
    }
}