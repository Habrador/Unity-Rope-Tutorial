using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Copypasta
{
    //Like Unity's Vector2 but with doubles instead of float for greater accuracy 
    [System.Serializable]
    public struct MyVector2Double
    {
        public double x, y;

        public MyVector2Double(double x, double y)
        {
            this.x = x;
            this.y = y;
        }



        //
        // Generate new vectors
        //

        public static MyVector2Double Right => new(1, 0);
        public static MyVector2Double Up => new(0, 1);
        public static MyVector2Double Zero => new(0, 0);
        public static MyVector2Double One => new(1, 1);



        //
        // Properties
        //

        public MyVector2Double normalized => Normalize(this);
        public double magnitude => Magnitude(this);
        public double sqrMagnitude => SqrMagnitude(this);
        public Vector2 ToVector2 => new((float)x, (float)y);



        //
        // Vector operations
        //

        public static double Dot(MyVector2Double a, MyVector2Double b)
        {
            double dotProduct = (a.x * b.x) + (a.y * b.y);

            return dotProduct;
        }

        public static double Magnitude(MyVector2Double a)
        {
            double magnitude = System.Math.Sqrt(SqrMagnitude(a));

            return magnitude;
        }

        public static double SqrMagnitude(MyVector2Double a)
        {
            double sqrMagnitude = (a.x * a.x) + (a.y * a.y);

            return sqrMagnitude;
        }

        public static double Distance(MyVector2Double a, MyVector2Double b)
        {
            double distance = Magnitude(a - b);

            return distance;
        }

        public static double SqrDistance(MyVector2Double a, MyVector2Double b)
        {
            double distance = SqrMagnitude(a - b);

            return distance;
        }

        public static MyVector2Double Normalize(MyVector2Double v)
        {
            double vMagnitude = Magnitude(v);

            MyVector2Double vNormalized = v * (1.0 / vMagnitude);

            return vNormalized;
        }

        //Returns true if the given vector is exactly equal to this vector
        //Due to floating point inaccuracies, this might return false for vectors which are essentially (but not exactly) equal
        public bool Equals(MyVector2Double other)
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
        public bool RoughlyEquals(MyVector2Double other)
        {
            double xDiff = this.x - other.x;
            double yDiff = this.y - other.y;

            double e = 0.00001;

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

        

        //
        // Operator overloads
        //

        public static MyVector2Double operator *(MyVector2Double vec, double a)
        {
            return new(vec.x * a, vec.y * a);
        }

        public static MyVector2Double operator *(double a, MyVector2Double vec)
        {
            return new(vec.x * a, vec.y * a);
        }

        public static MyVector2Double operator +(MyVector2Double vecA, MyVector2Double vecB)
        {
            return new(vecA.x + vecB.x, vecA.y + vecB.y);
        }

        public static MyVector2Double operator -(MyVector2Double vecA, MyVector2Double vecB)
        {
            return new(vecA.x - vecB.x, vecA.y - vecB.y);
        }

        public static MyVector2Double operator -(MyVector2Double a)
        {
            return a * -1.0;
        }

        //== and != are not implemented because it makes it confusing if you want to take floating point precision into account
        //Use Equals or RoughlyEquals
        //Division is not implemented because its slow anyway, use vec*(1/...) if you want to divide a vector
    }
}
