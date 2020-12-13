﻿using engenious;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome
{
    /// <summary>
    /// Entity die regelmäßig eine Updateevent bekommt
    /// </summary>
    public abstract class UpdateableEntity : Entity
    {
        /// <summary>
        /// Entity die regelmäßig eine Updateevent bekommt
        /// </summary>
        public UpdateableEntity() : base()
        {
        }

        /// <summary>
        /// Updatemethode der Entity
        /// </summary>
        /// <param name="gameTime">Spielzeit</param>
        public abstract void Update(GameTime gameTime);
        
    }
}
