// Listing 4.3
let describeAge age =
    let ageDescription =
        if age < 18 then "Child"
        elif age < 65 then "Adult"
        else "OAP"

    let greeting = "Hello"
    $"{greeting}! You are a '{ageDescription}'."

// Listing 4.5
let calculateAgeDescription age =
    if age < 18 then "Child"
    elif age < 65 then "Adult"
    else "OAP"

let describeAgeRefactored age =
    let ageDescription = calculateAgeDescription age
    let greeting = "Hello"
    printfn $"{greeting}! You are a '{ageDescription}'."

// Unit
let printAddition a b =
    let answer = a + b
    printfn $"{a} plus {b} equals {answer}."

// Unit as an input
let getTheCurrentTime = System.DateTime.Now
let x = getTheCurrentTime
let y = getTheCurrentTime

// Unit and side-effects
let addDays days =
    let newDays = System.DateTime.Today.AddDays days
    printfn $"You gave me {days} days and I gave you {newDays}"
    newDays

let result = addDays 3

let addSeveralDays () =
    addDays 3
    addDays 5
    addDays 7
