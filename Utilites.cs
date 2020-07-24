using SMath.Manager;
using SMath.Math.Numeric;
using System;
using System.Collections.Generic;

namespace MatrixExtensions
{ 
   static class Utilites
   {
      internal static string[] EntryMatrix2ArrStr(Entry matrix)
      {
         string source = matrix.ToString();
         source = source.Trim(new char[] { 'm', 'a', 't', '(', ')' });
         return source.Split(',');
      }

      internal static string[] EntryMatrix2ArrStr(BaseEntry matrix)
      {
         string source = matrix.ToString();
         source = source.Trim(new char[] { 'm', 'a', 't', '(', ')' });
         return source.Split(',');
      }

      internal static double[] EntryVec2Arr(Entry vector)
      {
         string source = vector.ToString();
         source = source.Trim(new char[] { 'm', 'a', 't', '(', ')' });
         //source = source.Replace("{", "");
         //source = source.Replace("}", "");
         string[] src = source.Split(',');

         int n = src.Length - 2;
         double[] res = new double[n];
         for (int i = 0; i < n; i++)
         {
            if (src[i].IndexOf('^') >= 0) res[i] = 0;
            else res[i] = Convert.ToDouble(src[i]);
         }
         
         return res;
      }

      internal static double Entry2Double(Entry prime)
      {
         string source = prime.ToString();
         return Convert.ToDouble(source);
      }

      internal static int Entry2Int(Entry prime)
      {
         string source = prime.ToString();
         return Convert.ToInt32(source);
      }
   }
}
