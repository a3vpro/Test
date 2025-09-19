//----------------------------------------------------------------------------
// Author           : Álvaro Ibáñez del Pino
// NickName         : aibanez
// Created          : 28-08-2021
//
// Last Modified By : aibanez
// Last Modified On : 18-11-2023
// Description      : v1.7.1
//
// Copyright        : (C)  2023 by Sothis/Nunsys. All rights reserved.       
//----------------------------------------------------------------------------
using Mth = System.Math;
using System;

namespace VisionNet.Core.Maths
{
    /// <summary>
    /// Supplies extension methods for <see cref="Random"/> that generate values from specialised distributions
    /// and domains, including Gaussian, exponential, and date-based variations as well as convenience helpers
    /// for booleans and strings. All routines are stateless and rely solely on the provided generator instance.
    /// </summary>
    public static class RandomExtension
    {
        /// <summary>
        /// Generates a normally distributed random number using the Box-Muller transform with the supplied mean and standard deviation.
        /// </summary>
        /// <param name="random">The random number generator instance providing uniform samples. Must not be null.</param>
        /// <param name="mean">The central tendency of the desired distribution, expressed in the same units as the result.</param>
        /// <param name="stdDev">The standard deviation of the distribution. Negative values invert the distribution.</param>
        /// <returns>A pseudo-random number drawn from a normal distribution parameterised by <paramref name="mean"/> and <paramref name="stdDev"/>.</returns>
        /// <exception cref="NullReferenceException">Thrown when <paramref name="random"/> is null.</exception>
        public static double NextGaussian(this Random random, double mean, double stdDev)
        {
            double u1 = 1.0 - random.NextDouble(); // Uniform(0,1] random doubles
            double u2 = 1.0 - random.NextDouble();
            double randStdNormal = Mth.Sqrt(-2.0 * Mth.Log(u1)) * Mth.Sin(2.0 * Mth.PI * u2); // Random normal(0,1)
            return mean + stdDev * randStdNormal; // Random normal(mean, stdDev^2)
        }

        /// <summary>
        /// Returns an exponentially distributed random number constrained between the provided minimum and maximum bounds.
        /// </summary>
        /// <param name="random">The random number generator instance providing uniform samples. Must not be null.</param>
        /// <param name="min">The inclusive lower bound of the desired sample range.</param>
        /// <param name="max">The inclusive upper bound of the desired sample range.</param>
        /// <param name="rate">The exponential rate λ, defined as the inverse of the mean. Values must be non-zero.</param>
        /// <returns>A pseudo-random number constrained to the interval between <paramref name="min"/> and <paramref name="max"/> with exponential weighting.</returns>
        /// <exception cref="DivideByZeroException">Thrown when <paramref name="rate"/> equals zero.</exception>
        /// <exception cref="NullReferenceException">Thrown when <paramref name="random"/> is null.</exception>
        public static double NextExp(this Random random, double min, double max, double rate = 1)
        {
            var exp_rate_a = Mth.Exp(-rate * min);
            return -Mth.Log(exp_rate_a - random.NextDouble() * (exp_rate_a - Mth.Exp(-rate * max))) / rate;
        }

        /// <summary>
        /// Generates a random number from a skewed normal distribution.
        /// </summary>
        /// <param name="random">The random object to use.</param>
        /// <param name="standardDeviation">The standard deviation of the skewed normal distribution.</param>
        /// <param name="skewness">The skewness of the skewed normal distribution. Positive skew means the tail is on the right; negative skew means the tail is on the left.</param>
        /// <param name="dbIteration">Number of iterations to create a skewed normal random number.</param>
        /// <returns>A skewed normal random number.</returns>
        private static double NextSkewedGaussian(this Random random, double standardDeviation = 1, double skewness = 0, int dbIteration = 10)
        {
            var variance = Mth.Pow(standardDeviation, 2);
            const double a = 2.236067977; // Square root of 5
            const double b = 0.222222222;
            const double c = 243 / 32;
            double finalrun, sumdbran = 0;
            double dbmom3, dbmean1, dbmean2, dbprob1, dbdelta1, dbdelta2, randomNumber1, randomNumber2, dbran, terma, termb;

            dbmom3 = Mth.Sqrt(dbIteration) * skewness * Mth.Pow(variance, 1.5);
            terma = b / variance;
            termb = Mth.Sqrt(Mth.Pow(dbmom3, 2) + c * Mth.Pow(variance, 3));
            dbmean1 = terma * (dbmom3 - termb);
            dbmean2 = terma * (dbmom3 + termb);
            dbprob1 = dbmean2 / (2 * a * dbmean1 * (dbmean1 - dbmean2));
            dbdelta1 = -a * dbmean1;
            dbdelta2 = a * dbmean2;

            // Loop for summation of double block random numbers in each final random number
            for (int i = 0; i < dbIteration; i++)
            {
                randomNumber1 = random.NextDouble();
                randomNumber2 = random.NextDouble();

                if (randomNumber1 < (2 * dbdelta1 * dbprob1))
                    dbran = dbmean1 + (2 * dbdelta1 * (randomNumber2 - 0.5));
                else
                    dbran = dbmean2 + (2 * dbdelta2 * (randomNumber2 - 0.5));

                // sumdbran is the sum of double block random numbers created by the iteration
                sumdbran += dbran;
            }

            // Calculate final skewed normal random number
            finalrun = sumdbran / Mth.Sqrt(dbIteration);

            return finalrun;
        }

