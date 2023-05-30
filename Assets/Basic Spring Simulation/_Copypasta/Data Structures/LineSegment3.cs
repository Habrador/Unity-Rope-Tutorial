using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Copypasta
{
    public struct LineSegment3
    {
        public Vector3 p1, p2;

        public LineSegment3(Vector3 p1, Vector3 p2)
        {
            this.p1 = p1;
            this.p2 = p2;
        }
    }
}

