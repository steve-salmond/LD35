using UnityEngine;
using System;

namespace Numbers
{

	/**
	 * A random number generator based on the Mersenne Twister algorithm.
	 * Original author of this code is Rei Hobara (see copyright information below.)
	 * Code has been modified to match the functionality of Unity's Random class. 
	 * 
	 * Copyright (C) Rei HOBARA 2007
	 * 
	 * Name:
	 *     MersenneTwister.cs
	 * Class:
	 *     Rei.Random.MersenneTwister
	 * Purpose:
	 *     A random number generator using Mersenne Twister.
	 * Remark:
	 *     This code is C# implementation of Mersenne Twister.
	 *     Mersenne Twister was introduced by Takuji Nishimura and Makoto Matsumoto.
	 *     See http://www.math.sci.hiroshima-u.ac.jp/~m-mat/MT/mt.html for detail of Mersenne Twister.
	 * History:
	 *     2007/10/6 initial release.	 
	 */

    public class Random 
	{

		// Properties
		// ----------------------------------------------------------------------------

		/** Seed that initialized this random sequence. */
		public int Seed
            { get { return seed; } set { SetSeed(value); } }


		// Members
		// ----------------------------------------------------------------------------

		#region Field

		/** Configuration constants for to the Mersenne Twister algorithm. */
		private const int N = 624;
		private const int M = 397;
		private const uint MATRIX_A = 0x9908b0dfU;
		private const uint UPPER_MASK = 0x80000000U;
		private const uint LOWER_MASK = 0x7fffffffU;
		private const uint TEMPER1 = 0x9d2c5680U;
		private const uint TEMPER2 = 0xefc60000U;
		private const int TEMPER3 = 11;
		private const int TEMPER4 = 7;
		private const int TEMPER5 = 15;
		private const int TEMPER6 = 18;

		/** Used to generate floating values in 0 (inclusive) to 1 (exclusive). */
		private const double UINT32_TO_UNIT_EXCLUSIVE = 1.0 / ((float) UInt32.MaxValue + 1.0);

		/** Used to generate floating values in 0 (inclusive) to 1 (inclusive). */
		private const double UINT32_TO_UNIT_INCLUSIVE = 1.0 / ((float) UInt32.MaxValue);

		/** State values for the Mersenne Twister algorithm. */
		private UInt32[] mt;
		private int mti;
		private UInt32[] mag01;
		
		/** Seed that was used to initialize this random sequence. */
		private int seed;


        #endregion


		// Lifecycle
		// ----------------------------------------------------------------------------

		/** Constructs a random number generator with an arbitrary seed value. */
		public Random() : this(Environment.TickCount) 
		{ 
		}

		/** Constructs a random number generator with the specified seed value. */
		public Random(int seed) 
		{
            mt = new UInt32[N];
            mti = N + 1;
            mag01 = new UInt32[] { 0x0U, MATRIX_A };

            SetSeed(seed);
        }


		// Public Interface
		// ----------------------------------------------------------------------------

        /** Sets the seed for this random generator. */
        public Random SetSeed(int seed)
        {
            this.seed = seed;
            mt[0] = (UInt32) seed;
            for (int i = 1; i < N; i++)
                mt[i] = (UInt32)(1812433253 * (mt[i - 1] ^ (mt[i - 1] >> 30)) + i);

            return this;
        }

        /** Returns a random single precision value between min [inclusive] and max [inclusive]. */
        public float Range(Vector2 range)
            { return Range(range.x, range.y); }

        /** Returns a random single precision value between min [inclusive] and max [inclusive]. */
        public float Range(float min, float max) {
			
			// Check that a valid range has been specified.
			if (min > max)
				throw new ArgumentOutOfRangeException("max", max, "max must be >= min");
			
			double range = max - min;
			double random = NextUnitDoubleInclusive();
			double mappedRange = random * range;
			float mappedRangeFloat = (float) mappedRange;
			return min + mappedRangeFloat;
		}

		/** 
		 * Returns a integer value between min [inclusive] and max [exclusive]. 
		 * If max equals min, min will be returned. 
		 * The returned value will never be max unless min equals max.
		 */
		public int Range(int min, int max) {

			// Check that a valid range has been specified.
			if (min > max)
				throw new ArgumentOutOfRangeException("max", max, "max must be >= min");

			long range = max - min;
			long random = NextUInt32();
			long divisor = (long) UInt32.MaxValue;
			long value = min + (random * range) / divisor;
			return (int) value;
		}

		/** 
		 * Returns a integer value between min [inclusive] and max [inclusive]. 
		 */
		public int RangeInclusive(int min, int max) {
			
			// Check that a valid range has been specified.
			if (min > max)
				throw new ArgumentOutOfRangeException("max", max, "max must be >= min");
			
			long range = (max - min) + 1;
			long random = NextUInt32();
			long divisor = (long) UInt32.MaxValue;
			long value = min + (random * range) / divisor;
			return (int) value;
		}

		/** Returns a binary decision (true, false). */
		public bool Binary()
			{ return RangeInclusive(0, 1) == 0; }

		/** Returns a floating point value between 0 [inclusive] and 1 [inclusive]. */
		public float NextFloat()
			{ return (float) NextUnitDoubleInclusive(); }

		/** Returns a double precision value between 0 [inclusive] and 1 [inclusive]. */
		public double NextDouble()
			{ return (double) NextUnitDoubleInclusive(); }