        /// <summary>
        /// Creates a random string using the specified character set and either fixed or variable length output.
        /// </summary>
        /// <param name="random">Random instance supplying the uniform distribution. Must not be null.</param>
        /// <param name="charSet">Set of characters eligible for selection. Must not be null or empty.</param>
        /// <param name="maxLength">Maximum output length in characters. Must be a positive integer.</param>
        /// <param name="fixedLenght">When <see langword="true"/>, the output length equals <paramref name="maxLength"/>; otherwise a random length in the inclusive range 1 to <paramref name="maxLength"/> is chosen.</param>
        /// <returns>A randomly generated string drawn from <paramref name="charSet"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="charSet"/> is empty or <paramref name="maxLength"/> is less than or equal to zero.</exception>
        /// <exception cref="NullReferenceException">Thrown when <paramref name="random"/> or <paramref name="charSet"/> is null.</exception>
        public static string NextString(this Random random, string charSet, int maxLength, bool fixedLenght = false)
        {
            if (charSet.Length == 0)
                throw new ArgumentOutOfRangeException(nameof(charSet), $"The length of {nameof(charSet)} must not be empty.");
            if (maxLength <= 0)
                throw new ArgumentOutOfRangeException(nameof(maxLength), $"The value of {nameof(maxLength)} must be greater than 0.");

            int length = fixedLenght ? maxLength : random.Next(1, maxLength);
            var stringChars = new char[length];
            for (int i = 0; i < stringChars.Length; i++)
                stringChars[i] = charSet[random.Next(charSet.Length)];

            return new string(stringChars);
        }

        /// <summary>
        /// Generates a random boolean where the probability of <see langword="true"/> is explicitly controlled.
        /// </summary>
        /// <param name="random">The <see cref="Random"/> instance providing uniform samples. Must not be null.</param>
        /// <param name="probability">The likelihood of a <see langword="true"/> result, expressed as a value between 0.0 and 1.0 inclusive.</param>
        /// <returns><see langword="true"/> with probability <paramref name="probability"/>; otherwise <see langword="false"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="probability"/> falls outside the inclusive range 0.0 to 1.0.</exception>
        /// <exception cref="NullReferenceException">Thrown when <paramref name="random"/> is null.</exception>
        public static bool NextBool(this Random random, double probability)
        {
            if (probability < 0.0 || probability > 1.0)
                throw new ArgumentOutOfRangeException(nameof(probability), "The probability must be between 0.0 and 1.0.");

            // Generate a random number between 0.0 and 1.0 and compare if it is less than the given probability.
            return random.NextDouble() < probability;
        }

        /// <summary>
        /// Generates a normally distributed random <see cref="DateTime"/> within a constrained interval centred on a specified mean.
        /// </summary>
        /// <param name="random">The <see cref="Random"/> instance supplying uniform samples. Must not be null.</param>
        /// <param name="minDate">The inclusive lower bound of the date range.</param>
        /// <param name="maxDate">The inclusive upper bound of the date range.</param>
        /// <param name="meanDate">The expected value of the returned dates. Must lie within the closed interval defined by <paramref name="minDate"/> and <paramref name="maxDate"/>.</param>
        /// <param name="stdDev">The standard deviation, expressed in days, that controls dispersion around <paramref name="meanDate"/>.</param>
        /// <returns>A random date constrained to the specified interval with distribution concentrated near <paramref name="meanDate"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="minDate"/> exceeds <paramref name="maxDate"/>, when <paramref name="meanDate"/> is before <paramref name="minDate"/>, or when <paramref name="meanDate"/> is after <paramref name="maxDate"/>.</exception>
        /// <exception cref="NullReferenceException">Thrown when <paramref name="random"/> is null.</exception>
        public static DateTime NextDateTime(this Random random, DateTime minDate, DateTime maxDate, DateTime meanDate, double stdDev)
        {
            if (minDate > maxDate)
                throw new ArgumentOutOfRangeException(nameof(minDate), "The minimum date cannot be greater than the maximum date.");
            if (minDate > meanDate)
                throw new ArgumentOutOfRangeException(nameof(meanDate), "The minimum date cannot be greater than the mean date.");
            if (maxDate < meanDate)
                throw new ArgumentOutOfRangeException(nameof(meanDate), "The maximum date cannot be lower than the mean date.");

            // Calculate the mean in terms of days from the minimum date.
            double mean = (meanDate - minDate).TotalDays;

            // Generate a normal random value with the given mean and standard deviation.
            double randomGaussian = NextGaussian(random, mean, stdDev);

            // Ensure the generated date is within the allowed range.
            double totalDays = randomGaussian.Clamp(0, (maxDate - minDate).TotalDays);
            DateTime randomDate = minDate.AddDays(totalDays);

            return randomDate;
        }
    }
}
