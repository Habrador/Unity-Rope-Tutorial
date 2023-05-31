using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spring
{
    //Spring data
    //Spring constant
    private readonly float k;
    //Rest length
    private readonly float restLength;
    //Spring mass 
    private readonly float m;
    //Radius of the wire the spring is made up of
    public readonly float springWireRadius;
    //Radius of the spring
    private readonly float springRadius;
    
    //Spring variables
    //The bottom pos of the spring
    public Vector2 pos;
    //The bottom vel of the spring
    public Vector2 vel;

    //Gravity
    private readonly Vector2 g = new(0f, -9.81f);



    public Spring(float k, float restLength, float m, float springWireRadius, float springRadius, Vector2 pos)
    {
        this.k = k;
        this.restLength = restLength;
        this.m = m;
        this.springWireRadius = springWireRadius;
        this.springRadius = springRadius;
        this.pos = pos;
    }



    //Calculate the spring forces
    //F = -kx
    //k - spring constant
    //x - x is extension from rest length
    public Vector2 CalculateSpringForce(Vector2 pos1, Vector2 pos2)
    {
        Vector2 node1ToNode2 = pos2 - pos1;

        float x2 = node1ToNode2.magnitude - restLength;

        Vector2 F = -k * x2 * node1ToNode2.normalized;

        return F;
    }



    //F is the spring force on the spring, which can be higher than just CalculateSpringForce() if the spring is connected to another spring
    public void UpdateSpringState(Vector2 F, float dt)
    {
        //Add gravity
        Vector2 F_gravity = m * g;

        F += F_gravity;


        //Calculate the acceleration on this node
        //F = m*a -> a = F/m
        Vector2 a = F / m;


        //Move the simulation forward one step
        this.vel += dt * a;
        this.pos += dt * this.vel;

        //Add some damping
        //this.vel *= 0.99f;
    }



    //Generate coordinates for a line that curves like a spring goinng from pos1 to pos2
    public List<Vector3> GetVisualSpringCoordinates(Vector3 pos1, Vector3 pos2)
    {
        List<Vector3> coordinates = new();


        int circleResolution = 10;
        int spirals = 5;
        int iterations = circleResolution * spirals;

        float angle = 90f;
        float angleStep = 360f / (float)circleResolution;

        float yPos = pos1.y;
        float yStep = (pos2 - pos1).magnitude / (float)iterations;

        for (int i = 0; i < iterations + 1; i++)
        {
            float x = springRadius * Mathf.Cos(angle * Mathf.Deg2Rad);
            float z = springRadius * Mathf.Sin(angle * Mathf.Deg2Rad);

            Vector3 vertex = new(x, yPos, z);

            vertex.x += pos1.x;
            vertex.z += pos1.z;

            coordinates.Add(vertex);

            angle += angleStep;
            yPos -= yStep;
        }

        //Rotate coordinates of a spring so they go from pos1 to pos 2
        RotateSpring(pos1, pos2, coordinates);

        return coordinates;
    }



    //Rotate coordinates of a spring so they go from pos1 to pos 2 
    private void RotateSpring(Vector3 pos1, Vector3 pos2, List<Vector3> coordinates)
    {
        //coordinates.Add(pos1);
        //coordinates.Add(pos1 - Vector3.up * (pos1 - pos2).magnitude);


        //Rotate the spring to match the direction pos1 -> pos2
        Vector3 pivotPoint = pos1;

        //Atan2 returns 0->180 counter-clockwise if above x-axis, 0->-180 if below x-axis
        float theta = Mathf.Atan2(pos2.y - pivotPoint.y, pos2.x - pivotPoint.x) * Mathf.Rad2Deg + 90f;

        //This will rotate it by for example 45 degrees but the spring is not starting along the x-axis where the degrees start
        //which is why we have to compensate by the 90 degrees in atan2 calculations
        for (int i = 0; i < coordinates.Count; i++)
        {
            Vector3 vec = coordinates[i];

            Vector2 vecRotated = RotateVec(vec.x, vec.y, theta, pivotPoint.x, pivotPoint.y);

            coordinates[i] = new(vecRotated.x, vecRotated.y, vec.z);
        }
    }



    //Rotate a vector with angle theta around a pivot point in 2d space
    private Vector2 RotateVec(float x, float y, float theta, float pivotX, float pivotY)
    {
        float thetaRad = theta * Mathf.Deg2Rad;

        //Subtract the pivot from the vector so the vector originates from origo
        float xZero = x - pivotX;
        float yZero = y - pivotY;

        float xRotated = xZero * Mathf.Cos(thetaRad) - yZero * Mathf.Sin(thetaRad);
        float yRotated = xZero * Mathf.Sin(thetaRad) + yZero * Mathf.Cos(thetaRad);

        xRotated += pivotX;
        yRotated += pivotY;

        return new(xRotated, yRotated);
    }
}
