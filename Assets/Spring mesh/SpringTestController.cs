using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpringTestController : MonoBehaviour
{
    public Transform springStartTransform;
    public Transform springEndTransform;

    public MeshFilter springMF;


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

    //The nodes
    private SpringNode3D node0;
    private SpringNode3D node1;

    //The spring
    private Spring3D spring01;



    private void Start()
    {
        node0 = new SpringNode3D(springStartTransform.position, true);
        node1 = new SpringNode3D(springEndTransform.position);
    
        spring01 = new Spring3D(k, restLength, m, springWireRadius, springRadius, node0, node1);
    }



    private void Update()
    {
        springEndTransform.position = node1.pos;


        Vector3 p0 = springStartTransform.position;
        Vector3 p1 = springEndTransform.position;

        List<Vector3> springCoordinates = spring01.GetVisualSpringCoordinates(p0, p1, 50);

        Mesh springMesh = Copypasta.DisplayGraphics.GenerateThiccLineMesh(springCoordinates, springWireRadius, 5);

        springMF.mesh = springMesh;
    }



    private void FixedUpdate()
    {
        //Calculate the spring forces which will also update the forces on the nodes connected to the springs
        spring01.CalculateSpringForce();


        //Update each node with the forces
        float dt = Time.fixedDeltaTime;

        node0.UpdateNodeState(dt);
        node1.UpdateNodeState(dt);
    }
}
