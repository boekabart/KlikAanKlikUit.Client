using System;
using System.Collections.Generic;
using System.Linq;

namespace Glueware.KlikAanKlikUit.Client
{
    public static class AssortedExtensions
    {
        public static Tuple<T1, T2> AsTuple<T1, T2>(this IEnumerable<object> src)
        {
            var betterSrc = src as object[] ?? src.ToArray();
            if (betterSrc.Length != 2)
                throw new ArgumentException("src");
            return new Tuple<T1, T2>((T1)betterSrc[0], (T2)betterSrc[1]);
        }

        public static IEnumerable<Tuple<T1, T2>> AsTuples<T1, T2>(this IEnumerable<object> src)
        {
            var betterSrc = src as object[] ?? src.ToArray();
            if (betterSrc.Length % 2 != 0)
                throw new ArgumentException("src");
            return !betterSrc.Any()
                ? new Tuple<T1, T2>[0]
                : new[] { betterSrc.Take(2).AsTuple<T1, T2>() }.Concat(betterSrc.Skip(2).AsTuples<T1, T2>());
        }
    }
}