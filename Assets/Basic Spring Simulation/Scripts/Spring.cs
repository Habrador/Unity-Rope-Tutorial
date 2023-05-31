using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//A spring going between two nodes
public class Spring
{
    private readonly SpringData springData;

    //The two nodes this spring is connected to
    private readonly SpringNode node1;
    private readonly SpringNode node2;



    public Spring(float k, float restLength, float m, float springWireRadius, float springRadius, SpringNode node1, SpringNode node2)
    {
        this.springData = new(k, restLength, m, springWireRadius, springRadius);
    
        this.node1 = node1;
        this.node2 = node2;
    }



    //Calculate the spring forces
    //F = -kx
    //k - spring constant
    //x - extension from rest length
    public void CalculateSpringForce()
    {
        Vector2 node1ToNode2 = node2.pos - node1.pos;

        float x2 = node1ToNode2.magnitude - springData.restLength;

        Vector2 F = -springData.k * x2 * node1ToNode2.normalized;

        //This is the force on node2
        node2.force += F;

        //The force on node1 is -F
        node1.force += -F;

        //The total force on node 1 which is shared between the springs
        //Vector2 F1_tot = F1 + -F2;

        //The total force on node 2
        //Vector2 F2_tot = F2;
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
            float x = springData.radius * Mathf.Cos(angle * Mathf.Deg2Rad);
            float z = springData.radius * Mathf.Sin(angle * Mathf.Deg2Rad);

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