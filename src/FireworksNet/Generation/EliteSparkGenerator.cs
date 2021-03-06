﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using FireworksNet.Fit;
using FireworksNet.Model;

namespace FireworksNet.Generation
{
    /// <summary>
    /// Elite strategy spark generator, as described in 2012 paper.
    /// </summary>
    public abstract class EliteSparkGenerator : SparkGeneratorBase<EliteExplosion>
    {
        private readonly IEnumerable<Dimension> dimensions;
        private readonly IFit polynomialFit;

        /// <summary>
        /// Gets the type of the generated spark.
        /// </summary>
        public override FireworkType GeneratedSparkType { get { return FireworkType.EliteFirework; } }

        /// <summary>
        /// Initializes a new instance of the <see cref="EliteSparkGenerator"/> class.
        /// </summary>
        /// <param name="dimensions">The dimensions to fit generated sparks into.</param>
        /// <param name="polynomialFit">The polynomial fit.</param>
        protected EliteSparkGenerator(IEnumerable<Dimension> dimensions, IFit polynomialFit)
        {
            if (dimensions == null)
            {
                throw new ArgumentNullException(nameof(dimensions));
            }

            if (polynomialFit == null)
            {
                throw new ArgumentNullException(nameof(polynomialFit));
            }

            this.dimensions = dimensions;
            this.polynomialFit = polynomialFit;
        }

        /// <summary>
        /// Calculates elite point.
        /// </summary>
        /// <param name="func">The function to calculate elite point.</param>
        /// <param name="variationRange">Represents an interval to calculate 
        /// elite point.</param>
        /// <returns>The coordinate of elite point.</returns>
        protected abstract double CalculateElitePoint(Func<double, double> func, Range variationRange);

        /// <summary>
        /// Creates the spark from the explosion.
        /// </summary>
        /// <param name="explosion">The explosion that contains the collection 
        /// of sparks.</param>
        /// <param name="birthOrder">The number of spark in the collection of sparks born by
        /// this generator within one step.</param>
        /// <returns>A spark for the specified explosion.</returns>
        /// <exception cref="System.ArgumentNullException"> if <paramref name="explosion"/>
        /// is <c>null</c>.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException"> if <paramref name="birthOrder"/>
        /// is less than zero.</exception>
        public override Firework CreateSpark(EliteExplosion explosion, int birthOrder)
        {
            if (explosion == null)
            {
                throw new ArgumentNullException(nameof(explosion));
            }

            if (birthOrder < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(birthOrder));
            }

            Debug.Assert(explosion.Fireworks != null, "Fireworks collection is null");
            Debug.Assert(this.dimensions != null, "Dimension collection is null");
            Debug.Assert(this.polynomialFit != null, "Polynomial fit is null");

            IDictionary<Dimension, Func<double, double>> fitnessLandscapes = this.ApproximateFitnessLandscapes(explosion.Fireworks);
            IDictionary<Dimension, double> elitePointCoordinates = new Dictionary<Dimension, double>();

            foreach (KeyValuePair<Dimension, Func<double, double>> fitnessLandscape in fitnessLandscapes)
            {
                double coordinate = this.CalculateElitePoint(fitnessLandscape.Value, fitnessLandscape.Key.VariationRange);
                elitePointCoordinates[fitnessLandscape.Key] = coordinate;
            }

            return new Firework(this.GeneratedSparkType, explosion.StepNumber, birthOrder, elitePointCoordinates);
        }

        /// <summary>
        /// Approximates fitness landscape in each one dimensional search space.
        /// </summary>
        /// <param name="fireworks">The collection of <see cref="Firework"/>s to obtain
        /// coordinates in each one dimensional search space.</param>
        /// <returns>A map. Key is a <see cref="Dimension"/>. Value is a approximated
        /// curve by coordinates and qualities of <see cref="Firework"/>s.</returns>
        /// <exception cref="System.ArgumentNullException"> if <paramref name="fireworks"/>
        /// is <c>null</c>.</exception>
        protected virtual IDictionary<Dimension, Func<double, double>> ApproximateFitnessLandscapes(IEnumerable<Firework> fireworks)
        {
            if (fireworks == null)
            {
                throw new ArgumentNullException(nameof(fireworks));
            }

            IList<Firework> currentFireworks = new List<Firework>(fireworks);

            double[] qualities = new double[currentFireworks.Count];

            int current = 0;
            foreach (Firework firework in currentFireworks)
            {
                qualities[current] = firework.Quality;
                current++;
            }

            Dictionary<Dimension, Func<double, double>> fitnessLandscapes = new Dictionary<Dimension, Func<double, double>>();

            foreach (Dimension dimension in this.dimensions)
            {
                double[] coordinates = new double[currentFireworks.Count];

                current = 0;
                foreach (Firework firework in currentFireworks)
                {
                    coordinates[current] = firework.Coordinates[dimension];
                    current++;
                }

                fitnessLandscapes[dimension] = this.polynomialFit.Approximate(coordinates, qualities);
            }

            return fitnessLandscapes;
        }
    }
}