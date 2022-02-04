# reactive-rx
discovering the world of reactive programming using Rx .net
This is my practicing to based on the following resources:
 * <a href="http://introtorx.com/Content/v1.0.10621.0/00_Foreword.html" target="_blank"> Introduction to Rx </a>
 * <a href="https://app.pluralsight.com/library/courses/dotnet-code-rx-taming-asynchronous/table-of-contents" target="_blank">Taming Asynchronous .NET Code with Rx</a> 

# Sequence Basics: 
------------------------------------------------------------
## Aggregation
- Count, Min, Max, Sum and Average
- Functional folds 
- Blocking Operatos: First, Last,Single, FirstOrDefault, LastOrDefault, SingleOrDefault
- Aggregate Operator
- Scan Operator
- MaxBy and MinBy Operators
- GroupBy Operator
## CreatingSequence
- Simple factory methods
- Observable.Create
- Empty, Return, Never and Throw With Observable.Create
- The Powerfull Of Observable.Create Factory Method
- Functional unfolds: Unfolding Infinite Sequences using Corecursion
- Observable.Range 
- Observable.Generate and Create your own Observable.Range
- Observable.Interval
- Observable.Timer 
- Timing Operators using Observable.Generate
- Transitioning into IObservable<T>
- From FromEventPattern
- From FromEvent
- From Task usnig ToObservable() Extension method
- From IEnumerable<T> using ToObservable() Extension Method
- From AMP Pattern
## InspectionOperators
- Any Operator
- All Operator
- Contains Operator
- DefaultIfEmpty Operator
- ElementAt Operator
- SequenceEquals

## KeyTypes
- Implementing IObservable and IObserver
- Subject<T>
- ReplaySubject<T> with buffer size
- ReplaySubject<T> with window 
- BehaviorSubject<T>
- AsyncSubject<T>
- Implicit contracts
- ISubject Interface
- Subject factory

## LifetimeManagement
- Lifetime Management
- IDisposable Type And Dispose Method
- OnError and OnCompleted
- IDisposable Important Note
- Resource management vs. memory management
## Reducingsequence
- Reducing a sequence
- Where Operator
- Distinct Operator
- Distinct Until Changed Operator
- IgnoreElements
- Skip, Take, SkipWhile, TakeWhile, TakeLast, and SkipLast
- SkipUntil and TakeUntil
## Transformation
- Introduction
- Select Operator
- Cast and OfType
- Timestamp
- TimeInterval
- Materialize and Dematerialize
- SelectMany
- ABC's of functional programming

# Taming The Sequence
-----------------------------------------------
## AdvancedErrorHandling
- Catch, Finally, Using, OnErrorResumeNext, and Retry
- Catch Operator
## CombiningSequences
- Sequential Concatenation
- Concat Operator
- Repeat Operator
- StartWith
- Concurrent Sequences Operators
- Amb
- Merge Operator
- Switch Operator : Who Come, I will work with you only ^-^
- Pairing sequences : CombineLatest
- Pairing sequences : Zip
## HotAndColdObservables
- Hot ane Cold Observables
- Publish and Connect
- RefCount
- Race Condition When Subscribe Before Connect
- PublishLatest
- Replay
## LeavingTheMonad
- ForEach : Blocking Method
- ForEachAsync Non Blocking Operator
- ToEnumerable
- ToTask Operator: Removed From newer RX Version
- ToEvent<T>
- ToEventPattern
## SideEffects
- Issues With Side Effects
- Mutable elements cannot be protected
- Do Operator
- Encapsulating with AsObservable
- Finally
## TimeShifted
- Time-shifted sequences
- Buffer Operator
- Overlapping Buffers
- Overlapping Buffers with Time
- Delay Operator
- Sample Operator
- Throttle Operator
- Timeout Operator

