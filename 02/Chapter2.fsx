let getEvenNumbers = Seq.filter (fun number -> number % 2 = 0)
let squareNumbers = Seq.map (fun x -> x * x)
let getEvenNumbersThenSquare = getEvenNumbers >> squareNumbers

let evenNumbers = [ 1..10 ] |> getEvenNumbers
let evenNumbersSquared = [ 1..10 ] |> getEvenNumbersThenSquare

// Listing 2.1
open System

/// A train carriage can have a number of different features...
type Feature =
    | Quiet
    | Wifi
    | Toilet

/// A carriage can be either first or second class
type CarriageClass =
    | First
    | Second

/// Carriages can be either for passengers or buffet carts
type CarriageKind =
    | Passenger of CarriageClass
    | Buffet of {| ColdFood: bool; HotFood: bool |}

/// A carriage has a unique number on the train
type CarriageNumber = CarriageNumber of int

/// A carriage is composed of all of these things.
type Carriage = {
    Number: CarriageNumber
    Kind: CarriageKind
    Features: Feature Set
    NumberOfSeats: int
}

type TrainId = TrainId of string
type Station = Station of string

/// Each stop is a station and a time of arrival
type Stop = Station * TimeOnly

/// A train has a unique id, and a list of carriages. It always has an
/// origin and destination, and a list of stops in between. It *might*
/// also have a station where the driver changes.
type Train = {
    Id: TrainId
    Carriages: Carriage list
    Origin: Stop
    Stops: Stop list
    Destination: Stop
    DriverChange: Station option
}
