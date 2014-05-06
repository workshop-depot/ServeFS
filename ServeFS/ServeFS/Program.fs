module Program

open System
open System.Threading
open System.ServiceProcess
open System.Diagnostics
open System.Collections
open System.ComponentModel
open System.Configuration.Install
open Microsoft.FSharp.Data.UnitSystems.SI.UnitSymbols

open Servo

// *** SAMPLE.begin *** 
// this part can be placed in a separate file/module

Conf.ServiceName <- "SampleService"
Conf.DisplayName <- "Sample Service Display Name"
Conf.Description <- "Sample Service Description"

/// separating implementations lets us debugging it as a console application
type SampleServiceImp () =
    let stopped = new ManualResetEvent(false)

    member x.OnStart (args: String[]) = 
        Trace.WriteLine("Sample Service Implementation: OnStart (running...)")

        async {
            stopped.WaitOne() |> ignore
        } |> Async.Start

    member x.OnStop () = 
        Trace.WriteLine("Sample Service Implementation: OnStop")
        stopped.Set() |> ignore
        Trace.Flush()

type Composed () =
    inherit ServiceBase()

    let imp = new SampleServiceImp()
    do
        base.ServiceName <- Conf.ServiceName
        base.CanHandlePowerEvent <- false
        base.CanHandleSessionChangeEvent <- false
        base.CanPauseAndContinue <- false
        base.CanShutdown <- false
        base.CanStop <- true
        base.AutoLog <- true

    override x.OnStart (args: String[]) =
        //base.RequestAdditionalTime(1000 * 60 * 3)
        imp.OnStart (args)

    override x.OnStop() =
        //base.RequestAdditionalTime(1000 * 60 * 3)
        imp.OnStop()

// *** SAMPLE.end *** 

[<EntryPoint>]
let main (argv: String[]) = 
    Toolbox.Runner(new Composed())
    0 
