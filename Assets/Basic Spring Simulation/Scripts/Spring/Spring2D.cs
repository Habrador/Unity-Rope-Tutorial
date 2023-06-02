using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spring2D : Spring
{
    private readonly SpringNode2D node1;
    private readonly SpringNode2D node2;



    public Spring2D(SpringData springData, SpringNode2D node1, SpringNode2D node2) : base(springData)
    {
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
}
