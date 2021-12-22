using System;
using System.IO;
using engenious;
using OctoAwesome.Serialization;

namespace OctoAwesome.Definitions.Items
{
    /// <summary>
    ///     Basisklasse für alle nicht-lebendigen Spielelemente (für lebendige Spielelemente siehe <see cref="Entity" />
    /// </summary>
    public abstract class Item : IItem, IInventoryable, ISerializable
    {
        private readonly IDefinitionManager _definitionManager;

        /// <summary>
        ///     Erzeugt eine neue Instanz der Klasse Item.
        /// </summary>
        public Item(IItemDefinition definition, IMaterialDefinition material)
        {
            Definition = definition;
            Material = material;
            Condition = 99;

            _definitionManager = TypeContainer.Get<IDefinitionManager>();
        }

        public virtual int VolumePerUnit => 1;

        public virtual int StackLimit => 1;

        /// <summary>
        ///     Der Zustand des Items
        /// </summary>
        public int Condition { get; set; }

        /// <summary>
        ///     Die Koordinate, an der das Item in der Welt herumliegt, falls es nicht im Inventar ist
        /// </summary>
        public Coordinate? Position { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public IItemDefinition Definition { get; protected set; }

        /// <summary>
        /// 
        /// </summary>
        public IMaterialDefinition Material { get; protected set; }

        public virtual int Hit(IMaterialDefinition material, BlockInfo blockInfo, decimal volumeRemaining,
            int volumePerHit)
        {
            //TODO Condition Berechnung

            if (!Definition.CanMineMaterial(material))
                return 0;

            if (material is ISolidMaterialDefinition { Granularity: > 1 }) return 0;

            if (Material.Hardness * 1.2f < material.Hardness)
                return 0;

            //(Hardness Effectivity + Fracture Effectivity) / 2
            return ((Material.Hardness - material.Hardness) * 3 + 100) * volumePerHit / 100;
        }

        public virtual void Serialize(BinaryWriter writer)
        {
            writer.Write(Definition.GetType().FullName!);
            writer.Write(Material.GetType().FullName!);

            InternalSerialize(writer);
        }

        public virtual void Deserialize(BinaryReader reader)
        {
            Definition = _definitionManager.GetDefinitionByTypeName<IItemDefinition>(reader.ReadString());
            Material = _definitionManager.GetDefinitionByTypeName<IMaterialDefinition>(reader.ReadString());

            InternalDeserialize(reader);
        }

        protected void InternalSerialize(BinaryWriter writer)
        {
            writer.Write(Condition);
            writer.Write(Position.HasValue);

            if (!Position.HasValue) 
                return;

            writer.Write(Position.Value.Planet);
            writer.Write(Position.Value.GlobalBlockIndex.X);
            writer.Write(Position.Value.GlobalBlockIndex.Y);
            writer.Write(Position.Value.GlobalBlockIndex.Z);
            writer.Write(Position.Value.BlockPosition.X);
            writer.Write(Position.Value.BlockPosition.Y);
            writer.Write(Position.Value.BlockPosition.Z);
        }

        public static void Serialize(BinaryWriter writer, Item item)
        {
            writer.Write(item.Definition.GetType().FullName!);
            writer.Write(item.Material.GetType().FullName!);

            item.InternalSerialize(writer);
        }

        protected void InternalDeserialize(BinaryReader reader)
        {
            Condition = reader.ReadInt32();

            if (!reader.ReadBoolean()) 
                return;

            // Position
            var planet = reader.ReadInt32();
            var blockX = reader.ReadInt32();
            var blockY = reader.ReadInt32();
            var blockZ = reader.ReadInt32();
            var posX = reader.ReadSingle();
            var posY = reader.ReadSingle();
            var posZ = reader.ReadSingle();

            Position = new Coordinate(planet, new Index3(blockX, blockY, blockZ), new Vector3(posX, posY, posZ));
        }

        public static Item Deserialize(BinaryReader reader, Type itemType, IDefinitionManager manager)
        {
            var definition = manager.GetDefinitionByTypeName<IItemDefinition>(reader.ReadString());
            var material = manager.GetDefinitionByTypeName<IMaterialDefinition>(reader.ReadString());

            var item = Activator.CreateInstance(itemType, definition, material) as Item;

            item?.InternalDeserialize(reader);
            return item;
        }
    }
}