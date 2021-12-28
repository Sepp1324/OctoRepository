using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using OctoAwesome.Components;
using OctoAwesome.Serialization;

namespace OctoAwesome
{
    /// <summary>
    ///     Base Class for all Component based Entities.
    /// </summary>
    /// <typeparam name="T">Type of Component</typeparam>
    public class ComponentList<T> : IEnumerable<T> where T : IComponent, ISerializable
    {
        private readonly Dictionary<Type, T> _components = new();
        private readonly Action<T> _insertValidator;
        private readonly Action<T> _onInserter;
        private readonly Action<T> _onRemover;
        private readonly Action<T> _removeValidator;

        public ComponentList() { }

        public ComponentList(Action<T> insertValidator, Action<T> removeValidator, Action<T> onInserter,
            Action<T> onRemover)
        {
            _insertValidator = insertValidator;
            _removeValidator = removeValidator;
            _onInserter = onInserter;
            _onRemover = onRemover;
        }

        public T this[Type type] => _components.TryGetValue(type, out var result) ? result : default;

        public IEnumerator<T> GetEnumerator() => _components.Values.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => _components.Values.GetEnumerator();

        /// <summary>
        ///     Adds a new Component to the List.
        /// </summary>
        /// <param name="component">Component</param>
        public void AddComponent<V>(V component) where V : T => AddComponent(component, false);


        public void AddComponent<V>(V component, bool replace) where V : T
        {
            var type = component.GetType();

            if (_components.ContainsKey(type))
            {
                if (replace)
                    RemoveComponent<V>();
                else
                    return;
            }

            _insertValidator?.Invoke(component);
            _components.Add(type, component);
            _onInserter?.Invoke(component);
        }

        /// <summary>
        ///     Checks wether the component of <typeparamref name="V" /> is present in the internal dictionary as a key
        /// </summary>
        /// <typeparam name="V">The type to search in the internal dictionary</typeparam>
        /// <returns>
        ///     <list type="bullet">
        ///         <item><see langword="true" /> if the component was found</item>
        ///         <item><see langword="false" /> if the component was not found</item>
        ///     </list>
        /// </returns>
        public bool ContainsComponent<V>()
        {
            var type = typeof(V);
            if (type.IsAbstract || type.IsInterface) return _components.Any(x => type.IsAssignableFrom(x.Key));
            return _components.ContainsKey(type);
        }

        /// <summary>
        ///     Returns the Component of the given Type or null
        /// </summary>
        /// <typeparam name="V">Component Type</typeparam>
        /// <returns>Component</returns>
        public V GetComponent<V>()
        {
            if (_components.TryGetValue(typeof(V), out var result))
                return (V)(object)result;

            return default;
        }

        /// <summary>
        ///     Removes the Component of the given Type.
        /// </summary>
        /// <typeparam name="V">Component Type</typeparam>
        /// <returns></returns>
        public bool RemoveComponent<V>() where V : T
        {
            if (!_components.TryGetValue(typeof(V), out var component))
                return false;

            _removeValidator?.Invoke(component);

            if (!_components.Remove(typeof(V))) 
                return false;

            _onRemover?.Invoke(component);
            return true;

        }

        /// <summary>
        ///     Serialisiert die Entität mit dem angegebenen BinaryWriter.
        /// </summary>
        /// <param name="writer">Der BinaryWriter, mit dem geschrieben wird.</param>
        public virtual void Serialize(BinaryWriter writer)
        {
            writer.Write(_components.Count);

            foreach (var component in _components)
            {
                writer.Write(component.Key.AssemblyQualifiedName!);
                component.Value.Serialize(writer);
            }
        }

        /// <summary>
        ///     Deserialisiert die Entität aus dem angegebenen BinaryReader.
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
                    //components.Add(type, component);
                    AddComponent(component);
                }

                component.Deserialize(reader);
            }
        }
    }
}