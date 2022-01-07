using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirstRX.TamingTheSequence
{

    // Not Implemented yet
    #region Catch, Finally, Using, OnErrorResumeNext, and Retry
    // 
    // Catch, Finally, Using, OnErrorResumeNext, and Retry
    // --------------------------------------------------------------------------------
    // 
    #endregion

    #region Catch Operator
    // 
    // Catch Operator
    // --------------------------------------------------------------------------------
    // Just like a catch in SEH(Structured Exception Handling), with Rx you have the
    // option of swallowing an exception, wrapping it in another exception or performing
    // some other logic.
    // We already know that observable sequences can handle erroneous situations with the
    // OnError construct.A useful method in Rx for handling an OnError notification is the
    // Catch extension method. Catch allows you to intercept a specific Exception type and
    // then continue with another sequence.

    // in generate we can convert onError cases to onNext of another sequence.
    // 
    public class CatchOperator
    {
        public static void Test()
        {
            //FilteringTypesOfException();
            //ExceptionNotMappedToNewSource();
            //GeneralCatch();
            //CatchWithIgnoringInfoAboutException();
            CatchManyObservables();
        }



        public static void FilteringTypesOfException()
        {
            var source = Observable.Create<string>(
                (observer) =>
                {
                    observer.OnNext("hello");
                    observer.OnNext("World");
                    observer.OnError(new InvalidOperationException("Error in sequence"));
                    return Disposable.Empty;
                });

            var dispose1 = source.Subscribe(Console.WriteLine, error => Console.WriteLine($"OnError: {error}"), () => Console.WriteLine("completed."));


            // handler error in another sequence 


            IObservable<string> mappingErrors = source.Catch<string, InvalidOperationException>(ex => Observable.Return("Invalid Operation Exception"));

            mappingErrors.Subscribe(c =>
            {
                Console.WriteLine($"Mapped Value: {c}");
            });
        }



        // Catch can map only onerror for InvalidOperationException, so 
        // when ArgumentException thrown , it will not be catched in Catch Operator onNext, 
        // it will be onError
        public static void ExceptionNotMappedToNewSource()
        {
            var source = Observable.Create<string>(
                (observer) =>
                {
                    observer.OnNext("hello");
                    observer.OnNext("World");
                    observer.OnError(new ArgumentException("Argument Exception")); // not handled in mappingErrors sequence
                    return Disposable.Empty;
                });

            var dispose1 = source.Subscribe(Console.WriteLine, error => Console.WriteLine($"OriginalError: {error}"), () => Console.WriteLine("completed."));


            // in this way catch will not handle ArgumentException
            IObservable<string> mappingErrors = source.Catch<string, InvalidOperationException>(ex => Observable.Return("Invalid Operation Exception"));

            mappingErrors.Subscribe(c =>Console.WriteLine($"Mapped Value: {c}") , error => Console.WriteLine($"Catch Operator Error: {error}"));
        }


        // canverting any onError to onNext
        public static void GeneralCatch()
        {
            var source = Observable.Create<string>(
                (observer) =>
                {
                    observer.OnNext("hello");
                    observer.OnNext("World");
                    observer.OnError(new ArgumentException("Argument Exception"));
                    return Disposable.Empty;
                });

            var dispose1 = source.Subscribe(Console.WriteLine, error => Console.WriteLine($"OnError: {error}"), () => Console.WriteLine("completed."));


            // handling any error
            IObservable<string> mappingErrors = source.Catch<string,Exception>(ex=> Observable.Return("Catching Any Exception"));

            mappingErrors.Subscribe(c =>
            {
                Console.WriteLine($"Mapped Value: {c}");
            });
        }

        // handling any error and ignore the information about the exception
        public static void CatchWithIgnoringInfoAboutException()
        {
            var source = Observable.Create<string>(
                (observer) =>
                {
                    observer.OnNext("hello");
                    observer.OnNext("World");
                    observer.OnError(new ArgumentException("Argument Exception"));
                    return Disposable.Empty;
                });



            // handling any error and ignore the information about the exception, only use the passed Observable to Catch 
            // on the case of OnError
            IObservable<string> mappingErrors = source.Catch(Observable.Return("Catching Any Exception and ignore any exception info"));

            mappingErrors.Subscribe(c =>
            {
                Console.WriteLine($"Mapped Value: {c}");
            });
        }


        // handling any error and ignore the information about the exception
        public static void CatchManyObservables()
        {
            var source = Observable.Create<string>(
                (observer) =>
                {
                    observer.OnNext("source1 value 1");
                    observer.OnNext("source1 value 2");
                    observer.OnError(new ArgumentException("Argument Exception"));
                    observer.OnNext("source1 value 3");

                    return Disposable.Empty;
                });


            var source2 = Observable.Create<string>(
                (observer) =>
                {
                    observer.OnError(new InvalidOperationException("invalid operation exception."));
                    observer.OnNext("source2 value 1");
                    observer.OnNext("source2 value 2");
                    return Disposable.Empty;
                });

            var source3 = Observable.Create<string>(
                (observer) =>
                {
                    observer.OnNext("source3 value 1");
                    // observer.OnError(new InvalidOperationException("invalid operation exception.")); 
                    // if the previous exception line is uncommented, error will be thrown in Catch subscription
                    return Disposable.Empty;
                });

            var source4 = Observable.Create<string>( // source4 will be ignored as the source3 has no exceptions
                (observer) =>
                {
                    observer.OnNext("source4 value 1");
                    return Disposable.Empty;
                });


            // catch will ignore any OnError: except the last source errors will be at OnError of Catch Operator.
            IObservable<string> mappingErrors = Observable.Catch(source,source2,source3,source4); // each source will catch the previous source to it.

            mappingErrors.Subscribe(c => Console.WriteLine($"Mapped Value: {c}") , 
                error=> Console.WriteLine($"Error:{error}"),
                () => Console.WriteLine("Catch Completed.")            
            );
        }


    }

    #endregion




}
