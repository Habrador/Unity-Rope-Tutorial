using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Copypasta
{
    //Like Unity's Vector2 
    [System.Serializable]
    public struct MyVector2
    {
        public float x, y;

        public MyVector2(float x, float y)
        {
            this.x = x;
            this.y = y;
        }



        //
        // Generate new vectors
        //

        public static MyVector2 Right => new(1f, 0f);
        public static MyVector2 Up => new(0f, 1f);
        public static MyVector2 Zero => new(0f, 0f);
        public static MyVector2 One => new(1f, 1f);



        //
        // Properties
        //

        public MyVector2 normalized => Normalize(this);
        public double magnitude => Magnitude(this);
        public double sqrMagnitude => SqrMagnitude(this);
        public Vector2 ToVector2 => new(x, y);



        //
        // Vector operations
        //

        //Returns true if the given vector is exactly equal to this vector
        //Due to floating point inaccuracies, this might return false for vectors which are essentially (but not exactly) equal
        public bool Equals(MyVector2 other)
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

        //Returns true if two vectors are approximately equal
        //To allow for floating point inaccuracies, the two vectors are considered equal if the magnitude of their difference is less than 1e-5
        public bool RoughlyEquals(MyVector2 b)
        {
            float xDiff = this.x - b.x;
            float yDiff = this.y - b.y;

            float e = 0.00001f;

            //If all of the differences are around 0
            if (
                xDiff < e && xDiff > -e &&
                yDiff < e && yDiff > -e)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //Dot
        public static float Dot(MyVector2 a, MyVector2 b)
        {
            float dotProduct = (a.x * b.x) + (a.y * b.y);

            return dotProduct;
        }


        //Length of vector a
        public static float Magnitude(MyVector2 a)
        {
            float magnitude = Mathf.Sqrt(SqrMagnitude(a));

            return magnitude;
        }

        //Square lenght of a vector, which doesn't involve a sqrt so faster
        public static float SqrMagnitude(MyVector2 a)
        {
            float sqrMagnitude = (a.x * a.x) + (a.y * a.y);

            return sqrMagnitude;
        }


        //The distance between two vectors
        public static float Distance(MyVector2 a, MyVector2 b)
        {
            float distance = Magnitude(a - b);

            return distance;
        }

        public static float SqrDistance(MyVector2 a, MyVector2 b)
        {
            float distance = SqrMagnitude(a - b);

            return distance;
        }


        //A vector with length 1
        public static MyVector2 Normalize(MyVector2 v)
        {
            float vMagnitude = Magnitude(v);

            MyVector2 vNormalized = v * (1f / vMagnitude);

            return vNormalized;
        }



        //
        // Operator overloads
        //

        public static MyVector2 operator +(MyVector2 a, MyVector2 b)
        {
            return new MyVector2(a.x + b.x, a.y + b.y);
        }

        public static MyVector2 operator -(MyVector2 a, MyVector2 b)
        {
            return new MyVector2(a.x - b.x, a.y - b.y);
        }

        public static MyVector2 operator *(MyVector2 a, float b)
        {
            return new MyVector2(a.x * b, a.y * b);
        }

        public static MyVector2 operator *(float b, MyVector2 a)
        {
            return new MyVector2(a.x * b, a.y * b);
        }

        public static MyVector2 operator -(MyVector2 a)
        {
            return a * -1f;
        }

        //== and != are not implemented because it makes it confusing if you want to take floating point precision into account
        //Use Equals or RoughlyEquals
        //Division is not implemented because its slow anyway, use vec*(1/...) if you want to divide a vector
    }
}
