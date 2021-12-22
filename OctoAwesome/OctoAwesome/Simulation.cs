using System;
using System.Collections.Generic;
using System.Linq;
using engenious;
using OctoAwesome.Common;
using OctoAwesome.Components;
using OctoAwesome.EntityComponents;
using OctoAwesome.Notifications;
using OctoAwesome.Pooling;

namespace OctoAwesome
{
    /// <summary>
    ///     Schnittstelle zwischen Applikation und Welt-Modell.
    /// </summary>
    public sealed class Simulation : INotificationObserver
    {
        private readonly List<Entity> _entities = new();
        private readonly IPool<EntityNotification> _entityNotificationPool;

        private readonly IExtensionResolver _extensionResolver;
        private readonly List<FunctionalBlock> _functionalBlocks = new();
        private readonly IDisposable _simulationSubscription;

        /// <summary>
        ///     Erzeugt eine neue Instanz der Klasse Simulation.
        /// </summary>
        public Simulation(IResourceManager resourceManager, IExtensionResolver extensionResolver, IGameService service)
        {
            ResourceManager = resourceManager;
            _simulationSubscription = resourceManager.UpdateHub.Subscribe(this, DefaultChannels.SIMULATION);
            _entityNotificationPool = TypeContainer.Get<IPool<EntityNotification>>();

            _extensionResolver = extensionResolver;
            State = SimulationState.Ready;
            UniverseId = Guid.Empty;
            Service = service;

            Components = new(ValidateAddComponent, ValidateRemoveComponent, null, null);

            extensionResolver.ExtendSimulation(this);
        }

        public IResourceManager ResourceManager { get; }

        public bool IsServerSide { get; set; }

        /// <summary>
        ///     List of all Simulation Components.
        /// </summary>
        public ComponentList<SimulationComponent> Components { get; }

        /// <summary>
        ///     Der aktuelle Status der Simulation.
        /// </summary>
        public SimulationState State { get; private set; }

        /// <summary>
        ///     Die Guid des aktuell geladenen Universums.
        /// </summary>
        public Guid UniverseId { get; }

        /// <summary>
        ///     Dienste des Spiels.
        /// </summary>
        public IGameService Service { get; }

        /// <summary>
        ///     List of all Entities.
        /// </summary>
        public IReadOnlyList<Entity> Entities => _entities;

        public IReadOnlyList<FunctionalBlock> FunctionalBlocks => _functionalBlocks;

        public void OnNext(Notification value)
        {
            if (_entities.Count < 1 && !IsServerSide)
                return;

            switch (value)
            {
                case EntityNotification entityNotification:
                    switch (entityNotification.Type)
                    {
                        case EntityNotification.ActionType.Remove:
                            RemoveEntity(entityNotification.EntityId);
                            break;
                        case EntityNotification.ActionType.Add:
                            Add(entityNotification.Entity);
                            break;
                        case EntityNotification.ActionType.Update:
                            EntityUpdate(entityNotification);
                            break;
                        case EntityNotification.ActionType.Request:
                            RequestEntity(entityNotification);
                            break;
                    }
                    break;
                case FunctionalBlockNotification functionalBlockNotification:
                    if (functionalBlockNotification.Type == FunctionalBlockNotification.ActionType.Add)
                        Add(functionalBlockNotification.Block);
                    break;
            }
        }

        public void OnError(Exception error) => throw error;

        public void OnCompleted() { }

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
        ///     Erzeugt ein neues Spiel (= Universum)
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


            var guid = ResourceManager.NewUniverse(name, numericSeed);

            Start();

            return guid;
        }

        /// <summary>
        ///     Lädt ein Spiel (= Universum).
        /// </summary>
        /// <param name="guid">Die Guid des Universums.</param>
        public bool TryLoadGame(Guid guid)
        {
            if (!ResourceManager.TryLoadUniverse(guid))
                return false;

            Start();
            return true;
        }

        private void Start()
        {
            if (State != SimulationState.Ready)
                throw new();

            State = SimulationState.Running;
        }

        /// <summary>
        ///     Updatemethode der Simulation
        /// </summary>
        /// <param name="gameTime">Spielzeit</param>
        public void Update(GameTime gameTime)
        {
            if (State != SimulationState.Running)
                return;

            foreach (var planet in ResourceManager.Planets)
                planet.Value.GlobalChunkCache.BeforeSimulationUpdate(this);

            //Update all Entities
            for (var i = 0; i < _entities.Count; i++)
            {
                var entity = _entities[i];
                if (entity is UpdateableEntity updateableEntity)
                    updateableEntity.Update(gameTime);
            }

            // Update all Components
            foreach (var component in Components)
                if (component.Enabled)
                    component.Update(gameTime);

            foreach (var planet in ResourceManager.Planets)
                planet.Value.GlobalChunkCache.AfterSimulationUpdate(this);
        }

