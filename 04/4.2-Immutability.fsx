let name = "isaac"
let mutable age = 42
age <- 43

if age = 43 then
    ()

// Listing 4.6
let mutable gas = 100.0

let drive distance =
    if distance = "far" then gas <- gas / 2.0
    elif distance = "medium" then gas <- gas - 10.0
    else gas <- gas - 1.0

drive "far"
drive "medium"
drive "near"
gas

let driveImmutable gas distance =
    if distance = "far" then gas / 2.0
    elif distance = "medium" then gas - 10.0
    else gas - 1.0

let initialState = 100.0
let firstState = driveImmutable initialState "far"
let secondState = driveImmutable firstState "medium"
let finalState = driveImmutable secondState "near"

// Exercise 4.3
let drive43 gas distance =
    if distance > 50 then gas / 2.0
    elif distance > 25 then gas - 10.0
    elif distance > 0 then gas - 1.0
    else gas
