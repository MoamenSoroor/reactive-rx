using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using FirstRX.SequenceBasics;
using FirstRX.TamingTheSequence;

namespace FirstRX
{
    class Program
    {
        static void Main(string[] args)
        {

            // Key Types
            // ----------------
            //ObservableAndObserverImplemetationTest.Test();
            //SubjectTest.Test();
            //ReplaySubjectsTest.Test();
            //ReplaySubjectWithTimeoutTest.Test();
            //BehaviouralSubjectTest.Test();
            //AsyncSubjectTest.Test();


            // lifetime management
            // ----------------
            //NoOnErrorActionRegisteredExceptionThrown.Test();
            //TheRightWayTohandleException.Test();
            //IDisposableTypeAndDisposeMethod.Test();

            // ceratingSequences
            // ----------------
            //ObservableCreateMethodTest.Test();
            //EmptyReturnNeverThrowWithObservableCreateMethod.Test();
            //ThePowerfullOfObservableCreateFactoryMethod.Test();
            //FunctionalUnfoldsTest.Test();
            //ObservableRangeTest.Test();
            //ObservableGenerateMethod.Test();
            //ObservableIntervalMethod.Test();
            //ObservableTimerTest.Test();
            //TimingOperatorsUsingObservableGenerateMethod.Test();
            //ObservableFromEventPattern.Test();
            //ObservableFromEvent.Test();

            //ObservableFromDelegates.Test();
            //ObservableFromTask.Test();
            //ObservableFromEnumerable.Test();

            // Reducing a sequence
            // ---------------------
            //FilteringWithWhereOperator.Test();
            //FilteringWithDistinctOperator.Test();
            //FilteringWithDistinctUntilChangedOperator.Test();
            //IgnoreElementsOperator.Test();
            //SkipTakeSkipWhileTakeWhileOperators.Test();
            //SkipUntilTakeUntilOperators.Test();

            // Inspection Operators
            // --------------------
            //AnyOperator.Test();
            //AllOperator.Test();
            //ContainsOperator.Test();
            //DefaultIfEmptyOperator.Test();
            //ElementAtOperator.Test();
            //SequenceEqualsOperator.Test();

            // Aggregates Operatos
            // ----------------
            //CountMinMaxSumAverage.Test();
            //BlockingOperators.Test();
            //AggregateOperator.Test();
            //ScanOperator.Test();
            //MaxByMinByOperators.Test();
            //GroupByOperator.Test(); // not working fix it

            // transformations
            // ----------------
            //SelectOperator.Test();
            //CastAndOfTypes.Test();
            //TimestampOperator.Test();
            //TimeIntervalOperator.Test();
            //MaterializeAndDematerialize.Test();


            // SideEffects
            // -----------------
            //ProblemOfMutation.Test();
            //IssuesWithSideEffects.Test();
            //DoOperator.Test();


            //ForEachAsyncOperator.Test();
            //ToEnumerableOperator.Test();
            //ToEventPatternOperator.Test();

            // Handling Exceptions:
            // -----------------------------
            //CatchOperator.Test();


            // combining the sequences
            // -----------------------------
            //ConcatOperator.Test();
            //RepeatOperator.Test();
            //StartWithOperator.Test();
            //AmpOperator.Test();
            //MergeOperator.Test();
            //SwitchOperator.Test();
            //CombineLatestOperator.Test();
            //ZipOperator.Test();


            // time shifted Operator
            // -----------------------------
            //BufferOperator.Test();
            //OverlappingBuffersByCount.Test();
            //OverlappingBuffersByTime.Test();
            //DelayOperator.Test();
            //SampleOperator.Test();
            //ThrottleOperator.Test();
            //TimeoutOperator.Test();

            // Hot and Cold Observables
            // ------------------------
            //PublishAndConnect.Test();
            //RefCountOperator.Test();
            //PublishLatestOperator.Test();
            ReplayExtensionMethod.Test();

            Console.WriteLine("<<< Press Any Key to Continue. >>>");
            Console.ReadLine();
        }


    }

}
