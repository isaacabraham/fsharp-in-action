open System.Text.Json

// Listing 10.2
type Brand = Brand of string

type Strings =
    | Six
    | Seven
    | Eight
    | Twelve

[<RequireQualifiedAccess>]
type Pickup =
    | Single
    | Humbucker

type Kind =
    | Acoustic
    | Electric of Pickup list

type Guitar = {
    Brand: Brand
    Strings: Strings
    Kind: Kind
}

let ibanezElectric =
    """{ "Brand" : "Ibanez",
"Strings" : "6",
"Pickups" : [ "H", "S", "H" ] }"""

JsonSerializer.Deserialize<Guitar> ibanezElectric

(*
    Using custom converters with STJ
    This next section illustrates how to use custom converters with STJ. It's not
    necessary for this chapter and not what I would recommend for serializtion,
    but it's useful to know about it nonetheless.
*)

open System.Text.Json.Serialization

// First some basic parsers for our domain types...
module Strings =
    let read value =
        match value with
        | "6" -> Six
        | "7" -> Seven
        | "8" -> Eight
        | "12" -> Twelve
        | _ -> failwith $"Invalid value '{value}'"

    let write value =
        match value with
        | Six -> "6"
        | Seven -> "7"
        | Eight -> "8"
        | Twelve -> "12"

module Pickup =
    let write value =
        match value with
        | Pickup.Single -> "S"
        | Pickup.Humbucker -> "H"

    let read value =
        match value with
        | "S" -> Pickup.Single
        | "H" -> Pickup.Humbucker
        | _ -> failwith $"Invalid value {value}"

// A generic factory function to create JsonConverters
let convert<'T> read (write: _ -> string) =
    { new JsonConverter<'T>() with
        override _.Read(reader, typeToConvert, options) = reader.GetString() |> read
        override _.Write(writer, value, options) = write value |> writer.WriteStringValue
    }

let stringsConverter = convert Strings.read Strings.write
let pickupConverter = convert Pickup.read Pickup.write
let brandConverter = convert Brand (fun (Brand v) -> v)

// Now we can use these converters to deserialize our JSON
let options = JsonSerializerOptions()
options.Converters.Add stringsConverter
options.Converters.Add brandConverter
options.Converters.Add pickupConverter

type SimpleGuitar = {
    Brand: Brand
    Strings: Strings
    Pickups: Pickup list
}

let ibanezJson =
    """{ "Brand" : "Ibanez",
      "Strings" : "12",
      "Pickups" : [ "H", "S", "H" ] }"""

// Deserialize using the converters specified within the options symbol.
let ibanez = JsonSerializer.Deserialize<SimpleGuitar>(ibanezJson, options)
// You can also go back the other way...
JsonSerializer.Serialize(ibanez, options)

// Listing 10.3
#r "nuget:FsToolkit.ErrorHandling"
open FsToolkit.ErrorHandling

type RawGuitar = {
    Brand: string
    Strings: string
    Pickups: string list
}

open System

let tryAsFullGuitar (raw: RawGuitar) = result {
    let! brand =
        if String.IsNullOrWhiteSpace raw.Brand then
            Error "Brand is mandatory"
        else
            Ok(Brand raw.Brand)

    let! strings =
        match raw.Strings with
        | "6" -> Ok Six
        | "7" -> Ok Seven
        | "8" -> Ok Eight
        | "12" -> Ok Twelve
        | value -> Error $"Invalid value '{value}'"

    let! pickups =
        raw.Pickups
        |> List.traverseResultM (fun pickup ->
            match pickup with
            | "S" -> Ok Pickup.Single
            | "H" -> Ok Pickup.Humbucker
            | value -> Error $"Invalid value {value}")

    return {
        Guitar.Brand = brand
        Guitar.Strings = strings
        Guitar.Kind =
            match pickups with
            | [] -> Acoustic
            | pickups -> Electric pickups
    }
}

let ibanezElectricAgain =
    """{ "Brand" : "Ibanez",
"Strings" : "6",
"Pickups" : [ "H", "S", "H" ] }"""

let getOk x =
    x |> Result.valueOr (fun error -> failwith error)

let ibanezGuitar =
    ibanezElectricAgain |> JsonSerializer.Deserialize |> tryAsFullGuitar |> getOk

// Serializing F# into JSON
let yamahaAcoustic =
    """{ "Brand" : "Yamaha",
      "Strings" : "6",
      "Pickups" : [] }"""
    |> JsonSerializer.Deserialize
    |> tryAsFullGuitar
    |> getOk

let createReport (guitars: Guitar list) =
    guitars
    |> List.countBy (fun guitar -> guitar.Brand)
    |> List.map (fun ((Brand brand), count) -> {| Brand = brand; Guitars = count |})
    |> JsonSerializer.Serialize

[ ibanezGuitar; ibanezGuitar; ibanezGuitar; ibanezGuitar; yamahaAcoustic ]
|> createReport

// Anonymous records
let test =
    """{ "name" : "value", "age": 10 }"""
    |> JsonSerializer.Deserialize<{| name: string; age: int |}>

test.age // 10
test.name // "value"
