open Microsoft.AspNetCore.Builder
open Giraffe
open Microsoft.AspNetCore.Http
open System.Threading.Tasks
open FsToolkit.ErrorHandling

let giraffeApp: HttpHandler = text "Hello, world!"

let giraffeAppV2: HttpHandler =
    GET >=> route "/device/status" >=> text "device is operating correctly"

// Listing 13.3
let isGet (next: HttpFunc) (ctx: HttpContext) =
    if ctx.Request.Method = "GET" then
        next ctx
    else
        Task.FromResult None

let exampleUseOfIsGet: HttpHandler = isGet >=> text "Yes, it's a get!"
// GET http://localhost:5000 (returns "Yes, it's a get!")
// POST http://localhost:5000 (returns 404)

// Listing 13.5
let exampleOfChoose: HttpHandler =
    choose [
        GET >=> route "/device/status" >=> text "device is operating correctly"
        POST >=> route "/device/execute" >=> text "executed a command!"
    ]
// GET http://localhost:5000/device/status
// POST http://localhost:5000/device/execute


module CustomHandler =
    // Listing 13.6
    module Db =
        let tryFindDeviceStatus (deviceId: int) = task {
            if deviceId < 50 then return Some "ACTIVE"
            elif deviceId < 100 then return Some "IDLE"
            else return None
        }

    let getDeviceStatus next (ctx: HttpContext) = task {
        let deviceId =
            ctx.TryGetQueryStringValue "deviceId"
            |> Option.defaultWith (fun _ -> failwith "Missing device id")
            |> int

        let! deviceStatus = Db.tryFindDeviceStatus deviceId

        match deviceStatus with
        | None -> return! RequestErrors.NOT_FOUND "No device id found" next ctx
        | Some status ->
            return!
                json
                    {|
                        DeviceId = deviceId
                        Status = status
                    |}
                    next
                    ctx
    }

let myFunctionalWebApp: HttpHandler =
    GET >=> route "/device/status" >=> CustomHandler.getDeviceStatus
// GET http://localhost:5000/device/status?deviceId=12  // Active
// GET http://localhost:5000/device/status?deviceId=52  // Idle
// GET http://localhost:5000/device/status?deviceId=152 // 404
// GET http://localhost:5000/device/status              // 500

// Parameterisation
let parameterisedGetDeviceStatus (deviceId: int) = json {| DeviceId = deviceId |}

let simpleParameterisation: HttpHandler =
    GET >=> routef "/device/status/%i" parameterisedGetDeviceStatus
// GET http://localhost:5000/device/status/12   // {| deviceId = 12 |}
// GET http://localhost:5000/device/status/test // 404

// POST Body
let postDeviceStatus next (ctx: HttpContext) = task {
    let! request = ctx.BindModelAsync<{| DeviceId: int |}>()
    return! json {| DeviceId = request.DeviceId |} next ctx
}

let postBody: HttpHandler = POST >=> route "/device/status" >=> postDeviceStatus

(*
    POST http://localhost:5000/device/status
    content-type: application/json

    { "deviceId" : 13 }
*)

// Listing 13.7
module CustomValidation =
    type DeviceStatusError =
        | NoDeviceIdSupplied
        | InvalidDeviceId of string
        | NoSuchDeviceId of int

        member this.Description =
            match this with
            | NoDeviceIdSupplied -> "No device Id was provided."
            | InvalidDeviceId text -> $"'{text}' is not a valid Device Id"
            | NoSuchDeviceId deviceId -> $"Device Id {deviceId} does not exist."

    type DeviceStatus =
        | Active
        | Idle

        member this.Description =
            match this with
            | Active -> "Active"
            | Idle -> "Idle"

        static member Parse text =
            match text with
            | "ACTIVE" -> Active
            | "IDLE" -> Idle
            | status -> failwith $"Invalid device status {status}"

    type DeviceStatusResponse = {
        DeviceId: int
        DeviceStatus: DeviceStatus
    }

    let tryGetDeviceStatus maybeDeviceId = taskResult {
        let! (rawDeviceId: string) = maybeDeviceId |> Result.requireSome NoDeviceIdSupplied
        let! deviceId = Option.tryParse rawDeviceId |> Result.requireSome (InvalidDeviceId rawDeviceId)
        let! deviceStatus = CustomHandler.Db.tryFindDeviceStatus deviceId

        let! deviceStatus = deviceStatus |> Result.requireSome (NoSuchDeviceId deviceId)

        return {
            DeviceId = deviceId
            DeviceStatus = DeviceStatus.Parse deviceStatus
        }
    }

    // GET http://localhost:5000/device/status?deviceId=12  // Active
    // GET http://localhost:5000/device/status?deviceId=52  // Idle
    // GET http://localhost:5000/device/status?deviceId=152 // 404
    // GET http://localhost:5000/device/status              // 500
    let warehouseApi next (ctx: HttpContext) = task {
        let maybeDeviceId = ctx.TryGetQueryStringValue "deviceId"
        let! deviceStatus = tryGetDeviceStatus maybeDeviceId

        match deviceStatus with
        | Error errorCode -> return! RequestErrors.BAD_REQUEST errorCode.Description next ctx
        | Ok deviceInfo ->
            return!
                json
                    {|
                        deviceInfo with
                            DeviceStatus = deviceInfo.DeviceStatus.Description
                    |}
                    next
                    ctx
    }

    let app: HttpHandler = GET >=> route "/device/status" >=> warehouseApi

