using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;

namespace FirstRX
{
    class Program
    {
        static void Main(string[] args)
        {

            //ObservableAndObserverImplemetationTest.Test();
            //SubjectTest.Test();
            //ReplaySubjectWithTimeoutTest.Test();
            //BehaviouralSubjectTest.Test();
            //AsyncSubjectTest.Test();


            // lifetime management
            //NoOnErrorActionRegisteredExceptionThrown.Test();
            //TheRightWayTohandleException.Test();
            //IDisposableTypeAndDisposeMethod.Test();

            // ceratingSequences
            //ObservableCreateMethodTest.Test();
            //EmptyReturnNeverThrowWithObservableCreateMethod.Test();
            //ThePowerfullOfObservableCreateFactoryMethod.Test();
            //FunctionalUnfoldsTest.Test();
            //ObservableRangeTest.Test();
            //ObservableGenerateMethod.Test();
            //ObservableIntervalMethod.Test();
            //ObservableTimerTest.Test();
            //TimingOperatorsUsingObservableGenerateMethod.Test();

            //ObservableFromDelegates.Test();
            //ObservableFromTask.Test();
            //ObservableFromEnumerable.Test();

            // Reducing a sequence

            //FilteringWithWhereOperator.Test();
            //FilteringWithDistinctOperator.Test();
            //FilteringWithDistinctUntilChangedOperator.Test();
            //IgnoreElementsOperator.Test();
            //SkipTakeSkipWhileTakeWhileOperators.Test();
            //SkipUntilTakeUntilOperators.Test();

            // Inspection Operators
            //AnyOperator.Test();
            //AllOperator.Test();
            //ContainsOperator.Test();
            //DefaultIfEmptyOperator.Test();
            //ElementAtOperator.Test();
            SequenceEqualsOperator.Test();
            
            Console.ReadLine();
        }

        #region Generate Sequences

        public static void GenerateSequence1()
        {
            Observable.Range(0, 10).Subscribe(value => Console.WriteLine(value));


        }


        public static void GenerateSequence2()
        {

        }


        public static void GenerateSequence3()
        {

        }


        public static void GenerateSequence4()
        {

        }


        public static void GenerateSequence5()
        {

        }


        public static void GenerateSequence6()
        {

        }


        public static void GenerateSequence7()
        {

        }


        public static void GenerateSequence8()
        {

        }


        public static void GenerateSequence9()
        {

        }


        public static void GenerateSequence10()
        {

        }
        #endregion


    }



    #region Implementing IObservable and IObserver

    public class MyConsoleObserver<T> : IObserver<T>
    {
        public void OnCompleted()
        {
            var color = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Sequence Completed.");
            Console.ForegroundColor = color;

        }

        public void OnError(Exception error)
        {
            var color = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Sequence Faulted with {0}", error);
            Console.ForegroundColor = color;
        }

        public void OnNext(T value)
        {
            Console.WriteLine("Received value {0}", value);
        }
    }



    public class SequenceOfNumbers : IObservable<int>
    {
        private List<IObserver<int>> observers = new List<IObserver<int>>();
        public IDisposable Subscribe(IObserver<int> observer)
        {
            observers.Add(observer);
            return Disposable.Create(() => observers.Remove(observer));
        }

        int number = 10;
        public void RunSequence()
        {
            observers.ForEach(o => o.OnNext(number));
            number += 10;
        }

        public void FaultedSequence()
        {
            observers.ForEach(o => o.OnError(new Exception("Sequence is faulted.")));
        }

        public void CompletedSequence()
        {
            observers.ForEach(o => o.OnCompleted());
        }

    }

    public class ObservableAndObserverImplemetationTest
    {
        public static void Test()
        {
            Console.WriteLine("Implementing IObservable and IObserver Test");


            var console = new MyConsoleObserver<int>();
            var file = new OutputFileObserver<int>();

            var observable = new SequenceOfNumbers();
            IDisposable consoleDispose = observable.Subscribe(console);
            IDisposable fileDispose = observable.Subscribe(file);

            observable.RunSequence();
            observable.RunSequence();
            observable.RunSequence();

            fileDispose.Dispose();

            observable.RunSequence();

            //observable.CompletedSequence();
            observable.FaultedSequence();



        }
    }


