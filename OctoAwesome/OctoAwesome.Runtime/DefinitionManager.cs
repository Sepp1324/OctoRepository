using System;
using System.Collections.Generic;
using System.Linq;
using OctoAwesome.Definitions;

namespace OctoAwesome.Runtime
{
    /// <summary>
    ///     Definition Manager, der Typen aus Erweiterungen nachlädt.
    /// </summary>
    public class DefinitionManager : IDefinitionManager
    {
        private readonly IExtensionResolver _extensionResolver;

        public DefinitionManager(IExtensionResolver extensionResolver)
        {
            _extensionResolver = extensionResolver;

            Definitions = extensionResolver.GetDefinitions<IDefinition>().ToArray();

            // collect items
            ItemDefinitions = Definitions.OfType<IItemDefinition>().ToArray();

            // collect blocks
            BlockDefinitions = Definitions.OfType<IBlockDefinition>().ToArray();

            // collect materials
            MaterialDefinitions = Definitions.OfType<IMaterialDefinition>().ToArray();
        }

        /// <summary>
        ///     Liefert eine Liste von Defintions.
        /// </summary>
        /// <returns></returns>
        public IDefinition[] Definitions { get; }

        /// <summary>
        ///     Liefert eine Liste aller bekannten Item Definitions (inkl. Blocks, Resources, Tools)
        /// </summary>
        /// <returns></returns>
        public IItemDefinition[] ItemDefinitions { get; }

        /// <summary>
        ///     Liefert eine Liste der bekannten Blocktypen.
        /// </summary>
        /// <returns></returns>
        public IBlockDefinition[] BlockDefinitions { get; }

        public IMaterialDefinition[] MaterialDefinitions { get; }

        /// <summary>
        ///     Liefert die BlockDefinition zum angegebenen Index.
        /// </summary>
        /// <param name="index">Index der BlockDefinition</param>
        /// <returns>BlockDefinition</returns>
        public IBlockDefinition GetBlockDefinitionByIndex(ushort index)
        {
            if (index == 0)
                return null;

            return (IBlockDefinition)Definitions[(index & Blocks.TypeMask) - 1];
        }

        /// <summary>
        ///     Liefert den Index der angegebenen BlockDefinition.
        /// </summary>
        /// <param name="definition">BlockDefinition</param>
        /// <returns>Index der Block Definition</returns>
        public ushort GetDefinitionIndex(IDefinition definition) => (ushort)(Array.IndexOf(Definitions, definition) + 1);

        /// <summary>
        ///     Liefert den Index der angegebenen BlockDefinition.
        /// </summary>
        /// <typeparam name="T">BlockDefinition Type</typeparam>
        /// <returns>Index der Block Definition</returns>
        public ushort GetDefinitionIndex<T>() where T : IDefinition
        {
            var i = 0;
            IDefinition definition = default;
            foreach (var d in Definitions)
                switch (i)
                {
                    case > 0 when d.GetType() == typeof(T):
                        throw new InvalidOperationException("Multiple Object where found that match the condition");
                    case 0 when d.GetType() == typeof(T):
                        definition = d;
                        ++i;
                        break;
                }

            return GetDefinitionIndex(definition);
        }

        /// <summary>
        ///     Gibt die Liste von Instanzen des angegebenen Definition Interfaces zurück.
        /// </summary>
        /// <typeparam name="T">Typ der Definition</typeparam>
        /// <returns>Auflistung von Instanzen</returns>
        public IEnumerable<T> GetDefinitions<T>() where T : class, IDefinition =>_extensionResolver.GetDefinitions<T>(); // TODO: Caching (Generalisiertes IDefinition-Interface für Dictionary (+1 von Maxi am 07.04.2021))

        public T GetDefinitionByTypeName<T>(string typeName) where T : IDefinition
        {
            var searchedType = typeof(T);
            if (typeof(IBlockDefinition).IsAssignableFrom(searchedType))
                return GetDefinitionFromArrayByTypeName<T>(typeName, BlockDefinitions);
            if (typeof(IItemDefinition).IsAssignableFrom(searchedType))
                return GetDefinitionFromArrayByTypeName<T>(typeName, ItemDefinitions);
            return typeof(IMaterialDefinition).IsAssignableFrom(searchedType) ? GetDefinitionFromArrayByTypeName<T>(typeName, MaterialDefinitions) : default;
        }

        private static T GetDefinitionFromArrayByTypeName<T>(string typeName, IDefinition[] array) where T : IDefinition
        {
            foreach (var definition in array)
                if (string.Equals(definition.GetType().FullName, typeName))
                    return (T)definition;

            return default;
        }
    }
}