using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Copypasta
{
    //Like Unity's Vector3 
    [System.Serializable]
    public struct MyVector3
    {
        public float x, y, z;

        public MyVector3(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        //Don't add MyVector3(Vector3 v) because then we are back at the problem when Unity is casting Vector2->Vector3 which might introduce bugs



        //
        // Generate new vectors
        //

        public static MyVector3 Right => new(1f, 0f, 0f);
        public static MyVector3 Forward => new(0f, 0f, 1f);
        public static MyVector3 Up => new(0f, 1f, 0f);
        public static MyVector3 Zero => new(0f, 0f, 0f);
        public static MyVector3 One => new(1f, 1f, 1f);



        //
        // Properties
        //

        public MyVector3 normalized => Normalize(this);
        public double magnitude => Magnitude(this);
        public double sqrMagnitude => SqrMagnitude(this);
        public Vector3 ToVector3 => new(x, y, z);



        //
        // Vector operations
        //

        public static float Dot(MyVector3 a, MyVector3 b)
        {
            float dotProduct = (a.x * b.x) + (a.y * b.y) + (a.z * b.z);

            return dotProduct;
        }

        public static float Magnitude(MyVector3 a)
        {
            float magnitude = Mathf.Sqrt(SqrMagnitude(a));

            return magnitude;
        }

        public static float SqrMagnitude(MyVector3 a)
        {
            float sqrMagnitude = (a.x * a.x) + (a.y * a.y) + (a.z * a.z);

            return sqrMagnitude;
        }

        public static float Distance(MyVector3 a, MyVector3 b)
        {
            float distance = Magnitude(a - b);

            return distance;
        }

        public static float SqrDistance(MyVector3 a, MyVector3 b)
        {
            float distance = SqrMagnitude(a - b);

            return distance;
        }

        public static MyVector3 Normalize(MyVector3 v)
        {
            float vMagnitude = Magnitude(v);

            MyVector3 vNormalized = v * (1f / vMagnitude);

            return vNormalized;
        }

        public static MyVector3 Cross(MyVector3 a, MyVector3 b)
        {
            float x = (a.y * b.z) - (a.z * b.y);
            float y = (a.z * b.x) - (a.x * b.z);
            float z = (a.x * b.y) - (a.y * b.x);

            MyVector3 crossProduct = new(x, y, z);

            return crossProduct;
        }

        //Returns true if the given vector is exactly equal to this vector
        //Due to floating point inaccuracies, this might return false for vectors which are essentially (but not exactly) equal
        public bool Equals(MyVector3 other)
        {
            if (this.x == other.x && this.y == other.y && this.z == other.z)
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
        public bool RoughlyEquals(MyVector3 other)
        {
            float xDiff = this.x - other.x;
            float yDiff = this.y - other.y;
            float zDiff = this.z - other.z;

            float e = 0.00001f;

            //If all of the differences are around 0
            if (
                xDiff < e && xDiff > -e &&
                yDiff < e && yDiff > -e &&
                zDiff < e && zDiff > -e)
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

        public static MyVector3 operator +(MyVector3 a, MyVector3 b)
        {
            return new MyVector3(a.x + b.x, a.y + b.y, a.z + b.z);
        }

        public static MyVector3 operator -(MyVector3 a, MyVector3 b)
        {
            return new MyVector3(a.x - b.x, a.y - b.y, a.z - b.z);
        }

        public static MyVector3 operator *(MyVector3 a, float b)
        {
            return new MyVector3(a.x * b, a.y * b, a.z * b);
        }

        public static MyVector3 operator *(float b, MyVector3 a)
        {
            return new MyVector3(a.x * b, a.y * b, a.z * b);
        }

        public static MyVector3 operator -(MyVector3 a)
        {
            return a * -1f;
        }

        //== and != are not implemented because it makes it confusing if you want to take floating point precision into account
        //Use Equals or RoughlyEquals
        //Division is not implemented because its slow anyway, use vec*(1/...) if you want to divide a vector
    }
}
