﻿using Microsoft.Xna.Framework;
using OctoAwesome.Runtime;
using System;
using System.Collections.Generic;

namespace OctoAwesome.Client.Components
{
    public class ActorProxy : IPlayerController
    {
        public Coordinate Position => throw new NotImplementedException();

        public float Radius => throw new NotImplementedException();

        public float Angle => throw new NotImplementedException();

        public float Height => throw new NotImplementedException();

        public bool OnGround => throw new NotImplementedException();

        public bool FlyMode { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public float Tilt => throw new NotImplementedException();

        public Vector2 Move { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Vector2 Head { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public IEnumerable<InventorySlot> Inventory => throw new NotImplementedException();

        public void Apply(Index3 blockIndex, InventorySlot tool, OrientationFlags orientation)
        {
            throw new NotImplementedException();
        }

        public void Interact(Index3 blockIndex)
        {
            throw new NotImplementedException();
        }

        public void Jump()
        {
            throw new NotImplementedException();
        }
    }
}
