// Example aggregation signatures
type Sum = int seq -> int
type Average = float seq -> float
type Count<'T> = 'T seq -> int

// Listing 7.12
let numbers = [ 1..10 ] |> List.map string

for n in numbers do
    printfn $"Number {n}"

for n in 1..10 do
    printfn "Number %i" n

let mutable counter = 1

while counter <= 10 do
    printfn $"Number {counter}"
    counter <- counter + 1

// Sum with a for loop
let inputs = [ 1..10 ]

let sum inputs =
    let mutable accumulator = 0

    for input in inputs do
        accumulator <- accumulator + input

    accumulator

// Sum using fold
let sumFunctional inputs =
    inputs |> Seq.fold (fun accumulator input -> accumulator + input) 0

// Sum using fold with logging
let sumLogging inputs =
    inputs
    |> Seq.fold
        (fun accumulator input ->
            printfn $"Acc {accumulator} + input {input} =>"
            accumulator + input)
        0

sumLogging [ 1..5 ]

// Working with dates
open System

let allDates = seq {
    let mutable theDate = DateTime.MinValue

    while theDate <= DateTime.MaxValue do
        theDate
        theDate <- theDate.AddDays 1.
}

let mondays =
    allDates
    |> Seq.skipWhile (fun d -> d.Year < 2020 || d.Month < 2)
    |> Seq.filter (fun d -> d.DayOfWeek = DayOfWeek.Monday)
    |> Seq.takeWhile (fun d -> d.Year = 2020 && d.Month < 6)
    |> Seq.toArray

// Treating console input as a sequence
let processInputCommands commands =
    commands
    |> Seq.takeWhile (fun cmd -> cmd <> 'x')
    |> Seq.iter (fun cmd ->
        printfn ""

        if cmd = 'w' then printfn "Withdrawing money!"
        elif cmd = 'd' then printfn "Depositing money!"
        else printfn $"You executed command {cmd}")

[ 'w'; 'd'; 'z'; 'x'; 'w' ] |> processInputCommands
