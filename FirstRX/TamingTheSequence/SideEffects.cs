using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirstRX.TamingTheSequence
{
    #region Issues With Side Effects
    // 
    // Issues With Side Effects
    // --------------------------------------------------------------------------------
    // 

    // 
    public class IssuesWithSideEffects
    {
        public static void Test()
        {
            //Problem();
            SolutionWithSelect();
            SolutionWithScan();

        }


        // write to global state will affect the execution of the sequence
        // if one has subscribed then another came, the first will affects the second
        // so indeces will be faulty.
        public static void Problem()
        {
            var letters = Observable.Range(0, 3).Select(i => (char)(i + 65));
            var index = -1;
            var result = letters.Select(
            c =>
            {
                index++;
                return c;
            });
            result.Subscribe(
            c => Console.WriteLine("Received {0} at index {1}", c, index),
            () => Console.WriteLine("completed"));
            result.Subscribe(
            c => Console.WriteLine("Received {0} at index {1}", c, index),
            () => Console.WriteLine("completed"));
        }

        public static void SolutionWithSelect()
        {
            var result = Observable.Range(0, 3).Select(i => (char)(i + 65))
                .Select((value,index) => new { Value=value, Index = index });

            result.Subscribe(
            c => Console.WriteLine("Received {0} at index {1}", c.Value, c.Index),
            () => Console.WriteLine("completed"));
            result.Subscribe(
            c => Console.WriteLine("Received {0} at index {1}", c.Value, c.Index),
            () => Console.WriteLine("completed"));
        }


        public static void SolutionWithScan()
        {
            var letters = Observable.Range(0, 3).Select(i => (char)(i + 65));

            var result = letters.Scan(
                new { Value = new char(), Index = -1 },
                (acc,val) => new { Value=val, Index= acc.Index + 1 }
                );

            result.Subscribe(
            c => Console.WriteLine("Received {0} at index {1}", c.Value, c.Index),
            () => Console.WriteLine("completed"));
            result.Subscribe(
            c => Console.WriteLine("Received {0} at index {1}", c.Value, c.Index),
            () => Console.WriteLine("completed"));
        }
    }

    #endregion

    #region Mutable elements cannot be protected

    // 
    // Mutable elements cannot be protected

    // --------------------------------------------------------------------------------
    // 

    // you should make sure that consumers of the sequence can be assured that the data
    // they get is the data that the source produced. Not being able to mutate elements
    // may seem limiting as a consumer, but these needs are best met via the
    // Transformation operators which provide better encapsulation.
    // 
    public class ProblemOfMutation
    {


        public static void Test()
        {
            var emps = new List<Employee>()
            {
                new Employee{Id =1,Name="Ahmed"},
                new Employee{Id =2,Name="Mohammed"}
            };

            var query1 = emps.Select((v, i) =>
            {
                v.Name += i;
                return v;
            });

            var query2 = emps.Select((v, i) =>
            {
                v.Name += i * 2;
                return v;
            });

            // executing of each query depends on the result of the other
            // changing of order of the executing will affect change the results
            query1.ToList().ForEach(Console.WriteLine);

            query2.ToList().ForEach(Console.WriteLine);




        }


        class Employee
        {
            public int Id { get; set; }

            public string Name { get; set; }

            public override string ToString()
            {
                return $"Employee: Id:{Id:10}, Name:{Name:20}";
            }

        }


    }

    #endregion

    #region Do Operator
    // 
    // Do Operator
    // --------------------------------------------------------------------------------
    // 

    // 
    public class DoOperator
    {
        public static void Test()
        {
            var seq = Observable.Interval(TimeSpan.FromSeconds(1))
                .Take(5)
                .Do(v => LogInfo(v),v=> LogError(v),()=> Console.WriteLine("Do Completed")) // logging
                .Select(a => (a + 1) * 10);
           
            seq.Subscribe(Console.WriteLine, () => Console.WriteLine("Completed"));
        }

        public static void LogInfo<T>(T data)
        {

            var color = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Log Information at {DateTime.Now}: {data}");
            Console.ForegroundColor = color;
        }

        public static void LogError<T>(T data)
        {
            var color = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(data);
            Console.ForegroundColor = color;

        }
    }

    #endregion

    #region Encapsulating with AsObservable

    // 
    // Encapsulating with AsObservable

    // --------------------------------------------------------------------------------
    // 

    // 
    public class EncapsulatingwithAsObservable

    {
        public static void Test()
        {

        }
    }

    #endregion

    #region Finally
    // 
    // Finally
    // --------------------------------------------------------------------------------
    // Side effects should be avoided where possible.Any combination of concurrency
    // with shared state will commonly demand the need for complex locking, deep
    // understanding of CPU architectures and how they work with the locking and
    // optimization features of the language you use.The simple and preferred approach
    // is to avoid shared state, favor immutable data types and utilize query composition
    // and transformation.Hiding side effects into Where or Select clauses can make for
    // very confusing code.If a side effect is required, then the Do method expresses
    // intent that you are creating a side effect by being explicit.

    #endregion


}
