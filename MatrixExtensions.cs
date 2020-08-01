using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SMath.Manager;
using SMath.Math;
using SMath.Math.Numeric;

namespace MatrixExtensions
{
   public class MatrixExtensions : IPluginHandleEvaluation, IPluginLowLevelEvaluationFast
   {
      AssemblyInfo[] asseblyInfos;

      public void Initialize()
      {
         asseblyInfos = new AssemblyInfo[] { new AssemblyInfo("SMath Studio", new Version(0, 99), new Guid("a37cba83-b69c-4c71-9992-55ff666763bd")) };

         AppDomain currentDomain = AppDomain.CurrentDomain;
         currentDomain.AssemblyResolve += new ResolveEventHandler(CurrentDomain_AssemblyResolve);

         System.Reflection.Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
         {
            System.Reflection.Assembly[] asms = AppDomain.CurrentDomain.GetAssemblies();
            for (int i = 0; i < asms.Length; ++i)
            {
               if (asms[i].FullName == args.Name)
                  return asms[i];
            }
            return null;
         }
      }

      public TermInfo[] GetTermsHandled(SessionProfile sessionProfile)
      {
         //functions definitions
         return new TermInfo[]
                  {
                  new TermInfo("list", TermType.Function,
                  "Возвращает нулевой вектор заданной ('число') размерности.",
                  FunctionSections.MatricesAndVectors, true,
                  new ArgumentInfo(ArgumentSections.RealNumber)),
                  
                  new TermInfo("listDistinct", TermType.Function,
                  "Возвращает вектор уникальных значений, содержащихся в ('матрица').",
                  FunctionSections.MatricesAndVectors, true,
                  new ArgumentInfo(ArgumentSections.Matrix)),
                                     
                  new TermInfo("listSortAsText", TermType.Function,
                  "Возвращает сортированный вектор значений, содержащихся в ('матрица').",
                  FunctionSections.MatricesAndVectors, true,
                  new ArgumentInfo(ArgumentSections.Matrix)),
                                    
                  new TermInfo("listLength", TermType.Function,
                  "Возвращает длину вектора ('вектор').",
                  FunctionSections.MatricesAndVectors, true,
                  new ArgumentInfo(ArgumentSections.ColumnVector)),
                  
                  new TermInfo("listNonZeros", TermType.Function,
                  "Возвращает вектор не нулевых значений, содержащихся в ('матрица').",
                  FunctionSections.MatricesAndVectors, true,
                  new ArgumentInfo(ArgumentSections.Matrix)),

                  //new TermInfo("listDistinct", TermType.Function,
                  //"Возвращает вектор или вектор-строку уникальных значений, содержащихся в ('матрица'), а" +
                  //" ('число') указывает на вид возвращаемого вектора: 0 - вектор; 1 - вектор-строка.",
                  //FunctionSections.MatricesAndVectors, true,
                  //new ArgumentInfo(ArgumentSections.Matrix), new ArgumentInfo(ArgumentSections.RealNumber)),
                  
                  new TermInfo("listAdd", TermType.Function,
                  "Добавление нового значения ('аргумент') в конец вектора ('вектор').",
                  FunctionSections.MatricesAndVectors, true,
                  new ArgumentInfo(ArgumentSections.ColumnVector), new ArgumentInfo(ArgumentSections.Default)),
                  
                  new TermInfo("listInsert", TermType.Function,
                  "Добавление нового значения ('аргумент') в вектор ('вектор') по указанному индексу ('число').",
                  FunctionSections.MatricesAndVectors, true,
                  new ArgumentInfo(ArgumentSections.ColumnVector), new ArgumentInfo(ArgumentSections.Default), 
                  new ArgumentInfo(ArgumentSections.RealNumber)),
                                    
                  new TermInfo("listInsertRange", TermType.Function,
                  "Добавление нового диапазона значений ('3:вектор') в вектор ('1:вектор') по указанному индексу ('число').",
                  FunctionSections.MatricesAndVectors, true,
                  new ArgumentInfo(ArgumentSections.ColumnVector), new ArgumentInfo(ArgumentSections.RealNumber), 
                  new ArgumentInfo(ArgumentSections.ColumnVector)),
                                    
                  new TermInfo("listRemoveAt", TermType.Function,
                  "Удаление элемента вектора ('вектор') по указанному индексу ('число').",
                  FunctionSections.MatricesAndVectors, true,
                  new ArgumentInfo(ArgumentSections.ColumnVector), new ArgumentInfo(ArgumentSections.RealNumber)),
                                    
                  new TermInfo("listRemoveRange", TermType.Function,
                  "Удаление диапазона элементов вектора ('1:вектор') по указанному индексу начала ('2:число') и количеству ('3:число') значений удаляемого диапазона.",
                  FunctionSections.MatricesAndVectors, true,
                  new ArgumentInfo(ArgumentSections.ColumnVector), new ArgumentInfo(ArgumentSections.RealNumber), 
                  new ArgumentInfo(ArgumentSections.RealNumber)),

                  new TermInfo("matrixRInsert", TermType.Function,
                  "Вставка нулевой строки в матрицу ('матрица') по указанному индексу ('число').",
                  FunctionSections.MatricesAndVectors, true,
                  new ArgumentInfo(ArgumentSections.Matrix), new ArgumentInfo(ArgumentSections.RealNumber)),

                  new TermInfo("matrixRInsert", TermType.Function,
                  "Вставка строки в матрицу ('1:матрица') по указанному индексу ('число') из вектора ('3:матрица').",
                  FunctionSections.MatricesAndVectors, true,
                  new ArgumentInfo(ArgumentSections.Matrix), new ArgumentInfo(ArgumentSections.RealNumber),
                  new ArgumentInfo(ArgumentSections.Matrix)),
                  
                  new TermInfo("matrixCInsert", TermType.Function,
                  "Вставка нулевого столбца в матрицу ('матрица') по указанному индексу ('число').",
                  FunctionSections.MatricesAndVectors, true,
                  new ArgumentInfo(ArgumentSections.Matrix), new ArgumentInfo(ArgumentSections.RealNumber)),

                  new TermInfo("matrixCInsert", TermType.Function,
                  "Вставка столбца в матрицу ('1:матрица') по указанному индексу ('число') из вектора ('3:матрица').",
                  FunctionSections.MatricesAndVectors, true,
                  new ArgumentInfo(ArgumentSections.Matrix), new ArgumentInfo(ArgumentSections.RealNumber),
                  new ArgumentInfo(ArgumentSections.Matrix)),

                  };
      }

