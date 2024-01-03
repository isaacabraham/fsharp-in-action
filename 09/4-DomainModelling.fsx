// Exercise 9.3
open System
#r "nuget:FSToolkit.ErrorHandling, 2.13.0"
open FsToolkit.ErrorHandling

/// A train carriage can have a number of different features...
type Feature =
    | Quiet
    | Wifi
    | Toilet

/// Multiple classes
type CarriageClass =
    | First
    | Second

/// Carriages can be either for passengers or the buffet cart
type CarriageKind =
    | Passenger of CarriageClass
    | Buffet of {| ColdFood: bool; HotFood: bool |}

/// A carriage has a unique number on the train
type CarriageNumber = CarriageNumber of int

/// A carriage has a number, kind, a list of features and a finite number of seats.
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

/// A train has a unique id, and a list of carriages. It always has an origin and destination,
/// and may a list of stops in between. It *might* also have a station where the driver changes.
type Train = {
    Id: TrainId
    Carriages: Carriage list
    Origin: Stop
    Stops: Stop list
    Destination: Stop
    DriverChange: Station option
}

module TrainFunctions =
    /// A function that calculates the total number of seats in a train
    let getTotalNumberOfSeats train =
        train.Carriages |> List.sumBy (fun carriage -> carriage.NumberOfSeats)

    let findCarriagesWithFeature feature train =
        train.Carriages
        |> List.filter (fun carriage -> carriage.Features.Contains feature)

    type CalculateStopError =
        | NoSuchStop of Station
        | InvalidOrder of Station * Station
        | DuplicateStop of Station

    let calculateTimeBetweenStops (start, stop) train = result {
        if start = stop then
            return! Error(DuplicateStop start)

        let allStops = [ train.Origin; yield! train.Stops; train.Destination ]

        let! originStop =
            allStops
            |> List.tryFind (fun (station, time) -> station = start)
            |> Result.requireSome (NoSuchStop start)

        let! destinationStop =
            allStops
            |> List.tryFind (fun (station, time) -> station = stop)
            |> Result.requireSome (NoSuchStop stop)

        let originStopTime = snd originStop
        let destinationStopTime = snd destinationStop

        if originStopTime > destinationStopTime then
            return! Error(InvalidOrder(start, stop))

        return destinationStopTime - originStopTime
    }

TimeOnly(0, 0).ToTimeSpan() - TimeOnly(1, 0).ToTimeSpan()

let exampleTrain = {
    Id = TrainId "ABC123"
    Carriages = [
        {
            Number = CarriageNumber 1
            Kind = Passenger First
            Features = Set [ Wifi; Quiet; Toilet ]
            NumberOfSeats = 45
        }
        {
            Number = CarriageNumber 2
            Kind = Passenger Second
            Features = Set [ Toilet ]
            NumberOfSeats = 65
        }
        {
            Number = CarriageNumber 3
            Kind = Buffet {| ColdFood = true; HotFood = true |}
            Features = Set [ Wifi ]
            NumberOfSeats = 12
        }
    ]
    Origin = Station "London St Pancras", TimeOnly(9, 00)
    Stops = [ Station "Ashford", TimeOnly(10, 30); Station "Lille", TimeOnly(11, 30) ]
    Destination = Station "Paris Nord", TimeOnly(12, 15)
    DriverChange = Some(Station "Ashford")
}

let exampleTrainSeats = exampleTrain |> TrainFunctions.getTotalNumberOfSeats
let wifiCarriages = exampleTrain |> TrainFunctions.findCarriagesWithFeature Wifi

let londonParisNord =
    exampleTrain
    |> TrainFunctions.calculateTimeBetweenStops (Station "London St Pancras", Station "Paris Nord")

let londonParis =
    exampleTrain
    |> TrainFunctions.calculateTimeBetweenStops (Station "London St Pancras", Station "Paris")

let parisLondon =
    exampleTrain
    |> TrainFunctions.calculateTimeBetweenStops (Station "Paris Nord", Station "London St Pancras")

let londonLondon =
    exampleTrain
    |> TrainFunctions.calculateTimeBetweenStops (Station "London St Pancras", Station "London St Pancras")
