// Options are the answer
let optionalNumber: int option = Some 10
let missingNumber: int option = None
let mandatoryNumber: int = 10

mandatoryNumber.CompareTo 55
missingNumber.CompareTo 55
missingNumber + mandatoryNumber

let description =
    match optionalNumber with
    | Some number -> $"The number is {number}"
    | None -> "There is no number"

// Option.map
type Customer = { Age: int }

let getAge customer : int option =
    match customer with
    | Some c -> Some c.Age
    | None -> None

let getAge c = c.Age
let classifyCustomer c = if c < 18 then "Child" else "Adult"
let getAgeOptional customer : int option = customer |> Option.map getAge

let optionalClassification optionalCustomer =
    optionalCustomer |> Option.map getAge |> Option.map classifyCustomer

let optionalClassificationShort optionalCustomer =
    optionalCustomer |> Option.map (getAge >> classifyCustomer)

// Exercise 9.1
let tryGetFileContents filename =
    if System.IO.File.Exists filename then
        Some(System.IO.File.ReadAllText filename)
    else
        None

let countWords (text: string) = text.Split ' ' |> Array.length

let countWordsInFile filename =
    filename |> tryGetFileContents |> Option.map countWords

// Option.bind

type CustomerOptionalAge = { Name: string; Age: int option }
let getAge c = c.Age
let theCustomer = Some { Name = "Isaac"; Age = Some 42 }
let optionalAgeDoubleOpt: int option option = theCustomer |> Option.map getAge
let optionalAge: int option = theCustomer |> Option.bind getAge

let optionalAgePatternMatch =
    match theCustomer with
    | Some theCustomer ->
        match theCustomer.Age with
        | Some age -> Some age
        | None -> None
    | None -> None

// .NET Interop
open System
let optionalName: string option = null |> Option.ofObj
let optionalNameTwo = "Isaac" |> Option.ofObj
let optionalAge = Nullable() |> Option.ofNullable
let optionalAgeTwo = Nullable 123 |> Option.ofNullable
