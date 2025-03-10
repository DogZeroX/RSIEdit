﻿using System.Collections.Generic;
using Importer.RSI;

namespace Importer.DMI
{
    public record DmiState(string Name, DirectionTypes Directions = DirectionTypes.None, int Frames = 1, List<float>? Delay = null)
    {
        public RsiState ToRsiState()
        {
            var delays = new List<List<float>>();

            if (Delay != null)
            {
                for (var i = 0; i < (int) Directions; i++)
                {
                    delays.Add(Delay);
                }
            }

            return new RsiState(Name, Directions, delays);
        }
    }
}
