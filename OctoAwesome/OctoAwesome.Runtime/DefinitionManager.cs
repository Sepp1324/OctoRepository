using System;
using System.Collections.Generic;
using System.Linq;

namespace OctoAwesome.Runtime
{
    /// <summary>
    /// Definition Manager, der Typen aus Erweiterungen nachlädt.
    /// </summary>
    public class DefinitionManager : IDefinitionManager
    {
        private readonly IBlockDefinition[] _blockDefinitions;
        private readonly IDefinition[] _definitions;
        private readonly IExtensionResolver _extensionResolver;
        private readonly IItemDefinition[] _itemDefinitions;

        public DefinitionManager(IExtensionResolver extensionResolver)
        {
            _extensionResolver = extensionResolver;

            _definitions = extensionResolver.GetDefinitions<IDefinition>().ToArray();

            // Items sammeln
            _itemDefinitions = _definitions.OfType<IItemDefinition>().ToArray();

            // Blöcke sammeln
            _blockDefinitions = _definitions.OfType<IBlockDefinition>().ToArray();
        }

        /// <summary>
        /// Liefert eine Liste von Defintions.
        /// </summary>
        /// <returns></returns>
        public IDefinition[] GetDefinitions() => _definitions;

        /// <summary>
        /// Liefert eine Liste aller bekannten Item Definitions (inkl. Blocks, Resources, Tools)
        /// </summary>
        /// <returns></returns>
        public IItemDefinition[] GetItemDefinitions() => _itemDefinitions;

        /// <summary>
        /// Liefert eine Liste der bekannten Blocktypen.
        /// </summary>
        /// <returns></returns>
        public IBlockDefinition[] GetBlockDefinitions() => _blockDefinitions;

        /// <summary>
        /// Liefert die BlockDefinition zum angegebenen Index.
        /// </summary>
        /// <param name="index">Index der BlockDefinition</param>
        /// <returns>BlockDefinition</returns>
        public IDefinition GetDefinitionByIndex(ushort index) => index == 0 ? null : _definitions[(index & Blocks.TypeMask) - 1];

        /// <summary>
        /// Liefert den Index der angegebenen BlockDefinition.
        /// </summary>
        /// <param name="definition">BlockDefinition</param>
        /// <returns>Index der Block Definition</returns>
        public ushort GetDefinitionIndex(IDefinition definition) => (ushort) (Array.IndexOf(_definitions, definition) + 1);

        /// <summary>
        /// Liefert den Index der angegebenen BlockDefinition.
        /// </summary>
        /// <typeparam name="T">BlockDefinition Type</typeparam>
        /// <returns>Index der Block Definition</returns>
        public ushort GetDefinitionIndex<T>() where T : IDefinition
        {
            var definition = _definitions.SingleOrDefault(d => d.GetType() == typeof(T));
            return GetDefinitionIndex(definition);
        }

        /// <summary>
        /// Gibt die Liste von Instanzen des angegebenen Definition Interfaces zurück.
        /// </summary>
        /// <typeparam name="T">Typ der Definition</typeparam>
        /// <returns>Auflistung von Instanzen</returns>
        public IEnumerable<T> GetDefinitions<T>() where T : IDefinition => _extensionResolver.GetDefinitions<T>();
    }
}