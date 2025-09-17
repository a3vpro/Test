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
    public static class RandomExtension
    {
        /// <summary>
        /// Returns a normally distributed random number, with the specified mean and standard deviation.
        /// </summary>
        /// <param name="random">The random number generator instance.</param>
        /// <param name="mean">The mean of the distribution.</param>
        /// <param name="stdDev">The standard deviation of the distribution.</param>
        /// <returns>A random number with a normal distribution.</returns>
        public static double NextGaussian(this Random random, double mean, double stdDev)
        {
            double u1 = 1.0 - random.NextDouble(); // Uniform(0,1] random doubles
            double u2 = 1.0 - random.NextDouble();
            double randStdNormal = Mth.Sqrt(-2.0 * Mth.Log(u1)) * Mth.Sin(2.0 * Mth.PI * u2); // Random normal(0,1)
            return mean + stdDev * randStdNormal; // Random normal(mean, stdDev^2)
        }

        /// <summary>
        /// Returns a random number between min and max, with an exponential distribution.
        /// </summary>
        /// <param name="random">The random number generator instance.</param>
        /// <param name="min">The minimum value of the random number.</param>
        /// <param name="max">The maximum value of the distribution.</param>
        /// <param name="rate">The rate parameter is the inverse of the mean. Default value 1/mean = 1 gives a mean of 1.</param>
        /// <returns>A random number between min and max, with a distribution skewed towards higher values.</returns>
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
        /// Creates a random string using the specified character set.
        /// </summary>
        /// <param name="random">Random instance to use.</param>
        /// <param name="charSet">Set of characters used in the random output.</param>
        /// <param name="maxLength">Maximum length of the output string.</param>
        /// <param name="fixedLenght">If true, the string will have a fixed length; otherwise, the length will be random.</param>
        /// <returns>A randomly generated string.</returns>
        /// <exception cref="ArgumentException">Thrown if any arguments are invalid.</exception>
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
        /// Generates a random boolean value based on the specified probability of returning true.
        /// </summary>
        /// <param name="random">The Random instance that extends this method.</param>
        /// <param name="probability">The probability of returning true, expressed as a number between 0.0 and 1.0, where 1.0 means always true.</param>
        /// <returns>A random boolean value where the probability of true is the given value.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if probability is not in the range of 0.0 to 1.0.</exception>
        public static bool NextBool(this Random random, double probability)
        {
            if (probability < 0.0 || probability > 1.0)
                throw new ArgumentOutOfRangeException(nameof(probability), "The probability must be between 0.0 and 1.0.");

            // Generate a random number between 0.0 and 1.0 and compare if it is less than the given probability.
            return random.NextDouble() < probability;
        }

        /// <summary>
        /// Generates a random date within a specified range, adjusted by a mean date and a standard deviation.
        /// </summary>
        /// <param name="random">The Random instance that extends this method.</param>
        /// <param name="minDate">The minimum date that can be generated.</param>
        /// <param name="maxDate">The maximum date that can be generated.</param>
        /// <param name="meanDate">The mean date, which is the center of the normal distribution.</param>
        /// <param name="stdDev">The standard deviation in days, which defines how spread out the dates will be around the mean.</param>
        /// <returns>A random date within the specified range.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if minDate is greater than maxDate, or if the mean date is outside the bounds of the range.</exception>
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
