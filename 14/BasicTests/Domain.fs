namespace global

// Listing 14.1
type Instrument =
    | Guitar
    | Drums
    | Bass
    | Keyboard

[<AutoOpen>]
module Behaviours =
    let play instrument =
        match instrument with
        | Guitar -> "Wah wah"
        | Drums -> "Ba dumm tss"
        | Bass -> "Ba ba bom"
        | Keyboard -> "Blip blop"
