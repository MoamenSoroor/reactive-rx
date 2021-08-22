using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;

namespace FirstRX.SequenceBasics
{
    #region Count, Min, Max, Sum and Average
    // 
    // Count, Min, Max, Sum and Average
    // --------------------------------------------------------------------------------
    // all of that operators executes on completed sequence, 

    // The Min and Max methods have overloads that allow you to provide a
    // custom implementation of an IComparer<T> 
    // 
    public class CountMinMaxSumAverage
    {
        public static void Test()
        {

            var seq = Observable.Generate(1, i => i < 5, i => i + 1, i => i, t => TimeSpan.FromMilliseconds(200));
            //var seq = Observable.Range(1, 10);

            seq.Count()
                .Subscribe(
                    val => Console.WriteLine($"Count: {val}"),
                    () => Console.WriteLine("Count Completed"));

            seq.Min()
                .Subscribe(
                    val => Console.WriteLine($"Min: {val}"),
                    () => Console.WriteLine("Min Completed"));

            seq.Max()
                .Subscribe(
                    val => Console.WriteLine($"Max: {val}"),
                    () => Console.WriteLine("Max Completed"));

            seq.Max()
                .Subscribe(
                    val => Console.WriteLine($"Avg: {val}"),
                    () => Console.WriteLine("Avg Completed"));


        }
    }

    #endregion

    #region Functional folds 
    //
    // Functional folds
    // --------------------------------------------------------------------------------
    // Finally we arrive at the set of methods in Rx that meet the functional
    // description of catamorphism/fold.These methods will take an IObservable<T>
    // and produce a T.
    // 
    // Caution should be prescribed whenever using any of these fold methods on an
    // observable sequence, as they are all blocking. The reason you need to be
    // careful with blocking methods is that you are moving from an asynchronous
    // paradigm to a synchronous one, and without care you can introduce concurrency
    // problems such as locking UIs and deadlocks. We will take a deeper look into
    // these problems in a later chapter when we look at concurrency.

    // Note that all blocking Aggergate operators that are blocking has the following 
    // attribute that mark them as obsolute.
    // [Obsolete("This blocking operation is no longer supported. Instead, use the async version in combination with C# and Visual Basic async/await support. In case you need a blocking operation, use Wait or convert the resulting observable sequence to a Task object and block.")]


    // 
    public class FunctionalFolds
    {
        public static void Test()
        {

        }
    }

    #endregion

    #region Blocking Operatos: First, Last,Single, FirstOrDefault, LastOrDefault, SingleOrDefault
    // 
    // Blocking Operatos: First, Last,Single, FirstOrDefault, LastOrDefault, SingleOrDefault
    // --------------------------------------------------------------------------------

    // Note about First ( as a sample and the other operators is the same idea)
    // If the source sequence does not have any values ( i.e. is an empty sequence )
    // then the First method will throw an exception.
    // You can cater for this in three ways:

    // - Use a try/catch blocks around the First() call
    // - Use Take(1) instead.However, this will be asynchronous, not blocking.
    // - Use FirstOrDefault extension method instead

    // 
    public class BlockingOperators
    {
        public static void Test()
        {
            // I comments the code to avoid code warning.

            //var seq = Observable.Generate(
            //    1, 
            //    i => i < 5,
            //    i => i + 1,
            //    i => i, 
            //    t => TimeSpan.FromMilliseconds(200)
            //    );

            //// blocking functions
            //Console.WriteLine($"First: {seq.First()}");
            //Console.WriteLine($"FirstOrDefault: {seq.FirstOrDefault()}");
            //Console.WriteLine($"Last: {seq.Last()}");
            //Console.WriteLine($"LastOrDefault: {seq.LastOrDefault()}");

            //var seq2 = Observable.Timer(TimeSpan.FromMilliseconds(200));

            //Console.WriteLine($"Single: {seq2.Single()}");
            //Console.WriteLine($"SingleOrDefault: {seq2.SingleOrDefault()}");

        }
    }

