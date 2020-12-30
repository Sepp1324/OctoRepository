using System;
using System.Collections.Generic;
using System.Linq;
using OctoAwesome.Definitions;

namespace OctoAwesome.Runtime
{
    /// <summary>
    /// Definition Manager, der Typen aus Erweiterungen nachlädt.
    /// </summary>
    public class DefinitionManager : IDefinitionManager
    {
        private readonly IExtensionResolver _extensionResolver;

        public DefinitionManager(IExtensionResolver extensionResolver)
        {
            _extensionResolver = extensionResolver;

            Definitions = extensionResolver.GetDefinitions<IDefinition>().ToArray();

            // Collect Items
            ItemDefinitions = Definitions.OfType<IItemDefinition>().ToArray();
            
            // Collect Blocks
            BlockDefinitions = Definitions.OfType<IBlockDefinition>().ToArray();
            
            //Collect Materials
            MaterialDefinitions = Definitions.OfType<IMaterialDefinition>().ToArray();
        }

        /// <summary>
        /// Returns a List of Definitions.
        /// </summary>
        /// <returns></returns>
        public IDefinition[] Definitions { get; }

        /// <summary>
        /// Returns a List of all known Item-Definitions (Blocks, Resources, Tools)
        /// </summary>
        /// <returns></returns>
        public IItemDefinition[] ItemDefinitions { get; }

        /// <summary>
        /// Returns a List of all know Block-Definitions
        /// </summary>
        /// <returns></returns>
        public IBlockDefinition[] BlockDefinitions { get; }

        /// <summary>
        /// Returns a List of all known Material-Definitions
        /// </summary>
        /// <returns></returns>
        public IMaterialDefinition[] MaterialDefinitions { get; }

        /// <summary>
        /// Liefert die BlockDefinition zum angegebenen Index.
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
        /// Liefert den Index der angegebenen BlockDefinition.
        /// </summary>
        /// <param name="definition">BlockDefinition</param>
        /// <returns>Index der Block Definition</returns>
        public ushort GetDefinitionIndex(IDefinition definition) => (ushort)(Array.IndexOf(Definitions, definition) + 1);

        /// <summary>
        /// Liefert den Index der angegebenen BlockDefinition.
        /// </summary>
        /// <typeparam name="T">BlockDefinition Type</typeparam>
        /// <returns>Index der Block Definition</returns>
        public ushort GetDefinitionIndex<T>() where T : IDefinition
        {
            var definition = Definitions.SingleOrDefault(d => d.GetType() == typeof(T));
            return GetDefinitionIndex(definition);
        }

        /// <summary>
        /// Gibt die Liste von Instanzen des angegebenen Definition Interfaces zurück.
        /// </summary>
        /// <typeparam name="T">Typ der Definition</typeparam>
        /// <returns>Auflistung von Instanzen</returns>
        public IEnumerable<T> GetDefinitions<T>() where T : IDefinition => _extensionResolver.GetDefinitions<T>(); //TODO: Caching
    }
}
