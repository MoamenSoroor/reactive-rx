using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FirstRX.TamingTheSequence
{
    #region Time-shifted sequences

    // 
    // Time-shifted sequences

    // --------------------------------------------------------------------------------
    // 


    #endregion


    #region Buffer Operator
    // 
    // Buffer Operator
    // --------------------------------------------------------------------------------
    // Our first subject will be the Buffer method.In some situations, you may not want
    // a deluge of individual notifications to process.Instead, you might prefer to work
    // with batches of data.It may be the case that processing one item at a time is just
    // too expensive, and the trade-off is to deal with messages in batches, at the cost
    // of accepting a delay.
    // 
    // The Buffer operator allows you to store away a range of values and then re-publish
    // them as a list once the buffer is full.You can temporarily withhold a specified number
    // of elements, stash away all the values for a given time span, or use a combination
    // of both count and time.



    //public static IObservable<IList<TSource>> Buffer<TSource>(this IObservable<TSource> source, int count, int skip);
    //public static IObservable<IList<TSource>> Buffer<TSource>(this IObservable<TSource> source, int count);
    //public static IObservable<IList<TSource>> Buffer<TSource>(this IObservable<TSource> source, TimeSpan timeSpan, IScheduler scheduler);
    //public static IObservable<IList<TSource>> Buffer<TSource>(this IObservable<TSource> source, TimeSpan timeSpan, TimeSpan timeShift);
    //public static IObservable<IList<TSource>> Buffer<TSource>(this IObservable<TSource> source, TimeSpan timeSpan, TimeSpan timeShift, IScheduler scheduler);
    //public static IObservable<IList<TSource>> Buffer<TSource, TBufferBoundary>(this IObservable<TSource> source, IObservable<TBufferBoundary> bufferBoundaries);
    //public static IObservable<IList<TSource>> Buffer<TSource, TBufferOpening, TBufferClosing>(this IObservable<TSource> source, IObservable<TBufferOpening> bufferOpenings, Func<TBufferOpening, IObservable<TBufferClosing>> bufferClosingSelector);
    //public static IObservable<IList<TSource>> Buffer<TSource, TBufferClosing>(this IObservable<TSource> source, Func<IObservable<TBufferClosing>> bufferClosingSelector);
    //public static IObservable<IList<TSource>> Buffer<TSource>(this IObservable<TSource> source, TimeSpan timeSpan);
    //public static IObservable<IList<TSource>> Buffer<TSource>(this IObservable<TSource> source, TimeSpan timeSpan, int count);

    // 
    public class BufferOperator
    {
        public static void Test()
        {
            //BufferWithCount();
            //BufferWithTimeInterval();
            BufferWithCountAndTimeInterval();
        }

        static void BufferWithCount()
        {
            var source = Observable.Interval(TimeSpan.FromMilliseconds(100)).Take(100);

            source.Buffer(5).Subscribe(value => Console.WriteLine($"{string.Join(",", value)}"));

        }


        static void BufferWithTimeInterval()
        {
            var source = Observable.Interval(TimeSpan.FromMilliseconds(100)).Take(100);

            source.Buffer(TimeSpan.FromSeconds(1)).Subscribe(value => Console.WriteLine($"{string.Join(",", value)}"));
        
        }

        static void BufferWithCountAndTimeInterval()
        {
            var source = Observable.Interval(TimeSpan.FromMilliseconds(100)).Take(100);

            source.Buffer(TimeSpan.FromSeconds(1),5).Subscribe(value => Console.WriteLine($"{string.Join(",", value)}"));

        }

    }

    #endregion


    #region Overlapping Buffers
    // 
    // Overlapping Buffers
    // --------------------------------------------------------------------------------
    // There are three interesting things you can do with overlapping buffers:
    // 
    //  Overlapping behavior
    //      Ensure that current buffer includes some or all values from previous buffer
    //  Standard behavior
    //      Ensure that each new buffer only has new data
    //  Skip behavior
    //      Ensure that each new buffer not only contains new data exclusively, but also ignores one or more values since the previous buffer
    // 
    // 
    // You can apply the above scenarios as follows:
    // 1- Overlapping behavior
    //   skip<count*
    // 2- Standard behavior
    //   skip = count
    // 3- Skip behavior
    //   skip> count
    public class OverlappingBuffersByCount
    {
        public static void Test()
        {
            //BufferStandardBehavior();
            //BufferOverlappingBehavior();
            //BufferSkipBehavior();
            BufferFromTheBeginningWithScan();
        }

        static void BufferStandardBehavior()
        {
            var source = Observable.Interval(TimeSpan.FromMilliseconds(100)).Take(100);

            // for each buffer publication: take new 5 elements: the same like Buffer(5)
            source.Buffer(5,5).Subscribe(value => Console.WriteLine($"{string.Join(",", value)}"));

        }


        static void BufferOverlappingBehavior()
        {
            var source = Observable.Interval(TimeSpan.FromMilliseconds(100)).Take(100);

            // for each buffer publication: take 3 from the previous buffer and 2 from the new elements:
            source.Buffer(5,2).Subscribe(value => Console.WriteLine($"{string.Join(",", value)}"));

        }

        static void BufferSkipBehavior()
        {
            var source = Observable.Interval(TimeSpan.FromMilliseconds(100)).Take(100);

            // for each buffer publication: skip 2 elements (7-5) and take new 5 elements:
            source.Buffer(5,7).Subscribe(value => Console.WriteLine($"{string.Join(",", value)}"));

            // as you notic some values are drop from each publication.
            // output:
            // 0,1,2,3,4
            // 7,8,9,10,11
            // 14,15,16,17,18
            // 21,22,23,24,25

        }

        static void BufferFromTheBeginningWithScan()
        {
            var source = Observable.Interval(TimeSpan.FromMilliseconds(100)).Take(10);

            // for each buffer publication: skip 2 elements (7-5) and take new 5 elements:
            //source.Scan<long, string>("", (acc, value) => $"{acc},{value}").Subscribe(value => Console.WriteLine(value));
            source.Scan<long, List<long>>(new List<long>(),(acc, value) => new List<long>(acc) { value } )
                .Subscribe(value => Console.WriteLine($"{string.Join(",", value)}"));


        }


    }

    #endregion


    #region Overlapping Buffers with Time
    // 
    // Overlapping Buffers
    // --------------------------------------------------------------------------------
    // There are three interesting things you can do with overlapping buffers:
    // 
    //  Overlapping behavior
    //      Ensure that current buffer includes some or all values from previous buffer
    //  Standard behavior
    //      Ensure that each new buffer only has new data
    //  Skip behavior
    //      Ensure that each new buffer not only contains new data exclusively, but also ignores one or more values since the previous buffer
    // 
    // 
    // You can apply the above scenarios as follows:
    // 1- Overlapping behavior
    //   skip<count*
    // 2- Standard behavior
    //   skip = count
    // 3- Skip behavior
    //   skip> count
    public class OverlappingBuffersByTime
    {
        public static void Test()
        {
            //BufferStandardBehavior();
            //BufferOverlappingBehavior();
            BufferSkipBehavior();
        }

        static void BufferStandardBehavior()
        {
            var source = Observable.Interval(TimeSpan.FromMilliseconds(100)).Take(100);

            // for each buffer publication: take new elements at the last 1 second. ignoring the previous buffer publications
            source.Buffer(TimeSpan.FromMilliseconds(1000), TimeSpan.FromMilliseconds(1000)).Subscribe(value => Console.WriteLine($"{string.Join(",", value)}"));

        }


        static void BufferOverlappingBehavior()
        {
            var source = Observable.Interval(TimeSpan.FromMilliseconds(100)).Take(100);

            // for each buffer publication: take from the previous buffer publications since 700ms, and take from the new publications until 300ms
            source.Buffer(TimeSpan.FromMilliseconds(1000), TimeSpan.FromMilliseconds(300)).Subscribe(value => Console.WriteLine($"{string.Join(",", value)}"));

        }

        static void BufferSkipBehavior()
        {
            var source = Observable.Interval(TimeSpan.FromMilliseconds(100)).Take(100);

            // for each buffer publication: drop 500 ms between each buffer publication
            source.Buffer(TimeSpan.FromMilliseconds(1000), TimeSpan.FromMilliseconds(1500)).Subscribe(value => Console.WriteLine($"{string.Join(",", value)}"));

        }

        


    }

    #endregion




    #region Delay Operator
    // 
    // Delay Operator
    // --------------------------------------------------------------------------------
    // the entir sequence is delayed.

    // 
    public class DelayOperator
    {
        public static void Test()
        {
            var source = Observable.Interval(TimeSpan.FromSeconds(1))
            .Take(5)
            .Timestamp()
            .Delay(TimeSpan.FromSeconds(5));

            source.Dump("Delayed Sequence ");
        }



        public static void BookExample()
        {
            var source = Observable.Interval(TimeSpan.FromSeconds(1))
            .Take(5)
            .Timestamp();
            var delay = source.Delay(TimeSpan.FromSeconds(2));
            source.Subscribe(
            value => Console.WriteLine("source : {0}", value),
            () => Console.WriteLine("source Completed"));
            delay.Subscribe(
            value => Console.WriteLine("delay : {0}", value),
            () => Console.WriteLine("delay Completed"));
        }

    }

    #endregion

    #region Sample Operator
    // 
    // Sample Operator
    // --------------------------------------------------------------------------------
    // The Sample method simply takes the last value for every specified TimeSpan.
    // This is great for getting timely data from a sequence that produces too much
    // information for your requirements. This example shows sample in action.

    public class SampleOperator
    {
        public static void Test()
        {
            var source = Observable.Interval(TimeSpan.FromMilliseconds(150));
            // take value at each second.
            source.Sample(TimeSpan.FromSeconds(1)).Subscribe(Console.WriteLine);

        }
    }

    #endregion



    #region Throttle Operator
    // 
    // Throttle Operator
    // --------------------------------------------------------------------------------
    // 
    // The Throttle extension method provides a sort of protection against sequences
    // that produce values at variable rates and sometimes too quickly. Like the Sample
    // method, Throttle will return the last sampled value for a period of time. Unlike
    // Sample though, Throttle's period is a sliding window. Each time Throttle receives
    // a value, the window is reset. Only once the period of time has elapsed will the
    // last value be propagated. This means that the Throttle method is only useful for
    // sequences that produce values at a variable rate. Sequences that produce values at
    // a constant rate (like Interval or Timer) either would have all of their values
    // suppressed if they produced values faster than the throttle period, or all of
    // their values would be propagated if they produced values slower than the
    // throttle period.

    // 
    public class ThrottleOperator
    {
        public static void Test()
        {

            var source = Observable.Create<int>( ovserver=>
            {
                ovserver.OnNext(1);
                Thread.Sleep(100);

                ovserver.OnNext(2);
                Thread.Sleep(50);

                ovserver.OnNext(3); // will be pushed
                Thread.Sleep(210);

                ovserver.OnNext(4);
                Thread.Sleep(100);

                ovserver.OnNext(5); // will be pushed
                Thread.Sleep(210);

                ovserver.OnNext(6); // will be pushed
                Thread.Sleep(100);

                ovserver.OnCompleted();
                return Disposable.Empty;
            });
            // take value at each second.
            source.Throttle(TimeSpan.FromMilliseconds(200)).Subscribe(Console.WriteLine);
        }
    }

    #endregion


    #region Timeout Operator
    // 
    // Timeout Operator
    // --------------------------------------------------------------------------------
    // We have considered handling timeout exceptions previously in the chapter on Flow
    // control. The Timeout extension method allows us terminate the sequence with an error
    // if we do not receive any notifications for a given period. We can either specify the
    // period as a sliding window with a TimeSpan, or as an absolute time that the sequence
    // must complete by providing a DateTimeOffset.

    // 
    public class TimeoutOperator
    {
        public static void Test()
        {
            //TimeOutOnError();
            TimeOutRedirectToAnotherSequence();
            //TimeOutWithTimeSelector();
        }

        public static void TimeOutOnError()
        {
            var source = Observable.Interval(TimeSpan.FromMilliseconds(100)).Take(10).Concat(Observable.Interval(TimeSpan.FromSeconds(2)));
            var timeout = source.Timeout(TimeSpan.FromSeconds(1));
            timeout.Subscribe(Console.WriteLine, (err) => Console.WriteLine($"Error: {err}"), () => Console.WriteLine("Completed"));
        }

        public static void TimeOutRedirectToAnotherSequence()
        {
            var source = Observable.Interval(TimeSpan.FromMilliseconds(100)).Take(10).Concat(Observable.Interval(TimeSpan.FromSeconds(2)));
            var timeout = source.Timeout(TimeSpan.FromSeconds(1),Observable.Return(-1L)); // now timeout will not be onerror , it will be value -1
            timeout.Subscribe(Console.WriteLine, (err) => Console.WriteLine($"Error: {err}"), () => Console.WriteLine("Completed"));
        }

        public static void TimeOutWithTimeSelector()
        {
            //var source = Observable.Interval(TimeSpan.FromMilliseconds(100)).Take(10).Concat(Observable.Interval(TimeSpan.FromSeconds(2)));
            //var timeout = source.Timeout(value=> Observable.Return(1000) ); // now timeout will not be onerror , it will be value -1
            //timeout.Subscribe(Console.WriteLine, (err) => Console.WriteLine($"Error: {err}"), () => Console.WriteLine("Completed"));
        }

    }

    #endregion






}