    #endregion

    #region Aggregate Operator
    // 
    // Aggregate Operator
    // --------------------------------------------------------------------------------
    // IObservable<TAccumulate> Aggregate<TSource, TAccumulate>(
    // this IObservable<TSource> source,
    // TAccumulate seed,
    // Func<TAccumulate, TSource, TAccumulate> accumulator)

    // 
    public static class AggregateOperator
    {
        public static void Test()
        {
            var seq = Observable.Range(1, 10);

            // Implementing Sum with Aggregate 
            seq.Aggregate(0, (acc, val) => val + acc)
                .Subscribe(
                    val => Console.WriteLine($"Sum: {val}"),
                    () => Console.WriteLine("Sum Completed")
                    );


            // Implementing Count with Aggregate 
            seq.Aggregate(0, (acc, _) => acc + 1)
                .Subscribe(
                    val => Console.WriteLine($"Count: {val}"),
                    () => Console.WriteLine("Count Completed")
                    );


            // Implementing Max with Aggregate 
            seq.Aggregate((acc, val) => val > acc ? val : acc)
                .Subscribe(
                    val => Console.WriteLine($"Max: {val}"),
                    () => Console.WriteLine("Max Completed")
                    );


            // Implementing Min with Aggregate 
            seq.Aggregate((acc, val) => val < acc ? val : acc)
                .Subscribe(
                    val => Console.WriteLine($"Min: {val}"),
                    () => Console.WriteLine("Min Completed")
                    );

            // Using myMax
            seq.MyMax().Subscribe(
                    val => Console.WriteLine($"MyMax: {val}"),
                    () => Console.WriteLine("MyMax Completed")
                    );

        }



        public static IObservable<T> MyMax<T>(this IObservable<T> source)
        {
            return source.Aggregate((acc, val) =>
            {
                return Comparer<T>.Default.Compare(acc, val) > 0 ? acc : val;
            });
        }


        public static IObservable<T> MyMin<T>(this IObservable<T> source)
        {
            return source.Aggregate((acc, val) =>
            {
                return Comparer<T>.Default.Compare(acc, val) < 0 ? acc : val;
            });
        }
    }

    #endregion



    #region Scan Operator
    // 
    // Scan Operator
    // --------------------------------------------------------------------------------
    // The signatures for both Scan and Aggregate are the same; the difference is that
    // Scan will push the result from every call to the accumulator function. So instead
    // of being an aggregator that reduces a sequence to a single value sequence, it is
    // an accumulator that we return an accumulated value for each value of the source
    // sequence. In this example we produce a running total.

    // 
    public static class ScanOperator
    {
        public static void Test()
        {
            var seq = Observable.Interval(TimeSpan.FromMilliseconds(2000));

            // get Sum at each generated new sequence value
            seq.Scan((acc, val) => acc + val)
                .Subscribe(
                    val => Console.WriteLine($"Sum until now: {val}"),
                    () => Console.WriteLine("Sum Completed")
                    );



            // get Max at each generated new sequence value
            seq.Scan((acc, val) => acc > val ? acc : val)
                .Subscribe(
                    val => Console.WriteLine($"Max until now: {val}"),
                    () => Console.WriteLine("Max Completed")
                    );



            // RunningMax and RunningMin
            seq.RunningMax()
                .Subscribe(
                    val => Console.WriteLine($"Distinct Max until now: {val}"),
                    () => Console.WriteLine("Distinct Max Completed")
                    );

            // RunningMin and RunningMin
            seq.RunningMin()
                .Subscribe(
                    val => Console.WriteLine($"Distinct Min until now: {val}"),
                    () => Console.WriteLine("Distinct Min Completed")
                    );


        }


