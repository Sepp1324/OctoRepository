using System;

namespace OctoAwesome
{
    /// <summary>
    /// Liste von Flags zur Beschreibung der Block-Ausrichtung.
    /// </summary>
    [Flags]
    public enum OrientationFlags : byte
    {
        /// <summary>
        /// Null-Wert (keine Orientierung)
        /// </summary>
        None,

        #region Corner

        /// <summary>
        /// Beschreibt die Ecke auf -X, -Y, -Z
        /// </summary>
        Corner000 = 1,

        /// <summary>
        /// Beschreibt die Ecke auf -X, -Y, +Z
        /// </summary>
        Corner001 = 2,

        /// <summary>
        /// Beschreibt die Ecke auf -X, +Y, -Z
        /// </summary>
        Corner010 = 4,

        /// <summary>
        /// Beschreibt die Ecke auf -X, +Y, +Z
        /// </summary>
        Corner011 = 8,

        /// <summary>
        /// Beschreibt die Ecke auf +X, -Y, -Z
        /// </summary>
        Corner100 = 16,

        /// <summary>
        /// Beschreibt die Ecke auf +X, -Y, +Z
        /// </summary>
        Corner101 = 32,

        /// <summary>
        /// Beschreibt die Ecke auf +X, +Y, -Z
        /// </summary>
        Corner110 = 64,

        /// <summary>
        /// Beschreibt die Ecke auf +X, +Y, +Z
        /// </summary>
        Corner111 = 128,

        #endregion

        #region Sides

        /// <summary>
        /// Beschreibt die komplette Seite von -X
        /// </summary>
        SideWest =
            Corner000 |
            Corner001 |
            Corner010 |
            Corner011,

        /// <summary>
        /// Beschreibt die komplette Seite von +X
        /// </summary>
        SideEast =
            Corner100 |
            Corner101 |
            Corner110 |
            Corner111,

        /// <summary>
        /// Beschreibt die komplette Seite von -Y
        /// </summary>
        SideSouth =
            Corner000 |
            Corner001 |
            Corner100 |
            Corner101,

        /// <summary>
        /// Beschreibt die komplette Seite von +Y
        /// </summary>
        SideNorth =
            Corner010 |
            Corner011 |
            Corner110 |
            Corner111,

        /// <summary>
        /// Beschreibt die komplette Seite von -Z
        /// </summary>
        SideBottom =
            Corner010 |
            Corner000 |
            Corner110 |
            Corner100,

        /// <summary>
        /// Beschreibt die komplette Seite von +Z
        /// </summary>
        SideTop =
            Corner011 |
            Corner001 |
            Corner111 |
            Corner101,

        #endregion

        #region Edges

        /// <summary>
        /// Beschreibt die obere Kante der Westseite [-X,-Y,+Z] -> [-X,+Y,+Z]
        /// </summary>
        EdgeWestTop = Corner001 | Corner011,

        /// <summary>
        /// Beschreibt die untere Kante der Westseite [-X,-Y,-Z] -> [-X,+Y,-Z]
        /// </summary>
        EdgeWestBottom = Corner000 | Corner010,

        /// <summary>
        /// Beschreibt die obere Kante der Ostseite [+X,-Y,+Z] -> [+X,+Y,+Z]
        /// </summary>
        EdgeEastTop = Corner101 | Corner111,

        /// <summary>
        /// Beschreibt die untere Kante der Ostseite [+X,-Y,-Z] -> [+X,+Y,-Z]
        /// </summary>
        EdgeEastBottom = Corner100 | Corner110,

        /// <summary>
        /// Beschreibt die obere Kante der Nordseite [-X,+Y,+Z] -> [+X,+Y,+Z]
        /// </summary>
        EdgeNorthTop = Corner011 | Corner111,

        /// <summary>
        /// Beschreibt die untere Kante der Nordseite [-X,+Y,-Z] -> [+X,+Y,-Z]
        /// </summary>
        EdgeNorthBottom = Corner010 | Corner110,

        /// <summary>
        /// Beschreibt die obere Kante der Südseite [-X,-Y,+Z] -> [+X,-Y,+Z]
        /// </summary>
        EdgeSouthTop = Corner001 | Corner101,

        /// <summary>
        /// Beschreibt die untere Kante der Südseite [-X,-Y,-Z] -> [+X,-Y,-Z]
        /// </summary>
        EdgeSouthBottom = Corner000 | Corner100,

        /// <summary>
        /// Beschreibt die Senkrechte zwischen Nord und West [-X,+Y,-Z] -> [-X,+Y,+Z]
        /// </summary>
        EdgeNorthWest = Corner010 | Corner011,

        /// <summary>
        /// Beschreibt die Senkrechte zwischen Nord und Ost [+X,+Y,-Z] -> [+X,+Y,+Z]
        /// </summary>
        EdgeNorthEast = Corner110 | Corner111,

        /// <summary>
        /// Beschreibt die Senkrechte zwischen Süd und West [-X,-Y,-Z] -> [-X,-Y,+Z]
        /// </summary>
        EdgeSouthWest = Corner000 | Corner001,

        /// <summary>
        /// Beschreibt die Senkrechte zwischen Süd und Ost [+X,-Y,-Z] -> [+X,-Y,+Z]
        /// </summary>
        EdgeSouthEast = Corner100 | Corner101,

        #endregion
    }
}