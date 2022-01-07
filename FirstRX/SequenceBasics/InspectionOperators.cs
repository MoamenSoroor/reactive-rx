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
    #region Any Operator
    // 
    // Any Operator
    // --------------------------------------------------------------------------------
    // 

    // 
    public static class AnyOperator
    {
        public static void Test()
        {
            var subject = new Subject<int>();
            subject.Subscribe(Console.WriteLine, () => Console.WriteLine("Subject completed"));
            var any = subject.Any();
            any.Subscribe(b => Console.WriteLine("The subject has any values? {0}", b));
            subject.OnNext(1);
            subject.OnCompleted();
        }

        public static IObservable<bool> MyAny<T>(this IObservable<T> source)
        {
            return Observable.Create<bool>(
            o =>
            {
                var hasValues = false;
                return source
                    .Take(1)
                    .Subscribe(
                    _ => hasValues = true,
                    o.OnError,
                    () =>
                {
                    o.OnNext(hasValues);
                    o.OnCompleted();
                });
            });
        }
        public static IObservable<bool> MyAny<T>(
        this IObservable<T> source,
        Func<T, bool> predicate)
        {
            return source
            .Where(predicate)
            .MyAny();
        }

    }

    #endregion

    #region All Operator
    // 
    // All Operator
    // --------------------------------------------------------------------------------
    // it will not returns value until one of the sequence values not satifying the 
    // predicate, or the sequence is completed and satisfied the predicate (true is returned)

    // 
    public static class AllOperator
    {
        public static void Test()
        {
            var seq = Observable.Interval(TimeSpan.FromMilliseconds(500));
            seq.All(p => p < 5).Subscribe(Console.WriteLine, () => Console.WriteLine("Completed"));

            //var seq = Observable.Range(1, 10);
            //seq.All(p => p < 11).Subscribe(Console.WriteLine, () => Console.WriteLine("Completed"));




            //// Tests of MyAll Operator Implementation
            //// ------------------------------------------------------------------------
            //var seq = Observable.Interval(TimeSpan.FromMilliseconds(500));
            //seq.MyAll(p => p < 5).Subscribe(Console.WriteLine, () => Console.WriteLine("Completed"));


            //var seq = Observable.Range(1 , 10);
            //seq.MyAll( p => p < 11 ).Subscribe(Console.WriteLine, () => Console.WriteLine("Completed"));


            //// IsEmpty Operator using All Operator
            //// ------------------------------------------------------------------------
            //var seq = Observable.Range(1, 10);
            //seq.All(_ => false).Subscribe(Console.WriteLine, () => Console.WriteLine("Completed"));



        }



        public static void AllOperatorClarifiedWithSubject()
        {
            var subject = new Subject<int>();
            subject.Subscribe(Console.WriteLine, () => Console.WriteLine("Subject completed"));
            var all = subject.All(i => i < 5);
            all.Subscribe(b => Console.WriteLine("All values less than 5? {0}", b));
            subject.OnNext(1);
            subject.OnNext(2);
            subject.OnNext(6);
            subject.OnNext(2);
            subject.OnNext(1);
            subject.OnCompleted();
        }

        public static IObservable<bool> MyAll<T>(this IObservable<T> source, Func<T, bool> predicate)
        {
            return Observable.Create<bool>(
            o =>
            {
                var isFinished = false;
                return source
                    .Subscribe(
                    value =>
                    {
                        if (!predicate(value))
                        {
                            o.OnNext(false);
                            o.OnCompleted();
                            isFinished = true;
                        }
                    },
                    o.OnError,
                    () =>
                    {
                        if (!isFinished)
                        {
                            o.OnNext(true);
                            o.OnCompleted();

                        }
                    });
            });
        }

    }

    #endregion

    #region Contains Operator
    // 
    // Contains Operator
    // --------------------------------------------------------------------------------
    // like Any Operator , but it takes a value not a predicate,
    // and it can take IEqualityComparer<T> for custom equality

    // 
    public class ContainsOperator
    {
        public static void Test()
        {
            var seq = Observable.Range(1, 10);
            seq.Contains(5).Subscribe(Console.WriteLine, () => Console.WriteLine("Completed"));



            var seq2 = Observable.Create<Employee>((o) =>
            {
                o.OnNext(new Employee() { Id = 1, Name = "ahmed" });
                o.OnNext(new Employee() { Id = 2, Name = "mohammed" });
                o.OnNext(new Employee() { Id = 3, Name = "gamal" });
                return Disposable.Empty;
            });
            seq2.Contains(
                new Employee() { Id = 1, Name = "ahmed" }, 
                new EmployeeComparer()
                ).Subscribe(Console.WriteLine, () => Console.WriteLine("Completed"));

        }


        private class Employee
        {
            public static readonly Employee Null = new Employee() { Id = 0, Name = "" };

            public int Id { get; init; }
            public string Name { get; init; }

            public override string ToString()
            {
                return $"Employee: {Id}, {Name}";
            }
        }

        private class EmployeeComparer : IEqualityComparer<Employee>
        {
            public bool Equals(Employee x, Employee y)
            {
                x = x == null ? Employee.Null : x;
                y = y == null ? Employee.Null : y;

                // using the overrided tostring method in employee for equality
                return x.ToString() == y.ToString();
            }

            public int GetHashCode(Employee obj)
            {
                obj = obj == null ? Employee.Null : obj;
                return obj.ToString().GetHashCode();
            }
        }
    }

    #endregion

    #region DefaultIfEmpty Operator
    // 
    // DefaultIfEmpty Operator
    // --------------------------------------------------------------------------------
    // The DefaultIfEmpty extension method will return a single value if the source
    // sequence is empty. Depending on the overload used, it will either be the value
    // provided as the default, or Default(T). Default(T) will be the zero value for
    // struct types and will be null for classes. If the source is not empty then all
    // values will be passed straight on through.

    // 
    public class DefaultIfEmptyOperator
    {

        public static void Test()
        {

            Console.WriteLine("------------- Test DefaultIfEmpty With Not Empty Sequence --------------");
            TestDefaultIfEmptyWithNotEmptySequence();
            Console.WriteLine("------------- Test DefaultIfEmpty With Empty Sequence --------------");
            TestDefaultIfEmptyWithEmptySequence();

        }
        public static void TestDefaultIfEmptyWithNotEmptySequence()
        {
            var subject = new Subject<int>();
            subject.Subscribe( Console.WriteLine, () => Console.WriteLine("Subject completed"));
            var defaultIfEmpty = subject.DefaultIfEmpty();
            defaultIfEmpty.Subscribe( b => Console.WriteLine("defaultIfEmpty value: {0}", b), () => Console.WriteLine("defaultIfEmpty completed"));
            subject.OnNext(1);
            subject.OnNext(2);
            subject.OnNext(3);
            subject.OnCompleted();
        }



        public static void TestDefaultIfEmptyWithEmptySequence()
        {
            var subject = new Subject<int>();
            subject.Subscribe( Console.WriteLine, () => Console.WriteLine("Subject completed"));
            var defaultIfEmpty = subject.DefaultIfEmpty();
            defaultIfEmpty.Subscribe( b => Console.WriteLine("defaultIfEmpty value: {0}", b), () => Console.WriteLine("defaultIfEmpty completed"));
            var default42IfEmpty = subject.DefaultIfEmpty(42);
            default42IfEmpty.Subscribe( b => Console.WriteLine("default42IfEmpty value: {0}", b), () => Console.WriteLine("default42IfEmpty completed"));
            subject.OnCompleted();
        }


    }

    #endregion

    #region ElementAt Operator
    // 
    // ElementAt Operator
    // --------------------------------------------------------------------------------
    // The ElementAt extension method allows us to "cherry pick" out a value at a given
    // index. Like the IEnumerable<T> version it is uses a 0 based index.

    // As we can't check the length of an observable sequence it is fair to assume that
    // sometimes this method could cause problems. If your source sequence only produces
    // five values and we ask for ElementAt(5), the result sequence will error with an
    // ArgumentOutOfRangeException inside when the source completes. There are three
    // options we have:

    //1- Handle the OnError gracefully
    //2- Use.Skip(5).Take(1); This will ignore the first 5 values and the only take the
    //   6th value.If the sequence has less than 6 elements we just get an empty sequence,
    //   but no errors.
    //3- Use ElementAtOrDefault
    //   ElementAtOrDefault extension method will protect us in case the index is out of
    //   range, by pushing the Default(T) value. Currently there is not an option to
    //   provide your own default value.

    // 
    public class ElementAtOperator
    {
        public static void Test()
        {
            //var sequence = Observable.Range(1,10).Select((a,i)=> $"value {a} at {i}");
            var sequence = Observable.Range(0, 10); // values are the same as the index of each one
            sequence.ElementAt(3).Subscribe(Console.WriteLine, () => Console.WriteLine("Completed"));
            sequence.ElementAt(4).Subscribe(Console.WriteLine, () => Console.WriteLine("Completed"));
        }
    }

    #endregion

    #region SequenceEquals
    // 
    // SequenceEquals
    // --------------------------------------------------------------------------------
    // 
    // This method allows us to compare two observable sequences.As each source sequence
    // produces values, they are compared to a cache of the other sequence to ensure that
    // each sequence has the same values in the same order and that the sequences are the
    // same length.This means that the result sequence can return false as soon as the
    // source sequences produce diverging values, or true when both sources complete with
    // the same values.
    // 
    public class SequenceEqualsOperator
    {
        public static void Test()
        {
            var seq1 = Observable.Range(1, 10);
            
            var seq2 = Observable.Range(1, 10);
            
            var seq3 = Observable.Range(1, 11);
            
            var seq4 = Observable.Generate(
                1,
                i => i < 11,
                i => i + 1,
                i => i,
                t=> TimeSpan.FromMilliseconds(100)
            );
            
            seq1.SequenceEqual(seq2)
                .Subscribe(val=> Console.WriteLine($"seq1==seq2: {val}"), () => Console.WriteLine("Completed"));

            seq1.SequenceEqual(seq3)
                .Subscribe(val => Console.WriteLine($"seq1==seq3: {val}"), () => Console.WriteLine("Completed"));

            seq1.SequenceEqual(seq4) // returns true but it will wait for seq4 to finish.
                .Subscribe(val => Console.WriteLine($"seq1==seq4: {val}"), () => Console.WriteLine("Completed"));



        }

    }

    #endregion








}
