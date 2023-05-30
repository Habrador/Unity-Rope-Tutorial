using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Copypasta
{
    public struct Circle
    {
        public Vector3 center;

        public float radius;

        public Circle(Vector3 center, float radius)
        {
            this.center = center;
            this.radius = radius;
        }


        //Generate vertices on the edge of a circle in 2d space
        //Generates a double vertex on the start position so they line connects to a circle
        //^y
        //|
        //|
        //------->x
        //^y
        //|
        //|
        //------->z
        //^z
        //|
        //|
        //------->x
        public enum Space2D { XY, XZ, ZY };

        public List<Vector3> GetCircleVertices(Space2D space, int resolution = 100)
        {
            List<Vector3> vertices = new();

            float angleStep = 360f / resolution;

            float angle = 0f;

            for (int i = 0; i < resolution + 1; i++)
            {
                float x = this.radius * Mathf.Cos(angle * Mathf.Deg2Rad);
                float y = this.radius * Mathf.Sin(angle * Mathf.Deg2Rad);

                Vector3 vertex = new Vector3(x, y, 0f);

                if (space == Space2D.ZY)
                {
                    vertex = new Vector3(0f, y, x);
                }
                else if (space == Space2D.XZ)
                {
                    vertex = new Vector3(x, 0f, y);
                }

                vertex += this.center;

                vertices.Add(vertex);

                angle += angleStep;
            }

            return vertices;
        }
    }
}