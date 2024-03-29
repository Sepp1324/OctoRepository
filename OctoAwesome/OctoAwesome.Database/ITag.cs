﻿using System;

namespace OctoAwesome.Database
{
    public interface ITag
    {
        int Length { get; }

        byte[] GetBytes();

        void FromBytes(byte[] array, int startIndex);
        void WriteBytes(Span<byte> span);
    }
}