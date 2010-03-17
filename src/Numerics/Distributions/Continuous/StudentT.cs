// <copyright file="StudentT.cs" company="Math.NET">
// Math.NET Numerics, part of the Math.NET Project
// http://mathnet.opensourcedotnet.info
//
// Copyright (c) 2009 Math.NET
//
// Permission is hereby granted, free of charge, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use,
// copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following
// conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
// </copyright>

namespace MathNet.Numerics.Distributions
{
    using System;
    using System.Collections.Generic;
    using Properties;

    /// <summary>
    /// Implements the univariate Student t-distribution. For details about this distribution, see 
    /// <a href="http://en.wikipedia.org/wiki/Student%27s_t-distribution">Wikipedia - Student's t-distribution</a>.
    /// </summary>
    /// <remarks><para>We use a slightly generalized version (compared to Wikipedia) of the Student t-distribution.
    /// Namely, one which also parameterizes the location and scale. See the book "Bayesian Data Analysis" for more
    /// details.</para>
    /// <para>The distribution will use the <see cref="System.Random"/> by default. 
    /// Users can get/set the random number generator by using the <see cref="RandomSource"/> property.</para>
    /// <para>The statistics classes will check all the incoming parameters whether they are in the allowed
    /// range. This might involve heavy computation. Optionally, by setting Control.CheckDistributionParameters
    /// to false, all parameter checks can be turned off.</para></remarks>
    public class StudentT : IContinuousDistribution
    {
        /// <summary>
        /// Keeps track of the location of the Student t-distribution.
        /// </summary>
        private double _location;

        /// <summary>
        /// Keeps track of the degrees of freedom for the Student t-distribution.
        /// </summary>
        private double _dof;

        /// <summary>
        /// Keeps track of the scale for the Student t-distribution.
        /// </summary>
        private double _scale;

        /// <summary>
        /// The distribution's random number generator.
        /// </summary>
        private Random _random;

        /// <summary>
        /// Initializes a new instance of the StudentT class. This is a Student t-distribution with location 0.0
        /// scale 1.0 and degrees of freedom 1. The distribution will
        /// be initialized with the default <seealso cref="System.Random"/> random number generator.
        /// </summary>
        public StudentT() : this(0.0, 1.0, 1.0)
        {
        }

        /// <summary>
        /// Initializes a new instance of the StudentT class with a particular location, scale and degrees of
        /// freedom. The distribution will
        /// be initialized with the default <seealso cref="System.Random"/> random number generator.
        /// </summary>
        /// <param name="location">The location of the Student t-distribution.</param>
        /// <param name="scale">The scale of the Student t-distribution.</param>
        /// <param name="dof">The degrees of freedom for the Student t-distribution.</param>
        public StudentT(double location, double scale, double dof)
        {
            SetParameters(location, scale, dof);
            RandomSource = new Random();
        }

        /// <summary>
        /// A string representation of the distribution.
        /// </summary>
        /// <returns>a string representation of the distribution.</returns>
        public override string ToString()
        {
            return "StudentT(Location = " + _location + ", Scale = " + _scale + ", DoF = " + _dof + ")";
        }

        /// <summary>
        /// Checks whether the parameters of the distribution are valid. 
        /// </summary>
        /// <param name="location">The location of the Student t-distribution.</param>
        /// <param name="scale">The scale of the Student t-distribution.</param>
        /// <param name="dof">The degrees of freedom for the Student t-distribution.</param>
        /// <returns>True when the parameters are valid, false otherwise.</returns>
        private static bool IsValidParameterSet(double location, double scale, double dof)
        {
            if (scale <= 0.0 || dof <= 0.0 || Double.IsNaN(scale) || Double.IsNaN(location) || Double.IsNaN(dof))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Sets the parameters of the distribution after checking their validity.
        /// </summary>
        /// <param name="location">The location of the Student t-distribution.</param>
        /// <param name="scale">The scale of the Student t-distribution.</param>
        /// <param name="dof">The degrees of freedom for the Student t-distribution.</param>
        /// <exception cref="ArgumentOutOfRangeException">When the parameters don't pass the <see cref="IsValidParameterSet"/> function.</exception>
        private void SetParameters(double location, double scale, double dof)
        {
            if (Control.CheckDistributionParameters && !IsValidParameterSet(location, scale, dof))
            {
                throw new ArgumentOutOfRangeException(Resources.InvalidDistributionParameters);
            }

            _location = location;
            _scale = scale;
            _dof = dof;
        }

        /// <summary>
        /// Gets or sets the location of the Student t-distribution.
        /// </summary>
        public double Location
        {
            get
            {
                return _location;
            }

            set
            {
                SetParameters(value, _scale, _dof);
            }
        }

        /// <summary>
        /// Gets or sets the scale of the Student t-distribution.
        /// </summary>
        public double Scale
        {
            get
            {
                return _scale;
            }

            set
            {
                SetParameters(_location, value, _dof);
            }
        }

        /// <summary>
        /// Gets or sets the degrees of freedom of the Student t-distribution.
        /// </summary>
        public double DegreesOfFreedom
        {
            get
            {
                return _dof;
            }

            set
            {
                SetParameters(_location, _scale, value);
            }
        }

        #region IDistribution implementation

        /// <summary>
        /// Gets or sets the random number generator which is used to draw random samples.
        /// </summary>
        public Random RandomSource
        {
            get
            {
                return _random;
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException();
                }

                _random = value;
            }
        }