    #endregion


    #region Subject<T>
    // subjects works as Observable and a publisher
    public class SubjectTest
    {
        public static void Test()
        {
            Subject<string> subject = new Subject<string>();

            var consoleDispose = subject.Subscribe(Console.WriteLine);

            subject.OnNext("1fr sequence element");
            subject.OnNext("2nd sequence element");

            // another subscription
            var outputFileDispose = subject.Subscribe(new OutputFileObserver<string>());

            subject.OnNext("3rd sequence element");
            subject.OnNext("4th sequence element");

            consoleDispose.Dispose();
            outputFileDispose.Dispose();

            subject.OnNext("5th sequence element");
            subject.OnNext("6th sequence element");

            subject.OnCompleted();

        }


    }

    #endregion


    #region ReplaySubject<T> with buffer size
    // subjects works as Observable and a publisher
    // replay subject caches the previous published values, 
    // you can specify bufferSize, to limit the number of values that is cached
    // or you can specify a window that each time it tick, buffer will be cleared.
    // or make both timeout(window) and bufferSize;

    // NOTE: that if you didn't specify a timeout or bufferSize,
    //      memory leakage will happen with infinite sequences

    // NOTE: that a replay-one-subject will still
    // cache its value once it has been completed. 

    // important constructors: 
    //public ReplaySubject();
    //public ReplaySubject(int bufferSize);
    //public ReplaySubject(TimeSpan window);
    //public ReplaySubject(int bufferSize, TimeSpan window);


    public class ReplaySubjectsTest
    {
        public static void Test()
        {

            // buffer size of 2
            ReplaySubject<string> subject = new ReplaySubject<string>(2);

            // output file subscription
            var outputFileDispose = subject.Subscribe(new OutputFileObserver<string>());


            subject.OnNext("1 sequence element");
            subject.OnNext("2 sequence element");
            subject.OnNext("3 sequence element");
            subject.OnNext("4 sequence element");

            // the previous 2 values in the sequence will be pushed to the next subscription.
            var consoleDispose = subject.Subscribe(Console.WriteLine);

            subject.OnNext("5 sequence element");
            subject.OnNext("6 sequence element");
            subject.OnCompleted();


            //consoleDispose.Dispose();
            //outputFileDispose.Dispose();

        }


    }

    #endregion

    #region ReplaySubject<T> with window 
    // window is (a duration that each time it passes, buffer will be cleared );

    // you can specify a window (TimeSpan)

    // NOTE: that if you didn't specify a window or bufferSize,
    //      memory leakage will happen with infinite sequences

    // important constructors: 
    //public ReplaySubject();
    //public ReplaySubject(int bufferSize);
    //public ReplaySubject(TimeSpan window);
    //public ReplaySubject(int bufferSize, TimeSpan window);


    public class ReplaySubjectWithTimeoutTest
    {
        public static void Test()
        {

            // buffer size of 2
            ReplaySubject<string> subject = new ReplaySubject<string>(TimeSpan.FromMilliseconds(200));

            // output file subscription
            var outputFileDispose = subject.Subscribe(new OutputFileObserver<string>());


            subject.OnNext("1 sequence element");
            Thread.Sleep(100);
            subject.OnNext("2 sequence element");
            Thread.Sleep(100);
            subject.OnNext("3 sequence element");
            Thread.Sleep(100);
            subject.OnNext("4 sequence element");

            // the previous 2 values in the sequence will be pushed to the next subscription.
            var consoleDispose = subject.Subscribe(Console.WriteLine);

            subject.OnNext("5 sequence element");
            subject.OnNext("6 sequence element");
            subject.OnCompleted();


            //consoleDispose.Dispose();
            //outputFileDispose.Dispose();

        }


    }

    #endregion

