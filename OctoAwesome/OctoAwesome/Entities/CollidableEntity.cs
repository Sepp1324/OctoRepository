﻿using Microsoft.Xna.Framework;
using System.Xml.Serialization;

namespace OctoAwesome.Entities
{
    public class CollidableEntity : Entity, ICollidable
    {
        /// <summary>
        /// Die Masse der Entität. 
        /// </summary>
        public float Mass { get; set; }

        /// <summary>
        /// Gibt an, ob der Spieler an Boden ist
        /// </summary>
        [XmlIgnore]
        public bool OnGround { get; set; }

        /// <summary>
        /// Kraft die von aussen auf die Entität wirkt.
        /// </summary>
        [XmlIgnore]
        public Vector3 ExternalForce { get; set; }

        /// <summary>
        /// Der Radius des Entities in Blocks.
        /// </summary>
        public float Radius { get; set; }

        /// <summary>
        /// Die Körperhöhe des Entities in Blocks
        /// </summary>
        public float Height { get; set; }
    }
}
