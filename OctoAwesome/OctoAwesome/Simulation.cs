using engenious;
using OctoAwesome.Common;
using OctoAwesome.EntityComponents;
using OctoAwesome.Notifications;
using OctoAwesome.Pooling;
using System;
using System.Collections.Generic;
using System.Linq;
using OctoAwesome.Database;

namespace OctoAwesome
{
    /// <summary>
    /// Schnittstelle zwischen Applikation und Welt-Modell.
    /// </summary>
    public sealed class Simulation : INotificationObserver
    {
        public IResourceManager ResourceManager { get; private set; }

        public bool IsServerSide { get; set; }

        /// <summary>
        /// List of all Simulation Components.
        /// </summary>
        public ComponentList<SimulationComponent> Components { get; private set; }

        /// <summary>
        /// Der aktuelle Status der Simulation.
        /// </summary>
        public SimulationState State { get; private set; }

        /// <summary>
        /// Die Guid des aktuell geladenen Universums.
        /// </summary>
        public Guid UniverseId { get; private set; }

        /// <summary>
        /// Dienste des Spiels.
        /// </summary>
        public IGameService Service { get; }

        /// <summary>
        /// List of all Entities.
        /// </summary>
        public List<Entity> Entities => _entities.ToList();

        private readonly IExtensionResolver _extensionResolver;
        private readonly HashSet<Entity> _entities = new HashSet<Entity>();
        private readonly IDisposable _simulationSubscription;
        private readonly IPool<EntityNotification> _entityNotificationPool;
        
        private IdManager _entityIdManager;

        /// <summary>
        /// Erzeugt eine neue Instanz der Klasse Simulation.
        /// </summary>
        public Simulation(IResourceManager resourceManager, IExtensionResolver extensionResolver, IGameService service)
        {
            ResourceManager = resourceManager;
            _simulationSubscription = resourceManager.UpdateHub.Subscribe(this, DefaultChannels.Simulation);
            _entityNotificationPool = TypeContainer.Get<IPool<EntityNotification>>();

            _extensionResolver = extensionResolver;
            State = SimulationState.Ready;
            UniverseId = Guid.Empty;
            Service = service;

            Components = new ComponentList<SimulationComponent>(
                ValidateAddComponent, ValidateRemoveComponent, null, null);

            extensionResolver.ExtendSimulation(this);
        }

        private void ValidateAddComponent(SimulationComponent component)
        {
            if (State != SimulationState.Ready)
                throw new NotSupportedException("Simulation needs to be in Ready mode to add Components");
        }

        private void ValidateRemoveComponent(SimulationComponent component)
        {
            if (State != SimulationState.Ready)
                throw new NotSupportedException("Simulation needs to be in Ready mode to remove Components");
        }

        /// <summary>
        /// Erzeugt ein neues Spiel (= Universum)
        /// </summary>
        /// <param name="name">Name des Universums.</param>
        /// <param name="rawSeed">Seed für den Weltgenerator.</param>
        /// <returns>Die Guid des neuen Universums.</returns>
        public Guid NewGame(string name, string rawSeed)
        {
            int numericSeed;

            if (string.IsNullOrWhiteSpace(rawSeed))
            {
                var rand = new Random();
                numericSeed = rand.Next(int.MaxValue);
            }
            else if (int.TryParse(rawSeed, out var seed))
            {
                numericSeed = seed;
            }
            else
            {
                numericSeed = rawSeed.GetHashCode();
            }

            Guid guid = ResourceManager.NewUniverse(name, numericSeed);

            Start();

            return guid;
        }

        /// <summary>
        /// Lädt ein Spiel (= Universum).
        /// </summary>
        /// <param name="guid">Die Guid des Universums.</param>
        public void LoadGame(Guid guid)
        {
            ResourceManager.LoadUniverse(guid);
            Start();
        }

        private void Start()
        {
            if (State != SimulationState.Ready)
                throw new Exception();

            _entityIdManager = new IdManager(ResourceManager.GetEntityIds());
            State = SimulationState.Running;
        }

        /// <summary>
        /// Updatemethode der Simulation
        /// </summary>
        /// <param name="gameTime">Spielzeit</param>
        public void Update(GameTime gameTime)
        {
            if (State != SimulationState.Running)
                return;

            foreach (var planet in ResourceManager.Planets)
                planet.Value.GlobalChunkCache.BeforeSimulationUpdate(this);

            //Update all Entities
            foreach (var entity in Entities.OfType<UpdateableEntity>())
                entity.Update(gameTime);

            // Update all Components
            foreach (var component in Components.Where(c => c.Enabled))
                component.Update(gameTime);

            foreach (var planet in ResourceManager.Planets)
                planet.Value.GlobalChunkCache.AfterSimulationUpdate(this);
        }

