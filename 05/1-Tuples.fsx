// Tuple basics
let theAuthor = "isaac", "abraham"
let firstName, secondName = theAuthor

// Tuple signatures
let name = "isaac", "abraham", 42, "london"

// Wildcards
let nameAndAge = "Jane", "Smith", 25
let forename, surname, _ = nameAndAge

// Listing 5.1
let makeDoctor name =
    let _, sname = name
    "Dr", sname

// Deconstruction
let makeDoctorDecon (_, sname) = "Dr", sname

// Nesting
let nameAndAgeNested = ("Joe", "Bloggs"), 28
let nameNested, age = nameAndAgeNested
let (forenameNested, surnameNested), theAge = nameAndAgeNested

// Exercise 5.1
let buildPerson (forename: string, surname: string, age: int) =
    $"{forename} {surname}", (age, (if age < 18 then "child" else "adult"))

let p1 = buildPerson ("Joe", "Bloggs", 28)
let fullName, (janesAge, description) = buildPerson ("Jane", "Smith", 17)

// Value Tuple
let makeDoctorStruct (name: struct (string * string)) =
    let struct (_, sname) = name
    struct ("Dr", sname)

let implicitStructName = struct ("Joe", "Bloggs") // explicit struct
makeDoctorStruct implicitStructName

makeDoctorStruct ("Joe", "Bloggs") // implicit struct wrapping

// Out parameters
open System
let parsed, theDate = DateTime.TryParse "03 Dec 2020"

if parsed then
    printfn $"Day of week is {theDate.DayOfWeek}!"
