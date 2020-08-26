﻿using engenious;
using OctoAwesome.EntityComponents;
using OctoAwesome.Notifications;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

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
        /// List of all Entities.
        /// </summary>
        public List<Entity> Entities => entities.ToList();

        private int nextId = 1;

        private readonly IExtensionResolver extensionResolver;

        private readonly HashSet<Entity> entities = new HashSet<Entity>();
        private readonly IDisposable simmulationSubscription;

        /// <summary>
        /// Erzeugt eine neue Instaz der Klasse Simulation.
        /// </summary>
        public Simulation(IResourceManager resourceManager, IExtensionResolver extensionResolver)
        {
            ResourceManager = resourceManager;
            simmulationSubscription = resourceManager.UpdateHub.Subscribe(this, DefaultChannels.Simulation);

            this.extensionResolver = extensionResolver;
            State = SimulationState.Ready;

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
        /// <param name="seed">Seed für den Weltgenerator.</param>
        /// <returns>Die Guid des neuen Universums.</returns>
        public Guid NewGame(string name, int? seed = null)
        {
            if (seed == null)
            {
                var rand = new Random();
                seed = rand.Next(int.MaxValue);
            }

            Guid guid = ResourceManager.NewUniverse(name, seed.Value);

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

            State = SimulationState.Running;
        }

        /// <summary>
        /// Updatemethode der Simulation
        /// </summary>
        /// <param name="gameTime">Spielzeit</param>
        public void Update(GameTime gameTime)
        {
            if (State == SimulationState.Running)
            {
                ResourceManager.GlobalChunkCache.BeforeSimulationUpdate(this);

                //Update all Entities
                foreach (var entity in Entities.OfType<UpdateableEntity>())
                    entity.Update(gameTime);

                // Update all Components
                foreach (var component in Components.Where(c => c.Enabled))
                    component.Update(gameTime);

                ResourceManager.GlobalChunkCache.AfterSimulationUpdate(this);
            }
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
            //while (entites.Count > 0)
            //    RemoveEntity(Entities.First());

            State = SimulationState.Finished;
            // thread.Join();

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

            if (entity.Simulation != null)
                throw new NotSupportedException("Entity can't be part of more than one simulation");

            if (entities.Contains(entity))
                return;

            extensionResolver.ExtendEntity(entity);
            entity.Initialize(ResourceManager);
            entity.Simulation = this;

            if (entity.Id == 0)
                entity.Id = nextId++;
            else
                nextId++;

            entities.Add(entity);

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
                throw new NotSupportedException("Adding Entities only allowed in running or paused state");

            foreach (var component in Components)
                component.Remove(entity);

            entities.Remove(entity);
            entity.Id = 0;
            entity.Simulation = null;

            ResourceManager.SaveEntity(entity);
        }
        public void RemoveEntity(int entityId)
            => RemoveEntity(entities.First(e => e.Id == entityId));

        public void OnNext(Notification value)
        {
            if (entities.Count < 1 && !IsServerSide)
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

        public void OnError(Exception error)
        {
            throw error;
        }

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
            var entity = entities.FirstOrDefault(e => e.Id == notification.EntityId);
            if (entity == null)
            {
                ResourceManager.UpdateHub.Push(new EntityNotification(notification.EntityId)
                {
                    Type = EntityNotification.ActionType.Request
                }, DefaultChannels.Network);
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

            var entity = entities.FirstOrDefault(e => e.Id == entityNotification.EntityId);

            if (entity == null)
                return;

            var remoteEntity = new RemoteEntity(entity);
            remoteEntity.Components.AddComponent(new PositionComponent { Position = new Coordinate(0, new Index3(0, 0, 78), new Vector3(0, 0, 0)) });
            remoteEntity.Components.AddComponent(new RenderComponent { Name = "Wauzi", ModelName = "dog", TextureName = "texdog", BaseZRotation = -90 }, true);
            remoteEntity.Components.AddComponent(new BodyComponent() { Mass = 50f, Height = 2f, Radius = 1.5f });

            ResourceManager.UpdateHub.Push(new EntityNotification()
            {
                Entity = remoteEntity,
                Type = EntityNotification.ActionType.Add
            }, DefaultChannels.Network);
        }
    }
}
