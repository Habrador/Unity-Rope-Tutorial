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
    public readonly float springWireRadius;
    //Radius of the spring
    public readonly float radius;
   


    public SpringData(float k, float restLength, float m, float springWireRadius, float radius)
    {
        this.k = k;
        this.restLength = restLength;
        this.m = m;
        this.springWireRadius = springWireRadius;
        this.radius = radius;
    }
}
