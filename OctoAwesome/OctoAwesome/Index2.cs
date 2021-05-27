using System;
using engenious;

namespace OctoAwesome
{
    /// <summary>
    /// Struktur zur Definierung einer zweidimensionalen Index-Position.
    /// </summary>
    public struct Index2
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
        /// Initialisierung
        /// </summary>
        /// <param name="x">X Anteil</param>
        /// <param name="y">Y Anteil</param>
        public Index2(int x, int y)
        {
            X = x;
            Y = y;
        }

        /// <summary>
        /// Initialisierung
        /// </summary>
        /// <param name="value">Initialwerte</param>
        public Index2(Index2 value) : this(value.X, value.Y) { }

        /// <summary>
        /// Initialisierung
        /// </summary>
        /// <param name="value">Initialwerte (X und Y Anteil wird übernommen)</param>
        public Index2(Index3 value) : this(value.X, value.Y) { }

        /// <summary>
        /// Normalisiert die X-Achse auf die angegebene Größe.
        /// </summary>
        /// <param name="size">Maximalwert für X</param>
<<<<<<< HEAD
        public void NormalizeX(int size) => X = NormalizeAxis(X, size);
=======
        public void NormalizeX(int size)
            => X = NormalizeAxis(X, size);
>>>>>>> feature/performance

        /// <summary>
        /// Normalisiert die X-Achse auf die angegebene Größe.
        /// </summary>
        /// <param name="size">2D-Größe (X-Anzeil wird genommen)</param>
<<<<<<< HEAD
        public void NormalizeX(Index2 size) => NormalizeX(size.X);
=======
        public void NormalizeX(Index2 size)
            => NormalizeX(size.X);
>>>>>>> feature/performance

        /// <summary>
        /// Normalisiert die X-Achse auf die angegebene Größe.
        /// </summary>
        /// <param name="size">3D-Größe (X-Anzeil wird genommen)</param>
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
        public void NormalizeY(int size) => Y = NormalizeAxis(Y, size);
=======
        public void NormalizeY(int size)
            => Y = NormalizeAxis(Y, size);
>>>>>>> feature/performance

        /// <summary>
        /// Normalisiert die Y-Achse auf die angegebene Größe.
        /// </summary>
        /// <param name="size">2D-Größe (Y-Anzeil wird genommen)</param>
        public void NormalizeY(Index2 size)
            => NormalizeY(size.Y);

        /// <summary>
        /// Normalisiert die Y-Achse auf die angegebene Größe.
        /// </summary>
        /// <param name="size">3D-Größe (Y-Anzeil wird genommen)</param>
<<<<<<< HEAD
        public void NormalizeY(Index3 size) => NormalizeY(size.Y);
=======
        public void NormalizeY(Index3 size)
            => NormalizeY(size.Y);
>>>>>>> feature/performance

        /// <summary>
        /// Normalisiert den Wert von X und Y auf den angegebenen Grenzbereich.
        /// </summary>
        /// <param name="x">Größe in X-Richtung</param>
        /// <param name="y">Größe in Y-Richtung</param>
        public void NormalizeXY(int x, int y)
        {
            NormalizeX(x);
            NormalizeY(y);
        }

        /// <summary>
        /// Normalisiert den Wert von X und Y auf den angegebenen Grenzbereich.
        /// </summary>
        /// <param name="size">2D Size</param>
<<<<<<< HEAD
        public void NormalizeXY(Index2 size) => NormalizeXY(size.X, size.Y);
=======
        public void NormalizeXY(Index2 size)
            => NormalizeXY(size.X, size.Y);
>>>>>>> feature/performance

        /// <summary>
        /// Normalisiert den Wert von X und Y auf den angegebenen Grenzbereich.
        /// </summary>
        /// <param name="size">3D Size</param>
<<<<<<< HEAD
        public void NormalizeXY(Index3 size) => NormalizeXY(size.X, size.Y);
=======
        public void NormalizeXY(Index3 size)
            => NormalizeXY(size.X, size.Y);
>>>>>>> feature/performance

