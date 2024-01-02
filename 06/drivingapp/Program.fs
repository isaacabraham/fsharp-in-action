// Exercise 6.4
open System
open Car

printfn "How far do you want to drive?"
let distance = Console.ReadLine() |> int

let startGas = 8.0
let remaining = startGas |> driveRecord distance

if remaining.IsOutOfGas then
    printfn "You ran out of gas!"
else
    printfn $"You have {remaining.GasRemaining} gas left."
