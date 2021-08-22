using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirstRX.TamingTheSequence
{
    #region Time-shifted sequences

    // 
    // Time-shifted sequences

    // --------------------------------------------------------------------------------
    // 


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





}
