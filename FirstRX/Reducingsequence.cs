using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;

namespace FirstRX
{
    // Reducing a sequence
    #region Reducing a sequence
    //    we will now look at the various methods that can reduce an observable sequence.
    //    We can categorize operators that reduce a sequence to the following:

    // 1- Filter and partition operators
    //   Reduce the source sequence to a sequence with at most the same number of elements
    // 2- Aggregation operators
    //   Reduce the source sequence to a sequence with a single element
    // 3- Fold operators
    //   Reduce the source sequence to a single element as a scalar value


    // the creation of an observable sequence from a scalar value is defined as anamorphism
    // or described as an 'unfold'.

    // Other popular names for fold are 'reduce', 'accumulate' and 'inject'.

    // 
    // <<<<< Not Right English words but i will continue -__-
    //       then we can also say that other names of unfold are unreduce,
    //       unaccumulate, and uninject
    // >>>>>


    #endregion


    #region Where Operator
    // 
    // Where Operator
    // --------------------------------------------------------------------------------
    // 

    // 
    public class FilteringWithWhereOperator
    {
        public static void Test()
        {
            var oddNumbers = Observable.Range(0, 20)
                .Where(value => value % 2 != 0); // fileter only odd numbers

            oddNumbers.Subscribe(Console.WriteLine, () => Console.WriteLine("Completed."));

        }
    }

    #endregion


    #region Distinct Operator
    // 
    // Distinct Operator
    // --------------------------------------------------------------------------------
    // 

    // 
    public class FilteringWithDistinctOperator
    {
        public static void Test()
        {
            Console.WriteLine("---------- Distinct ----------");
            UsingSubjectsForClarity();

            Console.WriteLine("---------- Distinct With Key Selector ----------");
            DistinctWithSelector();

            Console.WriteLine("---------- Distinct With EqualityComparer ----------");
            DistinctWithEqualityComparer();
        }


        public static void UsingCreate()
        {
            var sequence = Observable.Create<int>((o) =>
            {
                o.OnNext(1);
                o.OnNext(2);
                o.OnNext(3);
                o.OnNext(1);
                o.OnNext(1);
                o.OnNext(2);
                o.OnNext(3);
                o.OnCompleted();
                return Disposable.Empty;
            });

            var distinctSequence = sequence.Distinct();
            sequence.Subscribe(
                (value) => Console.WriteLine("normal sequence value: {0}", value),
                () => Console.WriteLine("normal sequence completed")
            );

            distinctSequence.Subscribe(
                (value) => Console.WriteLine("Distinct sequence value: {0}", value),
                () => Console.WriteLine("Distinct sequence completed")
            );

        }


        public static void UsingSubjectsForClarity()
        {
            var subject = new Subject<int>();
            var distinct = subject.Distinct();
            subject.Subscribe(
            i => Console.WriteLine("{0}", i),
            () => Console.WriteLine("subject.OnCompleted()"));
            distinct.Subscribe(
            i => Console.WriteLine("distinct.OnNext({0})", i),
            () => Console.WriteLine("distinct.OnCompleted()"));
            subject.OnNext(1);
            subject.OnNext(2);
            subject.OnNext(3);
            subject.OnNext(1);
            subject.OnNext(1);
            subject.OnNext(4);
            subject.OnCompleted();
        }


        public static void DistinctWithSelector()
        {
            var sequence = Observable.Create<Employee>((o) =>
            {
                o.OnNext(new Employee() { Id = 1, Name = "ahmed" });
                o.OnNext(new Employee() { Id = 2, Name = "mohammed" });
                o.OnNext(new Employee() { Id = 3, Name = "gamal" });
                o.OnNext(new Employee() { Id = 1, Name = "samir" });
                o.OnNext(new Employee() { Id = 1, Name = "waleed" });
                o.OnNext(new Employee() { Id = 2, Name = "kamal" });
                return Disposable.Empty;
            });

            // making uniqueness depends on the id, by using keySelector of the Distinct operator
            var distinctSequence = sequence.Distinct(o => o.Id);

            distinctSequence.Subscribe(
                (value) => Console.WriteLine("Distinct sequence value: {0}", value),
                () => Console.WriteLine("Distinct sequence completed")
            );

        }