        /// <summary>
        /// Normalisiert den Wert von X und Y auf den angegebenen Grenzbereich.
        /// </summary>
        /// <param name="index">Der zu normalisierende Index2</param>
        /// <param name="size">3D Size</param>
        public static Index2 NormalizeXY(Index2 index, Index3 size)
        {
            index.NormalizeXY(size);
            return index;
        }

        /// <summary>
        /// Ermittelt die kürzeste Entfernung zum Ziel auf einer normalisierten X-Achse.
        /// </summary>
        /// <param name="x">Ziel</param>
        /// <param name="size">Normalisierungsgröße</param>
        /// <returns>Entfernung</returns>
<<<<<<< HEAD
        public int ShortestDistanceX(int x, int size) => ShortestDistanceOnAxis(X, x, size);
=======
        public int ShortestDistanceX(int x, int size)
            => ShortestDistanceOnAxis(X, x, size);
>>>>>>> feature/performance

        /// <summary>
        /// Ermittelt die kürzeste Entfernung zum Ziel auf einer normalisierten Y-Achse.
        /// </summary>
        /// <param name="y">Ziel</param>
        /// <param name="size">Normalisierungsgröße</param>
        /// <returns>Entfernung</returns>
<<<<<<< HEAD
        public int ShortestDistanceY(int y, int size) => ShortestDistanceOnAxis(Y, y, size);
=======
        public int ShortestDistanceY(int y, int size)
            => ShortestDistanceOnAxis(Y, y, size);
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
            => new Index2(ShortestDistanceX(destination.X, size.X),
                        ShortestDistanceY(destination.Y, size.Y));
>>>>>>> feature/performance

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
        public int LengthSquared() => (X * X) + (Y * Y);
=======
        public int LengthSquared()
            => (X * X) + (Y * Y);
>>>>>>> feature/performance

        /// <summary>
        /// Addition von zwei Indices2
        /// </summary>
        /// <param name="i1">1. Summand</param>
        /// <param name="i2">2. Summand</param>
        /// <returns></returns>
<<<<<<< HEAD
        public static Index2 operator +(Index2 i1, Index2 i2) => new Index2(i1.X + i2.X, i1.Y + i2.Y);
=======
        public static Index2 operator +(Index2 i1, Index2 i2)
            => new Index2(i1.X + i2.X, i1.Y + i2.Y);
>>>>>>> feature/performance

        /// <summary>
        /// Subtraktion von zwei Indices2
        /// </summary>
        /// <param name="i1">Minuend</param>
        /// <param name="i2">Subtrahend</param>
        /// <returns></returns>
<<<<<<< HEAD
        public static Index2 operator -(Index2 i1, Index2 i2) => new Index2(i1.X - i2.X, i1.Y - i2.Y);
=======
        public static Index2 operator -(Index2 i1, Index2 i2)
            => new Index2(i1.X - i2.X, i1.Y - i2.Y);
>>>>>>> feature/performance

        /// <summary>
        /// Multiplikation eines Index2 mit einem Skalierungsfaktor
        /// </summary>
        /// <param name="i1">Index</param>
        /// <param name="scale">Skalierungsfaktor</param>
        /// <returns></returns>
<<<<<<< HEAD
        public static Index2 operator *(Index2 i1, int scale) => new Index2(i1.X * scale, i1.Y * scale);
=======
        public static Index2 operator *(Index2 i1, int scale)
            => new Index2(i1.X * scale, i1.Y * scale);
>>>>>>> feature/performance

        /// <summary>
        /// Division eines Index2 durch einen Skalierungsfaktor
        /// </summary>
        /// <param name="i1">Index</param>
        /// <param name="scale">Skalierungsfaktor</param>
        /// <returns></returns>
<<<<<<< HEAD
        public static Index2 operator /(Index2 i1, int scale) => new Index2(i1.X / scale, i1.Y / scale);
=======
        public static Index2 operator /(Index2 i1, int scale)
            => new Index2(i1.X / scale, i1.Y / scale);
>>>>>>> feature/performance

