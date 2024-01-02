// Listing 3.1
let addTenThenDouble theNumber =
    let addedTen = theNumber + 10
    let answer = addedTen * 2
    printfn $"({theNumber} + 10) * 2 is {answer}"
    let website = System.Uri "https://fsharp.org"
    answer

// Listing 3.2 (mostly!)
let addTenThenDoubleVerbose (theNumber: int) : int =
    let addedTen: int = theNumber + 10
    let answer: int = addedTen * 2
    printfn $"({theNumber} + 10) * 2 is {answer}"
    let website: System.Uri = new System.Uri("https://fsharp.org")
    answer

// The let keyword
let doACalculation theNumber =
    let twenty = 20
    let answer = twenty + theNumber
    let foo = System.Uri "https://fsharp.org"
    answer

let isaac = 42
let olderIsaac = isaac + 1
let youngerIsaac = isaac - 1

// same as
let olderIsaacSimple = 42 + 1
let youngerIsaacSimple = 42 - 1

let fname = "Frank"
let sname = "Schmidt"
let fullName = $"{fname} {sname}"
let greetingText = $"Greetings, {fullName}"

// Scoping

// Exercise 3.1
let exercise31 a b c =
    let inProgress = a + b
    let answer = inProgress * c
    $"The answer is {answer}"

let greetingTextScoped =
    let fullName =
        let fname = "Frank"
        let sname = "Schmidt"
        $"{fname} {sname}"

    $"Greetings, {fullName}"

// Nested Functions
let greetingTextWithFunction person =
    let makeFullName fname sname = $"{fname} {sname}"
    let fullName = makeFullName "Frank" "Schmidt"
    $"Greetings {fullName} from {person}."

// Listing 3.3
let greetingTextWithFunctionScoped =
    let city = "London"
    let makeFullName fname sname = $"{fname} {sname} from {city}"
    let fullName = makeFullName "Frank" "Schmidt"
    let surnameCity = $"{sname} from {city}" // won’t compile
    $"Greetings, {fullName}"

// Listing 3.4
let add (a: int) (b: int) : int =
    let answer: int = a + b
    answer

// Exercise 3.2
let addNoTypeAnnotations a b =
    let answer = a + b
    answer

// Inferring generics
let explicit = ResizeArray<int>()
let typeHole = ResizeArray<_>()
let omitted = ResizeArray()

typeHole.Add 99
omitted.Add 10

// Automatic generalization
let combineElements<'T> (a: 'T) (b: 'T) (c: 'T) =
    let output = ResizeArray<'T>()
    output.Add a
    output.Add b
    output.Add c
    output

combineElements<int> 1 2 3

let combineElementsNoTypeAnnotations a b c =
    let output = ResizeArray()
    output.Add a
    output.Add b
    output.Add c
    output

combineElements 1 2 3

// Unexpected type inference behavior
let calculateGroup age =
    if age < 18 then "Child"
    elif age < 65 then "Adult"
    else "Pensioner"

let sayHello someValue =
    let group =
        if someValue < 10.0 then
            calculateGroup 15
        else
            calculateGroup 35

    "Hello " + group

let result = sayHello 10.5

// Limitations of type inference
let addThreeDays (theDate: System.DateTime) = theDate.AddDays 3

let addAYearAndThreeDays theDate =
    let threeDaysForward = addThreeDays theDate
    theDate.AddYears 1
