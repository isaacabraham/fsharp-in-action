#r "nuget:FsCheck"

open System
open FsCheck

// Our function that we want to test.
let flipCase (text: string) =
    text
    |> Seq.map (fun c -> if Char.IsUpper c then Char.ToLower c else Char.ToUpper c)
    // |> Seq.map (fun c -> if c <> 'e' then if Char.IsUpper c then Char.ToLower c else Char.ToUpper c else c) // Broken implementation - use it to test how FSCheck identifes a minimal failing case!
    |> Seq.toArray
    |> String

type Word = Word of string

/// This type contains functions that will shape how we generate data.
/// In this case, we want to generate strings that contain only letters and spaces.
type Arbitraries =
    static member Word() =
        { new Arbitrary<Word>() with
            override _.Generator =
                Gen.elements ([ 'a' .. 'z' ] @ [ 'A' .. 'Z' ] @ [ ' ' ])
                |> Gen.arrayOf
                |> Gen.filter (Array.isEmpty >> not)
                |> Gen.map (String >> Word)

            override _.Shrinker(Word value) =
                Arb.Default.String().Shrinker value |> Seq.map Word
        }

/// The properties that we want to prove (or disprove!). Each property is a function
type Properties =
    /// Property 1: Length of the string does not change.
    static member sameNumberOfLetters(Word original) =
        let flipped = flipCase original

        original.Length = flipped.Length

    /// Property 2: Every letter changes case.
    static member changesCase(Word original) =
        let flipped = flipCase original

        Seq.zip flipped original
        |> Seq.forall (fun (a, b) ->
            if Char.IsLetter a then
                if Char.IsUpper a then Char.IsLower b else Char.IsUpper b
            else
                true)

    /// Property 3: Every letter is the same.
    static member doesNotChangeText(Word original) =
        let flipped = flipCase original

        String.Compare(flipped, original, StringComparison.OrdinalIgnoreCase) = 0

/// The configuration that uses our custom generator.
let configuration = {
    // Try changing this to Config.Verbose to see individual test cases.
    Config.Quick with
        Arbitrary = [ typeof<Arbitraries> ]
}

// Check a single property.
Check.One(configuration, Properties.changesCase)

// Check all properties at once.
Check.All<Properties> configuration

//TODO: Try "breaking" the logic of flipCase and see what happens!