        /// <summary>
        /// Überprüft, ob beide gegebenen Indices gleich sind.
        /// </summary>
        /// <param name="i1"></param>
        /// <param name="i2"></param>
        /// <returns></returns>
<<<<<<< HEAD
        public static bool operator ==(Index2 i1, Index2 i2) => i1.Equals(i2);
=======
        public static bool operator ==(Index2 i1, Index2 i2)
            => i1.Equals(i2);
>>>>>>> feature/performance

        /// <summary>
        /// Überprüft, ob beide gegebenen Indices nicht gleich sind.
        /// </summary>
        /// <param name="i1"></param>
        /// <param name="i2"></param>
        /// <returns></returns>
<<<<<<< HEAD
        public static bool operator !=(Index2 i1, Index2 i2) => !i1.Equals(i2);
=======
        public static bool operator !=(Index2 i1, Index2 i2)
            => !i1.Equals(i2);
>>>>>>> feature/performance

        /// <summary>
        /// Implizite Umwandlung eines Index2 in einen Vector2. Möglicherweise entstehen dadurch Rundungsfehler.
        /// </summary>
        /// <param name="index"></param>
<<<<<<< HEAD
        public static implicit operator Vector2(Index2 index) => new Vector2(index.X, index.Y);
=======
        public static implicit operator Vector2(Index2 index)
            => new Vector2(index.X, index.Y);
>>>>>>> feature/performance

        /// <summary>
        /// Normalisiert einen Integer auf die angegebene Maximalgröße.
        /// </summary>
        /// <param name="value">Wert</param>
        /// <param name="size">Normalisierungsgröße</param>
        /// <returns></returns>
        public static int NormalizeAxis(int value, int size)
        {
            // Sicherheitsabfrage für die Normalisierungsgröße
            if (size < 1)
                throw new ArgumentException("Size darf nicht kleiner als 1 sein");

            value %= size;

            if (value < 0)
                value += size;

            return value;
        }

        /// <summary>
        /// Ermittelt die kürzeste Entfernung von Ursprung zum Ziel auf einer normalisierten Achse.
        /// </summary>
        /// <param name="origin">Ursprungswert</param>
        /// <param name="destination">Zielwert</param>
        /// <param name="size">Normalisierungsgröße</param>
        /// <returns></returns>
        public static int ShortestDistanceOnAxis(int origin, int destination, int size)
        {
            origin = NormalizeAxis(origin, size);
            destination = NormalizeAxis(destination, size);

            int half = size / 2;
            int distance = destination - origin;

            if (distance > half)
                distance -= size;
            else if (distance < -half)
                distance += size;

            return distance;
        }

        /// <summary>
        /// Gibt einen string zurück, der den akteullen Index2 darstellt.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
            => $"({X}/{Y})";

        /// <summary>
        /// Überprüft, ob das gegebene Objekt (falls ein <see cref="Index2"/> gleich der aktuellen Instanz ist.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj is Index2 other)
                return other.X == X && other.Y == Y;

            return false;
        }

        /// <summary>
        /// Gibt einen möglichst eindeutigen Hashwert für den aktuellen Index2 zurück.
        /// </summary>
        /// <returns></returns>
<<<<<<< HEAD
        public override int GetHashCode() => (X << 16) + Y;
=======
        public override int GetHashCode()
            => (X << 16) + Y;
>>>>>>> feature/performance

        /// <summary>
        /// Null-Index
        /// </summary>
        public static Index2 Zero => new Index2(0, 0);

        /// <summary>
        /// Index(1,1)
        /// </summary>
        public static Index2 One => new Index2(1, 1);

        /// <summary>
        /// Einheitsindex für X
        /// </summary>
        public static Index2 UnitX => new Index2(1, 0);

        /// <summary>
        /// Einheitsindex für Y
        /// </summary>
        public static Index2 UnitY => new Index2(0, 1);
    }
}
