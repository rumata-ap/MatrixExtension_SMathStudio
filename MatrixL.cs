using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatrixExtensions
{
   public class MatrixL<T>
   {
      List<List<T>> src;
      T init;
      int r;
      int c;

      public MatrixL(int r, int c, T initValue)
      {
         this.r = r;
         this.c = c;
         init = initValue;
         src = new List<List<T>>(r);
         for (int i = 0; i < r; i++)
         {
            src.Add(new List<T>(c));
            for (int j = 0; j < c; j++)
            {
               src[i].Add(initValue);
            }
         }
      }

      public T GetEl(int idxR, int idxC)
      {
         return src[idxR][idxC];
      }

      public void SetEl(int idxR, int idxC, T el)
      {
         src[idxR][idxC] = el;
      }

      public void InsertRow(int idxR)
      {
         if (idxR > r) return;

         src.Insert(idxR, new List<T>(c));
         r++;
         for (int i = 0; i < c; i++)
         {
            src[idxR].Add(init);
         }
      }

      public void InsertRow(int idxR, List<T> row)
      {
         if (idxR > r) return;

         src.Insert(idxR, new List<T>(c));
         r++;
         int n = c;
         if (row.Count < c) n = row.Count;

         for (int i = 0; i < n; i++)
         {
            src[idxR].Add(row[i]);
         }

         if (row.Count < c)
         {
            for (int i = n; i < c; i++)
            {
               src[idxR].Add(init);
            }
         }
      }
      
      public void InsertCol(int idxC)
      {
         if (idxC > c) return;

         c++;
         for (int i = 0; i < r; i++)
         {
            src[i].Insert(idxC, init);
         }
      }

      public void InsertCol(int idxC, List<T> col)
      {
         if (idxC > c) return;

         c++;
         for (int i = 0; i < r; i++)
         {
            src[i].Insert(idxC, init);
         }

         int n = r;
         if (col.Count < r) n = col.Count;

         foreach (List<T> item in src)
         {
            for (int i = 0; i < n; i++)
            {
               item.Insert(idxC, col[i]);
            }
            if (col.Count < r)
            {
               for (int i = n; i < r; i++)
               {
                  item.Insert(idxC, init);
               }
            }
         }
      }

   }
}
