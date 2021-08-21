﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace FirstRX
{
    class Program
    {
        static void Main(string[] args)
        {

            Utils.RenameNamespaces();



            // Key Types
            // ----------------
            //ObservableAndObserverImplemetationTest.Test();
            //SubjectTest.Test();
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

            Console.ReadLine();
        }


    }

}
