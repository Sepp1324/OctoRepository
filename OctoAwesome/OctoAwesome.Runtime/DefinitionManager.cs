using OctoAwesome.Definitions;
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
<<<<<<< HEAD
        private readonly IExtensionResolver _extensionResolver;
=======
        private IDefinition[] definitions;

        private IItemDefinition[] itemDefinitions;

        private IBlockDefinition[] blockDefinitions;

        private IExtensionResolver extensionResolver;
>>>>>>> feature/performance

        public DefinitionManager(IExtensionResolver extensionResolver)
        {
            this.extensionResolver = extensionResolver;

<<<<<<< HEAD
            Definitions = extensionResolver.GetDefinitions<IDefinition>().ToArray();

            // collect items
            ItemDefinitions = Definitions.OfType<IItemDefinition>().ToArray();
            
            // collect blocks
            BlockDefinitions = Definitions.OfType<IBlockDefinition>().ToArray();

            // collect materials
            MaterialDefinitions = Definitions.OfType<IMaterialDefinition>().ToArray();
=======
            definitions = extensionResolver.GetDefinitions<IDefinition>().ToArray();

            // Items sammeln
            itemDefinitions = definitions.OfType<IItemDefinition>().ToArray();
            
            // Blöcke sammeln
            blockDefinitions = definitions.OfType<IBlockDefinition>().ToArray();
>>>>>>> feature/performance
        }

        /// <summary>
        /// Liefert eine Liste von Defintions.
        /// </summary>
        /// <returns></returns>
<<<<<<< HEAD
        public IDefinition[] Definitions { get; }
=======
        public IEnumerable<IDefinition> GetDefinitions()
        {
            return definitions;
        }
>>>>>>> feature/performance

        /// <summary>
        /// Liefert eine Liste aller bekannten Item Definitions (inkl. Blocks, Resources, Tools)
        /// </summary>
        /// <returns></returns>
<<<<<<< HEAD
        public IItemDefinition[] ItemDefinitions { get; }

=======
        public IEnumerable<IItemDefinition> GetItemDefinitions()
        {
            return itemDefinitions;
        }
                
>>>>>>> feature/performance
        /// <summary>
        /// Liefert eine Liste der bekannten Blocktypen.
        /// </summary>
        /// <returns></returns>
<<<<<<< HEAD
        public IBlockDefinition[] BlockDefinitions { get; }

        public IMaterialDefinition[] MaterialDefinitions { get; }
=======
        public IEnumerable<IBlockDefinition> GetBlockDefinitions()
        {
            return blockDefinitions;
        }
>>>>>>> feature/performance

        /// <summary>
        /// Liefert die BlockDefinition zum angegebenen Index.
        /// </summary>
        /// <param name="index">Index der BlockDefinition</param>
        /// <returns>BlockDefinition</returns>
        public IBlockDefinition GetBlockDefinitionByIndex(ushort index)
        {
            if (index == 0)
                return null;

<<<<<<< HEAD
            return (IBlockDefinition)Definitions[(index & Blocks.TypeMask) - 1];
=======
            return (IBlockDefinition)definitions[(index & Blocks.TypeMask) - 1];
>>>>>>> feature/performance
        }

        /// <summary>
        /// Liefert den Index der angegebenen BlockDefinition.
        /// </summary>
        /// <param name="definition">BlockDefinition</param>
        /// <returns>Index der Block Definition</returns>
<<<<<<< HEAD
        public ushort GetDefinitionIndex(IDefinition definition) => (ushort)(Array.IndexOf(Definitions, definition) + 1);
=======
        public ushort GetDefinitionIndex(IDefinition definition)
        {
            return (ushort)(Array.IndexOf(definitions, definition) + 1);
        }
>>>>>>> feature/performance

        /// <summary>
        /// Liefert den Index der angegebenen BlockDefinition.
        /// </summary>
        /// <typeparam name="T">BlockDefinition Type</typeparam>
        /// <returns>Index der Block Definition</returns>
        public ushort GetDefinitionIndex<T>() where T : IDefinition
        {
<<<<<<< HEAD
            var definition = Definitions.SingleOrDefault(d => d.GetType() == typeof(T));
=======
            IDefinition definition = definitions.SingleOrDefault(d => d.GetType() == typeof(T));
>>>>>>> feature/performance
            return GetDefinitionIndex(definition);
        }

        /// <summary>
        /// Gibt die Liste von Instanzen des angegebenen Definition Interfaces zurück.
        /// </summary>
        /// <typeparam name="T">Typ der Definition</typeparam>
        /// <returns>Auflistung von Instanzen</returns>
<<<<<<< HEAD
        public IEnumerable<T> GetDefinitions<T>() where T : class, IDefinition => _extensionResolver.GetDefinitions<T>();
=======
        public IEnumerable<T> GetDefinitions<T>() where T : IDefinition
        {
            // TODO: Caching (Generalisiertes IDefinition-Interface für Dictionary)
            return extensionResolver.GetDefinitions<T>();
        }
>>>>>>> feature/performance
    }
}
