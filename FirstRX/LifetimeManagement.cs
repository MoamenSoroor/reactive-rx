using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;

namespace FirstRX
{

    #region Lifetime Management
    // Rx provides fine grained control to the lifetime of subscriptions to queries. While
    // using familiar interfaces, you can deterministically release resources associated
    // to queries. This allows you to make the decisions on how to most effectively manage
    // your resources, ideally keeping the scope as tight as possible.

    //Just subscribes to the Observable for its side effects. 
    // All OnNext and OnCompleted notifications are ignored.
    // OnError notifications are re-thrown as Exceptions.
    //IDisposable Subscribe<TSource>(this IObservable<TSource> source);

    //The onNext Action provided is invoked for each value.
    //OnError notifications are re-thrown as Exceptions.
    //IDisposable Subscribe<TSource>(this IObservable<TSource> source, Action<TSource> onNext);

    //The onNext Action is invoked for each value.
    //The onError Action is invoked for errors
    //IDisposable Subscribe<TSource>(this IObservable<TSource> source,
    //Action<TSource> onNext,
    //Action<Exception> onError);

    //The onNext Action is invoked for each value.
    //The onCompleted Action is invoked when the source completes.
    //OnError notifications are re-thrown as Exceptions.
    //IDisposable Subscribe<TSource>(this IObservable<TSource> source,
    //Action<TSource> onNext,
    //Action onCompleted);

    //The complete implementation
    //IDisposable Subscribe<TSource>(this IObservable<TSource> source,
    //Action<TSource> onNext,
    //Action<Exception> onError,
    //Action onCompleted);

    // NOTE: Subscribe Method Exists in Observable and ObservableExtenstions

    // NOTE:  A key point to note is that if you use an overload that does not specify a delegate
    // for the OnError notification, any OnError notifications will be re-thrown as an exception.
    // It is normally best to use an overload that specifies a delegate to cater for
    // OnError notifications.

    class NoOnErrorActionRegisteredExceptionThrown
    {
        public static void Test()
        {

            // Note Exception will be thrown. you should subscribe with OnError Action to 
            // handle the exception


            var subject = new Subject<string>();


            subject.Subscribe(Console.WriteLine);
            subject.OnNext("hello");


            try
            {

                // exception will by thrown
                subject.OnError(new Exception("Not valid anything, I am a Stupid Exception."));


            } // catch will handle the error
            catch (Exception ex)
            {
                Console.WriteLine("-----------------------------------");
                Console.WriteLine(ex);
            }





        }
    }


    class TheRightWayTohandleException
    {
        public static void Test()
        {
            // the write way to handle the Exception
            // is to subscribe at OnError Action delegate
            // -------------------------------------------------------------------------
            Console.WriteLine("<<< Test OnError Action Subscription >>>");
            var subject = new Subject<string>();
            subject.Subscribe(
                (next) => Console.WriteLine(next),
                (err) => Console.WriteLine("Exception: {0}", err.Message));
            subject.OnNext("hello");
            subject.OnError(new Exception("Not valid anything, I am a Stupid Exception."));
        }
    }

    #endregion


    #region IDisposable Type And Dispose Method

    class IDisposableTypeAndDisposeMethod
    {
        public static void Test()
        {

            // Note there is no error thrown, as you should subscribe with OnError Action to 
            // handle the exception
            // 

            var subject = new Subject<int>();

            IDisposable dispose1 = subject.Subscribe((value) => Console.WriteLine("1st subscription received {0}", value));
            IDisposable dispose2 = subject.Subscribe((value) => Console.WriteLine("2nd subscription received {0}", value));

            subject.OnNext(1);
            subject.OnNext(2);

            dispose2.Dispose();
            subject.OnNext(3);
            subject.OnNext(4);

            // 1st will recieve 1 2 3 4 but 2nd will recieve 1 2



        }
    }
    #endregion


    #region OnError and OnCompleted
    //Both the OnError and OnCompleted signify the completion of a sequence.If your
    //sequence publishes an OnError or OnCompleted it will be the last publication and
    //no further calls to OnNext can be performed.In this example we try to publish an
    //OnNext call after an OnCompleted and the OnNext is ignored:

    public class OnErrorOnCompeleteActions
        {

