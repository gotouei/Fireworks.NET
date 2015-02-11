﻿using System;
using System.Collections.Generic;
using FireworksNet.Model;

namespace FireworksNet.Problems.Benchmark
{
    /// <summary>
    /// Represents Rosenbrock test function, as used in 2010 paper.
    /// </summary>
    /// <remarks>http://en.wikipedia.org/wiki/Test_functions_for_optimization</remarks>
    public sealed class Rosenbrock2010 : BenchmarkProblem
    {
        private const int dimensionality = 30;
        private const double minDimensionValue = -100.0;
        private const double maxDimensionValue = 100.0;
        private const double minInitialDimensionValue = 30.0;
        private const double maxInitialDimensionValue = 50.0;
        private const double knownBestQuality = 0.0;
        private const ProblemTarget problemTarget = ProblemTarget.Minimum;

        /// <summary>
        /// Initializes a new instance of the <see cref="Rosenbrock2010"/> class.
        /// </summary>
        /// <param name="dimensions">Dimensions of the problem.</param>
        /// <param name="initialDimensionRanges">Initial dimension ranges, to be used to 
        /// create initial fireworks.</param>
        /// <param name="targetFunction">Quality function.</param>
        /// <param name="knownSolution">Known solution.</param>
        /// <param name="stopCondition">Algorithm stop condition.</param>
        /// <param name="target">Problem target.</param>
        private Rosenbrock2010(IList<Dimension> dimensions, IDictionary<Dimension, Range> initialDimensionRanges, Func<IDictionary<Dimension, double>, double> targetFunction, Solution knownSolution, ProblemTarget target)
            : base(dimensions, initialDimensionRanges, targetFunction, knownSolution, target)
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="Rosenbrock2010"/> class.
        /// </summary>
        /// <returns><see cref="Rosenbrock2010"/> instance that represents
        /// Rosenbrock test function, as used in 2010 paper.</returns>
        public static Rosenbrock2010 Create()
        {
            Dimension[] dimensions = new Dimension[Rosenbrock2010.dimensionality];
            IDictionary<Dimension, Range> initialDimensionRanges = new Dictionary<Dimension, Range>(Rosenbrock2010.dimensionality);
            IDictionary<Dimension, double> knownBestCoordinates = new Dictionary<Dimension, double>(Rosenbrock2010.dimensionality);
            for (int i = 0; i < Rosenbrock2010.dimensionality; i++)
            {
                dimensions[i] = new Dimension(new Range(Rosenbrock2010.minDimensionValue, Rosenbrock2010.maxDimensionValue));
                initialDimensionRanges.Add(dimensions[i], new Range(Rosenbrock2010.minInitialDimensionValue, Rosenbrock2010.maxInitialDimensionValue));
                knownBestCoordinates.Add(dimensions[i], 0.0);
            }

            Func<IDictionary<Dimension, double>, double> func = new Func<IDictionary<Dimension, double>, double>(
                (c) =>
                {
                    double value = 0.0;
                    for (int i = 0; i < Rosenbrock2010.dimensionality - 1; i++)
                    {
                        double dimensionCoordinate = c[dimensions[i]];
                        double nextDimensionCoordinate = c[dimensions[i + 1]];
                        value += 100 * Math.Pow(nextDimensionCoordinate - Math.Pow(dimensionCoordinate, 2.0), 2.0) + Math.Pow(dimensionCoordinate - 1.0, 2.0);
                    }

                    return value;
                }
            );

            return new Rosenbrock2010(dimensions, initialDimensionRanges, func, new Solution(knownBestCoordinates, Rosenbrock2010.knownBestQuality), Rosenbrock2010.problemTarget);
        }
    }
}