        /// <summary>
        ///     Beendet das aktuelle Spiel (nicht die Applikation)
        /// </summary>
        public void ExitGame()
        {
            if (State != SimulationState.Running && State != SimulationState.Paused)
                throw new Exception("Simulation is not running");

            State = SimulationState.Paused;

            //TODO: unschön, Dispose Entity's, Reset Extensions
            _entities.ToList().ForEach(entity => Remove(entity));
            //while (entites.Count > 0)
            //    RemoveEntity(Entities.First());

            State = SimulationState.Finished;
            // thread.Join();

            ResourceManager.UnloadUniverse();
            _simulationSubscription?.Dispose();
        }

        /// <summary>
        ///     Fügt eine Entity der Simulation hinzu
        /// </summary>
        /// <param name="entity">Neue Entity</param>
        public void Add(Entity entity)
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

            if (entity.Id == Guid.Empty)
                entity.Id = Guid.NewGuid();

            _entities.Add(entity);

            foreach (var component in Components)
                if (component is IHoldComponent<Entity> holdComponent)
                    holdComponent.Add(entity);
        }
        public void Add(FunctionalBlock block)
        {
            if (block is null)
                throw new ArgumentNullException(nameof(block));

            if (!(State == SimulationState.Running || State == SimulationState.Paused))
                throw new NotSupportedException(
                    $"Adding {nameof(FunctionalBlock)} only allowed in running or paused state");

            if (block.Simulation != null && block.Simulation != this)
                throw new NotSupportedException($"{nameof(FunctionalBlock)} can't be part of more than one simulation");

            if (_functionalBlocks.Contains(block))
                return;

            //extensionResolver.ExtendEntity(entity);
            block.Initialize(ResourceManager);
            block.Simulation = this;

            if (block.Id == Guid.Empty)
                block.Id = Guid.NewGuid();

            _functionalBlocks.Add(block);

            foreach (var component in Components)
                if (component is IHoldComponent<FunctionalBlock> holdComponent)
                    holdComponent.Add(block);
        }

        /// <summary>
        ///     Entfernt eine Entity aus der Simulation
        /// </summary>
        /// <param name="entity">Entity die entfert werden soll</param>
        public void Remove(Entity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            if (entity.Id == Guid.Empty)
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
                if (component is IHoldComponent<Entity> holdComponent)
                    holdComponent.Remove(entity);

            _entities.Remove(entity);
            entity.Id = Guid.Empty;
            entity.Simulation = null;
        }

        public void Remove(FunctionalBlock block)
        {
            if (block == null)
                throw new ArgumentNullException(nameof(block));

            if (block.Id == Guid.Empty)
                return;

            if (block.Simulation != this)
            {
                if (block.Simulation == null)
                    return;

                throw new NotSupportedException(
                    $"{nameof(FunctionalBlock)} can't be removed from a foreign simulation");
            }

            if (!(State == SimulationState.Running || State == SimulationState.Paused))
                throw new NotSupportedException(
                    $"Removing {nameof(FunctionalBlock)} only allowed in running or paused state");

            //ResourceManager.SaveEntity(block);

            foreach (var component in Components)
                if (component is IHoldComponent<FunctionalBlock> holdComponent)
                    holdComponent.Remove(block);

            _functionalBlocks.Remove(block);
            block.Id = Guid.Empty;
            block.Simulation = null;
        }

        public void RemoveEntity(Guid entityId) => Remove(_entities.First(e => e.Id == entityId));

        public void OnUpdate(SerializableNotification notification)
        {
            if (!IsServerSide)
                ResourceManager.UpdateHub.Push(notification, DefaultChannels.NETWORK);
        }

        private void EntityUpdate(EntityNotification notification)
        {
            var entity = _entities.FirstOrDefault(e => e.Id == notification.EntityId);
            if (entity == null)
            {
                var entityNotification = _entityNotificationPool.Get();
                entityNotification.EntityId = notification.EntityId;
                entityNotification.Type = EntityNotification.ActionType.Request;
                ResourceManager.UpdateHub.Push(entityNotification, DefaultChannels.NETWORK);
                entityNotification.Release();
            }
            else
            {
                entity.Push(notification.Notification);
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
            remoteEntity.Components.AddComponent(new BodyComponent { Mass = 50f, Height = 2f, Radius = 1.5f });
            remoteEntity.Components.AddComponent(new RenderComponent { Name = "Wauzi", ModelName = "dog", TextureName = "texdog", BaseZRotation = -90 }, true);
            remoteEntity.Components.AddComponent(new PositionComponent { Position = new(0, new(0, 0, 78), new(0, 0)) });

            var newEntityNotification = _entityNotificationPool.Get();
            newEntityNotification.Entity = remoteEntity;
            newEntityNotification.Type = EntityNotification.ActionType.Add;

            ResourceManager.UpdateHub.Push(newEntityNotification, DefaultChannels.NETWORK);
            newEntityNotification.Release();
        }
    }
}