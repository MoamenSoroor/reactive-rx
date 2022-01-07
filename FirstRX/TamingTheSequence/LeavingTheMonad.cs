using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FirstRX.TamingTheSequence
{
    #region ForEach : Blocking Method
    // 
    // ForEach : Blocking Method
    // --------------------------------------------------------------------------------
    // 

    // 
    public class ForEachOperator
    {
        public static void Test()
        {
            var source = Observable.Interval(TimeSpan.FromSeconds(1))
                            .Take(5);

            // Bocking Function [Obsolete("This blocking operation is no longer supported. Instead, use the async version in
            // combination with C# and Visual Basic async/await support. In case you need a blocking operation, use Wait or
            // convert the resulting observable sequence to a Task object and block.")]
            source.ForEach(i => Console.WriteLine("received {0} @ {1}", i, DateTime.Now));

        }
    }

    #endregion



    #region ForEachAsync Non Blocking Operator
    // 
    // ForEachAsync Non Blocking Operator
    // --------------------------------------------------------------------------------
    // leaving the monad to Async Programming Paradigm

    // 
    public class ForEachAsyncOperator
    {
        public static void Test()
        {
            //LeavingTheMonadToAsyncPradigm();
            HandlingErrors();
        }



        public static void LeavingTheMonadToAsyncPradigm()
        {
            var source = Observable.Interval(TimeSpan.FromSeconds(1))
                            .Take(5);

            // Bocking Function
            var task = source.ForEachAsync(i => Console.WriteLine("received {0} @ {1}", i, DateTime.Now));

            // 
            task.GetAwaiter().OnCompleted(() => Console.WriteLine("Completed"));

            Console.WriteLine("completed @ {0}", DateTime.Now);
        }



        public static void HandlingErrors()
        {
            var source = Observable.Throw<int>(new Exception("Fail"));

            Task task = source.ForEachAsync(Console.WriteLine);

            task.ContinueWith(f => Console.WriteLine("Faulted: {0}", f.Exception.InnerException?.Message)
                , TaskContinuationOptions.OnlyOnFaulted);

        }
    }

    #endregion



    #region ToEnumerable
    // 
    // ToEnumerable
    // --------------------------------------------------------------------------------
    // The source observable sequence will be subscribed to when you start to enumerate
    // the sequence (i.e. lazily). In contrast to the ForEach extension method, using the
    // ToEnumerable method means you are only blocked when you try to move to the next
    // element and it is not available. Also, if the sequence produces values faster
    // than you consume them, they will be cached for you.

    // 
    public class ToEnumerableOperator
    {
        public static void Test()
        {
            var period = TimeSpan.FromMilliseconds(500);
            var source = Observable.Timer(TimeSpan.Zero, period)
            .Take(5);
            var result = source.ToEnumerable();
            foreach (var value in result)
            {
                Console.WriteLine(value);
            }
            Console.WriteLine("done");
        }
    }

    #endregion



    #region ToTask Operator: Removed From newer RX Version
    // 
    // ToTask Operator
    // --------------------------------------------------------------------------------
    // his method will ignore multiple values, only returning the last value.

    // 
    public class ToTaskOperator
    {
        public static void Test()
        {

            //var source = Observable.Interval(TimeSpan.FromSeconds(1)).Take(5);
            //var result = source.asTas(); //Will arrive in 5 seconds. 
            //Console.WriteLine(result.Result);
        }
    }

    #endregion


    #region ToEvent<T>
    // 
    // ToEvent<T>
    // --------------------------------------------------------------------------------
    // 
    //    The ToEvent method returns an IEventSource<T>, which will have a single event
    //    member on it: OnNext.

    //    public interface IEventSource<T>
    //    {
    //        event Action<T> OnNext;
    //    }
    // 
    public class ToEventOperator
    {
        public static void Test()
        {
            var source = Observable.Interval(TimeSpan.FromSeconds(1)).Take(5);
            var result = source.ToEvent();
            result.OnNext += val => Console.WriteLine(val);
        }
    }





    #endregion


    #region ToEventPattern
    // 
    // ToEventPattern
    // --------------------------------------------------------------------------------
    // Now that we know how to get back into .NET events, let's take a break and
    // remember why Rx is a better model.

    // In C#, events have a curious interface. Some find the += and -= operators an
    // unnatural way to register a callback
    
    // - Events are difficult to compose
    // - Events do not offer the ability to be easily queried over time
    // - Events are a common cause of accidental memory leaks
    // - Events do not have a standard pattern for signaling completion
    // - Events provide almost no help for concurrency or multithreaded applications.
    //   For instance, raising an event on a separate thread requires you to do all of the plumbing

    // 
    public class ToEventPatternOperator
    {
        private static event EventHandler<MyEventArgs> MyEvent; 
        public static void Test()
        {
            // convert event to observable
            var source = Observable.FromEventPattern<MyEventArgs>(handler => MyEvent += handler, handler => MyEvent += handler);
            // convert observable to event
            var result = source.ToEventPattern();
            result.OnNext += (sender, eventArgs) => Console.WriteLine(eventArgs.Value);


            FireEvent();
        }

        static void FireEvent()
        {
            MyEvent?.Invoke(null, new MyEventArgs(10));
            Thread.Sleep(100);
            MyEvent?.Invoke(null, new MyEventArgs(20));
            Thread.Sleep(100);
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






}
