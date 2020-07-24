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
                  new TermInfo("listDistinct", TermType.Function,
                  "Возвращает вектор уникальных значений, содержащихся в ('матрица').",
                  FunctionSections.MatricesAndVectors, true,
                  new ArgumentInfo(ArgumentSections.Matrix)),

                  new TermInfo("listDistinct", TermType.Function,
                  "Возвращает вектор или вектор-строку уникальных значений, содержащихся в ('матрица'), а" +
                  " ('число') указывает на вид возвращаемого вектора: 0 - вектор; 1 - вектор-строка.",
                  FunctionSections.MatricesAndVectors, true,
                  new ArgumentInfo(ArgumentSections.Matrix), new ArgumentInfo(ArgumentSections.RealNumber)),
                  };
      }

      public bool TryEvaluateExpression(Entry value, Store context, out Entry result)
      {
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

         if (value.Type == TermType.Function && value.ArgsCount == 2 && value.Text == "listDistinct")
         {
            Entry arg1 = Computation.Preprocessing(value.Items[0], context);
            Entry arg2 = Computation.Preprocessing(value.Items[1], context);
            
            int dir = Utilites.Entry2Int(arg2);
            TNumber tmp = Computation.NumericCalculation(arg1, context);

            List<string> vector = new List<string>(Utilites.EntryMatrix2ArrStr(tmp.obj));
            vector.RemoveAt(vector.Count - 1);
            vector.RemoveAt(vector.Count - 1);
            List<string> distinct = vector.Distinct().ToList();

            //string res = "null";

            List<Term> answer = new List<Term>();
            foreach (string item in distinct)
            {
               answer.AddRange(TermsConverter.ToTerms(item));
            }

            if (dir==0)
            {
               answer.AddRange(TermsConverter.ToTerms(distinct.Count.ToString()));
               answer.AddRange(TermsConverter.ToTerms(1.ToString()));
               answer.Add(new Term(Functions.Mat, TermType.Function, 2 + distinct.Count));
            }
            else
            {             
               answer.AddRange(TermsConverter.ToTerms(1.ToString()));
               answer.AddRange(TermsConverter.ToTerms(distinct.Count.ToString()));
               answer.Add(new Term(Functions.Mat, TermType.Function, 2 + distinct.Count));
            }

            result = Entry.Create(answer.ToArray());
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
