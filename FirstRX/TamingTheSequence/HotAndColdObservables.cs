using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FirstRX.TamingTheSequence
{
    #region Hot ane Cold Observables
    // 
    // Hot ane Cold Observables
    // --------------------------------------------------------------------------------
    // we will look at how to describe and handle two styles of observable sequences:

    // - Cold: Sequences that are passive and start producing notifications on request
    //         (when subscribed to)
    // - Hot: Sequences that are active and produce notifications regardless of subscriptions.


    // Examples of hot observables that could publish regardless of whether
    // there are any subscribers would be:
    // - Subjects 
    // - mouse movements (UI Events)
    // - timer events
    // - broadcasts like ESB channels or UDP network packets.
    // - price ticks from a trading exchange


    // Some examples of cold observables would be:
    // - From Observable.Create, Observable.Timer, or Observable.Interval, or any one like them
    // - asynchronous request (e.g.when using Observable.FromAsyncPattern)
    // - whenever Observable.Create is used
    // - subscriptions to queues
    // - on-demand sequences

    #endregion


    #region Publish and Connect

    // 
    // Publish and Connect
    // --------------------------------------------------------------------------------
    // If we want to be able to share the actual data values and not just the observable
    // instance, we can use the Publish() extension method. This will return an
    // IConnectableObservable<T>, which extends IObservable<T> by adding a single
    // Connect() method. By using the Publish() then Connect() method, we can get this
    // sharing functionality.

    // actually we convert cold observables to hot observables,
    // so it publish sequence even if there are no subscribers,
    // also, sequence values are shared between subscribers.
    // when subscriber subscribes to source, it will be notified with
    // they latest values of the sequence

    // 
    public class PublishAndConnect
    {
        public static void Test()
        {
            var source = Observable.Interval(TimeSpan.FromMilliseconds(1500))
                .Do(value=> Console.WriteLine($"Publishing: {value}"))
                .Publish();
            var connection = source.Connect(); // now, source will publish it's sequence values. even if no subscribers.


            Console.WriteLine("TODO >>>>>> Press Any Key to Subscribe the first subsciber.");
            Console.ReadKey();
            var subscriber1 = source.Subscribe(v => Console.WriteLine($"First Subscriber: {v}"), () => Console.WriteLine("1st Completed"));



            Console.WriteLine("TODO >>>>>> Press Any Key to Subscribe the Second subsciber.");
            Console.ReadKey();
            var subscriber2 = source.Subscribe(v => Console.WriteLine($"Second Subscriber: {v}"), () => Console.WriteLine("2nd Completed"));


            Console.WriteLine("TODO >>>>>> Press Any Key to Dispose the first subsciber.");
            Console.ReadKey();
            subscriber1.Dispose();


            Console.WriteLine("TODO >>>>>> Press Any Key to Dispose the Second subsciber.");
            Console.ReadKey();
            subscriber2.Dispose();



            Console.WriteLine("TODO >>>>>> Press Any Key to Disconnect the source");
            Console.ReadKey();
            connection.Dispose();



            Console.WriteLine("TODO >>>>>> Press Any Key to Connect Again the source");
            Console.ReadKey();
            connection = source.Connect();


            Console.WriteLine("TODO >>>>>> Press Any Key to Exit");
            Console.ReadKey();
            connection.Dispose();

            Console.WriteLine("============= End =============");
        }
    }

    #endregion


    #region RefCount
    // 
    // RefCount
    // --------------------------------------------------------------------------------
    // The Publish/RefCount pair is extremely useful for taking a cold observable and
    // sharing it as a hot observable sequence for subsequent observers.

    // RefCount Returns an observable sequence that stays connected to the source as long as
    //     there is at least one subscription to the observable sequence.

    // 
    // This will "magically" implement our requirements for automatic disposal
    // and lazy connection.RefCount will take an IConnectableObservable<T> and turn
    // it back into an IObservable<T> while automatically implementing the "connect"
    // and "disconnect" behavior we are looking for.



    // 
    public class RefCountOperator
    {
        public static void Test()
        {
            var period = TimeSpan.FromSeconds(1);
            var source = Observable.Interval(period)
            .Do(l => Console.WriteLine("Publishing {0}", l)) //side effect to show it is running
            .Publish()
            .RefCount();
            //source.Connect(); Use RefCount instead now 
            Console.WriteLine("TODO >>>>>> Press Any Key to Subscribe the first subsciber.");
            Console.ReadKey();
            var subscriber1 = source.Subscribe(v => Console.WriteLine($"First Subscriber: {v}"), () => Console.WriteLine("1st Completed"));



            Console.WriteLine("TODO >>>>>> Press Any Key to Subscribe the Second subsciber.");
            Console.ReadKey();
            var subscriber2 = source.Subscribe(v => Console.WriteLine($"Second Subscriber: {v}"), () => Console.WriteLine("2nd Completed"));


            Console.WriteLine("TODO >>>>>> Press Any Key to Dispose the first subsciber.");
            Console.ReadKey();
            subscriber1.Dispose();


            Console.WriteLine("TODO >>>>>> Press Any Key to Dispose the Second subsciber.");
            Console.ReadKey();
            subscriber2.Dispose();

            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
        }
    }

    #endregion


    #region Race Condition When Subscribe Before Connect
    // 
    // Race Condition When Subscribe Before Connect
    // --------------------------------------------------------------------------------
    // The Publish/RefCount pair is extremely useful for taking a cold observable and
    // sharing it as a hot observable sequence for subsequent observers. RefCount()
    // also allows us to avoid a race condition. In the example above, we subscribed to
    // the sequence before a connection was established. This is not always possible,
    // especially if we are exposing the sequence from a method. By using the RefCount
    // method we can mitigate the subscribe/connect race condition because of the
    // auto-connect behavior.



    #endregion



    #region PublishLatest
    // 
    // PublishLatest
    // --------------------------------------------------------------------------------
    // You get equivalent semantics to AsyncSubject<T> where only the last value is
    // published, and only once the sequence completes.

    // 
    public class PublishLatestOperator
    {
        public static void Test()
        {
            var period = TimeSpan.FromSeconds(1);
            var observable = Observable.Interval(period)
            .Take(5)
            .Do(l => Console.WriteLine("Publishing {0}", l)) //side effect to show it is running
            .PublishLast();
            observable.Connect();
            Console.WriteLine("Press any key to subscribe");
            Console.ReadKey();
            var subscription = observable.Subscribe(i => Console.WriteLine("subscription : {0}", i));
            Console.WriteLine("Press any key to unsubscribe.");
            Console.ReadKey();
            subscription.Dispose();
            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
        }
    }

    #endregion





    #region Replay
    // 
    // Replay
    // --------------------------------------------------------------------------------
    // we can use Replay, if we have hot observable and we want it to act like cold
    // and we can ensure that all values are pushed to all subscribers. even if some of 
    // them are lately subscribe. (Mulicasting Hot Observable)

    // 
    public class ReplayExtensionMethod
    {
        public static void Test()
        {
            var period = TimeSpan.FromSeconds(1);
            var hot = Observable.Interval(period)
            .Take(3)
            .Publish();
            hot.Connect();
            Thread.Sleep(period); //Run hot and ensure a value is lost.
            var observable = hot.Replay();
            observable.Connect();
            observable.Subscribe(i => Console.WriteLine("first subscription : {0}", i));
            Thread.Sleep(period);
            observable.Subscribe(i => Console.WriteLine("second subscription : {0}", i));
            Console.ReadKey();
            observable.Subscribe(i => Console.WriteLine("third subscription : {0}", i));
            Console.ReadKey();
        }
    }

    #endregion




}
