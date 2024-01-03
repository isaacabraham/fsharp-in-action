// Higher order functions
let executeCalculation logger a b =
    let answer = a + b
    logger $"Adding {a} and {b} = {answer}"
    answer

let logToConsole message = printfn $"{message}"

let logToFile message =
    System.IO.File.AppendAllText("log.txt", $"{message}")

executeCalculation logToConsole 10 20
executeCalculation logToFile 10 20

// Exercise 7.1
let calculate (operation, name) a b =
    let answer = operation a b
    printfn $"{name} {a} and {b} = {answer}"
    answer

calculate ((fun x y -> x + y), "add") 5 10
calculate ((fun x y -> x * y), "multiply") 5 10
