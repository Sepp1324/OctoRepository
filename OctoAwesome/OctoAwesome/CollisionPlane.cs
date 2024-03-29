﻿using System.Collections.Generic;
using engenious;

namespace OctoAwesome
{
    /// <summary>
    ///     Stellt eine Fläche dar, welche mit anderen Flächen Kollidieren kann
    /// </summary>
    public struct CollisionPlane
    {
        /// <summary>
        ///     Normalenvector der Fläche
        /// </summary>
        public Vector3 normal;

        /// <summary>
        ///     Mittelpunkt der Fläche
        /// </summary>
        public Vector3 pos;

        /// <summary>
        ///     Erste Ecke der Fläche
        /// </summary>
        public Vector3 edgepos1;

        /// <summary>
        ///     Zweite Ecke der Fläche
        /// </summary>
        public Vector3 edgepos2;

        /// <summary>
        ///     Konstruktur
        /// </summary>
        /// <param name="pos1">Ecke 1</param>
        /// <param name="pos2">Ecke 2</param>
        /// <param name="normal">Normalenvektor</param>
        public CollisionPlane(Vector3 pos1, Vector3 pos2, Vector3 normal)
        {
            this.normal = normal;
            edgepos1 = pos1;
            edgepos2 = pos2;

            pos = (pos2 - pos1) / 2f + pos1;
        }

        /// <summary>
        ///     Gibt alle möglichen Flächen eines Blockes zurück
        /// </summary>
        /// <param name="pos">Position des Blockes</param>
        /// <param name="movevector">Bewegungsvector</param>
        /// <returns>Liste aller beteiligten Flächen</returns>
        public static IEnumerable<CollisionPlane> GetBlockCollisionPlanes(Index3 pos, Vector3 movevector)
        {
            switch (movevector.X)
            {
                //Ebene X
                case > 0:
                    yield return new(new(pos.X, pos.Y, pos.Z), new(pos.X, pos.Y + 1f, pos.Z + 1f), new(-1, 0));
                    break;
                case < 0:
                    yield return new(new(pos.X + 1f, pos.Y, pos.Z), new(pos.X + 1f, pos.Y + 1f, pos.Z + 1f), new(1, 0));
                    break;
            }

            switch (movevector.Y)
            {
                //Ebene Y
                case > 0:
                    yield return new(new(pos.X, pos.Y, pos.Z), new(pos.X + 1f, pos.Y, pos.Z + 1f), new(0, -1));
                    break;
                case < 0:
                    yield return new(new(pos.X, pos.Y + 1f, pos.Z), new(pos.X + 1f, pos.Y + 1f, pos.Z + 1f), new(0, 1));
                    break;
            }

            switch (movevector.Z)
            {
                //Ebene Z
                case > 0:
                    yield return new(new(pos.X, pos.Y, pos.Z), new(pos.X + 1f, pos.Y + 1f, pos.Z), new(0, 0, -1));
                    break;
                case < 0:
                    yield return new(new(pos.X, pos.Y, pos.Z + 1f), new(pos.X + 1f, pos.Y + 1f, pos.Z + 1f),
                        new(0, 0, 1));
                    break;
            }
        }

        /// <summary>
        ///     Gibt alle Flächen eines Spielers zurück
        /// </summary>
        /// <param name="radius">radius of the <see cref="Entity" /></param>
        /// <param name="height">height of the <see cref="Entity" /></param>
        /// <param name="velocity">velocity of the <see cref="Entity" /></param>
        /// <param name="coordinate"><see cref="Coordinate" /> ot the <see cref="Entity" /></param>
        /// <param name="invertVelocity">Gibt an ob die geschwindigkeit invertiert werden soll</param>
        /// <returns></returns>
        public static IEnumerable<CollisionPlane> GetEntityCollisionPlanes(float radius, float height, Vector3 velocity, Coordinate coordinate, bool invertVelocity = true)
        {
            var pos = coordinate.BlockPosition;
            var vel = invertVelocity ? new(-velocity.X, -velocity.Y, -velocity.Z) : velocity;

            switch (vel.X)
            {
                //Ebene X
                case > 0:
                    yield return new(new(pos.X - radius, pos.Y - radius, pos.Z), new(pos.X - radius, pos.Y + radius, pos.Z + height), new(-1, 0));
                    break;
                case < 0:
                    yield return new(new(pos.X + radius, pos.Y - radius, pos.Z), new(pos.X + radius, pos.Y + radius, pos.Z + height), new(1, 0));
                    break;
            }

            switch (vel.Y)
            {
                //Ebene Y
                case > 0:
                    yield return new(new(pos.X - radius, pos.Y - radius, pos.Z), new(pos.X + radius, pos.Y - radius, pos.Z + height), new(0, -1));
                    break;
                case < 0:
                    yield return new(new(pos.X - radius, pos.Y + radius, pos.Z), new(pos.X + radius, pos.Y + radius, pos.Z + height), new(0, 1));
                    break;
            }

            switch (vel.Z)
            {
                //Ebene Z
                case > 0:
                    yield return new(new(pos.X - radius, pos.Y - radius, pos.Z), new(pos.X + radius, pos.Y + radius, pos.Z), new(0, 0, -1));
                    break;
                case < 0:
                    yield return new(new(pos.X - radius, pos.Y - radius, pos.Z + height), new(pos.X + radius, pos.Y + radius, pos.Z + height), new(0, 0, 1));
                    break;
            }
        }

