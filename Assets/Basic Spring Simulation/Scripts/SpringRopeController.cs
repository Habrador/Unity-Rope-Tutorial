using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpringRopeController : MonoBehaviour
{
    public Transform anchorPointTransform;

    public Transform springNode1Transform;

    public Transform springNode2Transform;

    //Spring data
    //Spring constant
    private readonly float k = 20f;
    //Rest length
    private readonly float restLength = 2f;
    //Spring mass 
    private readonly float m = 1f;

    //Spring node states state
    //Each spring consists of two nodes which can be shared if they are connected
    //Node 1 which is shared between spring 1 and spring 2
    private Vector2 pos1;
    private Vector2 vel1;
    //Node 2
    private Vector2 pos2;
    private Vector2 vel2;

    //Other data
    //Gravity
    private readonly Vector2 g = new(0f, -9.81f);



    private void Start()
    {
        //Where the spring starts
        float startY = anchorPointTransform.position.y - 3f;
        float startX = anchorPointTransform.position.x + 2f;

        pos1 = new(startX, startY);

        pos2 = new(pos1.x, pos1.y - 2f);
    }



    private void Update()
    {
        springNode1Transform.position = new Vector3(pos1.x, pos1.y, springNode1Transform.position.z);
        springNode2Transform.position = new Vector3(pos2.x, pos2.y, springNode2Transform.position.z);

        //Connect the nodes with a line
        Vector2 pos0 = new(anchorPointTransform.position.x, anchorPointTransform.position.y);

        Vector3 pos0_3d = new(pos0.x, pos0.y, 0f);
        Vector3 pos1_3d = new(pos1.x, pos1.y, 0f);
        Vector3 pos2_3d = new(pos2.x, pos2.y, 0f);

        List<Vector3> line = new();

        line.Add(pos0_3d);
        line.Add(pos1_3d);
        line.Add(pos2_3d);

        //Copypasta.DisplayGraphics.DisplayLine(line, Copypasta.Materials.ColorOptions.Red);

        List<Vector3> spring1Coordinates = GetVisualSpringCoordinates(pos0_3d, pos1_3d, 0.5f);

        Copypasta.DisplayGraphics.DisplayLine(spring1Coordinates, Copypasta.Materials.ColorOptions.Black);

        List<Vector3> spring2Coordinates = GetVisualSpringCoordinates(pos1_3d, pos2_3d, 0.5f);

        Copypasta.DisplayGraphics.DisplayLine(spring2Coordinates, Copypasta.Materials.ColorOptions.Black);
    }



    private void FixedUpdate()
    {
        //Calculate the spring forces
        //F = -kx
        //k - spring constant
        //x - x is extension from rest length

        //The fixed annchor node
        Vector2 pos0 = new(anchorPointTransform.position.x, anchorPointTransform.position.y);

        //Spring force 1
        Vector2 node0ToNode1 = pos1 - pos0;

        float x1 = node0ToNode1.magnitude - restLength;

        Vector2 F1 = -k * x1 * node0ToNode1.normalized;

        //Spring force 2
        Vector2 node1ToNode2 = pos2 - pos1;

        float x2 = node1ToNode2.magnitude - restLength;

        Vector2 F2 = -k * x2 * node1ToNode2.normalized;


        //Calculate the total force on each node

        //The total force on node 1 which is shared between the springs
        Vector2 F1_tot = F1 + -F2;

        //The total force on node 2
        Vector2 F2_tot = F2;


        //Add gravity
        Vector2 F_gravity = m * g;

        F1_tot += F_gravity;
        F2_tot += F_gravity;


        //Calculate the acceleration on each node
        //F = m*a -> a = F/m
        Vector2 a1 = F1_tot / m;
        Vector2 a2 = F2_tot / m;


        //Move the simulation

        float dt = Time.fixedDeltaTime;

        //v = v + dt*a
        vel1 += dt * a1;
        vel2 += dt * a2;

        //p = p + dt * v
        pos1 += dt * vel1;
        pos2 += dt * vel2;


        //Add some damping
        //vel1 *= 0.99f;
        //vel2 *= 0.99f;
    }



    //Generate coordinates for a line that curves like a spring
    private List<Vector3> GetVisualSpringCoordinates(Vector3 pos1, Vector3 pos2, float radius)
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
            float x = radius * Mathf.Cos(angle * Mathf.Deg2Rad);
            float z = radius * Mathf.Sin(angle * Mathf.Deg2Rad);

            Vector3 vertex = new(x, yPos, z);

            vertex.x += pos1.x;
            vertex.z += pos1.z;

            coordinates.Add(vertex);

            angle += angleStep;
            yPos -= yStep;
        }
        

        //coordinates.Add(pos1);
        //coordinates.Add(pos1 - Vector3.up * (pos1 - pos2).magnitude);

        
        //Rotate the spring to match the direction pos1 -> pos2
        Vector3 pivotPoint = pos1;

        float theta = Mathf.Atan2(pos2.y - pivotPoint.y, pos2.x - pivotPoint.x) * Mathf.Rad2Deg + 90f;

        for (int i = 0; i < coordinates.Count; i++)
        {
            Vector3 vec = coordinates[i];

            Vector2 vecRotated = RotateVec(vec.x, vec.y, theta, pivotPoint.x, pivotPoint.y);

            coordinates[i] = new(vecRotated.x, vecRotated.y, vec.z);
        }
        


        return coordinates;
    }



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