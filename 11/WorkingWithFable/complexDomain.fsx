open System

type ShippingStatus =
    | Fulfilled of
        {| FulfilledOn: DateTime
           PaidOn: DateTime option |}
    | Outstanding of {| DueOn: DateTime |}

type Order =
    { Id: int
      PlacedOn: DateTime
      Status: ShippingStatus
      Items: (string * decimal) list }

let order =
    { Id = 123
      PlacedOn = DateTime(2022, 10, 1)
      Status = Outstanding {| DueOn = DateTime(2022, 10, 3) |}
      Items = [ "F# In Action book", 40M; "New Laptop", 500M ] }

let totalValue order = order.Items |> List.sumBy snd

printfn $"Order {order.Id} has a value of {totalValue order} and was placed on {order.PlacedOn}"
