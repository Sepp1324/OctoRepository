using System.Collections.Generic;
using System.Net;

namespace OctoAwesome.Definitions
{
    /// <summary>
    /// Basisinterface für einen DefinitionManager, der z.B. Erweiterungen behandelt
    /// </summary>
    public interface IDefinitionManager
    {
        /// <summary>
        /// Returns a List of Definitions.
        /// </summary>
        /// <returns></returns>
        IDefinition[] Definitions { get; }

        /// <summary>
        /// Returns a List of all known Item-Definitions (Blocks, Resources, Tools)
        /// </summary>
        /// <returns></returns>
        IItemDefinition[] ItemDefinitions { get; }

        /// <summary>
        /// Returns a List of all know Block-Definitions
        /// </summary>
        /// <returns></returns>
        IBlockDefinition[] BlockDefinitions { get; }

        /// <summary>
        /// Returns a List of all known Material-Definitions
        /// </summary>
        /// <returns></returns>
        IMaterialDefinition[] MaterialDefinitions { get; }

        /// <summary>
        /// Liefert die BlockDefinition zum angegebenen Index.
        /// </summary>
        /// <param name="index">Index der BlockDefinition</param>
        /// <returns>BlockDefinition</returns>
        IBlockDefinition GetBlockDefinitionByIndex(ushort index);

        /// <summary>
        /// Liefert den Index der angegebenen BlockDefinition.
        /// </summary>
        /// <param name="definition">BlockDefinition</param>
        /// <returns>Index der Block Definition</returns>
        ushort GetDefinitionIndex(IDefinition definition);

        /// <summary>
        /// Liefert den Index der angegebenen BlockDefinition.
        /// </summary>
        /// <typeparam name="T">BlockDefinition Type</typeparam>
        /// <returns>Index der Block Definition</returns>
        ushort GetDefinitionIndex<T>() where T : IDefinition;

        /// <summary>
        /// Gibt die Liste von Instanzen des angegebenen Definition Interfaces zurück.
        /// </summary>
        /// <typeparam name="T">Typ der Definition</typeparam>
        /// <returns>Auflistung von Instanzen</returns>
        IEnumerable<T> GetDefinitions<T>() where T : IDefinition;
    }
}
