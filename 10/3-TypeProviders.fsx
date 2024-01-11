// Listing 10.4
#r "nuget:FSharp.Data, 6.3.0"
open FSharp.Data

[<Literal>]
let SampleJson =
    """{
"Brand": "Ibanez",
"Strings": "7",
"Pickups": [ "H", "S", "H" ] }"""

type GuitarJson = JsonProvider<SampleJson>
//type GuitarJson = JsonProvider<const(__SOURCE_DIRECTORY__ + "\\sample-guitars.json")>
let sample = GuitarJson.GetSample()
let description = $"{sample.Brand} has {sample.Strings} strings."

// Working with external data sources
type ManyGuitarsOverHttp =
    JsonProvider<"https://raw.githubusercontent.com/isaacabraham/fsharp-in-action/main/sample-guitars.json">

let inventory = ManyGuitarsOverHttp.GetSamples()
$"You have {inventory.Length} guitars in stock."

let fullInventory =
    ManyGuitarsOverHttp.Load(__SOURCE_DIRECTORY__ + "/full-inventory.json")

fullInventory[0]

// Listing 10.5
open FSharp.Data
//type LondonBoroughs = HtmlProvider<"https://en.wikipedia.org/wiki/List_of_London_boroughs">
type LondonBoroughs = HtmlProvider<const(__SOURCE_DIRECTORY__ + "\\List of London boroughs - Wikipedia.html")>

let boroughs =
    LondonBoroughs.GetSample().Tables.``List of boroughs and local authorities``

boroughs.Rows |> Array.map (fun row -> row.Borough)
