//
// (c) 2016 Digital Ruby, LLC
// http://www.digitalruby.com
// Code may not be redistributed in source form!
// Using this code in commercial games and apps is fine.
//

// if not using Unity, comment this out
#define UNITY

// if square root performance is a problem, you may try one of the other defines here, and leave the other two commented out
#define USE_MATH_SQUARE_ROOT
// #define USE_QUAKE_SQUARE_ROOT
// #define USE_FASTER_SQUARE_ROOT

// references for curves, splines and math that were invaluable in creating this code:
// http://stackoverflow.com/questions/9489736/catmull-rom-curve-with-no-cusps-and-no-self-intersections
// http://www.technologicalutopia.com/sourcecode/xnageometry/tableofcontents.htm
// http://blog.avangardo.com/2010/10/c-implementation-of-bezier-curvature-calculation/
// https://github.com/mono/sysdrawing-coregraphics/blob/master/System.Drawing.Drawing2D/GraphicsPath.cs
// https://github.com/retuxx/tinyspline
// http://www.codeproject.com/Articles/996281/NURBS-curve-made-easy
// http://catlikecoding.com/unity/tutorials/curves-and-splines/
// https://en.wikipedia.org/wiki/Fast_inverse_square_root
// http://blog.wouldbetheologian.com/2011/11/fast-approximate-sqrt-method-in-c.html

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace DigitalRubyShared
{
#if UNITY

    using UnityEngine;

#endif

    /// <summary>
    /// Utility class to assist in generating curves, splines, etc.
    /// </summary>
    public static class PathGenerator
    {

#if !UNITY

        public struct Vector3
        {
            public float x;
            public float y;
            public float z;

            public Vector3(float x, float y, float z)
            {
                this.x = x;
                this.y = y;
                this.z = z;
            }
        }

        public struct Vector4
        {
            public float x;
            public float y;
            public float z;
            public float w;

            public Vector4(float x, float y, float z, float w)
            {
                this.x = x;
                this.y = y;
                this.z = z;
                this.w = w;
            }
        }

#endif

#if !USE_MATH_SQUARE_ROOT

        [StructLayout(LayoutKind.Explicit)]
        private struct FloatIntUnion
        {
            [FieldOffset(0)]
            public float f;

            [FieldOffset(0)]
            public int tmp;
        }

#endif

        /// <summary>
        /// Whether paths are working in 2D or 3D space
        /// </summary>
        public static bool Is2D;

        /// <summary>
        /// Adjust how curves are calculated. This value should be positive. Generally leave as is unless you want some gnarly effects.
        /// </summary>
        private const float curveMultiplier = 3.0f;

        /// <summary>
        /// Adjusts how the spline curves. This value should be negative. Generally leave as is unless you want some gnarly effects.
        /// </summary>
        private const float splineMultiplier1 = -3.0f;

        /// <summary>
        /// Adjusts how the spline curves. This value should be positive. Generally leave as is unless you want some gnarly effects.
        /// </summary>
        private const float splineMultiplier2 = 3.0f;

        /// <summary>
        /// Adjusts how the spline curves. This value should be positive. Generally leave as is unless you want some gnarly effects.
        /// </summary>
        private const float splineMultiplier3 = 2.0f;

        /// <summary>
        /// In the event that the second distance of a spline is too small, force it to be this value
        /// </summary>
        private const float splineDistanceClamp = 1.0f;

        /// <summary>
        /// System.Math doesn't provide epsilon
        /// </summary>
        private const float splineEpsilon = 0.0001f;

        private static float SquareRoot(float x)
        {

#if USE_MATH_SQUARE_ROOT

            return (float)Math.Sqrt(x);

#elif USE_QUAKE_SQUARE_ROOT
            
            // the famous Quake square root hack
            FloatIntUnion u;
            u.tmp = 0;
            float xhalf = 0.5f * x;
            u.f = x;
            u.tmp = 0x5f375a86 - (u.tmp >> 1);
            u.f = u.f * (1.5f - xhalf * u.f * u.f);
            return u.f * x;
            
#else

            // a slightly faster but less accurate method
            FloatIntUnion u;
            u.tmp = 0;
            u.f = x;
            u.tmp -= 1 << 23; /* Subtract 2^m. */
            u.tmp >>= 1; /* Divide by 2. */
            u.tmp += 1 << 29; /* Add ((b + 1) / 2) * 2^m. */
            return u.f;

#endif

        }

        private static float Distance2D(ref Vector3 point1, ref Vector3 point2)
        {
            float deltaX = point2.x - point1.x;
            float deltaY = point2.y - point1.y;

            return SquareRoot((deltaX * deltaX) + (deltaY * deltaY));
        }

        private static float Distance3D(ref Vector3 point1, ref Vector3 point2)
        {
            float deltaX = point2.x - point1.x;
            float deltaY = point2.y - point1.y;
            float deltaZ = point2.z - point1.z;

            return SquareRoot((deltaX * deltaX) + (deltaY * deltaY) + (deltaZ * deltaZ));
        }

        private static void GetCurvePoint2D(ref Vector3 start, ref Vector3 end, ref Vector3 ctr1, ref Vector3 ctr2, float percent, out Vector3 point)
        {
            float percentSquared = percent * percent;
            float percentCubed = percentSquared * percent;
            float p1x = curveMultiplier * (ctr1.x - start.x);
            float p1y = curveMultiplier * (ctr1.y - start.y);
            float p2x = curveMultiplier * (ctr2.x - ctr1.x) - p1x;
            float p2y = curveMultiplier * (ctr2.y - ctr1.y) - p1y;
            float p3x = end.x - start.x - p1x - p2x;
            float p3y = end.y - start.y - p1y - p2y;
            float finalX = start.x + (p1x * percent) + (p2x * percentSquared) + (p3x * percentCubed);
            float finalY = start.y + (p1y * percent) + (p2y * percentSquared) + (p3y * percentCubed);

            point = new Vector3(finalX, finalY, 0.0f);
        }

        private static void GetCurvePoint3D(ref Vector3 start, ref Vector3 end, ref Vector3 ctr1, ref Vector3 ctr2, float percent, out Vector3 point)
        {
            float percentSquared = percent * percent;
            float percentCubed = percentSquared * percent;
            float p1x = (ctr1.x - start.x) * curveMultiplier;
            float p1y = (ctr1.y - start.y) * curveMultiplier;
            float p1z = (ctr1.z - start.z) * curveMultiplier;
            float p2x = ((ctr2.x - ctr1.x) * curveMultiplier) - p1x;
            float p2y = ((ctr2.y - ctr1.y) * curveMultiplier) - p1y;
            float p2z = ((ctr2.z - ctr1.z) * curveMultiplier) - p1z;
            float p3x = end.x - start.x - p1x - p2x;
            float p3y = end.y - start.y - p1y - p2y;
            float p3z = end.z - start.z - p1z - p2z;
            float finalX = start.x + (p1x * percent) + (p2x * percentSquared) + (p3x * percentCubed);
            float finalY = start.y + (p1y * percent) + (p2y * percentSquared) + (p3y * percentCubed);
            float finalZ = start.z + (p1z * percent) + (p2z * percentSquared) + (p3z * percentCubed);

            point = new Vector3(finalX, finalY, finalZ);
        }

        private static void CalculateSplinePoint(float p1, float p2, float p3, float p4, float distance1, float distance2, float distance3, out Vector4 point)
        {
            // lots of math here - look up catmull spline (it's in the links at the top of this source file) if you are confused

            float t1 = ((p2 - p1) / distance1 - (p3 - p1) / (distance1 + distance2) + (p3 - p2) / distance2) * distance2;
            float t2 = ((p3 - p2) / distance2 - (p4 - p2) / (distance2 + distance3) + (p4 - p3) / distance3) * distance2;

            point = new Vector4
            (
                p2,
                t1,
                (splineMultiplier1 * p2) + (splineMultiplier2 * p3) - (t2 + (splineMultiplier3 * t1)),
                (t1 + t2) + ((splineMultiplier3 * p2) - (splineMultiplier3 * p3))
            );
        }

        private static float CalculatePolynomial(ref Vector4 point, float t)
        {
            // polynomial is w*t^3+z*t^2+y*t+x
            float tSquared = t * t;
            float tCubed = tSquared * t;

            return (point.w * tCubed) + (point.z * tSquared) + (point.y * t) + point.x;
        }

        private static void ClampSplineDistances(ref float distance1, ref float distance2, ref float distance3)
        {
            // safety check for repeated points
            if (distance2 < splineEpsilon)
            {
                distance2 = splineDistanceClamp;
            }
            if (distance1 < splineEpsilon)
            {
                distance1 = distance2;
            }
            if (distance3 < splineEpsilon)
            {
                distance3 = distance2;
            }
        }

        private static void GetSplinePoint2D(ref Vector3 point1, ref Vector3 point2, ref Vector3 point3, ref Vector3 point4, float t, out Vector3 point)
        {
            float distance1 = Distance2D(ref point1, ref point2);
            float distance2 = Distance2D(ref point2, ref point3);
            float distance3 = Distance2D(ref point3, ref point4);

            ClampSplineDistances(ref distance1, ref distance2, ref distance3);

            // plot the spline on the x and y axis
            Vector4 resultPoint1;
            CalculateSplinePoint(point1.x, point2.x, point3.x, point4.x, distance1, distance2, distance3, out resultPoint1);
            Vector4 resultPoint2;
            CalculateSplinePoint(point1.y, point2.y, point3.y, point4.y, distance1, distance2, distance3, out resultPoint2);

            point = new Vector3(CalculatePolynomial(ref resultPoint1, t), CalculatePolynomial(ref resultPoint2, t), 0.0f);
        }

        private static void GetSplinePoint3D(ref Vector3 point1, ref Vector3 point2, ref Vector3 point3, ref Vector3 point4, float t, out Vector3 point)
        {
            float distance1 = Distance3D(ref point1, ref point2);
            float distance2 = Distance3D(ref point2, ref point3);
            float distance3 = Distance3D(ref point3, ref point4);

            ClampSplineDistances(ref distance1, ref distance2, ref distance3);
            
            // plot the spline on the x, y and z axis
            Vector4 resultPoint1;
            CalculateSplinePoint(point1.x, point2.x, point3.x, point4.x, distance1, distance2, distance3, out resultPoint1);
            Vector4 resultPoint2;
            CalculateSplinePoint(point1.y, point2.y, point3.y, point4.y, distance1, distance2, distance3, out resultPoint2);
            Vector4 resultPoint3;
            CalculateSplinePoint(point1.z, point2.z, point3.z, point4.z, distance1, distance2, distance3, out resultPoint3);

            point = new Vector3(CalculatePolynomial(ref resultPoint1, t), CalculatePolynomial(ref resultPoint2, t), CalculatePolynomial(ref resultPoint3, t));
        }

        /// <summary>
        /// Create a quad/bezier curve that curves using two control points from start to end.
        /// </summary>
        /// <param name="path">Receives the path points. Collection is not cleared.</param>
        /// <param name="start">Start point</param>
        /// <param name="end">End point</param>
        /// <param name="ctr1">Control point 1</param>
        /// <param name="ctr2">Control point 2</param>
        /// <param name="numberOfSegments">Number of segments. The higher this number, the more points are sampled.</param>
        /// <param name="startPercent">Start percent. This can be the return value of a previous call to maintain a smooth path.</param>
        /// <returns>Leftover percent. This can be passed back in to startPercent to continue a path smoothly.</returns>
        public static float CreateCurve(ICollection<Vector3> path, Vector3 start, Vector3 end, Vector3 ctr1, Vector3 ctr2, int numberOfSegments, float startPercent)
        {
            // make sure number of segments isn't too large or small
            numberOfSegments = Math.Min(1024, Math.Max(numberOfSegments, 4));
            float percent;
            float percentIncrement = 1.0f / (float)(numberOfSegments + 1);
            Vector3 point;

            if (Is2D)
            {
                for (percent = startPercent; percent <= 1.0f; percent += percentIncrement)
                {
                    GetCurvePoint2D(ref start, ref end, ref ctr1, ref ctr2, percent, out point);
                    path.Add(point);
                }
            }
            else
            {
                for (percent = startPercent; percent <= 1.0f; percent += percentIncrement)
                {
                    GetCurvePoint3D(ref start, ref end, ref ctr1, ref ctr2, percent, out point);
                    path.Add(point);
                }
            }

            return (percent - 1.0f);
        }

        /// <summary>
        /// Creates a spline path that curves around points in a list.
        /// </summary>
        /// <param name="path">Receives the path points. Collection is not cleared.</param>
        /// <param name="points">Points for the spline to follow. Must contain at least 4 points.</param>
        /// <param name="numberOfSegments">Total number of line segments for the spline. The higher this number, the more points are sampled.</param>
        /// <param name="closePath">True to loop back to the start, false otherwise</param>
        /// <returns>True if success, false if points length is too small</returns>
        public static bool CreateSpline(ICollection<Vector3> path, IList<Vector3> points, int numberOfSegments, bool closePath)
        {
            if (points.Count < 4)
            {
                return false;
            }

            // make sure number of segments isn't too large or small
            numberOfSegments = Math.Min(1024, Math.Max(numberOfSegments, 4));
            int numberOfPoints = (closePath ? points.Count : points.Count - 1);
            int previousPointIndex, currentPointIndex, nextPointIndex, nextNextPointIndex;
            int closePathInt = (closePath ? 1 : 0);
            float percent;
            float percentIncrement = (1.0f / (float)numberOfSegments) * (float)numberOfPoints;
            float start = 0.0f;
            Vector3 point, previousPoint, currentPoint, nextPoint, nextNextPoint;

            for (currentPointIndex = 0; currentPointIndex < numberOfPoints; currentPointIndex++)
            {
                // we will be using 4 points - the previous point, current point, next point and point after next
                // the points can wrap around back to the start of the list
                previousPointIndex = (currentPointIndex == 0 ? (numberOfPoints - closePathInt) : (currentPointIndex - 1));
                nextPointIndex = currentPointIndex + 1;
                nextNextPointIndex = currentPointIndex + 2;

                if (closePath && nextPointIndex > numberOfPoints - 1)
                {
                    nextPointIndex -= numberOfPoints;
                }
                if (nextNextPointIndex > numberOfPoints - 1)
                {
                    nextNextPointIndex = (closePath ? nextNextPointIndex - numberOfPoints : numberOfPoints);
                }

                // assign the points - these will be passed by ref for performance
                previousPoint = points[previousPointIndex];
                currentPoint = points[currentPointIndex];
                nextPoint = points[nextPointIndex];
                nextNextPoint = points[nextNextPointIndex];

                if (Is2D)
                {
                    // loop through and spline the points
                    for (percent = start; percent <= 1.0f; percent += percentIncrement)
                    {
                        GetSplinePoint2D(ref previousPoint, ref currentPoint, ref nextPoint, ref nextNextPoint, percent, out point);
                        path.Add(point);
                    }
                }
                else
                {
                    // loop through and spline the points
                    for (percent = start; percent <= 1.0f; percent += percentIncrement)
                    {
                        GetSplinePoint3D(ref previousPoint, ref currentPoint, ref nextPoint, ref nextNextPoint, percent, out point);
                        path.Add(point);
                    }
                }

                // reset start, keeping any additional amount over 1.0f to maintain a smooth curve
                start = percent - 1.0f;
            }

            return true;
        }
    }
}
