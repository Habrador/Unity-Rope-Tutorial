using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct SpringData
{
    //Spring constant
    public readonly float k;
    //Rest length
    public readonly float restLength;
    //Spring mass 
    public readonly float m;
    //Radius of the wire the spring is made up of
    public readonly float wireRadius;
    //Radius of the spring
    public readonly float radius;
    //Number of spirals
    public readonly int spirals;


    public SpringData(float k, float restLength, float m, float wireRadius, float radius, int spirals = 5)
    {
        this.k = k;
        this.restLength = restLength;
        this.m = m;
        this.wireRadius = wireRadius;
        this.radius = radius;
        this.spirals = 5;
    }
}