        public static void ScanWithSubjectsForClarity()
        {
            var numbers = new Subject<int>();
            var scan = numbers.Scan(0, (acc, current) => acc + current);
            numbers.Dump("numbers");
            scan.Dump("scan");
            numbers.OnNext(1);
            numbers.OnNext(2);
            numbers.OnNext(3);
            numbers.OnCompleted();
        }

        // RunningMax
        public static IObservable<T> RunningMax<T>(this IObservable<T> source)
        {
            return source.Scan((acc,val)=> 
                Comparer<T>.Default.Compare(acc,val)> 0 ? acc:val)
                .DistinctUntilChanged();
            // we can use Distinct but It is probably preferable to use the DistinctUntilChanged
            // so that we internally are not keeping a cache of all values
        }

        // RunningMin
        public static IObservable<T> RunningMin<T>(this IObservable<T> source)
        {
            return source.Scan((acc, val) =>
                Comparer<T>.Default.Compare(acc, val) < 0 ? acc : val)
                .DistinctUntilChanged();

            // we can use Distinct but It is probably preferable to use the DistinctUntilChanged
            // so that we internally are not keeping a cache of all values
        }


    }

    #endregion


    #region MaxBy and MinBy Operators
    // 
    // MaxBy and MinBy Operators
    // --------------------------------------------------------------------------------
    // 

    // 
    public class MaxByMinByOperators
    {
        public static void Test()
        {
            var seq = Observable.Range(1, 10);

            seq.MaxBy(v => v % 3)
                .Subscribe((v)=> Console.WriteLine(string.Join(", ",v)), 
                () => Console.WriteLine("Completed"));

            seq.MinBy(v => v % 3)
                .Subscribe((v) => Console.WriteLine(string.Join(", ", v)),
                () => Console.WriteLine("Completed"));


        }
    }

    #endregion




    #region GroupBy Operator
    // 
    // GroupBy Operator
    // --------------------------------------------------------------------------------
    // 

    // 
    public class GroupByOperator
    {
        public static void Test()
        {
            var seq = Observable.Create<Employee>(o =>
            {
                o.OnNext(new Employee() { Id = 1, Name = "Ahmed", City = "Cairo", Salary=8000 });
                o.OnNext(new Employee() { Id = 2, Name = "Mohammed", City = "Cairo",Salary = 7000 });
                o.OnNext(new Employee() { Id = 3, Name = "Mahmoud", City = "Giza", Salary = 3000 });
                o.OnNext(new Employee() { Id = 4, Name = "Rahma", City = "Giza", Salary = 4000 });
                o.OnNext(new Employee() { Id = 5, Name = "Abd-Allah", City = "Alex", Salary = 10000 });
                o.OnNext(new Employee() { Id = 6, Name = "Waleed", City = "Alex", Salary = 4000 });
                return Disposable.Empty;
            });

            seq.GroupBy(k => k.City).Subscribe(g =>
            {
                g.Max(emp=> emp.Salary).Subscribe((max)=> 
                    Console.WriteLine($"For Key: {g.Key} , Max Salary Emp: {max}")
                    , ()=> Console.WriteLine("Max Completed"));
                
            },
            ()=> Console.WriteLine("Completed")
            );
            Console.WriteLine("end");

        }




        public static void GroupByBookExample()
        {

            var source = Observable.Interval(TimeSpan.FromSeconds(0.1)).Take(10);
            var group = source.GroupBy(i => i % 3);
            group.Subscribe(
            grp =>
            grp.Min().Subscribe(
            minValue =>
            Console.WriteLine("{0} min value = {1}", grp.Key, minValue)),
            () => Console.WriteLine("Completed"));
        }


        private class Employee
        {
            public int Id { get; init; }
            public string Name { get; init; }
            public string City { get; init; }
            public double Salary { get; init; }

            public override string ToString()
            {
                return $"Employee: {Id:5}, {Name:40}, {City:40}, {Salary:18}";
            }
        }


    }

    #endregion





}