      public bool TryEvaluateExpression(Entry value, Store context, out Entry result)
      {
         //list
         if (value.Type == TermType.Function && value.ArgsCount == 1 && value.Text == "list")
         {
            Entry arg1 = Computation.Preprocessing(value.Items[0], context);

            TNumber tmp = Computation.NumericCalculation(arg1, context);
            TDouble n = (TDouble)tmp.obj;
            //List<string> vector = new List<string>(Utilites.EntryMatrix2ArrStr(tmp.obj));
            //vector.RemoveAt(vector.Count - 1);
            //vector.RemoveAt(vector.Count - 1);
            //List<string> distinct = vector.Distinct().ToList();

            List<Term> answer = new List<Term>();
            if (n.D <= 0)
            {
               answer.AddRange(TermsConverter.ToTerms(0.ToString()));
            }
            else
            {
               for (int i = 0; i < n.D; i++)
               {
                  answer.AddRange(TermsConverter.ToTerms("0"));
               }

               answer.AddRange(TermsConverter.ToTerms(n.D.ToString()));              
            }

            answer.AddRange(TermsConverter.ToTerms(1.ToString()));
            answer.Add(new Term(Functions.Mat, TermType.Function, 2 + (int)n.D));

            result = Entry.Create(answer.ToArray());
            return true;
         }
         
         //listLength
         if (value.Type == TermType.Function && value.ArgsCount == 1 && value.Text == "listLength")
         {
            Entry arg1 = Computation.Preprocessing(value.Items[0], context);

            List<string> vector = new List<string>(Utilites.EntryMatrix2ArrStr(arg1));

            List<Term> answer = new List<Term>();

            if (vector.Count<=2)
            {
               answer.AddRange(TermsConverter.ToTerms("0"));
            }
            else
            {
               answer.AddRange(TermsConverter.ToTerms(vector[vector.Count - 2]));
            }

            result = Entry.Create(answer.ToArray());
            return true;
         }

         //listDistinct
         if (value.Type == TermType.Function && value.ArgsCount == 1 && value.Text == "listDistinct")
         {
            Entry arg1 = Computation.Preprocessing(value.Items[0], context);

            TNumber tmp = Computation.NumericCalculation(arg1, context);

            List<string> vector = new List<string>(Utilites.EntryMatrix2ArrStr(tmp.obj));
            vector.RemoveAt(vector.Count - 1);
            vector.RemoveAt(vector.Count - 1);
            List<string> distinct = vector.Distinct().ToList();

            List<Term> answer = new List<Term>();
            foreach (string item in distinct)
            {
               answer.AddRange(TermsConverter.ToTerms(item));
            }

            answer.AddRange(TermsConverter.ToTerms(distinct.Count.ToString()));
            answer.AddRange(TermsConverter.ToTerms(1.ToString()));
            answer.Add(new Term(Functions.Mat, TermType.Function, 2 + distinct.Count));

            result = Entry.Create(answer.ToArray());
            return true;
         }
         
         //listSortAsText
         if (value.Type == TermType.Function && value.ArgsCount == 1 && value.Text == "listSortAsText")
         {
            Entry arg1 = Computation.Preprocessing(value.Items[0], context);

            TNumber tmp = Computation.NumericCalculation(arg1, context);

            BaseEntry be = tmp.obj;
            Term[] res = be.ToTerms(16, 16, FractionsType.Decimal, true);

            tmp = Computation.NumericCalculation(Entry.Create(res), context);

            List<string> vector = new List<string>(Utilites.EntryMatrix2ArrStr(tmp.obj));
            if (be.Type == BaseEntryType.Matrix)
            {
               vector.RemoveAt(vector.Count - 1);
               vector.RemoveAt(vector.Count - 1);
               vector.Sort();
            }

            List<Term> answer = new List<Term>();
            foreach (string item in vector)
            {
               answer.AddRange(TermsConverter.ToTerms(item));
            }

            answer.AddRange(TermsConverter.ToTerms(vector.Count.ToString()));
            answer.AddRange(TermsConverter.ToTerms(1.ToString()));
            answer.Add(new Term(Functions.Mat, TermType.Function, 2 + vector.Count));

            result = Entry.Create(answer.ToArray());
            return true;
         }
         
         //listAdd
         if (value.Type == TermType.Function && value.ArgsCount == 2 && value.Text == "listAdd")
         {
            Entry arg1 = Computation.Preprocessing(value.Items[0], context);
            Entry arg2 = Computation.Preprocessing(value.Items[1], context);

            TNumber tmp, tmp1;
            TMatrix m;
            try
            {
               tmp = Computation.NumericCalculation(arg1, context);
               tmp1 = Computation.NumericCalculation(arg2, context);
               TNumber count = tmp.Rows();
               m = tmp.Stack(new TNumber[] { new TNumber(0, 1) });
               m[count.ToInt32(), 0] = tmp1;
               
            }
            catch
            {
               tmp1 = Computation.NumericCalculation(arg2, context);
               m = new TMatrix(0);
               m[0, 0] = tmp1;
            }

            result = Entry.Create(m.ToTerms());

            return true;
         }

         //listNonZeros
         if (value.Type == TermType.Function && value.ArgsCount == 1 && value.Text == "listNonZeros")
         {
            Entry arg1 = Computation.Preprocessing(value.Items[0], context);

            TNumber tmp = Computation.NumericCalculation(arg1, context);

            List<string> vector = new List<string>(Utilites.EntryMatrix2ArrStr(tmp.obj));
            vector.RemoveAt(vector.Count - 1);
            vector.RemoveAt(vector.Count - 1);
            IEnumerable<string> select = from t in vector // определяем каждый объект из teams как t
                                           where t!="0" //фильтрация по критерию
                                           select t; // выбираем объект
            List<string> nonZeros = new List<string>(select);
            List<Term> answer = new List<Term>();
            foreach (string item in nonZeros)
            {
               answer.AddRange(TermsConverter.ToTerms(item));
            }

            answer.AddRange(TermsConverter.ToTerms(nonZeros.Count.ToString()));
            answer.AddRange(TermsConverter.ToTerms(1.ToString()));
            answer.Add(new Term(Functions.Mat, TermType.Function, 2 + nonZeros.Count));

            result = Entry.Create(answer.ToArray());
            return true;
         }

         //if (value.Type == TermType.Function && value.ArgsCount == 2 && value.Text == "listDistinct")
         //{
         //   Entry arg1 = Computation.Preprocessing(value.Items[0], context);
         //   Entry arg2 = Computation.Preprocessing(value.Items[1], context);
            
         //   int dir = Utilites.Entry2Int(arg2);
         //   TNumber tmp = Computation.NumericCalculation(arg1, context);

         //   List<string> vector = new List<string>(Utilites.EntryMatrix2ArrStr(tmp.obj));
         //   vector.RemoveAt(vector.Count - 1);
         //   vector.RemoveAt(vector.Count - 1);
         //   List<string> distinct = vector.Distinct().ToList();

         //   //string res = "null";

         //   List<Term> answer = new List<Term>();
         //   foreach (string item in distinct)
         //   {
         //      answer.AddRange(TermsConverter.ToTerms(item));
         //   }

         //   if (dir==0)
         //   {
         //      answer.AddRange(TermsConverter.ToTerms(distinct.Count.ToString()));
         //      answer.AddRange(TermsConverter.ToTerms(1.ToString()));
         //      answer.Add(new Term(Functions.Mat, TermType.Function, 2 + distinct.Count));
         //   }
         //   else
         //   {             
         //      answer.AddRange(TermsConverter.ToTerms(1.ToString()));
         //      answer.AddRange(TermsConverter.ToTerms(distinct.Count.ToString()));
         //      answer.Add(new Term(Functions.Mat, TermType.Function, 2 + distinct.Count));
         //   }

         //   result = Entry.Create(answer.ToArray());
         //   return true;
         //}

         if (value.Type == TermType.Function && value.ArgsCount == 2 && value.Text == "listRemoveAt")
         {
            Entry arg1 = Computation.Preprocessing(value.Items[0], context);
            Entry arg2 = Computation.Preprocessing(value.Items[1], context);

            TNumber tmp1 = Computation.NumericCalculation(arg1, context);
            TNumber tmp2 = Computation.NumericCalculation(arg2, context);

            BaseEntry be1 = tmp1.obj;
            BaseEntry be2 = tmp2.obj;

            List<string> vector = new List<string>(Utilites.EntryMatrix2ArrStr(be1));
            if (be1.Type == BaseEntryType.Matrix)
            {
               vector.RemoveAt(vector.Count - 1);
               vector.RemoveAt(vector.Count - 1);
               vector.RemoveAt((int)be2.ToDouble() - 1);
            }

            List<Term> answer = new List<Term>();
            foreach (string item in vector)
            {
               answer.AddRange(TermsConverter.ToTerms(item));
            }

            answer.AddRange(TermsConverter.ToTerms(vector.Count.ToString()));
            answer.AddRange(TermsConverter.ToTerms(1.ToString()));
            answer.Add(new Term(Functions.Mat, TermType.Function, 2 + vector.Count));

            result = Entry.Create(answer.ToArray());
            return true;
         }

         if (value.Type == TermType.Function && value.ArgsCount == 3 && value.Text == "listRemoveRange")
         {
            Entry arg1 = Computation.Preprocessing(value.Items[0], context);
            Entry arg2 = Computation.Preprocessing(value.Items[1], context);
            Entry arg3 = Computation.Preprocessing(value.Items[2], context);

            TNumber tmp1 = Computation.NumericCalculation(arg1, context);
            TNumber tmp2 = Computation.NumericCalculation(arg2, context);
            TNumber tmp3 = Computation.NumericCalculation(arg3, context);

            BaseEntry be1 = tmp1.obj;
            BaseEntry be2 = tmp2.obj;
            BaseEntry be3 = tmp3.obj;

            List<string> vector = new List<string>(Utilites.EntryMatrix2ArrStr(be1));
            if (be1.Type == BaseEntryType.Matrix)
            {
               vector.RemoveAt(vector.Count - 1);
               vector.RemoveAt(vector.Count - 1);
               vector.RemoveRange((int)be2.ToDouble() - 1, (int)be3.ToDouble());
            }

            List<Term> answer = new List<Term>();
            foreach (string item in vector)
            {
               answer.AddRange(TermsConverter.ToTerms(item));
            }

            answer.AddRange(TermsConverter.ToTerms(vector.Count.ToString()));
            answer.AddRange(TermsConverter.ToTerms(1.ToString()));
            answer.Add(new Term(Functions.Mat, TermType.Function, 2 + vector.Count));

            result = Entry.Create(answer.ToArray());
            return true;
         }

         if (value.Type == TermType.Function && value.ArgsCount == 3 && value.Text == "listInsert")
         {
            Entry arg1 = Computation.Preprocessing(value.Items[0], context);
            Entry arg2 = Computation.Preprocessing(value.Items[1], context);
            Entry arg3 = Computation.Preprocessing(value.Items[2], context);

            TNumber tmp1 = Computation.NumericCalculation(arg1, context);
            TNumber tmp2 = Computation.NumericCalculation(arg2, context);
            TNumber tmp3 = Computation.NumericCalculation(arg3, context);

            BaseEntry be1 = tmp1.obj;
            BaseEntry be2 = tmp2.obj;
            BaseEntry be3 = tmp3.obj;

            List<string> vector = new List<string>(Utilites.EntryMatrix2ArrStr(be1));

            if (be1.Type == BaseEntryType.Matrix)
            {
               vector.RemoveAt(vector.Count - 1);
               vector.RemoveAt(vector.Count - 1);
               vector.Insert((int)be2.ToDouble() - 1, be3.ToString());
            }

            List<Term> answer = new List<Term>();
            foreach (string item in vector)
            {
               answer.AddRange(TermsConverter.ToTerms(item));
            }

            answer.AddRange(TermsConverter.ToTerms(vector.Count.ToString()));
            answer.AddRange(TermsConverter.ToTerms(1.ToString()));
            answer.Add(new Term(Functions.Mat, TermType.Function, 2 + vector.Count));

            result = Entry.Create(answer.ToArray());
            return true;
         }

         if (value.Type == TermType.Function && value.ArgsCount == 3 && value.Text == "listInsertRange")
         {
            Entry arg1 = Computation.Preprocessing(value.Items[0], context);
            Entry arg2 = Computation.Preprocessing(value.Items[1], context);
            Entry arg3 = Computation.Preprocessing(value.Items[2], context);

            TNumber tmp1 = Computation.NumericCalculation(arg1, context);
            TNumber tmp2 = Computation.NumericCalculation(arg2, context);
            TNumber tmp3 = Computation.NumericCalculation(arg3, context);

            BaseEntry be1 = tmp1.obj;
            BaseEntry be2 = tmp2.obj;
            BaseEntry be3 = tmp3.obj;

            List<string> vector = new List<string>(Utilites.EntryMatrix2ArrStr(be1));
            List<string> vector1 = new List<string>(Utilites.EntryMatrix2ArrStr(be3));
            if (be1.Type == BaseEntryType.Matrix)
            {
               vector.RemoveAt(vector.Count - 1);
               vector.RemoveAt(vector.Count - 1);
               vector1.RemoveAt(vector.Count - 1);
               vector1.RemoveAt(vector.Count - 1);
               vector.InsertRange((int)be2.ToDouble() - 1, vector1);
            }

            List<Term> answer = new List<Term>();
            foreach (string item in vector)
            {
               answer.AddRange(TermsConverter.ToTerms(item));
            }

            answer.AddRange(TermsConverter.ToTerms(vector.Count.ToString()));
            answer.AddRange(TermsConverter.ToTerms(1.ToString()));
            answer.Add(new Term(Functions.Mat, TermType.Function, 2 + vector.Count));

            result = Entry.Create(answer.ToArray());
            return true;
         }

         if (value.Type == TermType.Function && value.ArgsCount == 2 && value.Text == "matrixRInsert")
         {
            Entry arg1 = Computation.Preprocessing(value.Items[0], context);
            Entry arg2 = Computation.Preprocessing(value.Items[1], context);

            TNumber tmp1 = Computation.NumericCalculation(arg1, context);
            TNumber tmp2 = Computation.NumericCalculation(arg2, context);

            MatrixL<TNumber> matrixL = Utilites.TMatrixToMatrixL(tmp1);
            matrixL.InsertRow(tmp2.ToInt32()-1);
            TMatrix m = Utilites.MatrixLToTMatrix(matrixL);

            result = Entry.Create(m.ToTerms());
            return true;
         }

         if (value.Type == TermType.Function && value.ArgsCount == 3 && value.Text == "matrixRInsert")
         {
            Entry arg1 = Computation.Preprocessing(value.Items[0], context);
            Entry arg2 = Computation.Preprocessing(value.Items[1], context);
            Entry arg3 = Computation.Preprocessing(value.Items[2], context);

            TNumber tmp1 = Computation.NumericCalculation(arg1, context);
            TNumber tmp2 = Computation.NumericCalculation(arg2, context);
            TNumber tmp3 = Computation.NumericCalculation(arg3, context);

            MatrixL<TNumber> matrixL = Utilites.TMatrixToMatrixL(tmp1);
            List<TNumber> vectorL = Utilites.TMatrixToMatrixL(tmp3).ToList();
            matrixL.InsertRow(tmp2.ToInt32() - 1, vectorL);
            TMatrix m = Utilites.MatrixLToTMatrix(matrixL);

            result = Entry.Create(m.ToTerms());
            return true;
         }
         
         if (value.Type == TermType.Function && value.ArgsCount == 2 && value.Text == "matrixCInsert")
         {
            Entry arg1 = Computation.Preprocessing(value.Items[0], context);
            Entry arg2 = Computation.Preprocessing(value.Items[1], context);

            TNumber tmp1 = Computation.NumericCalculation(arg1, context);
            TNumber tmp2 = Computation.NumericCalculation(arg2, context);

            MatrixL<TNumber> matrixL = Utilites.TMatrixToMatrixL(tmp1);
            matrixL.InsertCol(tmp2.ToInt32()-1);
            TMatrix m = Utilites.MatrixLToTMatrix(matrixL);

            result = Entry.Create(m.ToTerms());
            return true;
         }

         if (value.Type == TermType.Function && value.ArgsCount == 3 && value.Text == "matrixCInsert")
         {
            Entry arg1 = Computation.Preprocessing(value.Items[0], context);
            Entry arg2 = Computation.Preprocessing(value.Items[1], context);
            Entry arg3 = Computation.Preprocessing(value.Items[2], context);

            TNumber tmp1 = Computation.NumericCalculation(arg1, context);
            TNumber tmp2 = Computation.NumericCalculation(arg2, context);
            TNumber tmp3 = Computation.NumericCalculation(arg3, context);

            MatrixL<TNumber> matrixL = Utilites.TMatrixToMatrixL(tmp1);
            List<TNumber> vectorL = Utilites.TMatrixToMatrixL(tmp3).ToList();
            matrixL.InsertCol(tmp2.ToInt32() - 1, vectorL);
            TMatrix m = Utilites.MatrixLToTMatrix(matrixL);

            result = Entry.Create(m.ToTerms());
            return true;
         }

         result = null;
         return false;
      }


      public void Dispose()
      {
      }
   }
}