        public static void Test()
        {
            var subject = new Subject<int>();
            subject.Subscribe( Console.WriteLine, () => Console.WriteLine("Completed"));
            subject.OnCompleted();
            subject.OnNext(2); // will be ignored.
        }



    }


    // Of course, you could implement your own IObservable<T> that allows publishing after
    // an OnCompleted or an OnError, however it would not follow the precedence of the
    // current Subject types and would be a non-standard implementation.I think it would
    // be safe to say that the inconsistent behavior would cause unpredictable behavior
    // in the applications that consumed your code.

    #endregion


    #region IDisposable Important Note
    // 
    // IDisposable Important Note
    // ---------------------------------------------------------------
    // An interesting thing to consider is that when a sequence completes
    // or errors, you should still dispose of your subscription.


    public class TimeIt : IDisposable
    {
        private readonly string _name;
        private readonly Stopwatch _watch;
        public TimeIt(string name)
        {
            _name = name;
            _watch = Stopwatch.StartNew();
        }
        public void Dispose()
        {
            _watch.Stop();
            Console.WriteLine("{0} took {1}", _name, _watch.Elapsed);
        }
    }

    public class TimeItTest
    {

        public static void Test()
        {
            using (new TimeIt("Outer scope"))
            {
                using (new TimeIt("Inner scope A"))
                {
                    DoSomeWork("A");
                }
                using (new TimeIt("Inner scope B"))
                {
                    DoSomeWork("B");
                }
                Cleanup();
            }
        }

        private static void Cleanup()
        {
            Console.WriteLine("Clean Up.");
        }

        private static void DoSomeWork(string v)
        {
            Console.WriteLine("Do something.");
        }
    }

    //Creates a scope for a console foreground color. When disposed, will return to 
    //  the previous Console.ForegroundColor
    public class ManagedConsoleColor : IDisposable
    {
        private readonly System.ConsoleColor _previousColor;
        public ManagedConsoleColor(System.ConsoleColor color)
        {
            _previousColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
        }
        public void Dispose()
        {
            Console.ForegroundColor = _previousColor;
        }


    }
    // 
    public class IDisposableImportantNote
    {
        public static void Test()
        {
            Console.WriteLine("Normal color");
            using (new ManagedConsoleColor(System.ConsoleColor.Red))
            {
                Console.WriteLine("Now I am Red");
                using (new ManagedConsoleColor(System.ConsoleColor.Green))
                {
                    Console.WriteLine("Now I am Green");
                }
                Console.WriteLine("and back to Red");
            }
        }
    }

    #endregion


    #region Resource management vs. memory management

    // 
    // Resource management vs. memory management
    // --------------------------------------------------------------------------------
    // Many people who hear about the Dispose pattern for the first time complain that
    // the GC isn't doing its job. They think it should collect resources, and that this
    // is just like having to manage resources as you did in the unmanaged world.
    // The truth is that the GC was never meant to manage resources. It was designed to
    // manage memory and it is excellent in doing just that. 

    // This is both a testament to Microsoft for making.NET so easy to work with and also
    // a problem as it is a key part of the runtime to misunderstand.Considering this, I
    // thought it was prudent to note that subscriptions will not be automatically
    // disposed of.You can safely assume that the instance of IDisposable that is returned
    // to you does not have a finalizer and will not be collected when it goes out of scope.
    // If you call a Subscribe method and ignore the return value, you have lost your
    // only handle to unsubscribe. The subscription will still exist, and you have effectively
    // lost access to this resource, which could result in leaking memory and running unwanted
    // processes.


    // The exception to this cautionary note is when using the Subscribe extension methods.
    // These methods will internally construct behavior that will automatically detach
    // subscriptions when the sequence completes or errors.Even with the automatic detach
    // behavior; you still need to consider sequences that never terminate(by OnCompleted
    // or OnError). You will need the instance of IDisposable to terminate the subscription
    // to these infinite sequences explicitly.

    // By leveraging the common IDisposable interface, Rx offers the ability to have deterministic
    // control over the lifetime of your subscriptions.Subscriptions are independent, so the disposable
    // of one will not affect another.While some Subscribe extension methods utilize an automatically
    // detaching observer, it is still considered best practice to explicitly manage your
    // subscriptions, as you would with any other resource implementing IDisposable.

    #endregion








}
