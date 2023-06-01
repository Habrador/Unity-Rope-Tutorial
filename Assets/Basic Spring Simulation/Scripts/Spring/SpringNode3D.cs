using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpringNode3D : SpringNode
{
    //The state of the node
    public Vector3 pos;
    public Vector3 vel;

    //The total force on this node from the springs attached to it
    public Vector3 force;

    //Gravity
    private readonly Vector3 g = new(0f, -9.81f, 0f);



    public SpringNode3D(Vector3 pos, bool isFixed = false) : base (isFixed)
    {
        this.pos = pos;
    }



    public void UpdateNodeState(float dt)
    {
        if (isFixed)
        {
            return;
        }

        float m = 1f;

        //Add gravity
        Vector3 F_gravity = m * g;

        force += F_gravity;


        //Calculate the acceleration on this node
        //F = m*a -> a = F/m
        Vector3 a = force / m;


        //Move the simulation forward one step
        this.vel += dt * a;
        this.pos += dt * this.vel;

        //Add some damping
        //this.vel *= 0.99f;

        //Reset F
        force = Vector3.zero;
    }
}