        /// <summary>
        /// Gets or sets the mean of the Student t-distribution.
        /// </summary>
        public double Mean
        {
            get { return _location; }
        }

        /// <summary>
        /// Gets or sets the variance of the Student t-distribution.
        /// </summary>
        public double Variance
        {
            get
            {
                if (_dof > 2.0)
                {
                    return _dof / (_dof - 2.0) / _scale;
                }
                else if (_dof > 1.0)
                {
                    return Double.PositiveInfinity;
                }
                else
                {
                    throw new Exception(Resources.UndefinedMoment);
                }
            }
        }

        /// <summary>
        /// Gets or sets the standard deviation of the Student t-distribution.
        /// </summary>
        public double StdDev
        {
            get
            {
                if (_dof > 2.0)
                {
                    return Math.Sqrt(_dof / (_dof - 2.0));
                }
                else if (_dof > 1.0)
                {
                    return Double.PositiveInfinity;
                }
                else
                {
                    throw new Exception(Resources.UndefinedMoment);
                }
            }
        }

        /// <summary>
        /// Gets the entropy of the Student t-distribution.
        /// </summary>
        public double Entropy
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets the skewness of the Student t-distribution.
        /// </summary>
        public double Skewness
        {
            get { throw new NotImplementedException(); }
        }
        #endregion

        #region IContinuousDistribution implementation

        /// <summary>
        /// Gets the mode of the Student t-distribution.
        /// </summary>
        public double Mode
        {
            get { return _location; }
        }

        /// <summary>
        /// Gets the median of the Student t-distribution.
        /// </summary>
        public double Median
        {
            get { return _location; }
        }

        /// <summary>
        /// Gets the minimum of the Student t-distribution.
        /// </summary>
        public double Minimum
        {
            get { return Double.NegativeInfinity; }
        }

        /// <summary>
        /// Gets the maximum of the Student t-distribution.
        /// </summary>
        public double Maximum
        {
            get { return Double.PositiveInfinity; }
        }

        /// <summary>
        /// Computes the density of the Student t-distribution.
        /// </summary>
        /// <param name="x">The location at which to compute the density.</param>
        /// <returns>the density at <paramref name="x"/>.</returns>
        public double Density(double x)
        {
            double d = (x - _location) / _scale;
            return SpecialFunctions.Gamma((_dof + 1.0) / 2.0)
                * Math.Pow(1.0 + d * d / _dof, -0.5 * (_dof + 1.0))
                / SpecialFunctions.Gamma(_dof / 2.0)
                / Math.Sqrt(_dof * Math.PI)
                / _scale;
        }

        /// <summary>
        /// Computes the log density of the Student t-distribution.
        /// </summary>
        /// <param name="x">The location at which to compute the log density.</param>
        /// <returns>the log density at <paramref name="x"/>.</returns>
        public double DensityLn(double x)
        {
            double d = (x - _location) / _scale;
            return SpecialFunctions.GammaLn((_dof + 1.0) / 2.0)
                - 0.5 * (_dof + 1.0) * Math.Log(1.0 + d * d / _dof)
                - SpecialFunctions.GammaLn(_dof / 2.0)
                -0.5 * Math.Log(_dof * Math.PI)
                - Math.Log(_scale);
        }

        /// <summary>
        /// Computes the cumulative distribution function of the Student t-distribution.
        /// </summary>
        /// <param name="x">The location at which to compute the cumulative density.</param>
        /// <returns>the cumulative density at <paramref name="x"/>.</returns>
        public double CumulativeDistribution(double x)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Generates a sample from the Student t-distribution.
        /// </summary>
        /// <returns>a sample from the distribution.</returns>
        public double Sample()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Generates a sequence of samples from the Student t-distribution.
        /// </summary>
        /// <returns>a sequence of samples from the distribution.</returns>
        public IEnumerable<double> Samples()
        {
            throw new NotImplementedException();
        }
        #endregion

        /// <summary>
        /// Generates a sample from the Student t-distribution.
        /// </summary>
        /// <param name="rng">The random number generator to use.</param>
        /// <param name="location">The location of the Student t-distribution.</param>
        /// <param name="scale">The scale of the Student t-distribution.</param>
        /// <param name="dof">The degrees of freedom for the Student t-distribution.</param>
        /// <returns>a sample from the distribution.</returns>
        public static double Sample(Random rng, double location, double scale, double dof)
        {
            if (Control.CheckDistributionParameters && !IsValidParameterSet(location, scale, dof))
            {
                throw new ArgumentOutOfRangeException(Resources.InvalidDistributionParameters);
            }

            throw new NotImplementedException();
        }

        /// <summary>
        /// Generates a sequence of samples from the Student t-distribution using the <i>Box-Muller</i> algorithm.
        /// </summary>
        /// <param name="rng">The random number generator to use.</param>
        /// <param name="location">The location of the Student t-distribution.</param>
        /// <param name="scale">The scale of the Student t-distribution.</param>
        /// <param name="dof">The degrees of freedom for the Student t-distribution.</param>
        /// <returns>a sequence of samples from the distribution.</returns>
        public static IEnumerable<double> Samples(Random rng, double location, double scale, double dof)
        {
            if (Control.CheckDistributionParameters && !IsValidParameterSet(location, scale, dof))
            {
                throw new ArgumentOutOfRangeException(Resources.InvalidDistributionParameters);
            }

            throw new NotImplementedException();
        }
    }
}
