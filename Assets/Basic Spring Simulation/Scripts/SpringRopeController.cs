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

    //The three nodes
    private SpringNode node0;
    private SpringNode node1;
    private SpringNode node2;

    //The two springs connecting the 3 nodes
    private Spring spring01;
    private Spring spring12;


    //To display the history
    private readonly Queue<Vector3> node1History = new();
    private readonly Queue<Vector3> node2History = new();

    private float addToQueueTimer = 0f;

    private bool canSimulate = false;



    private void Start()
    {
        //Where the spring starts
        Vector2 pos0 = new(anchorPointTransform.position.x, anchorPointTransform.position.y);
        Vector2 pos1 = new(pos0.x + 3f, pos0.y + 3f);
        Vector2 pos2 = new(pos1.x, pos1.y - 2f);

        node0 = new(pos0, true);
        node1 = new(pos1);
        node2 = new(pos2);

        spring01 = new(k, restLength, m, springWireRadius, springRadius, node0, node1);
        spring12 = new(k, restLength, m, springWireRadius, springRadius, node1, node2);


        //Freeze the simulation before it starts
        StartCoroutine(WaitForSimulationToStart());
    }



    private IEnumerator WaitForSimulationToStart()
    {
        yield return new WaitForSeconds(3f);

        canSimulate = true;
    }



    private void Update()
    {
        Vector2 pos0 = node0.pos;
        Vector2 pos1 = node1.pos;
        Vector2 pos2 = node2.pos;

        springNode1Transform.position = new Vector3(pos1.x, pos1.y, springNode1Transform.position.z);
        springNode2Transform.position = new Vector3(pos2.x, pos2.y, springNode2Transform.position.z);
        
        Vector3 pos0_3d = new(pos0.x, pos0.y, 0f);
        Vector3 pos1_3d = new(pos1.x, pos1.y, 0f);
        Vector3 pos2_3d = new(pos2.x, pos2.y, 0f);


        //Connect the nodes with a line
        //List<Vector3> line = new();

        //line.Add(pos0_3d);
        //line.Add(pos1_3d);
        //line.Add(pos2_3d);

        //DisplayThiccLine(line);

        //return;


        //Display the springs
        //Copypasta.DisplayGraphics.DisplayLine(line, Copypasta.Materials.ColorOptions.Red);

        List<Vector3> spring1Coordinates = spring01.GetVisualSpringCoordinates(pos0_3d, pos1_3d);
        List<Vector3> spring2Coordinates = spring12.GetVisualSpringCoordinates(pos1_3d, pos2_3d);

        //Copypasta.DisplayGraphics.DisplayLine(spring1Coordinates, Copypasta.Materials.ColorOptions.White);
        //Copypasta.DisplayGraphics.DisplayLine(spring2Coordinates, Copypasta.Materials.ColorOptions.White);

        Copypasta.DisplayGraphics.DisplayThiccLine(spring1Coordinates, springWireRadius, 5, springMaterial);
        Copypasta.DisplayGraphics.DisplayThiccLine(spring2Coordinates, springWireRadius, 5, springMaterial);


        //Display the history of the spring positions 
        addToQueueTimer -= Time.deltaTime;

        if (addToQueueTimer < 0f)
        {
            Vector3 offset = Vector3.forward * 2f;

            node1History.Enqueue(pos1_3d + offset);
            node2History.Enqueue(pos2_3d + offset);

            addToQueueTimer = 0.05f;
        }

        //Copypasta.DisplayGraphics.DisplayLine(new(node1History), Copypasta.Materials.ColorOptions.Blue);
        Copypasta.DisplayGraphics.DisplayLine(new(node2History), Copypasta.Materials.ColorOptions.Black);
    }



    private void FixedUpdate()
    {
        if (!canSimulate)
        {
            return;
        }

        //The fixed anchor node (if we moved it it can be updated here)
        //Vector2 pos0 = new(anchorPointTransform.position.x, anchorPointTransform.position.y);


        //Calculate the spring forces which will also update the forces on the nodes connected to the springs
        spring01.CalculateSpringForce();
        spring12.CalculateSpringForce();


        //Update each node with the forces
        float dt = Time.fixedDeltaTime;

        node0.UpdateNodeState(dt);
        node1.UpdateNodeState(dt);
        node2.UpdateNodeState(dt);
    }
}