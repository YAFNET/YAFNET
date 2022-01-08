/*
 * Original version by Stephen Toub and Shawn Farkas.
 * Random pool and thread safety added by Markus Olsson (freakcode.com).
 * 
 * Original source: http://msdn.microsoft.com/en-us/magazine/cc163367.aspx
 * 
 * Some benchmarks (2009-03-18):
 * 
 *  Results produced by calling Next() 1 000 000 times on my machine (dual core 3Ghz)
 * 
 *      System.Random completed in 20.4993 ms (avg 0 ms) (first: 0.3454 ms)
 *      CryptoRandom with pool completed in 132.2408 ms (avg 0.0001 ms) (first: 0.025 ms)
 *      CryptoRandom without pool completed in 2 sec 587.708 ms (avg 0.0025 ms) (first: 1.4142 ms)
 *      
 *      |---------------------|------------------------------------|
 *      | Implementation      | Slowdown compared to System.Random |
 *      |---------------------|------------------------------------|
 *      | System.Random       | 0                                  |
 *      | CryptoRand w pool   | 6,6x                               |
 *      | CryptoRand w/o pool | 19,5x                              |
 *      |---------------------|------------------------------------|
 * 
 * ent (http://www.fourmilab.ch/) results for 16mb of data produced by this class:
 * 
 *  > Entropy = 7.999989 bits per byte.
 *  >
 *  > Optimum compression would reduce the size of this 16777216 byte file by 0 percent.
 *  >
 *  > Chi square distribution for 16777216 samples is 260.64, 
 *  > and randomly would exceed this value 50.00 percent of the times.
 *  >
 *  > Arithmetic mean value of data bytes is 127.4974 (127.5 = random).
 *  > Monte Carlo value for Pi is 3.141838823 (error 0.01 percent).
 *  > Serial correlation coefficient is 0.000348 (totally uncorrelated = 0.0).
 * 
 *  your mileage may vary ;)
 *  
 */
namespace YAF.Core.Utilities
{
    #region Using

    using System;
    using System.Security.Cryptography;

    #endregion

    /// <summary>
    /// The random generator.
    /// </summary>
    public class RandomGenerator
    {
        /// <summary>
        /// The crypto service provider.
        /// </summary>
        private readonly RNGCryptoServiceProvider cryptoServiceProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="RandomGenerator"/> class.
        /// </summary>
        public RandomGenerator()
        {
            this.cryptoServiceProvider = new RNGCryptoServiceProvider();
        }

        /// <summary>
        /// Generates a Random Number
        /// </summary>
        /// <param name="minValue">
        /// The min value.
        /// </param>
        /// <param name="maxExclusiveValue">
        /// The max exclusive value.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int Next(int minValue, int maxExclusiveValue)
        {
            if (minValue >= maxExclusiveValue)
            {
                throw new ArgumentOutOfRangeException("minValue must be lower than maxExclusiveValue");
            }

            var diff = (long)maxExclusiveValue - minValue;
            var upperBound = uint.MaxValue / diff * diff;

            uint ui;
            do
            {
                ui = this.GetRandomUInt();
            } 
            while (ui >= upperBound);
            
            return (int)(minValue + (ui % diff));
        }

        /// <summary>
        /// The get random u int.
        /// </summary>
        /// <returns>
        /// The <see cref="uint"/>.
        /// </returns>
        private uint GetRandomUInt()
        {
            var randomBytes = this.GenerateRandomBytes(sizeof(uint));
            return BitConverter.ToUInt32(randomBytes, 0);
        }

        /// <summary>
        /// The generate random bytes.
        /// </summary>
        /// <param name="bytesNumber">
        /// The bytes number.
        /// </param>
        /// <returns>
        /// Returns the byte
        /// </returns>
        private byte[] GenerateRandomBytes(int bytesNumber)
        {
            var buffer = new byte[bytesNumber];
            this.cryptoServiceProvider.GetBytes(buffer);
            return buffer;
        }
    }
}