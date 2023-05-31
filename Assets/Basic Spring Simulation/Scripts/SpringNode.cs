using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This node can be connected to multiple springs
public class SpringNode
{
    //The state of the node
    public Vector2 pos;
    public Vector2 vel;

    //Is this node connected to a wall?
    public bool isFixed;

    //The total force on this node from the springs attached to it
    public Vector2 force;

    //Gravity
    private readonly Vector2 g = new(0f, -9.81f);



    public SpringNode(Vector2 pos, bool isFixed = false)
    {
        this.pos = pos;;
        this.isFixed = isFixed;
    }



    public void UpdateNodeState(float dt)
    {
        if (isFixed)
        {
            return;
        }
    
        float m = 1f;
    
        //Add gravity
        Vector2 F_gravity = m * g;

        force += F_gravity;


        //Calculate the acceleration on this node
        //F = m*a -> a = F/m
        Vector2 a = force / m;


        //Move the simulation forward one step
        this.vel += dt * a;
        this.pos += dt * this.vel;

        //Add some damping
        //this.vel *= 0.99f;

        //Reset F
        force = Vector2.zero;        
    }
}
