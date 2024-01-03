// Listing 7.1
type Result = {
    HomeTeam: string
    HomeGoals: int
    AwayTeam: string
    AwayGoals: int
}

let create home hg away ag = {
    HomeTeam = home
    HomeGoals = hg
    AwayTeam = away
    AwayGoals = ag
}

let results = [
    create "Messiville" 1 "Ronaldo City" 2
    create "Messiville" 1 "Bale Town" 3
    create "Ronaldo City" 2 "Bale Town" 3
    create "Bale Town" 2 "Messiville" 1
]

// Listing 7.2
type TeamSummary = { Name: string; mutable AwayWins: int }
let summary = ResizeArray<TeamSummary>()

for result in results do
    if result.AwayGoals > result.HomeGoals then
        let mutable found = false

        for entry in summary do
            if entry.Name = result.AwayTeam then
                found <- true
                entry.AwayWins <- entry.AwayWins + 1

        if not found then
            summary.Add { Name = result.AwayTeam; AwayWins = 1 }

let mutable wonAwayTheMostImp = summary[0]

for row in summary do
    if row.AwayWins > wonAwayTheMostImp.AwayWins then
        wonAwayTheMostImp <- row

// Listing 7.3
let isAwayWin result = result.AwayGoals > result.HomeGoals

let wonAwayTheMost =
    results
    |> List.filter isAwayWin
    |> List.countBy (fun result -> result.AwayTeam)
    |> List.maxBy (fun (team, count) -> count)

wonAwayTheMost

// The F# List
let a = [ 1; 2; 3 ]
let b = [ 4; 5; 6 ]
let c = a @ b
let d = 0 :: a

// Exercise 7.2
results
|> List.filter (fun r -> r.AwayTeam = "Ronaldo City" || r.HomeTeam = "Ronaldo City")
|> List.length

// Exercise 7.3
results
|> List.collect (fun r -> [
    {|
        Team = r.HomeTeam
        Goals = r.HomeGoals
    |}
    {|
        Team = r.AwayTeam
        Goals = r.AwayGoals
    |}
])
|> List.groupBy (fun result -> result.Team)
|> List.map (fun (team, games) -> team, games |> List.sumBy (fun game -> game.Goals))
|> List.maxBy snd

// Listing 7.4
let numbers = [ 1..10 ]
let secondItem = numbers[1]
let itemsOneToSix = numbers[1..6]
