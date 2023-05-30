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

