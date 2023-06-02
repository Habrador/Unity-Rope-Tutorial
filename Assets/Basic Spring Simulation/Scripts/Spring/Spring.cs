using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//A spring going between two nodes
public abstract class Spring
{
    protected readonly SpringData springData;



    public Spring(SpringData springData)
    {
        this.springData = springData;
    }



    //Generate coordinates for a line that curves like a spring going from pos1 to pos2
    //The spring will be in x-y space but have a z-coordinate
    //The input z-coordinate is ignored
    //It will go downwards in -y-direction
    public List<Vector3> GetVisualSpringCoordinates(Vector3 pos1, Vector3 pos2, int circleResolution = 10)
    {
        List<Vector3> coordinates = new();
 

        int iterations = circleResolution * springData.spirals;

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



    //Rotate coordinates of a spring so they go from pos1 to pos 2 in 3d space
    private void RotateSpring(Vector3 pos1, Vector3 pos2, List<Vector3> coordinates)
    {
        Vector3 pivotPoint = pos1;

        //Atan2 returns 0->180 counter-clockwise if above x-axis, 0->-180 if below x-axis
        //float thetaZ = Mathf.Atan2(pos2.y - pivotPoint.y, pos2.x - pivotPoint.x) * Mathf.Rad2Deg + 90f;
        //float thetaX = Mathf.Atan2(pos2.y - pivotPoint.y, pos2.z - pivotPoint.z) * Mathf.Rad2Deg + 90f;
        //float thetaY = Mathf.Atan2(pos2.z - pivotPoint.z, pos2.x - pivotPoint.x) * Mathf.Rad2Deg + 90f;

        //To do it in 3d we better use Quaternions
        Vector3 lookDir = (pos2 - pos1).normalized;

        //Using Vector3.up makes the spring flip now and then when it rotates past an axle
        Quaternion rot = Quaternion.LookRotation(lookDir, Vector3.right);

        //Our vectors forward is in down direction, so we have to translate it to the forward direction, which is what LookRotation cares about
        rot *= Quaternion.FromToRotation(Vector3.down, Vector3.forward);

        //This will rotate it by for example 45 degrees but the spring is not starting along the x-axis where the degrees start
        //which is why we have to compensate by the 90 degrees in atan2 calculations
        for (int i = 0; i < coordinates.Count; i++)
        {
            Vector3 vec = coordinates[i];

            vec -= pivotPoint;

            //2d space
            //vec = Quaternion.Euler(0f, 0f, thetaZ) * vec;
            //vec = Quaternion.Euler(-thetaX, 0f, 0f) * vec;

            //3d space

            //Doesnt work!!!
            //vec = Quaternion.Euler(-thetaX, 0f, thetaZ) * vec;

            vec = rot * vec;         

            vec += pivotPoint;

            coordinates[i] = vec;
        }
    }
}
