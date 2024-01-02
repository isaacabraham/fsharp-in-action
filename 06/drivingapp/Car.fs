// Exercise 6.3
module Car

let drive distance gas =
    if distance > 50 then gas / 2.0
    elif distance > 25 then gas - 10.0
    elif distance > 0 then gas - 1.0
    else gas

// Exercise 6.4
type DriveResult = {
    GasRemaining: float
    IsOutOfGas: bool
}

let driveRecord distance gas =
    let gasLeft =
        if distance > 50 then gas / 2.0
        elif distance > 25 then gas - 10.0
        elif distance > 0 then gas - 1.0
        else gas

    {
        GasRemaining = gasLeft
        IsOutOfGas = gasLeft <= 0.0
    }
