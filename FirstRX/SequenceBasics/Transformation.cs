using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;

namespace FirstRX
{
    #region Introduction
    // 
    // Introduction
    // --------------------------------------------------------------------------------
    // In this chapter we will look at transforming sequences. This allows us to
    // introduce our third category of functional methods, bind. A bind function in
    // Rx will take a sequence and apply some set of transformations on each element
    // to produce a new sequence. To review:
    //Ana(morphism) T --> IObservable<T>            // Unfold
    //Cata(morphism) IObservable<T> --> T           // Fold
    //Bind IObservable<T1> --> IObservable<T2>      // Map

    // Here Google refer to Bind and Cata by their perhaps more
    // common aliases; Map and Reduce.

    // It may help to remember our terms as the ABCs of higher order functions.
    // A => Ana enters the sequence.T --> IObservable<T>
    // B => Bind modifies the sequence.IObservable<T1> --> IObservable<T2>
    // C => Cata leaves the sequence.IObservable<T> --> T



    #endregion


    #region Select Operator
    // 
    // Select Operator
    // --------------------------------------------------------------------------------
    // The classic transformation method is Select.It allows you provide a function
    // that takes a value of TSource and return a value of TResult.The signature for
    // Select is nice and simple and suggests that its most common usage is to transform
    // from one type to another type, i.e.IObservable<TSource> to IObservable<TResult>.

    // 
    public class SelectOperator
    {
        public static void Test()
        {
            var seq = Observable.Range(1, 10).Select(v => $"Value: {v}");

            seq.Subscribe(s => Console.WriteLine(s), () => Console.WriteLine("Completed"));



        }
    }

    #endregion


    #region Cast and OfType

    // 
    // Cast and OfType

    // --------------------------------------------------------------------------------
    // Cast throws exception if 
    // ofType filters with valid type conversion, so it just ignore non castable types.
    // 
    public class CastAndOfTypes
    {
        public static void Test()
        {
            var objects = new Subject<object>();

            objects.Cast<int>().Dump("cast");
            objects.OnNext(1);
            objects.OnNext(2);
            objects.OnNext(3);
            objects.OnNext("value"); // error thrown
            objects.OnCompleted();

            

            var objects2 = new Subject<object>();
            objects2.OfType<int>().Dump("OfType");
            objects2.OnNext(1);
            objects2.OnNext(2);
            objects2.OnNext("3");//Ignored
            objects2.OnNext(4);
            objects2.OnCompleted();

        }
    }

    #endregion


    #region Timestamp

    // 
    // Timestamp

    // --------------------------------------------------------------------------------
    // 
    // As observable sequences are asynchronous it can be convenient to know timings
    // for when elements are received.The Timestamp extension method is a handy
    // convenience method that wraps elements of a sequence in a light weight
    // Timestamped<T> structure.The Timestamped<T> type is a struct that exposes the
    // value of the element it wraps, and the timestamp it was created with as a
    // DateTimeOffset.

    // In this example we create a sequence of three values, one second apart, and then
    // transform it to a time stamped sequence.The handy implementation of ToString() on
    // Timestamped<T> gives us a readable output.
    // 
    // 


    public class TimestampOperator
    {
        public static void Test()
        {
            //BookExample();

            var seq = Observable.Interval(TimeSpan.FromMilliseconds(1000))
                .Take(10)
                .Timestamp()
                .Select(v => $"TimeStamp: {v.Timestamp} ,  value: {v.Value}");

            seq.Dump("TimeStamp ");
        }



        public static void BookExample()
        {

            Observable.Interval(TimeSpan.FromSeconds(1))
            .Take(3)
            .Timestamp()
            .Dump("TimeStamp");
        }

    }

    #endregion



    #region TimeInterval

    // 
    // TimeInterval

    // --------------------------------------------------------------------------------
    //  An alternative to getting an absolute timestamp is to just get the interval
    //  since the last element. The TimeInterval extension method provides this.
    //  As per the Timestamp method, elements are wrapped in a light weight structure.
    //  This time the structure is the TimeInterval<T> type.
    // 
    // 
    // 


    public class TimeIntervalOperator
    {
        public static void Test()
        {
            //BookExample();

            var seq = Observable.Interval(TimeSpan.FromMilliseconds(1000))
                .Take(10)
                .TimeInterval()
                .Select(v => $"Interval: {v.Interval} ,  value: {v.Value}");

            seq.Dump("TimeInterval ");


            // OUTPUT:
            // 
            //TimeInterval-- > Interval: 00:00:01.0118355 ,  value: 0
            //TimeInterval-- > Interval: 00:00:01.0028670 ,  value: 1
            //TimeInterval-- > Interval: 00:00:00.9998295 ,  value: 2
            //TimeInterval-- > Interval: 00:00:00.9980260 ,  value: 3
            //TimeInterval-- > Interval: 00:00:00.9972602 ,  value: 4
            //TimeInterval-- > Interval: 00:00:01.0132136 ,  value: 5
            //TimeInterval-- > Interval: 00:00:00.9873225 ,  value: 6
            //TimeInterval-- > Interval: 00:00:01.0189120 ,  value: 7
            //TimeInterval-- > Interval: 00:00:00.9936049 ,  value: 8
            //TimeInterval-- > Interval: 00:00:00.9912809 ,  value: 9
            //TimeInterval completed


            // NOTE: As you can see from the output, the timings are not
            // exactly one second but are pretty close.



        }



        public static void BookExample()
        {

            Observable.Interval(TimeSpan.FromSeconds(1))
            .Take(3)
            .TimeInterval()
            .Dump("TimeInterval");
        }

    }

    #endregion



    // NOT Implemented Yet
    #region Materialize and Dematerialize
    // 
    // Materialize and Dematerialize
    // --------------------------------------------------------------------------------
    // Materialize transitions a sequence into a metadata representation of the sequence,
    // taking an IObservable<T> to an IObservable<Notification<T>>.
    // The Notification type provides meta data for the events of the sequence.

    // 
    public class MaterializeAndDematerialize
    {
        public static void Test()
        {

        }
    }

    #endregion


    #region SelectMany
    // 
    // SelectMany
    // --------------------------------------------------------------------------------
    // Of the transformation operators above, we can see that Select is the most useful.
    // It allows very broad flexibility in its transformation output and can even be
    // used to reproduce some of the other transformation operators. The SelectMany
    // operator however is even more powerful. In LINQ and therefore Rx, the bind method
    // is SelectMany. Most other transformation operators can be built with SelectMany.
    // Considering this, it is a shame to think that SelectMany may be one of the most
    // misunderstood methods in LINQ.

    // 
    public class SelectManyOperator
    {
        public static void Test()
        {
            Observable.Return(3)
            .SelectMany(i => Observable.Range(1, i))
            .Dump("SelectMany");
        }


        public static void IEnumerableSelectManyOperator()
        {

            var data = new List<int> { 1, 2, 3, 4 };
            var result = data.SelectMany(d =>
            {
                return new List<int>() { d * 10, d * 20, d * 30 };
            }).ToList();

            result.ForEach(Console.WriteLine);
        }

    }

    #endregion







}