        /// <summary>
        /// Beendet das aktuelle Spiel (nicht die Applikation)
        /// </summary>
        public void ExitGame()
        {
            if (State != SimulationState.Running && State != SimulationState.Paused)
                throw new Exception("Simulation is not running");

            State = SimulationState.Paused;

            //TODO: unschön
            Entities.ForEach(entity => RemoveEntity(entity));
            State = SimulationState.Finished;
            ResourceManager.UnloadUniverse();
        }

        /// <summary>
        /// Fügt eine Entity der Simulation hinzu
        /// </summary>
        /// <param name="entity">Neue Entity</param>
        public void AddEntity(Entity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            if (!(State == SimulationState.Running || State == SimulationState.Paused))
                throw new NotSupportedException("Adding Entities only allowed in running or paused state");

            if (entity.Simulation != null && entity.Simulation != this)
                throw new NotSupportedException("Entity can't be part of more than one simulation");

            if (_entities.Contains(entity))
                return;

            _extensionResolver.ExtendEntity(entity);
            entity.Initialize(ResourceManager);
            entity.Simulation = this;

            if (entity.Id == -1)
                entity.Id = _entityIdManager.GetId();
            else
                _entityIdManager.ReserveId(entity.Id);

            _entities.Add(entity);

            foreach (var component in Components)
                component.Add(entity);
        }

        /// <summary>
        /// Entfernt eine Entity aus der Simulation
        /// </summary>
        /// <param name="entity">Entity die entfert werden soll</param>
        public void RemoveEntity(Entity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            if (entity.Id == 0)
                return;

            if (entity.Simulation != this)
            {
                if (entity.Simulation == null)
                    return;

                throw new NotSupportedException("Entity can't be removed from a foreign simulation");
            }

            if (!(State == SimulationState.Running || State == SimulationState.Paused))
                throw new NotSupportedException("Removing Entities only allowed in running or paused state");

            ResourceManager.SaveEntity(entity);

            foreach (var component in Components)
                component.Remove(entity);

            _entities.Remove(entity);
            _entityIdManager.ReleaseId(entity.Id);
            entity.Id = -1;
            entity.Simulation = null;
        }

        public void RemoveEntity(int entityId) => RemoveEntity(_entities.First(e => e.Id == entityId));

        public void OnNext(Notification value)
        {
            if (_entities.Count < 1 && !IsServerSide)
                return;

            switch (value)
            {
                case EntityNotification entityNotification:
                    if (entityNotification.Type == EntityNotification.ActionType.Remove)
                        RemoveEntity(entityNotification.EntityId);
                    else if (entityNotification.Type == EntityNotification.ActionType.Add)
                        AddEntity(entityNotification.Entity);
                    else if (entityNotification.Type == EntityNotification.ActionType.Update)
                        EntityUpdate(entityNotification);
                    else if (entityNotification.Type == EntityNotification.ActionType.Request)
                        RequestEntity(entityNotification);
                    break;
                default:
                    break;
            }
        }

        public void OnError(Exception error) => throw error;

        public void OnCompleted()
        {
        }

        public void OnUpdate(SerializableNotification notification)
        {
            if (!IsServerSide)
                ResourceManager.UpdateHub.Push(notification, DefaultChannels.Network);
        }

        private void EntityUpdate(EntityNotification notification)
        {
            var entity = _entities.FirstOrDefault(e => e.Id == notification.EntityId);
            if (entity == null)
            {
                var entityNotification = _entityNotificationPool.Get();
                entityNotification.EntityId = notification.EntityId;
                entityNotification.Type = EntityNotification.ActionType.Request;
                ResourceManager.UpdateHub.Push(entityNotification, DefaultChannels.Network);
                entityNotification.Release();
            }
            else
            {
                entity.Update(notification.Notification);
            }
        }

        private void RequestEntity(EntityNotification entityNotification)
        {
            if (!IsServerSide)
                return;

            var entity = _entities.FirstOrDefault(e => e.Id == entityNotification.EntityId);

            if (entity == null)
                return;

            var remoteEntity = new RemoteEntity(entity);
            remoteEntity.Components.AddComponent(new BodyComponent() { Mass = 50f, Height = 2f, Radius = 1.5f });
            remoteEntity.Components.AddComponent(new RenderComponent() { Name = "Wauzi", ModelName = "dog", TextureName = "texdog", BaseZRotation = -90 }, true);
            remoteEntity.Components.AddComponent(new PositionComponent() { Position = new Coordinate(0, new Index3(0, 0, 78), new Vector3(0, 0, 0)) });

            var newEntityNotification = _entityNotificationPool.Get();
            newEntityNotification.Entity = remoteEntity;
            newEntityNotification.Type = EntityNotification.ActionType.Add;

            ResourceManager.UpdateHub.Push(newEntityNotification, DefaultChannels.Network);
            newEntityNotification.Release();
        }
    }
}
