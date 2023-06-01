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
    private SpringNode node0;
    private SpringNode node1;

    //The spring
    private Spring spring01;



    private void Start()
    {
        spring01 = new Spring(k, restLength, m, springWireRadius, springRadius, null, null);
    }



    private void Update()
    {
        Vector3 p0 = springStartTransform.position;
        Vector3 p1 = springEndTransform.position;

        List<Vector3> springCoordinates = spring01.GetVisualSpringCoordinates(p0, p1, 50);

        Mesh springMesh = Copypasta.DisplayGraphics.GenerateThiccLineMesh(springCoordinates, springWireRadius, 5);

        springMF.mesh = springMesh;
    }
}
