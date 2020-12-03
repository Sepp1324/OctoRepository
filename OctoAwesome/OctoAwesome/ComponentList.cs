using OctoAwesome.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace OctoAwesome
{
    /// <summary>
    /// Base Class for all Component based Entities.
    /// </summary>
    /// <typeparam name="T">Type of Component</typeparam>
    public class ComponentList<T> : IEnumerable<T> where T : Component, ISerializable
    {
        private readonly Action<T> _insertValidator;
        private readonly Action<T> _removeValidator;
        private readonly Action<T> _onInserter;
        private readonly Action<T> _onRemover;

        private readonly Dictionary<Type, T> _components = new Dictionary<Type, T>();

        public T this[Type type]
        {
            get
            {
                if (_components.TryGetValue(type, out T result))
                    return result;

                return null;
            }
        }

        public ComponentList()
        {
        }

        public ComponentList(Action<T> insertValidator, Action<T> removeValidator, Action<T> onInserter, Action<T> onRemover)
        {
            _insertValidator = insertValidator;
            _removeValidator = removeValidator;
            _onInserter = onInserter;
            _onRemover = onRemover;
        }

        public IEnumerator<T> GetEnumerator() => _components.Values.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => _components.Values.GetEnumerator();

        /// <summary>
        /// Adds a new Component to the List.
        /// </summary>
        /// <param name="component">Component</param>
        public void AddComponent<V>(V component) where V : T => AddComponent(component, false);


        public void AddComponent<V>(V component, bool replace) where V : T
        {
            var type = component.GetType();

            if (_components.ContainsKey(type))
            {
                if (replace)
                {
                    RemoveComponent<V>();
                }
                else
                {
                    return;
                }
            }

            _insertValidator?.Invoke(component);
            _components.Add(type, component);
            _onInserter?.Invoke(component);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <returns></returns>
        public bool ContainsComponent<V>() => _components.ContainsKey(typeof(V));

        /// <summary>
        /// Returns the Component of the given Type or null
        /// </summary>
        /// <typeparam name="V">Component Type</typeparam>
        /// <returns>Component</returns>
        public V GetComponent<V>() where V : T
        {
            if (_components.TryGetValue(typeof(V), out T result))
                return (V)result;

            return null;
        }

        /// <summary>
        /// Removes the Component of the given Type.
        /// </summary>
        /// <typeparam name="V">Component Type</typeparam>
        /// <returns></returns>
        public bool RemoveComponent<V>() where V : T
        {
            if (!_components.TryGetValue(typeof(V), out T component))
                return false;

            _removeValidator?.Invoke(component);
           
            if (_components.Remove(typeof(V)))
            {
                _onRemover?.Invoke(component);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Serialisiert die Entität mit dem angegebenen BinaryWriter.
        /// </summary>
        /// <param name="writer">Der BinaryWriter, mit dem geschrieben wird.</param>
        public virtual void Serialize(BinaryWriter writer)
        {
            writer.Write(_components.Count);
          
            foreach (var component in _components)
            {
                writer.Write(component.Key.AssemblyQualifiedName);
                component.Value.Serialize(writer);
            }
        }

        /// <summary>
        /// Deserialisiert die Entität aus dem angegebenen BinaryReader.
        /// </summary>
        /// <param name="reader">Der BinaryWriter, mit dem gelesen wird.</param>
        public virtual void Deserialize(BinaryReader reader)
        {
            var count = reader.ReadInt32();
           
            for (var i = 0; i < count; i++)
            {
                var name = reader.ReadString();

                var type = Type.GetType(name);

                if (!_components.TryGetValue(type, out var component))
                {
                    component = (T)TypeContainer.GetUnregistered(type);
                    AddComponent(component);
                }
                component.Deserialize(reader);
            }
        }
    }
}
