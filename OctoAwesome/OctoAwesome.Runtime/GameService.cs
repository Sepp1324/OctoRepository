using engenious;
using OctoAwesome.Common;
using OctoAwesome.Definitions;
using System;
<<<<<<< HEAD

=======
using System.Collections.Generic;
using System.Linq;
>>>>>>> feature/performance
namespace OctoAwesome.Runtime
{
    // sealed -> prevent abuse of third party´s
    /// <summary>
    /// Diese Berechnungen sollten nicht der Extension überlassen werden.
    /// </summary>
    public sealed class GameService : IGameService
    {
        /// <summary>
        /// <see cref="IDefinitionManager"/> der lokalen Daten.
        /// </summary>
<<<<<<< HEAD
        public IDefinitionManager DefinitionManager => _manager.DefinitionManager;
=======
        public IDefinitionManager DefinitionManager => manager.DefinitionManager;
>>>>>>> feature/performance
        /// <summary>
        /// GAP.
        /// </summary>
        public const float GAP = 0.01f;
<<<<<<< HEAD
        private readonly IResourceManager _manager;
=======
        private IResourceManager manager;
>>>>>>> feature/performance
        /// <summary>
        /// Standart Konstruktor.
        /// </summary>
        /// <param name="resourceManager">ResourceManger</param>
<<<<<<< HEAD
        public GameService(IResourceManager resourceManager) => _manager = resourceManager;

=======
        public GameService(IResourceManager resourceManager)
        {
            manager = resourceManager;
        }
>>>>>>> feature/performance
        /// <summary>
        /// Gibt einen <see cref="ILocalChunkCache"/> zurück
        /// </summary>
        /// <param name="passive">Gibt an ob der Cache passiv ist</param>
        /// <param name="dimensions">Dimensionen des Caches</param>
        /// <param name="range">Ausdehnung des Caches</param>
        /// <returns></returns>
<<<<<<< HEAD
        public ILocalChunkCache GetLocalCache(bool passive, int dimensions, int range) => null;

=======
        public ILocalChunkCache GetLocalCache(bool passive, int dimensions, int range)
        {
            //new LocalChunkCache(manager.GlobalChunkCache, false, 2, 1);
            return null;
        }
>>>>>>> feature/performance
        /// <summary>
        /// Berechnet die Geschwindigkeit einer <see cref="Entity"/> nach der Kollision mit der Welt. (Original Lassi)
        /// </summary>
        /// <param name="gameTime">Simulation time</param>
        /// <param name="position">Position der <see cref="Entity"/></param>
        /// <param name="cache"><see cref="ILocalChunkCache"/> as workspace</param>
        /// <param name="radius">Radius der <see cref="Entity"/></param>
        /// <param name="height">Höhe der <see cref="Entity"/></param>
        /// <param name="deltaPosition">Positionsänderung zwischen zwei Simulationsdurchläufen</param>
        /// <param name="velocity">Berechnete Geschwindigkeit</param>
        /// <exception cref="ArgumentNullException">Cache</exception>
        /// <returns>Geschwindigkeit der <see cref="Entity"/> nach der Killisionsprüfung</returns>
<<<<<<< HEAD
        public Vector3 WorldCollision(GameTime gameTime, Coordinate position, ILocalChunkCache cache, float radius, float height, Vector3 deltaPosition, Vector3 velocity)
=======
        public Vector3 WorldCollision(GameTime gameTime, Coordinate position, ILocalChunkCache cache, float radius, float height, 
            Vector3 deltaPosition, Vector3 velocity)
>>>>>>> feature/performance
        {
            if (cache == null)
                throw new ArgumentNullException(nameof(cache));

<<<<<<< HEAD
            //Blocks finden die eine Kollision verursachen könnten
            var minX = (int) Math.Floor(Math.Min(position.BlockPosition.X - radius, position.BlockPosition.X - radius + deltaPosition.X));
            var maxX = (int) Math.Ceiling(Math.Max(position.BlockPosition.X + radius, position.BlockPosition.X + radius + deltaPosition.X));
            var minY = (int) Math.Floor(Math.Min(position.BlockPosition.Y - radius, position.BlockPosition.Y - radius + deltaPosition.Y));
            var maxY = (int) Math.Ceiling(Math.Max(position.BlockPosition.Y + radius, position.BlockPosition.Y + radius + deltaPosition.Y));
            var minZ = (int) Math.Floor(Math.Min(position.BlockPosition.Z, position.BlockPosition.Z + deltaPosition.Z));
            var maxZ = (int) Math.Ceiling(Math.Max(position.BlockPosition.Z + height, position.BlockPosition.Z + height + deltaPosition.Z));
=======
            Vector3 move = deltaPosition;

            //Blocks finden die eine Kollision verursachen könnten
            int minx = (int) Math.Floor(Math.Min(
                position.BlockPosition.X - radius,
                position.BlockPosition.X - radius + deltaPosition.X));
            int maxx = (int) Math.Ceiling(Math.Max(
                position.BlockPosition.X + radius,
                position.BlockPosition.X + radius + deltaPosition.X));
            int miny = (int) Math.Floor(Math.Min(
                position.BlockPosition.Y - radius,
                position.BlockPosition.Y - radius + deltaPosition.Y));
            int maxy = (int) Math.Ceiling(Math.Max(
                position.BlockPosition.Y + radius,
                position.BlockPosition.Y + radius + deltaPosition.Y));
            int minz = (int) Math.Floor(Math.Min(
                position.BlockPosition.Z,
                position.BlockPosition.Z + deltaPosition.Z));
            int maxz = (int) Math.Ceiling(Math.Max(
                position.BlockPosition.Z + height,
                position.BlockPosition.Z + height + deltaPosition.Z));
>>>>>>> feature/performance

            //Beteiligte Flächen des Spielers
            var playerPlanes = CollisionPlane.GetEntityCollisionPlanes(radius, height, velocity, position);

<<<<<<< HEAD
            for (var z = minZ; z <= maxZ; z++)
            {
                for (var y = minY; y <= maxY; y++)
                {
                    for (var x = minX; x <= maxX; x++)
                    {
                        var move = velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
=======
            for (int z = minz; z <= maxz; z++)
            {
                for (int y = miny; y <= maxy; y++)
                {
                    for (int x = minx; x <= maxx; x++)
                    {
                        move = velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
>>>>>>> feature/performance

                        Index3 pos = new Index3(x, y, z);
                        Index3 blockPos = pos + position.GlobalBlockIndex;
                        ushort block = cache.GetBlock(blockPos);

                        if (block == 0)
                            continue;

<<<<<<< HEAD
                        var blockPlanes = CollisionPlane.GetBlockCollisionPlanes(pos, velocity);
                        
                        foreach (var playerPlane in playerPlanes)
=======
                        var blockplanes = CollisionPlane.GetBlockCollisionPlanes(pos, velocity);
                        
                        foreach (var playerPlane in playerplanes)
>>>>>>> feature/performance
                        {
                            foreach (var blockPlane in blockPlanes)
                            {
                                if (!CollisionPlane.Intersect(blockPlane, playerPlane))
                                    continue;

                                var distance = CollisionPlane.GetDistance(blockPlane, playerPlane);
                                if (!CollisionPlane.CheckDistance(distance, move))
                                    continue;

<<<<<<< HEAD
                                var subVelocity = (distance / (float)gameTime.ElapsedGameTime.TotalSeconds);
                                var diff = velocity - subVelocity;
=======
                                var subvelocity = (distance / (float)gameTime.ElapsedGameTime.TotalSeconds);
                                var diff = velocity - subvelocity;
>>>>>>> feature/performance

                                float vx;
                                float vy;
                                float vz;

<<<<<<< HEAD
                                if (blockPlane.normal.X != 0 && (velocity.X > 0 && diff.X >= 0 && subVelocity.X >= 0 ||
                                    velocity.X < 0 && diff.X <= 0 && subVelocity.X <= 0))
                                    vx = subVelocity.X;
                                else
                                    vx = velocity.X;

                                if (blockPlane.normal.Y != 0 && (velocity.Y > 0 && diff.Y >= 0 && subVelocity.Y >= 0 ||
                                    velocity.Y < 0 && diff.Y <= 0 && subVelocity.Y <= 0))
                                    vy = subVelocity.Y;
                                else
                                    vy = velocity.Y;

                                if (blockPlane.normal.Z != 0 && (velocity.Z > 0 && diff.Z >= 0 && subVelocity.Z >= 0 ||
                                    velocity.Z < 0 && diff.Z <= 0 && subVelocity.Z <= 0))
                                    vz = subVelocity.Z;
=======
                                if (blockPlane.normal.X != 0 && (velocity.X > 0 && diff.X >= 0 && subvelocity.X >= 0 ||
                                    velocity.X < 0 && diff.X <= 0 && subvelocity.X <= 0))
                                    vx = subvelocity.X;
                                else
                                    vx = velocity.X;

                                if (blockPlane.normal.Y != 0 && (velocity.Y > 0 && diff.Y >= 0 && subvelocity.Y >= 0 ||
                                    velocity.Y < 0 && diff.Y <= 0 && subvelocity.Y <= 0))
                                    vy = subvelocity.Y;
                                else
                                    vy = velocity.Y;

                                if (blockPlane.normal.Z != 0 && (velocity.Z > 0 && diff.Z >= 0 && subvelocity.Z >= 0 ||
                                    velocity.Z < 0 && diff.Z <= 0 && subvelocity.Z <= 0))
                                    vz = subvelocity.Z;
>>>>>>> feature/performance
                                else
                                    vz = velocity.Z;

                                velocity = new Vector3(vx, vy, vz);
                                
                                if (vx == 0 && vy == 0 && vz == 0)
                                    return velocity;
                            }
                        }
                    }
                }
            }
            return velocity;
        }
        /// <summary>
        /// Bietet andere Dienste.
        /// </summary>
        /// <param name="serviceType">Type of Service</param>
        /// <returns></returns>
        public object GetService(Type serviceType) => throw new NotImplementedException();
    }
}
