using System;

namespace PopInGames
{
    public static class FloatExtensions
    {
        public static float Median(this float[] sourceNumbers)
        {
            if (sourceNumbers == null || sourceNumbers.Length == 0)
            {
                throw new Exception("Median of empty array not defined.");
            }

            float[] sortedPNumbers = (float[]) sourceNumbers.Clone();
            Array.Sort(sortedPNumbers);

            int size = sortedPNumbers.Length;
            int mid = size / 2;
            return size % 2 != 0 ? sortedPNumbers[mid] : (sortedPNumbers[mid] + sortedPNumbers[mid - 1]) / 2;
        }
    }
}