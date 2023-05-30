using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpringRopeController : MonoBehaviour
{
    public Transform anchorPointTransform;

    public Transform springNode1Transform;

    public Transform springNode2Transform;

    public Material springMaterial;

    //Spring data
    //Spring constant
    private readonly float k = 20f;
    //Rest length
    private readonly float restLength = 2f;
    //Spring mass 
    private readonly float m = 1f;
    //Radius of the wire the spring is made up of
    private readonly float springWireRadius = 0.07f;
    //Radius of the spring
    private readonly float springRadius = 0.7f;

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

    //To display the history
    private Queue<Vector3> node1History = new();
    private Queue<Vector3> node2History = new();

    private float addToQueueTimer = 0f;



    private void Start()
    {
        //Where the spring starts
        float startY = anchorPointTransform.position.y + 3f;
        float startX = anchorPointTransform.position.x + 3f;

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

        //DisplayThiccLine(line);

        //return;

        //Copypasta.DisplayGraphics.DisplayLine(line, Copypasta.Materials.ColorOptions.Red);

        List<Vector3> spring1Coordinates = GetVisualSpringCoordinates(pos0_3d, pos1_3d, springRadius);
        List<Vector3> spring2Coordinates = GetVisualSpringCoordinates(pos1_3d, pos2_3d, springRadius);

        //Copypasta.DisplayGraphics.DisplayLine(spring1Coordinates, Copypasta.Materials.ColorOptions.White);
        //Copypasta.DisplayGraphics.DisplayLine(spring2Coordinates, Copypasta.Materials.ColorOptions.White);

        DisplayThiccLine(spring1Coordinates);
        DisplayThiccLine(spring2Coordinates);


        //Display the history of the spring positions 
        addToQueueTimer -= Time.deltaTime;

        if (addToQueueTimer < 0f)
        {
            node1History.Enqueue(pos1_3d);
            node2History.Enqueue(pos2_3d);

            addToQueueTimer = 0.05f;
        }

        Copypasta.DisplayGraphics.DisplayLine(new(node1History), Copypasta.Materials.ColorOptions.Blue);
        Copypasta.DisplayGraphics.DisplayLine(new(node2History), Copypasta.Materials.ColorOptions.Yellow);
    }



    private void FixedUpdate()
    {
        //return;
    
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
        


        return coordinates;
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



    //Extrude a mesh along a line to make a thicc line
    private void DisplayThiccLine(List<Vector3> lineCoordinates)
    {
        //Generate mesh vertices at point 1 in a circle aligned along the first line segment
        //Extrude them my moving them along the normal towards the next point

        //Find a point on the circle which is a normal to the first line segment
        float lineRadius = springWireRadius;

        Vector3 a = lineCoordinates[0];
        Vector3 b = lineCoordinates[1];

        Vector3 lineDir = (b - a).normalized;

        Vector3 normal = Vector3.Cross(lineDir, Vector3.up).normalized;

        Vector3 p0 = normal * lineRadius;


        //Find the other vertices by rotating p0 around the line segment to get a full circle with some resolution
        List<Vector3> vertices = new();

        int resolution = 6;

        float angleStep = 360f / (float)resolution;

        float angle = 0f;

        //Rotate p0 around the line segment to get a full circle with some resolution
        for (int i = 0; i < resolution; i++)
        {
            Vector3 p = Quaternion.AngleAxis(angle, lineDir) * p0;

            p += a;

            vertices.Add(p);

            angle += angleStep;
        }


        //Add the rest of the vertices by moving the previous vertices along the normal towards the next point
        for (int i = 1; i < lineCoordinates.Count; i++)
        {
            Vector3 moveDist = lineCoordinates[i] - lineCoordinates[i - 1];

            for (int j = 0; j < resolution; j++)
            {
                Vector3 p = vertices[vertices.Count - resolution];

                vertices.Add(p + moveDist);
            }

        }

        //Copypasta.DisplayGraphics.DisplayLine(vertices, Copypasta.Materials.ColorOptions.Orange);

        //Generate the mesh triangles
        List<int> triangles = new();

        for (int i = resolution; i < vertices.Count; i += resolution)
        {
            for (int j = 0; j < resolution; j++)
            {
                int thisVertex = i + j;
                //The corresponding vertex on the previous circle
                int thisVertexPreviousCircle = thisVertex - resolution;

                int thisVertexPrevJIndex = j - 1;
                if (thisVertexPrevJIndex < 0)
                {
                    thisVertexPrevJIndex = resolution - 1;
                }

                //The vertex coming before this vertex
                int thisVertexPrev = thisVertexPrevJIndex + i;

                //The corresponding previous vertex on the previous circle
                int thisVertexPrevPreviousCircle = thisVertexPrev - resolution;

                //Build the triangles
                int aIndex = thisVertexPrevPreviousCircle;
                int bIndex = thisVertexPrev;
                int cIndex = thisVertexPreviousCircle;
                int dIndex = thisVertex;

                triangles.Add(aIndex);
                triangles.Add(bIndex);
                triangles.Add(dIndex);

                triangles.Add(aIndex);
                triangles.Add(dIndex);
                triangles.Add(cIndex);
            }
        }


        //Make a mesh
        Mesh m = new();

        m.SetVertices(vertices);
        m.SetTriangles(triangles, 0);

        m.RecalculateNormals();

        //Material baseMaterial = new(Shader.Find("Unlit/Color"));

        Graphics.DrawMesh(m, Vector3.zero, Quaternion.identity, springMaterial, 0, Camera.main, 0);
    }
}