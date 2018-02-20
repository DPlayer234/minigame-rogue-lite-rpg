namespace SAE.RoguePG
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    ///     Additional or extension methods for vectors
    /// </summary>
    public static class VectorExtension
    {
        /// <summary>
        ///     Returns a new vector, with each element being the result of multiplying the
        ///     corresponding elements of <paramref name="a"/> and <paramref name="b"/>.
        /// </summary>
        /// <param name="a">The first vector</param>
        /// <param name="b">The second vector</param>
        /// <returns>A new vector as described the summary</returns>
        public static Vector2 Multiply(Vector2 a, Vector2 b)
        {
            return new Vector2(
                a.x * b.x,
                a.y * b.y);
        }

        /// <summary>
        ///     Returns a new vector, with each element being the result of multiplying the
        ///     corresponding elements of <paramref name="a"/> and <paramref name="b"/>.
        /// </summary>
        /// <param name="a">The first vector</param>
        /// <param name="b">The second vector</param>
        /// <returns>A new vector as described the summary</returns>
        public static Vector3 Multiply(Vector3 a, Vector3 b)
        {
            return new Vector3(
                a.x * b.x,
                a.y * b.y,
                a.z * b.z);
        }

        /// <summary>
        ///     Returns a new vector, with each element being the result of dividing the
        ///     corresponding elements of <paramref name="a"/> and <paramref name="b"/>.
        /// </summary>
        /// <param name="a">The first vector</param>
        /// <param name="b">The second vector</param>
        /// <returns>A new vector as described the summary</returns>
        public static Vector2 Divide(Vector2 a, Vector2 b)
        {
            return new Vector2(
                a.x / b.x,
                a.y / b.y);
        }

        /// <summary>
        ///     Returns a new vector, with each element being the result of divide the
        ///     corresponding elements of <paramref name="a"/> and <paramref name="b"/>.
        /// </summary>
        /// <param name="a">The first vector</param>
        /// <param name="b">The second vector</param>
        /// <returns>A new vector as described the summary</returns>
        public static Vector3 Divide(Vector3 a, Vector3 b)
        {
            return new Vector3(
                a.x / b.x,
                a.y / b.y,
                a.z / b.z);
        }

        /// <summary>
        ///     Returns a new vector, which equals the given <paramref name="vector"/> rotated
        ///     around the origin by the given <paramref name="euler"/> angles. Z > X > Y
        /// </summary>
        /// <param name="vector">The vector to rotate</param>
        /// <param name="euler">The rotation to apply</param>
        /// <returns>A new vector</returns>
        public static Vector3 RotateVectorAroundOrigin(Vector3 vector, Vector3 euler)
        {
            Vector3 result = VectorExtension.RotateVectorAroundOriginZ(vector, euler.z * Mathf.Deg2Rad);
            result = VectorExtension.RotateVectorAroundOriginX(result, euler.x * Mathf.Deg2Rad);
            return VectorExtension.RotateVectorAroundOriginY(result, euler.y * Mathf.Deg2Rad);
        }

        /// <summary>
        ///     Rotates the given vector by <paramref name="radX"/> radians over the x axis
        /// </summary>
        /// <param name="vector">The vector to rotate</param>
        /// <param name="radX">The rotation to apply</param>
        /// <returns>A new vector</returns>
        private static Vector3 RotateVectorAroundOriginX(Vector3 vector, float radX)
        {
            float cos = Mathf.Cos(radX);
            float sin = Mathf.Sin(radX);

            return new Vector3(
                vector.x,
                cos * vector.y - sin * vector.z,
                cos * vector.z + sin * vector.y);
        }

        /// <summary>
        ///     Rotates the given vector by <paramref name="radY"/> radians over the y axis
        /// </summary>
        /// <param name="vector">The vector to rotate</param>
        /// <param name="radY">The rotation to apply</param>
        /// <returns>A new vector</returns>
        private static Vector3 RotateVectorAroundOriginY(Vector3 vector, float radY)
        {
            float cos = Mathf.Cos(radY);
            float sin = Mathf.Sin(radY);

            return new Vector3(
                cos * vector.x + sin * vector.z,
                vector.y,
                cos * vector.z - sin * vector.x);
        }

        /// <summary>
        ///     Rotates the given vector by <paramref name="radZ"/> radians over the z axis
        /// </summary>
        /// <param name="vector">The vector to rotate</param>
        /// <param name="radZ">The rotation to apply</param>
        /// <returns>A new vector</returns>
        private static Vector3 RotateVectorAroundOriginZ(Vector3 vector, float radZ)
        {
            float cos = Mathf.Cos(radZ);
            float sin = Mathf.Sin(radZ);

            return new Vector3(
                cos * vector.x - sin * vector.y,
                cos * vector.y + sin * vector.x,
                vector.z);
        }
    }
}
