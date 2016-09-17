﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using FireworksNet.Extensions;
using FireworksNet.Model;

namespace FireworksNet.Selection
{
    /// <summary>
    /// Selects <see cref="Firework"/>s that will stay around for the next step:
    /// randomly chooses the <see cref="Firework"/>s, per 2012 paper.
    /// </summary>
    public class RandomFireworkSelector : FireworkSelectorBase
    {
        private readonly System.Random randomizer;

        /// <summary>
        /// Initializes a new instance of the <see cref="RandomFireworkSelector"/> class.
        /// </summary>
        /// <param name="randomizer">The random number generator.</param>
        /// <param name="locationsNumber">The number of <see cref="Firework"/>s to be selected.</param>
        /// <exception cref="System.ArgumentNullException"> if <paramref name="randomizer"/>
        /// is <c>null</c>.
        /// </exception>
        public RandomFireworkSelector(System.Random randomizer, int locationsNumber)
            : base(locationsNumber)
        {
            if (randomizer == null)
            {
                throw new ArgumentNullException(nameof(randomizer));
            }

            this.randomizer = randomizer;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RandomFireworkSelector"/> class.
        /// </summary>
        /// <param name="randomizer">The random number generator.</param>
        /// <remarks>It is assumed that number of <see cref="Firework"/>s to be selected
        /// differs from step to step and hence is passed to the <c>Select</c> method.</remarks>
        public RandomFireworkSelector(System.Random randomizer)
            : this(randomizer, 0)
        {
        }

        /// <summary>
        /// Selects <paramref name="numberToSelect"/> random <see cref="Firework"/>s from
        /// the <paramref name="from"/> collection. Selected <see cref="Firework"/>s
        /// are stored in the new collection, <paramref name="from"/> is not modified.
        /// </summary>
        /// <param name="from">A collection to select <see cref="Firework"/>s
        /// from.</param>
        /// <param name="numberToSelect">The number of <see cref="Firework"/>s
        /// to select.</param>
        /// <returns>
        /// A subset of randomly selected <see cref="Firework"/>s.
        /// </returns>
        /// <exception cref="System.ArgumentNullException"> if <paramref name="from"/>
        /// is <c>null</c>.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException"> if <paramref name="numberToSelect"/>
        /// is less than zero or greater than the number of elements in <paramref name="from"/>.
        /// </exception>
        public override IEnumerable<Firework> SelectFireworks(IEnumerable<Firework> from, int numberToSelect)
        {
            if (from == null)
            {
                throw new ArgumentNullException(nameof(from));
            }

            if (numberToSelect < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(numberToSelect));
            }

            if (numberToSelect > from.Count())
            {
                throw new ArgumentOutOfRangeException(nameof(numberToSelect));
            }

            if (numberToSelect == from.Count())
            {
                return new List<Firework>(from);
            }

            List<Firework> selectedLocations = new List<Firework>(numberToSelect);
            if (numberToSelect > 0)
            {
                // 1. Generate random indices of fireworks
                IEnumerable<int> selectedFireworkIndices = this.randomizer.NextUniqueInt32s(numberToSelect, 0, from.Count());
                Debug.Assert(selectedFireworkIndices != null, "Generated numbers collection is null");

                // 2. Take fireworks by generated indices
                int currentFirework = 0;
                foreach(Firework firework in from)
                {
                    Debug.Assert(firework != null, "Firework in null");

                    if (selectedFireworkIndices.Contains(currentFirework))
                    {
                        selectedLocations.Add(firework);
                    }

                    currentFirework++;
                }
            }

            return selectedLocations;
        }
    }
}