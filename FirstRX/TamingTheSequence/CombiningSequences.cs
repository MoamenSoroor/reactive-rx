using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirstRX.TamingTheSequence
{

    #region Sequential Concatenation
    // 
    // Sequential Concatenation
    // --------------------------------------------------------------------------------
    //


    #endregion




    #region Concat Operator
    // 
    // Concat Operator
    // --------------------------------------------------------------------------------
    // when the first source is completed, the second will be started.

    // 
    public class ConcatOperator
    {
        public static void Test()
        {


            // [1,2,3,4,5]
            var seq1 = Observable.Interval(TimeSpan.FromMilliseconds(250)).Select(i => (i + 1) ).Take(5);

            // [6,7,8,9,10]
            var seq2 = Observable.Interval(TimeSpan.FromMilliseconds(500)).Select(i => (i + 6) * 10).Take(5);

            // element pushed as they came from first, but the no element in seq2 will be pushed 
            // until the first complete;
            seq1.Concat(seq2).Dump("concat");
        
        }



        public static void BookExample()
        {
            //Generate values 0,1,2 
            var s1 = Observable.Range(0, 3);
            //Generate values 5,6,7,8,9 
            var s2 = Observable.Range(5, 5);
            s1.Concat(s2)
            .Subscribe(Console.WriteLine);
        }

    }

    #endregion

    #region Repeat Operator
    // 
    // Repeat Operator
    // --------------------------------------------------------------------------------
    // Another simple extension method is Repeat. It allows you to simply repeat a sequence, either a specified or an infinite number of times.

    // 
    public class RepeatOperator
    {
        public static void Test()
        {
            Repeat2Times();
            //RepeatInfinitly();
        }

        public static void Repeat2Times()
        {
            var source = Observable.Interval(TimeSpan.FromMilliseconds(200)).Take(5);

            var seq = source.Repeat(2);

            seq.Dump("Repeat ");
        }

        public static void RepeatInfinitly()
        {
            var source = Observable.Interval(TimeSpan.FromMilliseconds(200)).Take(5);

            var seq = source.Repeat();

            seq.Dump("Repeat ");
        }
    }

    #endregion



    #region StartWith
    // 
    // StartWith
    // --------------------------------------------------------------------------------
    // StartWith
    // Another simple concatenation method is the StartWith extension method.It allows
    // you to prefix values to a sequence. The method signature takes a params array
    // of values so it is easy to pass in as many or as few values as you need.

    // 
    public class StartWithOperator
    {
        public static void Test()
        {
            Console.WriteLine(string.Join(", ",Enumerable.Range(-5, 5).Select(v=> (long)v)));

            var source = Observable.Interval(TimeSpan.FromMilliseconds(200)).Take(5);

            //var seq = source.StartWith(-5,-4,-3,-2,-1); // ok
            var seq = source.StartWith(Enumerable.Range(-5, 5).Select(v => (long)v)); // ok

            seq.Dump("StartWith ");
        }
    }

    #endregion



    #region Concurrent Sequences Operators
    // 
    // Concurrent Sequences Operators
    // --------------------------------------------------------------------------------
    // Concurrent sequences
    // The next set of methods aims to combine observable sequences that are producing
    // values concurrently. This is an important step in our journey to understanding
    // Rx. For the sake of simplicity, we have avoided introducing concepts related to
    // concurrency until we had a broad understanding of the simple concepts.

    #endregion


    #region Amb
    // 
    // Amb
    // --------------------------------------------------------------------------------
    // who first come, will continue, and the other will be ignored until finishing
    // the sequence.

    // 
    public class AmpOperator
    {
        public static void Test()
        {

            //[0,1,2,3,4]
            var source1 = Observable.Interval(TimeSpan.FromMilliseconds(300)).Take(5);
            
            //[5,6,7,8,9]
            var source2 = Observable.Interval(TimeSpan.FromMilliseconds(200)).Take(5).Select(v=> v + 5);

            // all 2 values will be printed. because it come first.
            var result = source1.Amb(source2);
            result.Dump("Amb Sequence");

            // Observable.Amb(source1, source2).Dump("Amb Sequence");
        }
    }

    #endregion



    #region Merge Operator
    // 
    // Merge Operator
    // --------------------------------------------------------------------------------
    // The Merge extension method does a primitive combination of multiple concurrent sequences.
    // As values from any sequence are produced, those values become part of the result sequence.

    // 
    public class MergeOperator
    {
        public static void Test()
        {
            //[0,1,2,3,4]
            var source1 = Observable.Interval(TimeSpan.FromMilliseconds(150)).Take(5);

            //[5,6,7,8,9]
            var source2 = Observable.Interval(TimeSpan.FromMilliseconds(250)).Take(5).Select(v => v + 5);

            var result = source1.Merge(source2);
            result.Dump("Merge Sequence");

            //var result = Observable.Merge(source1, source2);
            //result.Dump("Merge Sequence");

        }
    }

    #endregion


    #region Switch Operator : Who Come, I will work with you only ^-^
    // 
    // Switch Operator
    // --------------------------------------------------------------------------------
    // Switch
    // Receiving all values from a nested observable sequence is not always what you
    // need.In some scenarios, instead of receiving everything, you may only want the
    // values from the most recent inner sequence.A great example of this is live searches.
    // As you type, the text is sent to a search service and the results are returned to you
    // as an observable sequence.Most implementations have a slight delay before sending the
    // request so that unnecessary work does not happen.Imagine I want to search for
    // "Intro to Rx". I quickly type in "Into to" and realize I have missed the letter 'r'.
    // I stop briefly and change the text to "Intro ". By now, two searches have been sent
    // to the server. The first search will return results that I do not want. Furthermore,
    // if I were to receive data for the first search merged together with results for the
    // second search, it would be a very odd experience for the user. This scenario fits
    // perfectly with the Switch method.

    // 
    public class SwitchOperator
    {
        public static void Test()
        {
            //[0,1,2,3,4]
            var source1 = Observable.Timer(TimeSpan.Zero, TimeSpan.FromMilliseconds(100)).Take(5);

            //[5,6,7,8,9]
            var source2 = Observable.Timer(TimeSpan.FromMilliseconds(150), TimeSpan.FromMilliseconds(100)).Take(5).Select(v => v + 5);


            //[10,11,12,13,14]
            var source3 = Observable.Timer(TimeSpan.FromMilliseconds(350), TimeSpan.FromMilliseconds(100)).Take(5).Select(v => v + 10);


            var allSources = Observable.Create<IObservable<long>>(o =>
            {
                o.OnNext(source1);
                o.OnNext(source2);
                o.OnNext(source3);
                return Disposable.Empty;
            });

            var result = Observable.Switch(allSources);
            result.Dump("Switch ");

        }


        

    }

    #endregion




    #region Pairing sequences : CombineLatest
    // 
    // Pairing sequences : CombineLatest
    // --------------------------------------------------------------------------------
    // combine who came, with the previous one.
    // resultSelector will combine them, by default they returned as tuple.
    // if you passed the resultSelector, you can change the return of compination

    // 
    public class CombineLatestOperator
    {
        public static void Test()
        {
            //[0,1,2,3,4]
            var source1 = Observable.Interval(TimeSpan.FromMilliseconds(150)).Take(5);

            //[5,6,7,8,9]
            var source2 = Observable.Interval(TimeSpan.FromMilliseconds(250)).Take(5).Select(v => v + 5);

            var result = source1.CombineLatest(source2,(a,b)=> $"{a}-{b}"); 
            result.Dump("CombineLatest ");

            // Output:
            // CombineLatest-- > 0 - 5
            // CombineLatest-- > 1 - 5
            // CombineLatest-- > 2 - 5
            // CombineLatest-- > 2 - 6
            // CombineLatest-- > 3 - 6
            // CombineLatest-- > 3 - 7
            // CombineLatest-- > 4 - 7
            // CombineLatest-- > 4 - 8
            // CombineLatest-- > 4 - 9
            // CombineLatest completed
        }
    }

    #endregion

    #region Pairing sequences : Zip
    // 
    // Pairing sequences : Zip
    // --------------------------------------------------------------------------------
    // The Zip extension method is another interesting merge feature.Just like a zipper
    // on clothing or a bag, the Zip method brings together two sequences of values as
    // pairs; two by two.Things to note about the Zip function is that the result
    // sequence will complete when the first of the sequences complete, it will error
    // if either of the sequences error and it will only publish once it has a pair of
    // fresh values from each source sequence.So if one of the source sequences publishes
    // values faster than the other sequence, the rate of publishing will be dictated
    // by the slower of the two sequences.

    // 
    public class ZipOperator
    {
        public static void Test()
        {
            //[0,1,2,3,4]
            var source1 = Observable.Interval(TimeSpan.FromMilliseconds(150)).Take(5);

            //[5,6,7,8,9]
            var source2 = Observable.Interval(TimeSpan.FromMilliseconds(250)).Take(5).Select(v => v + 5);

            var result = source1.Zip(source2, (a, b) => $"{a}-{b}");
            result.Dump("Zip ");

            // Output:
            // Zip-- > 0 - 5
            // Zip-- > 1 - 6
            // Zip-- > 2 - 7
            // Zip-- > 3 - 8
            // Zip-- > 4 - 9
            // Zip completed

        }
    }

    #endregion


}