        /// <summary>
        ///     Gibt zurück ob zwei Flächen miteinander Kollidieren können(Achtung noch keine gedrehten Flächen)
        /// </summary>
        /// <param name="p1">Fläche 1</param>
        /// <param name="p2">Fläche 2</param>
        /// <returns>Ergebnis der Kollisions</returns>
        public static bool Intersect(CollisionPlane p1, CollisionPlane p2)
        {
            //TODO: Erweitern auf schräge Fläche
            var vec = p1.normal * p2.normal;

            var result = false;

            if (vec.X < 0)
            {
                var ry = p2.edgepos1.Y > p1.edgepos1.Y && p2.edgepos1.Y < p1.edgepos2.Y || p2.edgepos2.Y < p1.edgepos2.Y && p2.edgepos2.Y > p1.edgepos1.Y || p1.edgepos1.Y > p2.edgepos1.Y && p1.edgepos1.Y < p2.edgepos2.Y || p1.edgepos2.Y < p2.edgepos2.Y && p1.edgepos2.Y > p2.edgepos1.Y;

                var rz = p2.edgepos1.Z > p1.edgepos1.Z && p2.edgepos1.Z < p1.edgepos2.Z || p2.edgepos2.Z < p1.edgepos2.Z && p2.edgepos2.Z > p1.edgepos1.Z || p1.edgepos1.Z > p2.edgepos1.Z && p1.edgepos1.Z < p2.edgepos2.Z || p1.edgepos2.Z < p2.edgepos2.Z && p1.edgepos2.Z > p2.edgepos1.Z;

                result = rz && ry;
            }
            else if (vec.Y < 0)
            {
                var rx = p2.edgepos1.X > p1.edgepos1.X && p2.edgepos1.X < p1.edgepos2.X || p2.edgepos2.X < p1.edgepos2.X && p2.edgepos2.X > p1.edgepos1.X || p1.edgepos1.X > p2.edgepos1.X && p1.edgepos1.X < p2.edgepos2.X || p1.edgepos2.X < p2.edgepos2.X && p1.edgepos2.X > p2.edgepos1.X;

                var rz = p2.edgepos1.Z > p1.edgepos1.Z && p2.edgepos1.Z < p1.edgepos2.Z || p2.edgepos2.Z < p1.edgepos2.Z && p2.edgepos2.Z > p1.edgepos1.Z || p1.edgepos1.Z > p2.edgepos1.Z && p1.edgepos1.Z < p2.edgepos2.Z || p1.edgepos2.Z < p2.edgepos2.Z && p1.edgepos2.Z > p2.edgepos1.Z;


                result = rx && rz;
            }
            else if (vec.Z < 0)
            {
                var rx = p2.edgepos1.X > p1.edgepos1.X && p2.edgepos1.X < p1.edgepos2.X || p2.edgepos2.X < p1.edgepos2.X && p2.edgepos2.X > p1.edgepos1.X || p1.edgepos1.X > p2.edgepos1.X && p1.edgepos1.X < p2.edgepos2.X || p1.edgepos2.X < p2.edgepos2.X && p1.edgepos2.X > p2.edgepos1.X;

                var ry = p2.edgepos1.Y > p1.edgepos1.Y && p2.edgepos1.Y < p1.edgepos2.Y || p2.edgepos2.Y < p1.edgepos2.Y && p2.edgepos2.Y > p1.edgepos1.Y || p1.edgepos1.Y > p2.edgepos1.Y && p1.edgepos1.Y < p2.edgepos2.Y || p1.edgepos2.Y < p2.edgepos2.Y && p1.edgepos2.Y > p2.edgepos1.Y;

                result = rx && ry;
            }

            return result;
        }

        /// <summary>
        ///     Gibt den Abstand zweier Flächen zurück(Mittelpunkt zu Mittelpunkt)
        /// </summary>
        /// <param name="p1">Fläche 1</param>
        /// <param name="p2">Fläche 2</param>
        /// <returns>Abstand der Flächen zueinander</returns>
        public static Vector3 GetDistance(CollisionPlane p1, CollisionPlane p2)
        {
            var alpha = p1.normal * p2.normal;


            var dVector = new Vector3
            {
                X = alpha.X != 0 ? 1 : 0,
                Y = alpha.Y != 0 ? 1 : 0,
                Z = alpha.Z != 0 ? 1 : 0
            };

            var distance = (p1.pos - p2.pos) * dVector;

            return distance;
        }

        /// <summary>
        ///     Überprüft ob Vektor 2 größer als Vektor 1 ist
        /// </summary>
        /// <param name="d1">Vektor 1</param>
        /// <param name="d2">Vektor 2</param>
        /// <returns>Ergebnis</returns>
        public static bool CheckDistance(Vector3 d1, Vector3 d2)
        {
            if (d1.X == 0 || d1.Y == 0 || d1.Z == 0)
                return true;

            var diff = d1 - d2;

            var rx = d1.X > 0 ? diff.X < 0 : diff.X > 0;
            var ry = d1.Y > 0 ? diff.Y < 0 : diff.Y > 0;
            var rz = d1.Z > 0 ? diff.Z < 0 : diff.Z > 0;

            return rx || ry || rz;
        }
    }
}