using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrintScreen : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            Copypasta.UsefulMethods.PrintScreen("spring", "C:/Nerladdat/Temp/");
        }
    }
}