    #region BehaviorSubject<T>
    // BehaviorSubject<T> is similar to ReplaySubject<T> except it only remembers
    // the last publication. BehaviorSubject<T> also requires you to provide it
    // a default value of T.
    // This means that all subscribers will receive a value immediately
    // (unless it is already completed).
    public class BehaviouralSubjectTest
    {
        public static void Test()
        {

            // you must pass an initial sequence value
            BehaviorSubject<string> subject = new BehaviorSubject<string>("1 sequence element");

            // output file subscription
            var outputFileDispose = subject.Subscribe(new OutputFileObserver<string>());


            subject.OnNext("2 sequence element");
            subject.OnNext("3 sequence element");
            subject.OnNext("4 sequence element");

            // the previous value only will be pushed to the next subscription.
            var consoleDispose = subject.Subscribe(Console.WriteLine);

            subject.OnNext("5 sequence element");
            subject.OnNext("6 sequence element");
            subject.OnCompleted();

            // note that the next subscription will be notified with OnCompleted only.
            subject.Subscribe(new OutputFileObserver<string>(OutputFiles.Output2));

            //consoleDispose.Dispose();
            //outputFileDispose.Dispose();

        }


    }


    #endregion


    #region AsyncSubject<T>

    // AsyncSubject<T> is similar to the Replay and Behavior subjects in the way that it caches
    // values, however it will only store the last value, and only publish it when the sequence
    // is completed.The general usage of the AsyncSubject<T> is to only ever publish one value
    // then immediately complete.This means that is becomes quite comparable to Task<T>.
    public class AsyncSubjectTest
    {
        public static void Test()
        {

            var subject = new AsyncSubject<string>();
            subject.OnNext("a");
            subject.Subscribe(Console.WriteLine);
            subject.OnNext("b");
            subject.OnNext("c");
            // if you didn't call OnCompleted, no value would be pushed to the sequence
            // and if you call it, only last value in the sequence will be pushed.
            subject.OnCompleted();
            Console.ReadKey();
        }
    }

    #endregion

    #region Implicit contracts
    //There are implicit contacts that need to be upheld when working with Rx as
    //mentioned above. The key one is that once a sequence is completed, no more
    //activity can happen on that sequence. A sequence can be completed in one of
    //two ways, either by OnCompleted() or by OnError(Exception).

    public class SubjectInvalidUsageExampleTest
    {
        public void Test()
        {
            var subject = new Subject<string>();
            subject.Subscribe(Console.WriteLine);
            subject.OnNext("a");
            subject.OnNext("b");
            subject.OnCompleted();
            subject.OnNext("c");
        }
    }

    #endregion

    #region ISubject Interface

    //        ISubject interfaces
    //While each of the four subjects described in this chapter implement the
    //IObservable<T> and IObserver<T> interfaces, they do so via another set of interfaces:

    //        //Represents an object that is both an observable sequence as well as an observer.
    //        public interface ISubject<in TSource, out TResult>
    //          : IObserver<TSource>, IObservable<TResult>
    //        {
    //        }

    //As all the subjects mentioned here have the same type for both TSource and TResult, they implement this interface which is the superset of all the previous interfaces:
    // 
    //      //Represents an object that is both an observable sequence as well as an observer.
    //      public interface ISubject<T> : ISubject<T, T>, IObserver<T>, IObservable<T>
    //      {
    //      }

    #endregion


    #region Subject factory

    // Finally it is worth making you aware that you can also create a subject via a factory
    // method.Considering that a subject combines the IObservable<T> and IObserver<T> interfaces
    // , it seems sensible that there should be a factory that allows you to combine them yourself.
    // The Subject.Create(IObserver<TSource>, IObservable<TResult>) factory method provides just this.

    //Creates a subject from the specified observer used to publish messages to the subject
    //  and observable used to subscribe to messages sent from the subject

    //      public static ISubject<TSource, TResult> Create<TSource, TResult>(
    //      IObserver<TSource> observer,
    //      IObservable<TResult> observable)
    //      { ...}

    #endregion



    public class MySource
    {
        public bool IsStart { get; private set; }

        public MySource()
        {

        }


        public void StartOperation()
        {
            Console.WriteLine("Starting....");
            OnStart(new EventArgs());
            IsStart = true;
        }




        public void StopOperation()
        {
            Console.WriteLine("Stop....");
            OnStop(new EventArgs());
            IsStart = false;

        }

        public event EventHandler Start;
        public event EventHandler Stop;


        protected void OnStart(EventArgs args)
        {
            Start?.Invoke(this, args);
        }

        protected void OnStop(EventArgs args)
        {
            Stop?.Invoke(this, args);
        }

    }
}
