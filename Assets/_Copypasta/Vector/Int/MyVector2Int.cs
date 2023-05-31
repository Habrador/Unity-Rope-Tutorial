using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Copypasta
{
    public class MyVector2Int
    {
        public int x, y;

        public MyVector2Int(int x, int y)
        {
            this.x = x;
            this.y = y;
        }



        //
        // Generate new vectors
        //

        public static MyVector2Int Zero => new(0, 0);
        public static MyVector2Int One => new(1, 1);



        //
        // Properties
        //

        public Vector2Int ToVector2Int => new(x, y);



        //Returns true if the given vector is exactly equal to this vector
        public bool Equals(MyVector2Int other)
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

        public static MyVector2Int operator *(MyVector2Int vec, int a)
        {
            return new(vec.x * a, vec.y * a);
        }

        public static MyVector2Int operator *(int a, MyVector2Int vec)
        {
            return new(vec.x * a, vec.y * a);
        }

        public static MyVector2Int operator +(MyVector2Int vecA, MyVector2Int vecB)
        {
            return new(vecA.x + vecB.x, vecA.y + vecB.y);
        }

        public static MyVector2Int operator -(MyVector2Int vecA, MyVector2Int vecB)
        {
            return new(vecA.x - vecB.x, vecA.y - vecB.y);
        }

        public static MyVector2Int operator -(MyVector2Int a)
        {
            return a * -1;
        }
    }
}