module View =
    open Giraffe.ViewEngine

    let createPage pageTitle content =
        html [] [
            head [] [
                title [] [ str "Device Status Manager" ]
                link [
                    _rel "stylesheet"
                    _href "https://cdn.jsdelivr.net/npm/bulma@0.9.4/css/bulma.min.css"
                ]
            ]
            body [] [
                section [ _class "section" ] [
                    div [ _class "container" ] [ h1 [ _class "title" ] [ str pageTitle ]; content ]
                ]
            ]
        ]

    open CustomValidation

    let renderSuccess (device: DeviceStatusResponse) =
        div [
            let color =
                match device.DeviceStatus with
                | Idle -> "is-warning"
                | Active -> "is-success"

            _class $"notification {color}"
        ] [
            str $"Device ID {device.DeviceId} has status of {device.DeviceStatus.Description}"
        ]

    let renderError (error: DeviceStatusError) =
        div [ _class $"notification is-danger" ] [ str error.Description ]

    let warehouseView response =
        match response with
        | Ok status -> renderSuccess status |> createPage "Found device!"
        | Error error -> renderError error |> createPage "Request error!"
        |> htmlView

    // Listing 13.8
    /// A simple HTTP Handler which can return HTML of a single device.
    let simpleView next (ctx: HttpContext) = task {
        let maybeDeviceId = ctx.TryGetQueryStringValue "deviceId"

        let! deviceStatus = tryGetDeviceStatus maybeDeviceId

        let view =
            html [] [
                body [] [
                    h1 [] [ str "Device report" ]
                    match deviceStatus with
                    | Error errorCode -> str $"Error: {errorCode.Description}"
                    | Ok deviceInfo ->
                        str $"Success: {deviceInfo.DeviceId} has status {deviceInfo.DeviceStatus.Description}"
                ]
            ]

        return! htmlView view next ctx
    }

    // Listing 13.9
    let viewAllDevices devices next (ctx: HttpContext) = task {
        let view =
            createPage
                "Device List"
                (table [ _class "table is-bordered is-striped" ] [
                    thead [] [ tr [] [ th [] [ str "Device Id" ]; th [] [ str "Device Status" ] ] ]
                    tbody [] [
                        for device in devices do
                            tr [] [
                                td [] [ str $"{device.DeviceId}" ]
                                td [] [ str device.DeviceStatus.Description ]
                            ]
                    ]
                ])

        return! htmlView view next ctx
    }

    let processStatus deviceStatus =
        match deviceStatus with
        | Error(errorCode: DeviceStatusError) -> RequestErrors.BAD_REQUEST errorCode.Description
        | Ok deviceInfo ->
            json {|
                DeviceId = deviceInfo.DeviceId
                Status = deviceInfo.DeviceStatus.Description
            |}

    /// An HTTP Handler that can return HTML or JSON depending on the mode query string parameter.
    let deviceStatusWithHtml next (ctx: HttpContext) = task {
        let maybeDeviceId = ctx.TryGetQueryStringValue "deviceId"
        let! deviceStatus = tryGetDeviceStatus maybeDeviceId

        match ctx.TryGetQueryStringValue "mode" with
        | Some "html" -> return! warehouseView deviceStatus next ctx
        | Some "api" -> return! processStatus deviceStatus next ctx
        | Some _
        | None -> return! RequestErrors.BAD_REQUEST "You must specific mode=html or mode=api" next ctx
    }

    let threeDevices = [
        { DeviceId = 1; DeviceStatus = Active }
        { DeviceId = 2; DeviceStatus = Idle }
        { DeviceId = 3; DeviceStatus = Active }
    ]

    /// The full application - list all devices or get a single device in HTML or JSON.
    let fullApp: HttpHandler =
        choose [ GET >=> route "/all" >=> (viewAllDevices threeDevices); deviceStatusWithHtml ]

// Assign one of the HTTP Handlers above here to use in the app
let appToUse = View.simpleView

// Listing 13.4
let builder = WebApplication.CreateBuilder()
builder.Services.AddGiraffe() |> ignore
let app = builder.Build()
app.UseGiraffe appToUse
app.Run()
