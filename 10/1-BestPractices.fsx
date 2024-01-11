// Listing 10.1
open System
let isEvenSecond (args: Timers.ElapsedEventArgs) = args.SignalTime.Second % 2 = 0

let printTime (args: Timers.ElapsedEventArgs) =
    printfn $"Event was raised at {args.SignalTime}"

let t = new System.Timers.Timer(Interval = 1000., Enabled = true)

t.Elapsed |> Event.filter isEvenSecond |> Event.add printTime

t.Start()
t.Stop()

// Sequence comprehensions
let inputs: string seq = seq {
    while true do
        Console.Write "Please enter your command: "
        Console.ReadLine()
}

// Example implementation of traverse. Don't use this though - there are several variants in FsToolkit.ErrorHandling; see below.
let traverse results =
    // Get all "ok" values
    let oks = [
        for result in results do
            match result with
            | Ok x -> yield x
            | Error _ -> ()
    ]

    // Get all "error" values
    let errors = [
        for result in results do
            match result with
            | Ok _ -> ()
            | Error x -> yield x
    ]

    match oks, errors with
    | allOk, [] -> Ok allOk // no errors - return the Ok values as a list within a single Ok
    | _, errors -> Error errors // some errors - return them all as a list with a single Error

let allOks: Result<int list, obj list> = traverse [ Ok 1; Ok 2; Ok 3 ]
let oneError = traverse [ Ok 1; Ok 2; Error "Bad" ]
let twoErrors = traverse [ Ok 1; Error "Bad 1"; Error "Bad 2" ]

#r "nuget:FsToolkit.ErrorHandling"

// Monadic Result traverse takes just the first error.
let twoErrorsM =
    FsToolkit.ErrorHandling.List.traverseResultM id [ Ok 1; Error "Bad 1"; Error "Bad 2" ]

// Applicative Result traverse composes all errors together.
let twoErrorsA =
    FsToolkit.ErrorHandling.List.traverseResultA id [ Ok 1; Error "Bad 1"; Error "Bad 2" ]

// Applicative Validation traverse composes all lists-of-errors together.
let nestedErrors =
    FsToolkit.ErrorHandling.List.traverseValidationA id [ Ok 1; Error [ "Bad 1" ]; Error [ "Bad 2"; "Bad 3" ] ]