        public static void DistinctWithEqualityComparer()
        {
            var sequence = Observable.Create<Employee>((o) =>
            {
                o.OnNext(new Employee() { Id = 1, Name = "ahmed" });
                o.OnNext(new Employee() { Id = 2, Name = "mohammed" });
                o.OnNext(new Employee() { Id = 3, Name = "gamal" });
                o.OnNext(new Employee() { Id = 1, Name = "ahmed" });
                o.OnNext(new Employee() { Id = 2, Name = "mohammed" });
                o.OnNext(new Employee() { Id = 3, Name = "gamal" });

                return Disposable.Empty;
            });

            // making uniqueness depends on the id, by using keySelector of the Distinct operator
            var distinctSequence = sequence.Distinct(new EmployeeComparer());

            distinctSequence.Subscribe(
                (value) => Console.WriteLine("Distinct sequence value: {0}", value),
                () => Console.WriteLine("Distinct sequence completed")
            );

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



    #region Distinct Until Changed Operator
    // 
    // Distinct Until Changed Operator
    // --------------------------------------------------------------------------------
    // 

    // 
    public class FilteringWithDistinctUntilChangedOperator
    {
        public static void Test()
        {
            Console.WriteLine("---------- DistinctUntilChanged ----------");
            UsingSubjectsForClarity();

            Console.WriteLine("---------- DistinctUntilChanged With Key Selector ----------");
            DistinctUntilChangedWithSelector();

            Console.WriteLine("---------- DistinctUntilChanged With Equality Comparer----------");
            DistinctUntilChangedWithEqualityComparer();

        }


        public static void UsingCreate()
        {
            var sequence = Observable.Create<int>((o) =>
            {
                o.OnNext(1);
                o.OnNext(2);
                o.OnNext(3);
                o.OnNext(1);
                o.OnNext(1);
                o.OnNext(2);
                o.OnNext(3);
                o.OnCompleted();
                return Disposable.Empty;
            });

            var distinctUntilChangeSequence = sequence.DistinctUntilChanged();
            sequence.Subscribe(
                (value) => Console.WriteLine("normal sequence value: {0}", value),
                () => Console.WriteLine("normal sequence completed")
            );

            distinctUntilChangeSequence.Subscribe(
                (value) => Console.WriteLine("Distinct Until Change sequence value: {0}", value),
                () => Console.WriteLine("Distinct Until Change sequence completed")
            );
        }


        public static void UsingSubjectsForClarity()
        {
            var subject = new Subject<int>();
            var distinctUntilChanged = subject.DistinctUntilChanged();
            subject.Subscribe(
            i => Console.WriteLine("{0}", i),
            () => Console.WriteLine("subject.OnCompleted()"));
            distinctUntilChanged.Subscribe(
            i => Console.WriteLine("distinctUntilChanged.OnNext({0})", i),
            () => Console.WriteLine("distinctUntilChanged.OnCompleted()"));
            subject.OnNext(1);
            subject.OnNext(2);
            subject.OnNext(3);
            subject.OnNext(1);
            subject.OnNext(1); // filtered at distinct sequence
            subject.OnNext(4);
            subject.OnCompleted();
        }

        public static void DistinctUntilChangedWithSelector()
        {
            var sequence = Observable.Create<Employee>((o) =>
            {
                o.OnNext(new Employee() { Id = 1, Name = "ahmed" });
                o.OnNext(new Employee() { Id = 2, Name = "mohammed" });
                o.OnNext(new Employee() { Id = 3, Name = "gamal" });
                o.OnNext(new Employee() { Id = 1, Name = "samir" });
                o.OnNext(new Employee() { Id = 1, Name = "waleed" }); // ignored as it has the same previous id
                o.OnNext(new Employee() { Id = 2, Name = "kamal" });
                return Disposable.Empty;
            });

            // making uniqueness depends on the id, by using keySelector of the DistinctUntilChanged operator
            var distinctSequence = sequence.DistinctUntilChanged(o => o.Id);

            distinctSequence.Subscribe(
                (value) => Console.WriteLine("DistinctUntilChanged sequence value: {0}", value),
                () => Console.WriteLine("DistinctUntilChanged sequence completed")
            );

        }

        public static void DistinctUntilChangedWithEqualityComparer()
        {
            var sequence = Observable.Create<Employee>((o) =>
            {
                o.OnNext(new Employee() { Id = 1, Name = "ahmed" });
                o.OnNext(new Employee() { Id = 1, Name = "ahmed" }); // ignored
                o.OnNext(new Employee() { Id = 2, Name = "mohammed" });
                o.OnNext(new Employee() { Id = 2, Name = "mohammed" }); // // ignored
                o.OnNext(new Employee() { Id = 3, Name = "gamal" });
                o.OnNext(new Employee() { Id = 3, Name = "gamal" }); // ignored
                o.OnNext(new Employee() { Id = 1, Name = "ahmed" });
                o.OnNext(new Employee() { Id = 2, Name = "mohammed" });
                o.OnNext(new Employee() { Id = 3, Name = "gamal" });

                return Disposable.Empty;
            });

            // making uniqueness depends on the id, by using keySelector of the DistinctUntilChanged operator
            var distinctSequence = sequence.DistinctUntilChanged(new EmployeeComparer());

            distinctSequence.Subscribe(
                (value) => Console.WriteLine("DistinctUntilChanged sequence value: {0}", value),
                () => Console.WriteLine("DistinctUntilChanged sequence completed")
            );

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


    #region IgnoreElements
    // 
    // IgnoreElements
    // --------------------------------------------------------------------------------
    // The IgnoreElements extension method is a quirky little tool that allows you to receive
    // the OnCompleted or OnError notifications.We could effectively recreate it by using
    // a Where method with a predicate that always returns false.
    // 

    // 
    public class IgnoreElementsOperator
    {
        public static void Test()
        {
            var subject = new Subject<int>();
            //Could use subject.Where(_=>false);
            var noElements = subject.IgnoreElements();
            
            subject.Subscribe(
            i => Console.WriteLine("subject.OnNext({0})", i),
            () => Console.WriteLine("subject.OnCompleted()"));

            noElements.Subscribe(
            i => Console.WriteLine("noElements.OnNext({0})", i),
            () => Console.WriteLine("noElements.OnCompleted()"));
            subject.OnNext(1);
            subject.OnNext(2);
            subject.OnNext(3);
            subject.OnCompleted();
        }
    }

    #endregion


    #region Skip, Take, SkipWhile, TakeWhile, TakeLast, and SkipLast
    // 
    // Skip, Take, SkipWhile, TakeWhile, TakeLast, and SkipLast
    // --------------------------------------------------------------------------------
    // 

    // 
    public class SkipTakeSkipWhileTakeWhileOperators
    {
        public static void Test()
        {
            var sequence = Observable.Range(1, 10);

            Console.WriteLine("------- Take Operator -------");
            sequence.Take(5).Subscribe(Console.WriteLine, () => Console.WriteLine("Completed"));


            Console.WriteLine("------- Skip Operator -------");
            sequence.Skip(5).Subscribe(Console.WriteLine, () => Console.WriteLine("Completed"));


            Console.WriteLine("------- TakeWhile Operator -------");
            sequence.TakeWhile(x=> x < 7).Subscribe(Console.WriteLine, () => Console.WriteLine("Completed"));

            Console.WriteLine("------- SkipWhile Operator -------");
            sequence.SkipWhile(x => x < 7).Subscribe(Console.WriteLine, () => Console.WriteLine("Completed"));


            Console.WriteLine("------- TakeLast Operator -------");
            // it waits until the sequence marked as completed
            sequence.TakeLast(3).Subscribe(Console.WriteLine, () => Console.WriteLine("Completed"));

            Console.WriteLine("------- SkipLast Operator -------");

            // it will push the result one by one once it has queued more than 3 items
            sequence.SkipLast(3).Subscribe(Console.WriteLine, () => Console.WriteLine("Completed"));


        }



        // SkipLast Note
        // -----------------
        // The implementation will queue the specified number of notifications and once
        // the queue size exceeds the value, it can be sure that it may drain a value from the
        // queue.
        public static void SkipLastUsingSubjectsForClarity()
        {
            var subject = new Subject<int>();
            subject
            .SkipLast(2)
            .Subscribe(Console.WriteLine, () => Console.WriteLine("Completed"));
            Console.WriteLine("Pushing 1");
            subject.OnNext(1);
            Console.WriteLine("Pushing 2");
            subject.OnNext(2);
            Console.WriteLine("Pushing 3");
            subject.OnNext(3);
            Console.WriteLine("Pushing 4");
            subject.OnNext(4);
            subject.OnCompleted();

            // output
            //Pushing 1
            //Pushing 2
            //Pushing 3
            //1
            //Pushing 4
            //2
            //Completed
        }


    }

    #endregion


    #region SkipUntil and TakeUntil
    // 
    // SkipUntil and TakeUntil
    // --------------------------------------------------------------------------------
    // 

    // 
    public class SkipUntilTakeUntilOperators
    {
        public static void Test()
        {
            var timerSeq = Observable.Timer(TimeSpan.FromMilliseconds(1000));

            var seq = Observable.Interval(TimeSpan.FromMilliseconds(200));

            seq.TakeUntil(timerSeq)
               .Subscribe(Console.WriteLine, () => Console.WriteLine("Completed"));


            // is the same as TakeUntil but reversed
            //seq.SkipUntil(timerSeq)
            //   .Subscribe(Console.WriteLine, () => Console.WriteLine("Completed"));


        }


        public static void SkipUntilUsingSubjectsForClarity()
        {

            var subject = new Subject<int>();
            var otherSubject = new Subject<Unit>();
            subject
            .SkipUntil(otherSubject)
            .Subscribe(Console.WriteLine, () => Console.WriteLine("Completed"));
            subject.OnNext(1);
            subject.OnNext(2);
            subject.OnNext(3);
            otherSubject.OnNext(Unit.Default);
            subject.OnNext(4);
            subject.OnNext(5);
            subject.OnNext(6);
            subject.OnNext(7);
            subject.OnNext(8);
            subject.OnCompleted();

            // output
            //4
            //5
            //6
            //7
            //Completed
        }


        public static void TakeUntilUsingSubjectsForClarity()
        {

            var subject = new Subject<int>();
            var otherSubject = new Subject<Unit>();
            subject
            .TakeUntil(otherSubject)
            .Subscribe(Console.WriteLine, () => Console.WriteLine("Completed"));
            subject.OnNext(1);
            subject.OnNext(2);
            subject.OnNext(3);
            otherSubject.OnNext(Unit.Default);
            subject.OnNext(4);
            subject.OnNext(5);
            subject.OnNext(6);
            subject.OnNext(7);
            subject.OnNext(8);
            subject.OnCompleted();

            // output
            //1
            //2
            //3
            //Completed

        }

    }

    #endregion





}
