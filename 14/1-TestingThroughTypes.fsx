[<AutoOpen>]
module Domain =
    /// Single case DU with a private ctor - can only be created through this module or a public static method
    type ValidatedEmail =
        private
        | ValidatedEmail of string

        member this.Value =
            match this with
            | (ValidatedEmail e) -> e

        static member TryCreate(unvalidatedEmail: string) =
            if unvalidatedEmail.Contains "@" then
                Ok(ValidatedEmail unvalidatedEmail)
            else
                Error "Invalid email!"

let sendEmailSafe message (email: ValidatedEmail) = Ok "Sent message"

// doesn't compile - *must* use TryCreate method.
// let x = ValidatedEmail "foo@bar.com"

let sendResult =
    ValidatedEmail.TryCreate "isaac@email.com" // replace with "bla bla"
    |> Result.bind (sendEmailSafe "Welcome!")

#load "BasicTests/Domain.fs"

// Try adding a new instrument and see that you correctly get a warning in play, but not in playBad!
let playBad instrument =
    match instrument with
    | Guitar -> "Wah wah"
    | Drums -> "Ba dam tss"
    | Keyboard -> "Blip blop"
    | _ -> "Ba ba bom"
