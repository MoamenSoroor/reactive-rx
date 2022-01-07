using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace FirstRX.SequenceBasics
{
    #region Simple factory methods

    // 
    // Simple factory methods

    // --------------------------------------------------------------------------------
    // 

    // 
    public class SimpleFactoryMethods
    {
        public static void Test()
        {

            // Observable.Return
            // -------------------------------------------------------
            var singleValue = Observable.Return("hello world");

            //which could have also been simulated with a replay subject
            var subject = new ReplaySubject<string>();
            subject.OnNext("Value");
            subject.OnCompleted();



            //Observable.Empty
            //The next two examples only need the type parameter to unfold into an
            //observable sequence. The first is Observable.Empty<T>(). This returns an
            //empty IObservable<T> i.e.it just publishes an OnCompleted notification.
            // -------------------------------------------------------
            var empty = Observable.Empty<string>();
            //Behaviorally equivalent to
            var emptySubject = new ReplaySubject<string>();
            subject.OnCompleted();



            // Observable.Never
            // The Observable.Never<T>() method will return infinite sequence without
            // any notifications.

            var never = Observable.Never<string>();
            //similar to a subject without notifications
            var neverSubject = new Subject<string>();




            // Observable.Throw
            // Observable.Throw<T>(Exception) method needs the type parameter information,
            // it also need the Exception that it will OnError with.This method creates a
            // sequence with just a single OnError notification containing the exception
            // passed to the factory.

            var throws = Observable.Throw<string>(new Exception());
            //Behaviorally equivalent to
            var throwsSubject = new ReplaySubject<string>();
            subject.OnError(new Exception());



            // Observable.Create
            //Creates an observable sequence from a specified Subscribe method implementation.
            //public static IObservable<TSource> Create<TSource>(
            //Func<IObserver<TSource>, IDisposable> subscribe)
            //{ ...}

            //public static IObservable<TSource> Create<TSource>(
            //Func<IObserver<TSource>, Action> subscribe)
            //{ ...}

        }
    }

    #endregion


    #region Observable.Create
    // 
    // Observable.Create
    // --------------------------------------------------------------------------------
    // - it is better than subjects, you should use subject to test only, as 

    // - many of the operators (extension methods) have been carefully written to
    //   ensure correct and consistent lifetime of subscriptions and sequences are
    //   maintained.
    // - it support functional programming, immutablity, and async programming
    // - it is lazely evaluated,Lazy evaluation is a very important part of Rx. It opens
    //   doors to other powerful features such as scheduling and combination of sequences
    //   that we will see later. The delegate will only be invoked when
    //   a subscription is made.

    // 
    public class ObservableCreateMethodTest
    {
        public static void Test()
        {
            var sequence = NonBlocking();

            sequence.Subscribe(Console.WriteLine, Console.WriteLine, Console.WriteLine);


        }

        private static IObservable<string> BlockingMethod()
        {
            var subject = new ReplaySubject<string>();
            subject.OnNext("a");
            subject.OnNext("b");
            subject.OnCompleted();
            Thread.Sleep(1000);
            return subject;
        }
        private static IObservable<string> NonBlocking()
        {

            // when an IObserver is subscribed, the func will be executed (so it is lazely executed)
            return Observable.Create<string>(
                (IObserver<string> observer) =>
                {
                    observer.OnNext("first");
                    observer.OnNext("second");
                    observer.OnCompleted();
                    Thread.Sleep(1000);
                    return Disposable.Create(() => Console.WriteLine("Observer has unsubscribed"));
                    //    //or can return an Action like 
                    //    //return () => Console.WriteLine("Observer has unsubscribed"); 
                }

                );

        }


    }


    #endregion

    #region Empty, Return, Never and Throw With Observable.Create
    // 
    // Empty, Return, Never and Throw With Observable.Create
    // --------------------------------------------------------------------------------
    // 

    // 
    public class EmptyReturnNeverThrowWithObservableCreateMethod
    {
        public static void Test()
        {
            var empty = Empty<string>();
            empty.Subscribe(Console.WriteLine);

            Console.WriteLine("".PadLeft(50, '-'));

            var returnSeq = Return<string>("Hello World");
            returnSeq.Subscribe(Console.WriteLine);

            Console.WriteLine("".PadLeft(50, '-'));

            var throwsSeq = Throws<string>(new Exception("Not valid, error happened"));
            throwsSeq.Subscribe(Console.WriteLine, Console.WriteLine, Console.WriteLine);

        }

        public static IObservable<T> Empty<T>()
        {
            var observable = Observable.Create<T>(
                (IObserver<T> observer) =>
                {
                    observer.OnCompleted();
                    return Disposable.Empty;
                }
                );
            return observable;
        }


        public static IObservable<T> Return<T>(T value)
        {
            var observable = Observable.Create<T>(
                (IObserver<T> observer) =>
                {
                    observer.OnNext(value);
                    observer.OnCompleted();
                    return Disposable.Empty;
                }
                );
            return observable;
        }

        public static IObservable<T> Throws<T>(Exception ex)
        {
            var observable = Observable.Create<T>(
                (IObserver<T> observer) =>
                {
                    observer.OnError(ex);
                    observer.OnCompleted();
                    return Disposable.Empty;
                }
                );
            return observable;
        }

    }

    #endregion

    #region The Powerfull Of Observable.Create Factory Method
    // 
    // The Powerfull Of Observable.Create Factory Method
    // --------------------------------------------------------------------------------
    // 

    // 
    public class ThePowerfullOfObservableCreateFactoryMethod
    {
        public static void Test()
        {
            Console.WriteLine("<<<< Press any key to dispose the sequence >>>>");
            var sequence = TimeSequence();
            IDisposable disposeConsole = sequence.Subscribe(Console.WriteLine, () => Console.WriteLine("completed"));
            Console.ReadLine();
            disposeConsole.Dispose();


        }

        // Non Blocking Event Driven
        public static IObservable<string> TimeSequence(double interval = 1000)
        {
            IObservable<string> observable = Observable.Create<string>(
                (IObserver<string> observer) =>
                {
                    var timer = new System.Timers.Timer();
                    timer.Interval = interval;
                    timer.Elapsed += (object sender, ElapsedEventArgs e) =>
                    {
                        observer.OnNext($"Tick");
                    };
                    timer.Elapsed += Timer_Elapsed;
                    timer.Start();
                    return Disposable.Create(() =>
                    {

                        // if you forgot to release the event handler 
                        // it will still executed and will be tied to the timer 
                        // until it will be collected by garbage collector.
                        timer.Elapsed -= Timer_Elapsed;

                        // if you forgot to dispose timer, it will work infinitely.
                        timer.Dispose();

                        // observer.OnCompleted(); 
                        // if you call OnCompleted() here it will not be executed
                        // 

                    });


                    // return timer; // you can return timer as it is Disposable 
                }
                );
            return observable;
        }
        private static void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Console.WriteLine($"Timer info: {e.SignalTime}");
        }





    }

    #endregion

    #region Functional unfolds: Unfolding Infinite Sequences using Corecursion
    // 
    // Functional unfolds: Unfolding Sequences using Corecursion
    // --------------------------------------------------------------------------------
    // 
    // As a functional programmer you would come to expect the ability to unfold a
    // potentially infinite sequence.An issue we may face with Observable.Create is that
    // it can be a clumsy way to produce an infinite sequence.Our timer example above is
    // an example of an infinite sequence, and while this is a simple implementation it is
    // an annoying amount of code for something that effectively is delegating all the work
    // to the System.Timers.Timer class. The Observable.Create method also has poor support
    // for unfolding sequences using corecursion.
    // 


    // Corecursion
    // ---------------

    // Corecursion is a function to apply to the current state to produce the next state.
    // Using corecursion by taking a value, applying a function to it that extends that
    // value and repeating we can create a sequence.

    public class FunctionalUnfoldsTest
    {


        public static void Test()
        {
            var numbers = Unfold(1, i => i + 1);

            foreach (var i in numbers.Take(10))
            {
                Console.WriteLine(i);
            }



        }

        private static IEnumerable<T> Unfold<T>(T seed, Func<T, T> accumulator)
        {
            var current = seed;
            while (true)
            {
                yield return current;
                current = accumulator(current);
            }
        }


    }

    // using pull mechanism inside push mechainsm to be able to unfold infinite sequence
    public class PullInsidePushMechanism
    {


        public static void Test()
        {
            IObservable<Func<int>> seq = Unfold(1, (value) => value + 2);

            seq.Subscribe((next) =>
            {
                // now you can consume next
                Console.WriteLine($"1nd Subscriber: {next()}");
                Console.WriteLine($"1nd Subscriber: {next()}");
            });


            seq.Subscribe((next) =>
            {
                // now you can consume next
                Console.WriteLine($"2nd Subscriber: {next()}");
                Console.WriteLine($"2nd Subscriber: {next()}");
                Console.WriteLine($"2nd Subscriber: {next()}");
                Console.WriteLine($"2nd Subscriber: {next()}");

            });

        }

        // converting the push based mechanism to pull based mechanism
        public static IObservable<Func<T>> Unfold<T>(T seed, Func<T, T> accumulator)
        {
            IObservable<Func<T>> observable = Observable.Create<Func<T>>(
                (IObserver<Func<T>> observer) =>
                {

                    var current = seed;
                    observer.OnNext(() =>
                    {
                        var result = current;
                        current = accumulator(current);
                        return current;
                    });
                    return Disposable.Empty;
                });
            return observable;
        }

        public static IObservable<T> Unfold2<T>(T seed, Func<T, T> accumulator)
        {
            IObservable<T> observable = Observable.Create<T>(
                (IObserver<T> observer) =>
                {
                    bool end = false;
                    var current = seed;
                    while (!end)
                    {
                        observer.OnNext(current); 
                        // not that the observer can't lazy request a next value
                        // we push values to it, and he has no control except to dispose.
                        current = accumulator(current);

                    }
                    return Disposable.Create(() => end = true);
                });
            return observable;

        }

    }

    #endregion

    #region Observable.Range 
    // 
    // Observable.Range
    // --------------------------------------------------------------------------------
    // Observable.Range(int, int) simply returns a range of integers.The first integer
    // is the initial value and the second is the number of values to yield.This
    // example will write the values '10' through to '24' and then complete.


    // 
    public class ObservableRangeTest
    {
        public static void Test()
        {
            var seq = Observable.Range(1, 10);

            seq.Subscribe(Console.WriteLine, () => Console.WriteLine("Completed"));
        }
    }

    #endregion


    #region Observable.Generate and Create your own Observable.Range
    // 
    // Observable.Generate and Create your own Observable.Range
    // --------------------------------------------------------------------------------
    // In Rx the unfold method is called Observable.Generate.

    //  The simple version of Observable.Generate takes the following parameters:
    //  - an initial state
    //  - a predicate that defines when the sequence should terminate
    //  - a function to apply to the current state to produce the next state
    //  - a function to transform the state to the desired output

    //  public static IObservable<TResult> Generate<TState, TResult>(
    //      TState initialState,
    //      Func<TState, bool> condition,
    //      Func<TState, TState> iterate,
    //      Func<TState, TResult> resultSelector)

    // 
    public class ObservableGenerateMethod
    {
        public static void Test()
        {
            var rangeSeq = Range(1, 10);

            rangeSeq.Subscribe(Console.WriteLine, () => Console.WriteLine("Completed"));
        }

        public static IObservable<int> Range(int start, int count)
        {
            return Observable.Generate(start, (i) => i < start + count, (i) => i + 1, i => i);
        }

    }

    #endregion



    #region Observable.Interval
    // 
    // Observable.Interval
    // --------------------------------------------------------------------------------
    // 
    //Earlier in the chapter we used a System.Timers.Timer in our observable to
    //generate a continuous sequence of notifications.As mentioned in the example at the
    //time, this is not the preferred way of working with timers in Rx.As Rx provides
    //operators that give us this functionality it could be argued that to not use them
    //is to re-invent the wheel.More importantly the Rx operators are the preferred way
    //of working with timers due to their ability to substitute in schedulers which is
    //desirable for easy substitution of the underlying timer. There are at least three
    //various timers you could choose from for the example above:

    // - System.Timers.Timer
    // - System.Threading.Timer
    // - System.Windows.Threading.DispatcherTimer


    // By abstracting the timer away via a scheduler we are able to reuse the same code
    // for multiple platforms.More importantly than being able to write platform independent
    // code is the ability to substitute in a test-double scheduler/timer to enable testing.

    // 
    public class ObservableIntervalMethod
    {
        public static void Test()
        {
            Console.WriteLine("<<< Press Enter to stop the sequence >>>");

            IObservable<long> intervalSeq = Observable.Interval(TimeSpan.FromMilliseconds(500));

            var dispose = intervalSeq.Subscribe(Console.WriteLine, () => Console.WriteLine("completed"));

            Console.ReadLine();


            // Once subscribed, you must dispose of your subscription to stop the
            // sequence.It is an example of an infinite sequence.

            dispose.Dispose();

        }
    }

    #endregion

    #region Observable.Timer 
    // 
    // Observable.Timer 
    // --------------------------------------------------------------------------------
    // The Observable.Timer will however only publish one value (0) after the period of
    // time has elapsed, and then it will complete.

    class PushOneValueAfterPeriod
    {

        public static void Test()
        {
            Console.WriteLine("<<< Press Enter to unsubscribe observers and stop sequences >>>");

            var seq = Observable.Timer(TimeSpan.FromMilliseconds(3000));

            var disposeSeq = seq.Subscribe((value) => Console.WriteLine("Seq-Subscriber received {0}", value), () => Console.WriteLine("Seq3-Subscriber completed"));
            var disposeSeq2 = seq.Subscribe((value) => Console.WriteLine("Seq-Subscriber received {0}", value), () => Console.WriteLine("Seq3-Subscriber completed"));

            Console.ReadLine();
            disposeSeq.Dispose();
            disposeSeq2.Dispose();

        }

    }


    // use timer like interval with the control of the initial period
    public class ObservableTimerTest
    {
        public static void Test()
        {

            Console.WriteLine("<<< Press Enter to unsubscribe observers and stop sequences >>>");

            // first tick will be after 500 ms , then timer will work exactly like Interval Operator
            var seq = Observable.Timer(TimeSpan.FromMilliseconds(1000), TimeSpan.FromMilliseconds(2000));
            var disposeSeq = seq.Subscribe((value) => Console.WriteLine("Subscriber received {0}", value), () => Console.WriteLine("Subscriber completed"));


            //var seq3 = Interval(TimeSpan.FromMilliseconds(5000));
            //var disposeSeq3 = seq3.Subscribe((value)=> Console.WriteLine("Seq3-Subscriber received {0}", value), () => Console.WriteLine("Seq3-Subscriber completed"));

            Console.ReadLine();
            disposeSeq.Dispose();
            //disposeSeq3.Dispose();

        }


        // make Observer.Interval Operator with Timer operator
        public static IObservable<long> Interval(TimeSpan period)
        {
            return Observable.Timer(period, period);
        }

    }




    // Note that this now returns an IObservable of long not int. While Observable.Interval
    // would always wait the given period before producing the first value, this Observable.
    // Timer overload gives the ability to start the sequence when you choose.
    // With Observable.



    // Timer you can write the following to have an interval sequence
    // that started immediately. 
    class IntervalStartsImmediatelyUsingTimer
    {

        public static void Test()
        {
            Console.WriteLine("<<< Press Enter to unsubscribe observers and stop sequences >>>");

            var seq = Observable.Timer(TimeSpan.Zero, TimeSpan.FromMilliseconds(2000));

            var disposeSeq = seq.Subscribe((value) => Console.WriteLine("Seq-Subscriber received {0}", value), () => Console.WriteLine("Seq3-Subscriber completed"));

            Console.ReadLine();
            disposeSeq.Dispose();

        }

    }



    #endregion



    #region Timing Operators using Observable.Generate
    // 
    // Timing Operators using Observable.Generate
    // --------------------------------------------------------------------------------
    // we talk about the implementation of Generate method that takes timeSelector
    // parameter

    //  public static IObservable<TResult> Generate<TState, TResult>(
    //  TState initialState,
    //  Func<TState, bool> condition,
    //  Func<TState, TState> iterate,
    //  Func<TState, TResult> resultSelector,
    //  Func<TState, TimeSpan> timeSelector)

    // 
    public class TimingOperatorsUsingObservableGenerateMethod
    {
        public static void Test()
        {
            Console.WriteLine("<<< Press Enter to unsubscribe observers and stop sequences >>>");

            var seq = Interval(TimeSpan.FromMilliseconds(5000));
            var disposeSeq = seq.Subscribe((value) => Console.WriteLine("Subscriber received {0}", value), () => Console.WriteLine("Subscriber completed"));

            Console.ReadLine();
            disposeSeq.Dispose();
        }

        public static IObservable<long> Timer(TimeSpan dueTime)
        {
            return Observable.Generate(0L, i => i < 1, i => i + 1, i => i, t => dueTime);
        }
        public static IObservable<long> Timer(TimeSpan dueTime, TimeSpan period)
        {
            return Observable.Generate(0L, i => true, i => i + 1, i => i, i => i == 0L ? dueTime : period);

        }
        public static IObservable<long> Interval(TimeSpan period)
        {
            return Observable.Generate(0L, i => true, i => i + 1, i => i, t => period);

        }


    }

    #endregion



    #region Transitioning into IObservable<T>

    //You can also start a sequence by simply making a transition from an existing
    //synchronous or asynchronous paradigm into the Rx paradigm.

    // The Observable.Start method allows you to turn a long running Func<T> or Action
    // into a single value observable sequence. By default, the processing will be done
    // asynchronously on a ThreadPool thread. If the overload you use is a Func<T> then
    // the return type will be IObservable<T>. When the function returns its value, that
    // value will be published and then the sequence completed. If you use the overload
    // that takes an Action, then the returned sequence will be of type IObservable<Unit>.
    // The Unit type is a functional programming construct and is analogous to void.
    // In this case Unit is used to publish an acknowledgement that the Action is complete,
    // however this is rather inconsequential as the sequence is immediately completed
    // straight after Unit anyway. The Unit type itself has no value; it just serves as an
    // empty payload for the OnNext notification. Below is an example of
    // using both overloads

    // From delegates
    class ObservableFromDelegates
    {

        public static void Test()
        {
            StartAction();
            StartFunc();
        }

        static void StartAction()
        {
            var start = Observable.Start(() =>
            {
                Console.Write("Working away");
                for (int i = 0; i < 10; i++)
                {
                    Thread.Sleep(100);
                    Console.Write(".");
                }
            });
            // Unit is an equivelent to void, is is only to be passed to 
            // the OnNext method when Action Delegate finish its work async
            start.Subscribe(
            (Unit unit) => Console.WriteLine("Unit published"),
            () => Console.WriteLine("Action completed"));
        }
        static void StartFunc()
        {
            var start = Observable.Start(() =>
            {
                Console.Write("Working away");
                for (int i = 0; i < 10; i++)
                {
                    Thread.Sleep(100);
                    Console.Write(".");
                }
                return "Published value";
            });
            start.Subscribe(
            Console.WriteLine,
            () => Console.WriteLine("Action completed"));

        }
    }


    #endregion


    #region From FromEventPattern
    // 
    // From FromEventPattern
    // --------------------------------------------------------------------------------
    // Rx provides methods to take an event and turn it into an observable sequence.
    // There are several different varieties you can use.



    public class ObservableFromEventPattern
    {

        private static event EventHandler<MyEventArgs> MyEvent;
        public static void Test()
        {
            // convert event to observable
            var source = Observable.FromEventPattern<MyEventArgs>(handler => MyEvent += handler, handler => MyEvent += handler);

            source.Select(v => v.EventArgs.Value).Subscribe(Console.WriteLine);


            FireEvent();
        }

        static void FireEvent()
        {
            MyEvent?.Invoke(null, new MyEventArgs(10));
            Thread.Sleep(300);
            MyEvent?.Invoke(null, new MyEventArgs(20));
            Thread.Sleep(300);
            MyEvent?.Invoke(null, new MyEventArgs(30));
        }



        class MyEventArgs : EventArgs
        {
            private readonly long _value;
            public MyEventArgs(long value)
            {
                _value = value;
            }
            public long Value
            {
                get { return _value; }
            }
        }


        
    }




    #endregion


    #region From FromEvent
    // 
    // From FromEvent
    // --------------------------------------------------------------------------------
    // Rx provides methods to take an event and turn it into an observable sequence.
    // There are several different varieties you can use.



    public class ObservableFromEvent
    {


        public static void Test()
        {
            var foo = new Foo();

            //var observableBar = Observable.FromEvent<BarHandler, string>(
            //onNextHandler => (int x, string y) => onNextHandler("X:" + x + " Y:" + y),
            //h => foo.BarEvent += h,
            //h => foo.BarEvent -= h);

            var source = Observable.FromEvent<BarHandler, string>(onNext =>
            {
                BarHandler h = (string x, int y) => onNext($"{x}-{y}");
                return h;
            }, handler => foo.BarEvent += handler, handler => foo.BarEvent -= handler);

            source.Subscribe(Console.WriteLine);

            // fire event
            foo.RaiseBar("I am ",1);
            foo.RaiseBar("I am ",2);
            foo.RaiseBar("I am ",3);



        }


        delegate void BarHandler(string x, int y);

        class Foo
        {
            private BarHandler delegateChain;

            public event BarHandler BarEvent
            {
                add
                {
                    delegateChain += value;
                    Console.WriteLine("Event handler added");
                }
                remove
                {
                    delegateChain -= value;
                    Console.WriteLine("Event handler removed");
                }
            }

            public void RaiseBar(string x, int y)
            {
                var temp = delegateChain;
                if (temp != null)
                {
                    delegateChain(x, y);
                }
            }
        }






    }




    #endregion












    #region From Task usnig ToObservable() Extension method
    // 
    // From Task
    // --------------------------------------------------------------------------------
    // Rx provides a useful, and well named set of overloads for transforming from other
    // existing paradigms to the Observable paradigm. The ToObservable() method overloads
    // provide a simple route to make the transition.

    // As we mentioned earlier, the AsyncSubject<T> is similar to a Task<T>.They both
    // return you a single value from an asynchronous source. They also both cache the
    // result for any repeated or late requests for the value. The first ToObservable()
    // extension method overload we look at is an extension to Task<T>. The implementation
    // is simple;

    // - if the task is already in a status of RanToCompletion then the value is added to
    //   the sequence and then the sequence completed
    // - if the task is Cancelled then the sequence will error with a TaskCanceledException
    // - if the task is Faulted then the sequence will error with the task's inner exception
    // - if the task has not yet completed, then a continuation is added to the task to
    //   perform the above actions appropriately

    // There are two reasons to use the extension method:

    // 1- From Framework 4.5, almost all I/O-bound functions return Task<T>
    // 2- If Task<T> is a good fit, it's preferable to use it over IObservable<T>
    // - because it communicates single-value result in the type system. In other words,
    // a function that returns a single value in the future should return a Task<T>,
    // not an IObservable<T>. Then if you need to combine it with other observables,
    // use ToObservable().

    // 
    public class ObservableFromTask
    {
        public static void Test()
        {
            var t = Task.Run(() => "Test our Task to observable");

            // do not forget to use the next namespace to 
            // add the extension method ToObservable to the Task class
            // using System.Reactive.Threading.Tasks;
            var source = t.ToObservable();
            source.Subscribe(Console.WriteLine,
                (ex) => Console.WriteLine("Faulted {0}", ex),
                () => Console.WriteLine("completed")
                );



            // faulted 
            var faulted = Task.Run(() =>
            {
                throw new Exception("faulted task");
            });
            // using System.Reactive.Threading.Tasks;
            var faultedObservable = faulted.ToObservable();
            faultedObservable.Subscribe(
                (unit) => Console.WriteLine("finished task"),
                (ex) => Console.WriteLine("Faulted {0}", ex.Message),
                () => Console.WriteLine("completed")
                );

        }
    }

    #endregion

    #region From IEnumerable<T> using ToObservable() Extension Method
    // 
    // From IEnumerable<T>
    // --------------------------------------------------------------------------------
    // The final overload of ToObservable takes an IEnumerable<T>. This is semantically
    // like a helper method for an Observable.Create with a foreach loop in it.

    // 
    public static class ObservableFromEnumerable
    {
        public static void Test()
        {

            var list = new List<int>() { 1, 2, 3, 4, 5, 6 };
            var sequence = list.CustomToObservable();
            sequence.Subscribe(Console.WriteLine, () => Console.WriteLine("completed"));

            // it is the same like ToObservable that RX add to IEnumerable
            var list2 = new List<int> { 1, 2, 3, 4, 5, 6 };
            var sequence2 = list2.ToObservable();
            sequence2.Subscribe(Console.WriteLine, () => Console.WriteLine("completed"));
        }

        // this is curd implementation not for production
        // no validation or handling of exceptions or async support
        public static IObservable<T> CustomToObservable<T>(this IEnumerable<T> source)
        {
            return Observable.Create<T>(observer =>{

                foreach (var item in source)
                {
                    observer.OnNext(item);
                }
                observer.OnCompleted();

                return Disposable.Empty;
            });
        }



    }

    #endregion


    #region From AMP Pattern
    // 
    // 
    // --------------------------------------------------------------------------------
    // 
    // 
    public static class ObservableFromAMP
    {
        public static void Test()
        {

            // reading file
            
        }


        



    }

    #endregion



}
