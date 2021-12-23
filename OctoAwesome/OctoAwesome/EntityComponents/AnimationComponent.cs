using System;
using System.IO;
using engenious;
using engenious.Graphics;
using OctoAwesome.Components;

namespace OctoAwesome.EntityComponents
{
    /// <summary>
    /// </summary>
    public class AnimationComponent : Component, IEntityComponent, IFunctionalBlockComponent
    {
        /// <summary>
        /// </summary>
        public AnimationComponent() => Sendable = true;

        /// <summary>
        /// </summary>
        public float CurrentTime { get; set; }

        /// <summary>
        /// </summary>
        public float MaxTime { get; set; }

        /// <summary>
        /// </summary>
        public float AnimationSpeed { get; set; }

        /// <summary>
        /// </summary>
        /// <param name="writer"></param>
        public override void Serialize(BinaryWriter writer)
        {
            writer.Write(CurrentTime);
            writer.Write(MaxTime);
            writer.Write(AnimationSpeed);
            base.Serialize(writer);
        }

        /// <summary>
        /// </summary>
        /// <param name="reader"></param>
        public override void Deserialize(BinaryReader reader)
        {
            CurrentTime = reader.ReadSingle();
            MaxTime = reader.ReadSingle();
            AnimationSpeed = reader.ReadSingle();
            base.Deserialize(reader);
        }

        private float NextSmallerValue(float value)
        {
            return value switch
            {
                < 0 => BitConverter.Int32BitsToSingle(BitConverter.SingleToInt32Bits(value) + 1),
                > 0 => BitConverter.Int32BitsToSingle(BitConverter.SingleToInt32Bits(value) - 1),
                _ => -float.Epsilon
            };
        }

        public void Update(GameTime gameTime, Model model)
        {
            if (model.CurrentAnimation is null)
                return;

            CurrentTime = Math.Clamp(CurrentTime + AnimationSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds, 0,
                NextSmallerValue(MaxTime));

            model.UpdateAnimation(CurrentTime);
        }
    }
}