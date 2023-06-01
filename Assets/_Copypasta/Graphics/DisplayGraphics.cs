using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Copypasta
{
    public class DisplayGraphics
    {
        //
        // Draw a mesh
        //

        public static void DisplayMesh(Mesh mesh, Materials.ColorOptions color)
        {
            Material material = Materials.GetMaterial(color);

            DisplayMesh(mesh, material);
        }

        public static void DisplayMesh(Mesh mesh, Material material)
        {
            Graphics.DrawMesh(mesh, Vector3.zero, Quaternion.identity, material, 0, Camera.main, 0);
        }



        //
        // Draw a circle 
        //

        //The circle can become a triangle if you lower resolution!
        public static void DisplayCircle(Circle circle, Materials.ColorOptions color, Circle.Space2D space, int resolution = 100)
        {
            Mesh m = GenerateDisplayCircleMesh(circle, space, resolution);

            DisplayMesh(m, color);
        }


        //Draw 3 circles in each space
        public static void DisplayCircle3D(Circle circle, Materials.ColorOptions color, int resolution = 100)
        {
            DisplayGraphics.DisplayCircle(circle, color, Circle.Space2D.XY, resolution);
            DisplayGraphics.DisplayCircle(circle, color, Circle.Space2D.XZ, resolution);
            DisplayGraphics.DisplayCircle(circle, color, Circle.Space2D.ZY, resolution);
        }


        public static Mesh GenerateDisplayCircleMesh(Circle circle, Circle.Space2D space, int resolution = 100)
        {
            List<Vector3> vertices = circle.GetCircleVertices(space, resolution);

            Mesh m = GenerateDisplayLineMesh(vertices);

            return m;
        }



        //
        // Draw a line consisting of several vertices
        //
        
        //If they line consists of A-B-C-D, the vertices are A,B,C,D
        public static void DisplayLine(List<Vector3> vertices, Materials.ColorOptions color)
        {
            Mesh m = GenerateDisplayLineMesh(vertices);

            DisplayMesh(m, color);
        }


        public static Mesh GenerateDisplayLineMesh(List<Vector3> vertices)
        {
            if (vertices.Count < 2)
            {
                return null;
            }

            //Generate the indices
            List<int> indices = new();

            for (int i = 0; i < vertices.Count; i++)
            {
                indices.Add(i);
            }

            //Generate the mesh
            Mesh m = new();

            m.SetVertices(vertices);
            m.SetIndices(indices, MeshTopology.LineStrip, 0);

            return m;
        }



        //
        // Draw a thicc line 
        //

        public static void DisplayThiccLine(List<Vector3> lineCoordinates, float lineRadius, int lineResolution, Materials.ColorOptions color)
        {
            Mesh m = GenerateThiccLineMesh(lineCoordinates, lineRadius, lineResolution);

            DisplayMesh(m, color);
        }


        public static void DisplayThiccLine(List<Vector3> lineCoordinates, float lineRadius, int lineResolution, Material material)
        {
            Mesh m = GenerateThiccLineMesh(lineCoordinates, lineRadius, lineResolution);

            DisplayMesh(m, material);
        }


        //Extrude a circle mesh along a line to make a thicc line
        public static Mesh GenerateThiccLineMesh(List<Vector3> lineCoordinates, float lineRadius, int lineResolution)
        {
            //Generate mesh vertices at point 1 in a circle aligned along the first line segment

            //Find a point on the circle which is a normal to the first line segment
            Vector3 a = lineCoordinates[0];
            Vector3 b = lineCoordinates[1];

            Vector3 lineDir = (b - a).normalized;

            Vector3 normal = Vector3.Cross(lineDir, Vector3.up).normalized;

            Vector3 p0 = normal * lineRadius;


            //Find the other vertices by rotating p0 around the line segment to get a full circle with some resolution
            List<Vector3> vertices = new();

            float angleStep = 360f / (float)lineResolution;

            float angle = 0f;

            //Rotate p0 around the line segment to get a full circle with some resolution
            for (int i = 0; i < lineResolution; i++)
            {
                Vector3 p = Quaternion.AngleAxis(angle, lineDir) * p0;

                p += a;

                vertices.Add(p);

                angle += angleStep;
            }


            //Add the rest of the vertices by repeating the process
            //Using the line direction to move the vertices along the line is NOT working
            for (int i = 1; i < lineCoordinates.Count; i++)
            {
                Vector3 a2 = lineCoordinates[i - 1];
                Vector3 b2 = lineCoordinates[i];

                Vector3 lineDir2 = (b2 - a2).normalized;

                Vector3 normal2 = Vector3.Cross(lineDir2, Vector3.up).normalized;

                Vector3 p02 = normal2 * lineRadius;


                float angle2 = 0f;

                //Rotate p0 around the line segment to get a full circle with some resolution
                for (int j = 0; j < lineResolution; j++)
                {
                    Vector3 p = Quaternion.AngleAxis(angle2, lineDir2) * p02;

                    p += b2;

                    vertices.Add(p);

                    angle2 += angleStep;
                }

                //break;
            }

            //Copypasta.DisplayGraphics.DisplayLine(vertices, Copypasta.Materials.ColorOptions.Orange);

            //Generate the mesh triangles
            List<int> triangles = new();

            for (int i = lineResolution; i < vertices.Count; i += lineResolution)
            {
                for (int j = 0; j < lineResolution; j++)
                {
                    int thisVertex = i + j;
                    //The corresponding vertex on the previous circle
                    int thisVertexPreviousCircle = thisVertex - lineResolution;

                    int thisVertexPrevJIndex = j - 1;
                    if (thisVertexPrevJIndex < 0)
                    {
                        thisVertexPrevJIndex = lineResolution - 1;
                    }

                    //The vertex coming before this vertex
                    int thisVertexPrev = thisVertexPrevJIndex + i;

                    //The corresponding previous vertex on the previous circle
                    int thisVertexPrevPreviousCircle = thisVertexPrev - lineResolution;

                    //Build the triangles
                    int aIndex = thisVertexPrevPreviousCircle;
                    int bIndex = thisVertexPrev;
                    int cIndex = thisVertexPreviousCircle;
                    int dIndex = thisVertex;

                    triangles.Add(bIndex);
                    triangles.Add(aIndex);
                    triangles.Add(dIndex);

                    triangles.Add(dIndex);
                    triangles.Add(aIndex);
                    triangles.Add(cIndex);
                }
            }


            //Make a mesh
            Mesh m = new();

            m.SetVertices(vertices);
            m.SetTriangles(triangles, 0);

            m.RecalculateNormals();

            return m;
        }


        //
        // Draw vertices
        //

        public static void DisplayVertices(List<Vector3> vertices, Materials.ColorOptions color)
        {
            Mesh m = GenerateDisplayVerticesMesh(vertices);

            DisplayMesh(m, color);
        }


        public static Mesh GenerateDisplayVerticesMesh(List<Vector3> vertices)
        {
            //Generate the indices
            List<int> indices = new();

            for (int i = 0; i < vertices.Count; i++)
            {
                indices.Add(i);
            }

            //Generate the mesh
            Mesh m = new();

            m.SetVertices(vertices);
            m.SetIndices(indices, MeshTopology.Points, 0);

            return m;
        }



        //
        // Draw line segments that are not necessarily connected
        //
        
        //If they line consists of A-B-C-D, the vertices are A, B, B, C, C, D
        public static void DisplayLineSegments(List<LineSegment3> lineSegments, Materials.ColorOptions color)
        {
            Mesh m = GenerateDisplayLineSegmentsMesh(lineSegments);

            DisplayMesh(m, color);
        }


        public static Mesh GenerateDisplayLineSegmentsMesh(List<LineSegment3> lineSegments)
        {
            //Generate the vertices
            List<Vector3> vertices = new();

            for (int i = 0; i < lineSegments.Count; i++)
            {
                vertices.Add(lineSegments[i].p1);
                vertices.Add(lineSegments[i].p2);
            }

            //Generate the indices
            List<int> indices = new();

            for (int i = 0; i < vertices.Count; i++)
            {
                indices.Add(i);
            }

            //Generate the mesh
            Mesh m = new();

            m.SetVertices(vertices);
            m.SetIndices(indices, MeshTopology.Lines, 0);

            return m;
        }
    }
}

