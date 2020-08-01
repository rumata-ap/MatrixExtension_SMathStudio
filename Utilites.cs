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

      internal static MatrixL<TNumber> TMatrixToMatrixL(TNumber src)
      {
         if (src.obj.Type != BaseEntryType.Matrix) return null;
         int r = src.Rows().ToInt32();
         int c = src.Cols().ToInt32();
         MatrixL<TNumber> res = new MatrixL<TNumber>(r, c, new TNumber(0,1));
         TMatrix matrix = (TMatrix)src.obj;
         for (int i = 0; i < r; i++)
         {
            for (int j = 0; j < c; j++)
            {
               res[i, j] = matrix[i, j];
            }
         }

         return res;
      }

      internal static TMatrix MatrixLToTMatrix(MatrixL<TNumber> src)
      {
         TMatrix res = new TMatrix(new TNumber[src.R, src.C]);
         for (int i = 0; i < src.R; i++)
         {
            for (int j = 0; j < src.C; j++)
            {
               res[i, j] = src[i, j];
            }
         }
         return res;
      }
   }
}
