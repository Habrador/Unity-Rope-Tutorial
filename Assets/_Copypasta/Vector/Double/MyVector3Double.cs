using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Copypasta
{
    //Like Unity's Vector3 but with doubles instead of float for greater accuracy 
    [System.Serializable]
    public struct MyVector3Double
    {
        public double x, y, z;

        public MyVector3Double(double x, double y, double z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }



        //
        // Generate new vectors
        //

        public static MyVector3Double Right => new(1, 0, 0);
        public static MyVector3Double Forward => new(0, 0, 1);
        public static MyVector3Double Up => new(0, 1, 0);
        public static MyVector3Double Zero => new(0, 0, 0);
        public static MyVector3Double One => new(1, 1, 1);



        //
        // Properties
        //

        public MyVector3Double normalized => Normalize(this);
        public double magnitude => Magnitude(this);
        public double sqrMagnitude => SqrMagnitude(this);
        public Vector3 ToVector3 => new((float)x, (float)y, (float)z);



        //
        // Vector operations
        //

        public static double Dot(MyVector3Double a, MyVector3Double b)
        {
            double dotProduct = (a.x * b.x) + (a.y * b.y) + (a.z * b.z);

            return dotProduct;
        }

        public static double Magnitude(MyVector3Double a)
        {
            double magnitude = System.Math.Sqrt(SqrMagnitude(a));

            return magnitude;
        }

        public static double SqrMagnitude(MyVector3Double a)
        {
            double sqrMagnitude = (a.x * a.x) + (a.y * a.y) + (a.z * a.z);

            return sqrMagnitude;
        }

        public static double Distance(MyVector3Double a, MyVector3Double b)
        {
            double distance = Magnitude(a - b);

            return distance;
        }

        public static double SqrDistance(MyVector3Double a, MyVector3Double b)
        {
            double distance = SqrMagnitude(a - b);

            return distance;
        }

        public static MyVector3Double Normalize(MyVector3Double v)
        {
            double vMagnitude = Magnitude(v);

            MyVector3Double vNormalized = v * (1.0 / vMagnitude);

            return vNormalized;
        }

        public static MyVector3Double Cross(MyVector3Double a, MyVector3Double b)
        {
            double x = (a.y * b.z) - (a.z * b.y);
            double y = (a.z * b.x) - (a.x * b.z);
            double z = (a.x * b.y) - (a.y * b.x);

            MyVector3Double crossProduct = new(x, y, z);

            return crossProduct;
        }

        //Returns true if the given vector is exactly equal to this vector
        //Due to floating point inaccuracies, this might return false for vectors which are essentially (but not exactly) equal
        public bool Equals(MyVector3Double other)
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
        public bool RoughlyEquals(MyVector3Double other)
        {
            double xDiff = this.x - other.x;
            double yDiff = this.y - other.y;
            double zDiff = this.z - other.z;

            double e = 0.00001;

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

        public static MyVector3Double operator *(MyVector3Double vec, double a)
        {
            return new(vec.x * a, vec.y * a, vec.z * a);
        }

        public static MyVector3Double operator *(double a, MyVector3Double vec)
        {
            return new(vec.x * a, vec.y * a, vec.z * a);
        }

        public static MyVector3Double operator +(MyVector3Double vecA, MyVector3Double vecB)
        {
            return new(vecA.x + vecB.x, vecA.y + vecB.y, vecA.z + vecB.z);
        }

        public static MyVector3Double operator -(MyVector3Double vecA, MyVector3Double vecB)
        {
            return new(vecA.x - vecB.x, vecA.y - vecB.y, vecA.z - vecB.z);
        }

        public static MyVector3Double operator -(MyVector3Double a)
        {
            return a * -1.0;
        }

        //== and != are not implemented because it makes it confusing if you want to take floating point precision into account
        //Use Equals or RoughlyEquals
        //Division is not implemented because its slow anyway, use vec*(1/...) if you want to divide a vector
    }
}