        /** Returns a random integer. */
        public int NextInteger()
            { return NextInt32(); }

		/** Fills a byte buffer with random values. */
		public void NextBytes( byte[] buffer ) 
		{
			int i = 0;
			UInt32 r;
			while (i + 4 <= buffer.Length) {
				r = NextUInt32();
				buffer[i++] = (byte)r;
				buffer[i++] = (byte)(r >> 8);
				buffer[i++] = (byte)(r >> 16);
				buffer[i++] = (byte)(r >> 24);
			}
			if (i >= buffer.Length) return;
			r = NextUInt32();
			buffer[i++] = (byte)r;
			if (i >= buffer.Length) return;
			buffer[i++] = (byte)(r >> 8);
			if (i >= buffer.Length) return;
			buffer[i++] = (byte)(r >> 16);
		}

        /** Returns a vector that's within a unit circle. */
        public Vector2 InsideUnitCircle()
            { return InsideRadiusRange(0, 1); }

        /** Returns a vector that's within a range of radii. */
        public Vector2 InsideRadius(float radius)
            { return InsideRadiusRange(0, radius); }

        /** Returns a vector that's within a range of radii. */
        public Vector2 InsideRadiusRange(float min, float max)
        {
            var radius = Mathf.Sqrt(Range(min, max));
            var angle = Range(0, Mathf.PI * 2);
            var x = radius * Mathf.Cos(angle);
            var y = radius * Mathf.Sin(angle);
            return new Vector2(x, y);
        }

        /** 
		 * Returns a random value in [0..1] by sampling from the given distribution curve. 
		 * Uses Rejection Sampling (http://en.wikipedia.org/wiki/Rejection_sampling)
         * Assumes the curve has both and range domain within the interval [0..1].
		 */
        public float Sample(AnimationCurve curve, int attempts = 20)
        {
            // Perform rejection sampling (with limited attempts).
            for (int i = 0; i < attempts; i++)
            {
                float x = NextFloat();
                float y = NextFloat();
                if (y <= curve.Evaluate(x))
                    return x;
            }

            // Attempt limit was exceeded.
            return 0;
        }

        /** Returns a value in the given range [low..high] by sampling from a distribution curve. */
        public float Sample(AnimationCurve curve, float low, float high, int attempts = 20)
            { return low + Sample(curve, attempts) * (high - low); }

        /** Returns a value in the given range by sampling from a distribution curve. */
        public float Sample(AnimationCurve curve, Vector2 range, int attempts = 20)
            { return range.x + Sample(curve, attempts) * (range.y - range.x); }


        // Private Methods
        // ----------------------------------------------------------------------------

        /** Returns a double precision value in the range 0 [inclusive] to 1 [inclusive]. */
        private double NextUnitDoubleInclusive()
			{ return UINT32_TO_UNIT_INCLUSIVE * NextUInt32();	}

		/** Returns a double precision value in the range 0 [inclusive] to 1 [exclusive]. */
		private double NextUnitDoubleExclusive()
			{ return UINT32_TO_UNIT_EXCLUSIVE * NextUInt32();	}

		/** Returns an unsigned 32 bit integer value. */
		private uint NextUInt32() {
            UInt32 y;
            if (mti >= N) { gen_rand_all(); mti = 0; }
            y = mt[mti++];
            y ^= (y >> TEMPER3);
            y ^= (y << TEMPER4) & TEMPER1;
            y ^= (y << TEMPER5) & TEMPER2;
            y ^= (y >> TEMPER6);
            return y;
        }

		/** Returns a signed 32 bit integer value. */
		private Int32 NextInt32() {
			return (Int32)NextUInt32();
		}

		/** Returns an unsigned 64 bit integer value. */
		private UInt64 NextUInt64() {
			return ((UInt64)NextUInt32() << 32) | NextUInt32();
		}

		/** Returns a signed 64 bit integer value. */
		private Int64 NextInt64() {
			return ((Int64)NextUInt32() << 32) | NextUInt32();
		}

		/** 
		 * Returns an double precision value. 
		 * Note that the returned value is not 0..1, but spans the full range. 
		 */
		private double NextFullDouble() {
			UInt32 r1, r2;
			r1 = NextUInt32();
			r2 = NextUInt32();
			return (r1 * (double)(2 << 11) + r2) / (double)(2 << 53);
		}

		/**
		 * Internal method that updates the Mersenne Twister state.
		 */
		private void gen_rand_all() {
            int kk = 1;
            UInt32 y;
            UInt32 p;
            y = mt[0] & UPPER_MASK;
            do {
                p = mt[kk];
                mt[kk - 1] = mt[kk + (M - 1)] ^ ((y | (p & LOWER_MASK)) >> 1) ^ mag01[p & 1];
                y = p & UPPER_MASK;
            } while (++kk < N - M + 1);
            do {
                p = mt[kk];
                mt[kk - 1] = mt[kk + (M - N - 1)] ^ ((y | (p & LOWER_MASK)) >> 1) ^ mag01[p & 1];
                y = p & UPPER_MASK;
            } while (++kk < N);
            p = mt[0];
            mt[N - 1] = mt[M - 1] ^ ((y | (p & LOWER_MASK)) >> 1) ^ mag01[p & 1];
        }

    }
}
