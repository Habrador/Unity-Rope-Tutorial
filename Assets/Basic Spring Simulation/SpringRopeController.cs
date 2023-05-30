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
    }



    private void FixedUpdate()
    {
        //Calculate the spring forces
    
        //Spring force 1
        Vector2 anchorToSpringPos1 = pos1 - new Vector2(anchorPointTransform.position.x, anchorPointTransform.position.y);

        //F = -kx where x is extension from rest length
        float x1 = anchorToSpringPos1.magnitude - restLength;

        Vector2 F1 = -k * x1 * anchorToSpringPos1.normalized;

        //Spring force 2
        Vector2 springPos1ToSpringPos2 = pos2 - pos1;

        float x2 = springPos1ToSpringPos2.magnitude - restLength;

        Vector2 F2 = -k * x2 * springPos1ToSpringPos2.normalized;


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
}