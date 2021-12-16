using NonSucking.Framework.Extension.Rx.SumTypes;

namespace OctoAwesome.SumTypes
{
    public class Selection : Variant<BlockInfo, FunctionalBlock, Entity>
    {
        public Selection(BlockInfo value) : base(value)
        {
        }

        public Selection(FunctionalBlock value) : base(value)
        {
        }

        public Selection(Entity value) : base(value)
        {
        }

        public static implicit operator Selection(BlockInfo obj)
        {
            return new(obj);
        }

        public static implicit operator Selection(FunctionalBlock obj)
        {
            return new(obj);
        }

        public static implicit operator Selection(Entity obj)
        {
            return new(obj);
        }
    }
}