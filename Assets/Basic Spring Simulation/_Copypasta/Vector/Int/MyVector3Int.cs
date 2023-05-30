using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Copypasta
{
    public struct MyVector3Int
    {
        public int x, y, z;

        public MyVector3Int(int x, int y, int z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }



        //
        // Generate new vectors
        //

        public static MyVector3Int Zero => new(0, 0, 0);
        public static MyVector3Int One => new(1, 1, 1);



        //
        // Properties
        //

        public Vector3Int ToVector3Int => new(x, y, z);



        //Returns true if the given vector is exactly equal to this vector
        public bool Equals(MyVector3Int other)
        {
            if (this.x == other.x && this.y == other.y)
            {
                return true;
            }
            else
            {
                return false;
            }
        }




        //
        // Operator overloads
        //

        public static MyVector3Int operator *(MyVector3Int vec, int a)
        {
            return new(vec.x * a, vec.y * a, vec.z * a);
        }

        public static MyVector3Int operator *(int a, MyVector3Int vec)
        {
            return new(vec.x * a, vec.y * a, vec.z * a);
        }

        public static MyVector3Int operator +(MyVector3Int vecA, MyVector3Int vecB)
        {
            return new(vecA.x + vecB.x, vecA.y + vecB.y, vecA.z + vecB.z);
        }

        public static MyVector3Int operator -(MyVector3Int vecA, MyVector3Int vecB)
        {
            return new(vecA.x - vecB.x, vecA.y - vecB.y, vecA.z - vecB.z);
        }

        public static MyVector3Int operator -(MyVector3Int a)
        {
            return a * -1;
        }
    }
}