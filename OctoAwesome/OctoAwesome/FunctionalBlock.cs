using engenious;
using OctoAwesome.Components;

namespace OctoAwesome
{
    /// <summary>
    /// Base-Class for Functional Blocks
    /// </summary>
    public abstract class FunctionalBlock : ComponentContainer<IFunctionalBlockComponent>
    {
        /// <summary>
        /// Interaction with an <see cref="Entity"/>
        /// </summary>
        /// <param name="gameTime">Current GameTime</param>
        /// <param name="entity">InteractionPartner</param>
        public void Interact(GameTime gameTime, Entity entity) => OnInteract(gameTime, entity);

        /// <summary>
        /// Event for Interaction with an <see cref="Entity"/>
        /// </summary>
        /// <param name="gameTime">Current GameTime</param>
        /// <param name="entity">InteractionPartner</param>
        protected abstract void OnInteract(GameTime gameTime, Entity entity);
    }
}