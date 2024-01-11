open System.IO
open System

// Listing 11.1
let fileInfo = FileInfo($"{__SOURCE_DIRECTORY__}\\{__SOURCE_FILE__}")
let directoryInfo = fileInfo.Directory
let files = directoryInfo.GetFiles()

open System.Linq

let scriptFiles =
    files
    |> Seq.where (fun f -> Path.GetExtension f.Name = ".fsx")
    |> Seq.map (fun f -> f.Name)

let scriptFilesLinq =
    files.Where(fun f -> Path.GetExtension f.Name = ".fsx").Select(fun f -> f.Name)

let ctorSuccinct = FileInfo __SOURCE_FILE__
let ctorVerbose = new FileInfo(__SOURCE_FILE__)

// Interfaces
type MyDisposableType() =
    interface IDisposable with
        member _.Dispose() = printfn "Disposing!"

type MyInterface =
    abstract Capitalize: string -> string
    abstract Add: int -> int

// Listing 11.2
type MyImplementation() =
    interface MyInterface with
        member _.Capitalize text = text.ToUpper()
        member _.Add number = number + 1

let implementation = MyImplementation()
implementation.Capitalize "test" // compiler error!

let implementation: MyInterface = MyImplementation()
let text = implementation.Capitalize "test"

// A record of functions
type MyInterfaceAsRecord = {
    Capitalize: string -> string
    Add: int -> int
}

// Object expressions
let implementation =
    { new MyInterface with
        member _.Capitalize text = text.ToUpper()
        member _.Add number = number + 1
    }

let text = implementation.Capitalize "test"

// Interfaces on F# types
type Person = {
    Name: string
    Age: int
} with

    interface System.ICloneable with
        member this.Clone() = { Name = this.Name; Age = this.Age }

// Fluent APIs

/// A stub type to simulate a fluent API
type WebFramework() =
    member this.AddAuthentication() = this
    member this.AddCors() = this
    member this.AddCaching() = this

let setUpWebApp () =
    let framework = WebFramework()
    framework.AddAuthentication().AddCors().AddCaching() |> ignore // ignore required here to avoid warning
    Ok()

// Out parameters
let parsed, value = System.Int32.TryParse "123"

let parseOption parser (value: string) =
    match parser value with
    | true, v -> Some v
    | false, _ -> None

let parseIntOption = parseOption System.Int32.TryParse
let maybeANumber = parseIntOption "123"
let maybeNotANumber = parseIntOption "123S"

#r "nuget: FsToolkit.ErrorHandling, 2.0.0"
// Using an SRTP to seek out TryParse on the type defined on the LHS of the =
// See https://learn.microsoft.com/en-us/dotnet/fsharp/language-reference/generics/statically-resolved-type-parameters
let xInt: int option = FsToolkit.ErrorHandling.Option.tryParse "1"
let xBool: bool option = FsToolkit.ErrorHandling.Option.tryParse "true"
let xNotABool: bool option = FsToolkit.ErrorHandling.Option.tryParse "dxcj834"

// Single method interfaces
/// The IDisplayTime interface is used to print the time.
type IDisplayTime =
    /// Prints the time to the user.
    abstract Display: DateTime -> string

let makeIDisplayTime implementation =
    { new IDisplayTime with
        member _.Display date = implementation date
    }

let normalPrinter = makeIDisplayTime (fun date -> $"The time is now {date}!")

let shortPrinter =
    makeIDisplayTime (fun date -> $"It's {date.ToShortTimeString()}.")

normalPrinter.Display DateTime.UtcNow
shortPrinter.Display DateTime.UtcNow

// Partial application and pipelines
module File =
    let append path text =
        File.AppendAllText(path, text)
        path

    File.WriteAllText("text.txt", "test")

let fileInfo =
    "text.txt"
    |> File.ReadAllText
    |> fun text -> text.ToUpper() |> File.append "otherfile.txt" |> FileInfo
