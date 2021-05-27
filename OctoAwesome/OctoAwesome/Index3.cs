using System;
using engenious;

namespace OctoAwesome
{
    /// <summary>
    /// Struktur zur Definierung einer dreidimensionalen Index-Position.
    /// </summary>
    public struct Index3
    {
        /// <summary>
        /// X Anteil
        /// </summary>
        public int X;

        /// <summary>
        /// Y Anteil
        /// </summary>
        public int Y;

        /// <summary>
        /// Z Anteil
        /// </summary>
        public int Z;

        /// <summary>
        /// Initialisierung
        /// </summary>
        /// <param name="x">X-Anteil</param>
        /// <param name="y">Y-Anteil</param>
        /// <param name="z">Z-Anteil</param>
        public Index3(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        /// <summary>
        /// Initialisierung
        /// </summary>
        /// <param name="index">2D-Basis</param>
        /// <param name="z">Z-Anteil</param>
        public Index3(Index2 index, int z) : this(index.X, index.Y, z) { }

        /// <summary>
        /// Initialisierung
        /// </summary>
        /// <param name="index">3D-Basis</param>
        public Index3(Index3 index) : this(index.X, index.Y, index.Z) { }

        /// <summary>
        /// Normalisiert die X-Achse auf die angegebene Größe.
        /// </summary>
        /// <param name="size">Maximalwert für X</param>
<<<<<<< HEAD
        public void NormalizeX(int size) => X = Index2.NormalizeAxis(X, size);
=======
        public void NormalizeX(int size)
            => X = Index2.NormalizeAxis(X, size);
>>>>>>> feature/performance

        /// <summary>
        /// Normalisiert die X-Achse auf die angegebene Größe.
        /// </summary>
        /// <param name="size">2D-Size</param>
<<<<<<< HEAD
        public void NormalizeX(Index2 size) => NormalizeX(size.X);
=======
        public void NormalizeX(Index2 size)
            => NormalizeX(size.X);
>>>>>>> feature/performance

        /// <summary>
        /// Normalisiert die X-Achse auf die angegebene Größe.
        /// </summary>
        /// <param name="size">3D-Size</param>
<<<<<<< HEAD
        public void NormalizeX(Index3 size) => NormalizeX(size.X);
=======
        public void NormalizeX(Index3 size)
            => NormalizeX(size.X);
>>>>>>> feature/performance

        /// <summary>
        /// Normalisiert die Y-Achse auf die angegebene Größe.
        /// </summary>
        /// <param name="size">Maximalwert für Y</param>
<<<<<<< HEAD
        public void NormalizeY(int size) => Y = Index2.NormalizeAxis(Y, size);
=======
        public void NormalizeY(int size)
            => Y = Index2.NormalizeAxis(Y, size);
>>>>>>> feature/performance

        /// <summary>
        /// Normalisiert die Y-Achse auf die angegebene Größe.
        /// </summary>
        /// <param name="size">2D-Size</param>
<<<<<<< HEAD
        public void NormalizeY(Index2 size) => NormalizeY(size.Y);
=======
        public void NormalizeY(Index2 size)
            => NormalizeY(size.Y);
>>>>>>> feature/performance

        /// <summary>
        /// Normalisiert die Y-Achse auf die angegebene Größe.
        /// </summary>
        /// <param name="size">3D-Size</param>
<<<<<<< HEAD
        public void NormalizeY(Index3 size) => NormalizeY(size.Y);
=======
        public void NormalizeY(Index3 size)
            => NormalizeY(size.Y);
>>>>>>> feature/performance

        /// <summary>
        /// Normalisiert die Z-Achse auf die angegebene Größe.
        /// </summary>
        /// <param name="size">Maximalwert für Z</param>
<<<<<<< HEAD
        public void NormalizeZ(int size) => Z = Index2.NormalizeAxis(Z, size);
=======
        public void NormalizeZ(int size)
            => Z = Index2.NormalizeAxis(Z, size);
>>>>>>> feature/performance

        /// <summary>
        /// Normalisiert die Z-Achse auf die angegebene Größe.
        /// </summary>
        /// <param name="size">3D-Size</param>
<<<<<<< HEAD
        public void NormalizeZ(Index3 size) => NormalizeZ(size.Z);
=======
        public void NormalizeZ(Index3 size)
            => NormalizeZ(size.Z);
>>>>>>> feature/performance

        /// <summary>
        /// Normalisiert die X- und Y-Achse auf die angegebene Größe.
        /// </summary>
        /// <param name="x">X-Anteil</param>
        /// <param name="y">Y-Anteil</param>
        public void NormalizeXY(int x, int y)
        {
            NormalizeX(x);
            NormalizeY(y);
        }

        /// <summary>
        /// Normalisiert die X- und Y-Achse auf die angegebene Größe.
        /// </summary>
        /// <param name="size">Maximalwert für X und Y</param>
<<<<<<< HEAD
        public void NormalizeXY(Index2 size) => NormalizeXY(size.X, size.Y);
=======
        public void NormalizeXY(Index2 size)
            => NormalizeXY(size.X, size.Y);
>>>>>>> feature/performance

        /// <summary>
        /// Normalisiert die X- und Y-Achse auf die angegebene Größe.
        /// </summary>
        /// <param name="size">Maximalwert für X und Y</param>
<<<<<<< HEAD
        public void NormalizeXY(Index3 size) => NormalizeXY(size.X, size.Y);
=======
        public void NormalizeXY(Index3 size)
            => NormalizeXY(size.X, size.Y);
>>>>>>> feature/performance

        /// <summary>
        /// Normalisiert die X-, Y- und Z-Achse auf die angegebene Größe.
        /// </summary>
        /// <param name="x">X-Anteil</param>
        /// <param name="y">Y-Anteil</param>
        /// <param name="z">Z-Anteil</param>
        public void NormalizeXYZ(int x, int y, int z)
        {
            NormalizeX(x);
            NormalizeY(y);
            NormalizeZ(z);
        }

        /// <summary>
        /// Normalisiert die X-, Y- und Z-Achse auf die angegebene Größe.
        /// </summary>
        /// <param name="size">2D-Size</param>
        /// <param name="z">Z-Anteil</param>
<<<<<<< HEAD
        public void NormalizeXYZ(Index2 size, int z) => NormalizeXYZ(size.X, size.Y, z);
=======
        public void NormalizeXYZ(Index2 size, int z)
            => NormalizeXYZ(size.X, size.Y, z);
>>>>>>> feature/performance

        /// <summary>
        /// Normalisiert die X-, Y- und Z-Achse auf die angegebene Größe.
        /// </summary>
        /// <param name="size">Maximalwert für X, Y und Z</param>
<<<<<<< HEAD
        public void NormalizeXYZ(Index3 size) => NormalizeXYZ(size.X, size.Y, size.Z);
=======
        public void NormalizeXYZ(Index3 size)
            => NormalizeXYZ(size.X, size.Y, size.Z);
>>>>>>> feature/performance

        /// <summary>
        /// Ermittelt die kürzeste Entfernung zum Ziel auf einer normalisierten X-Achse.
        /// </summary>
        /// <param name="x">Ziel</param>
        /// <param name="size">Normalisierungsgröße</param>
        /// <returns>Entfernung</returns>
<<<<<<< HEAD
        public int ShortestDistanceX(int x, int size) => Index2.ShortestDistanceOnAxis(X, x, size);
=======
        public int ShortestDistanceX(int x, int size)
            => Index2.ShortestDistanceOnAxis(X, x, size);
>>>>>>> feature/performance

        /// <summary>
        /// Ermittelt die kürzeste Entfernung zum Ziel auf einer normalisierten Y-Achse.
        /// </summary>
        /// <param name="y">Ziel</param>
        /// <param name="size">Normalisierungsgröße</param>
        /// <returns>Entfernung</returns>
<<<<<<< HEAD
        public int ShortestDistanceY(int y, int size) => Index2.ShortestDistanceOnAxis(Y, y, size);
=======
        public int ShortestDistanceY(int y, int size)
            => Index2.ShortestDistanceOnAxis(Y, y, size);
>>>>>>> feature/performance

        /// <summary>
        /// Ermittelt die kürzeste Entfernung zum Ziel auf einer normalisierten Z-Achse.
        /// </summary>
        /// <param name="z">Ziel</param>
        /// <param name="size">Normalisierungsgröße</param>
        /// <returns>Entfernung</returns>
<<<<<<< HEAD
        public int ShortestDistanceZ(int z, int size) => Index2.ShortestDistanceOnAxis(Z, z, size);
=======
        public int ShortestDistanceZ(int z, int size)
            => Index2.ShortestDistanceOnAxis(Z, z, size);
>>>>>>> feature/performance

        /// <summary>
        /// Ermittelt die kürzeste Entfernung zum Ziel auf den normalisierten Achsen.
        /// </summary>
        /// <param name="destination">Ziel</param>
        /// <param name="size">Normalisierungsgröße</param>
        /// <returns>Entfernung</returns>
<<<<<<< HEAD
        public Index2 ShortestDistanceXY(Index2 destination, Index2 size) => new Index2(ShortestDistanceX(destination.X, size.X), ShortestDistanceY(destination.Y, size.Y));
=======
        public Index2 ShortestDistanceXY(Index2 destination, Index2 size)
            => new Index2(
                ShortestDistanceX(destination.X, size.X),
                ShortestDistanceY(destination.Y, size.Y));
>>>>>>> feature/performance

        /// <summary>
        /// Ermittelt die kürzeste Entfernung zum Ziel auf den normalisierten Achsen.
        /// </summary>
        /// <param name="destination">Ziel</param>
        /// <param name="size">Normalisierungsgröße</param>
        /// <returns>Entfernung</returns>
        public Index3 ShortestDistanceXY(Index3 destination, Index3 size)
            => new Index3(
                ShortestDistanceX(destination.X, size.X),
                ShortestDistanceY(destination.Y, size.Y),
                destination.Z - Z);

        /// <summary>
        /// Ermittelt die kürzeste Entfernung zum Ziel auf den normalisierten Achsen.
        /// </summary>
        /// <param name="destination">Ziel</param>
        /// <param name="size">Normalisierungsgröße</param>
        /// <returns>Entfernung</returns>
        public Index3 ShortestDistanceXY(Index3 destination, Index2 size)
            => new Index3(
                ShortestDistanceX(destination.X, size.X),
                ShortestDistanceY(destination.Y, size.Y),
                destination.Z - Z);

        /// <summary>
        /// Ermittelt die kürzeste Entfernung zum Ziel auf den normalisierten Achsen.
        /// </summary>
        /// <param name="destination">Ziel</param>
        /// <param name="size">Normalisierungsgröße</param>
        /// <returns>Entfernung</returns>
        public Index3 ShortestDistanceXYZ(Index3 destination, Index3 size)
            => new Index3(
                ShortestDistanceX(destination.X, size.X),
                ShortestDistanceY(destination.Y, size.Y),
                ShortestDistanceZ(destination.Z, size.Z));

        /// <summary>
        /// Ermittelt die Entferung zum Nullpunkt.
        /// </summary>
        /// <returns></returns>
<<<<<<< HEAD
        public double Length() => Math.Sqrt(LengthSquared());
=======
        public double Length()
            => Math.Sqrt(LengthSquared());
>>>>>>> feature/performance

        /// <summary>
        /// Ermittelt die Entfernung zum Nullpunkt im Quadrat.
        /// </summary>
        /// <returns></returns>
<<<<<<< HEAD
        public int LengthSquared() => X * X + Y * Y + Z * Z;
=======
        public int LengthSquared()
            => X * X + Y * Y + Z * Z;
>>>>>>> feature/performance

        /// <summary>
        /// Addiert zwei Indices3
        /// </summary>
        /// <param name="i1">1. Summand</param>
        /// <param name="i2">2. Summand</param>
        /// <returns></returns>
<<<<<<< HEAD
        public static Index3 operator +(Index3 i1, Index3 i2) => new Index3(i1.X + i2.X, i1.Y + i2.Y, i1.Z + i2.Z);
=======
        public static Index3 operator +(Index3 i1, Index3 i2)
            => new Index3(i1.X + i2.X, i1.Y + i2.Y, i1.Z + i2.Z);
>>>>>>> feature/performance

        /// <summary>
        /// Addiert einen Index3 und einen <see cref="Index2"/>
        /// </summary>
        /// <remarks>Der Z-Anteil des Index3 wird unverändert übernommen.</remarks>
        /// <param name="i1">1. Summand</param>
        /// <param name="i2">2. Summand (ohne Z-Anteil)</param>
        /// <returns></returns>
<<<<<<< HEAD
        public static Index3 operator +(Index3 i1, Index2 i2) => new Index3(i1.X + i2.X, i1.Y + i2.Y, i1.Z);
=======
        public static Index3 operator +(Index3 i1, Index2 i2)
            => new Index3(i1.X + i2.X, i1.Y + i2.Y, i1.Z);
>>>>>>> feature/performance

        /// <summary>
        /// Subtrahiert zwei Indices3
        /// </summary>
        /// <param name="i1">Minuend</param>
        /// <param name="i2">Subtrahend</param>
        /// <returns></returns>
<<<<<<< HEAD
        public static Index3 operator -(Index3 i1, Index3 i2) => new Index3(i1.X - i2.X, i1.Y - i2.Y, i1.Z - i2.Z);
=======
        public static Index3 operator -(Index3 i1, Index3 i2)
            => new Index3(i1.X - i2.X, i1.Y - i2.Y, i1.Z - i2.Z);
>>>>>>> feature/performance

        /// <summary>
        /// Subtrahiert einen Index2 von einem Index3
        /// </summary>
        /// <remarks>Der Z-Anteil des Index3 wird unverändert übernommen.</remarks>
        /// <param name="i1">Minuend</param>
        /// <param name="i2">Subtrahend</param>
        /// <returns></returns>
<<<<<<< HEAD
        public static Index3 operator -(Index3 i1, Index2 i2) => new Index3(i1.X - i2.X, i1.Y - i2.Y, i1.Z);
=======
        public static Index3 operator -(Index3 i1, Index2 i2)
            => new Index3(i1.X - i2.X, i1.Y - i2.Y, i1.Z);
>>>>>>> feature/performance

        /// <summary>
        /// Skaliert einen Index3 mit einem Integer.
        /// </summary>
        /// <param name="i1">Der zu skalierende Index3</param>
        /// <param name="scale">Der Skalierungsfaktor</param>
        /// <returns></returns>
<<<<<<< HEAD
        public static Index3 operator *(Index3 i1, int scale) => new Index3(i1.X * scale, i1.Y * scale, i1.Z * scale);
=======
        public static Index3 operator *(Index3 i1, int scale)
            => new Index3(i1.X * scale, i1.Y * scale, i1.Z * scale);
>>>>>>> feature/performance

        /// <summary>
        /// Multiplieziert wei Indices3 miteinander.
        /// </summary>
        /// <param name="i1">1. Faktor</param>
        /// <param name="i2">2. Faktor</param>
        /// <returns></returns>
<<<<<<< HEAD
        public static Index3 operator *(Index3 i1, Index3 i2) => new Index3(i1.X * i2.X, i1.Y * i2.Y, i1.Z * i2.Z);
=======
        public static Index3 operator *(Index3 i1, Index3 i2)
            => new Index3(i1.X * i2.X, i1.Y * i2.Y, i1.Z * i2.Z);
>>>>>>> feature/performance

        /// <summary>
        /// Dividiert einen Index3 durch einen Skalierungsfaktor.
        /// </summary>
        /// <param name="i1">Der Index3</param>
        /// <param name="scale">Der Skalierungsfaktor</param>
        /// <returns></returns>
<<<<<<< HEAD
        public static Index3 operator /(Index3 i1, int scale) => new Index3(i1.X / scale, i1.Y / scale, i1.Z / scale);
=======
        public static Index3 operator /(Index3 i1, int scale)
            => new Index3(i1.X / scale, i1.Y / scale, i1.Z / scale);
>>>>>>> feature/performance

        /// <summary>
        /// Überprüft, ob beide gegebenen Indices3 den gleichen Wert aufweisen.
        /// </summary>
        /// <param name="i1"></param>
        /// <param name="i2"></param>
        /// <returns></returns>
        public static bool operator ==(Index3 i1, Index3 i2) => i1.Equals(i2);

        /// <summary>
        /// Überprüft, ob beide gegebenen Indices3 nicht den gleichen Wert aufweisen.
        /// </summary>
        /// <param name="i1"></param>
        /// <param name="i2"></param>
        /// <returns></returns>
        public static bool operator !=(Index3 i1, Index3 i2) => !i1.Equals(i2);

        /// <summary>
        /// Implizite Umwandlung des aktuellen Index3 in einen Vector3.
        /// </summary>
        /// <remarks>Bei der Konvertierung von int zu float können Rundungsfehler auftreten!</remarks>
        /// <param name="index"></param>
        public static implicit operator Vector3(Index3 index) => new Vector3(index.X, index.Y, index.Z);

        /// <summary>
        /// Gibt einen string zurück, der den akteullen Index3 darstellt.
        /// </summary>
        /// <returns></returns>
        public override string ToString() => $"({X}/{Y}/{Z})";

        /// <summary>
        /// Überprüft, ob der gegebene Index3 den gleichen Wert aufweist, wie der aktuelle Index3.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj is Index3 other)
                return other.X == X && other.Y == Y && other.Z == Z;

            return false;
        }

        /// <summary>
        /// Gibt einen möglichst eindeutigen Hashwert für den aktuellen Index3 zurück.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode() => (X << 20) + (Y << 10) + Z;

        /// <summary>
        /// Null-Index
        /// </summary>
        public static Index3 Zero => new Index3(0, 0, 0);

        /// <summary>
        /// Gibts Index(1,1,1) zurück
        /// </summary>
        public static Index3 One => new Index3(1, 1, 1);

        /// <summary>
        /// Einheitsindex für X
        /// </summary>
        public static Index3 UnitX => new Index3(1, 0, 0);

        /// <summary>
        /// Einheitsindex für Y
        /// </summary>
        public static Index3 UnitY => new Index3(0, 1, 0);

        /// <summary>
        /// Einheitsindex für Z
        /// </summary>
        public static Index3 UnitZ => new Index3(0, 0, 1);
    }
}
