open System

// Learn more about F# at http://fsharp.org
// See the 'F# Tutorial' project for more help.

//[<EntryPoint>]
//let main argv = 
//    printfn "%A" argv
//    0 // return an integer exit code

[<STAThread>]
do
    let win=Gui_adapter.mainWindow
    win.KeyDown.Add(fun x-> 
        if( x.Key = Windows.Input.Key.V) then
            win.OpMode <- gui.MainWindow.OperateMode.Select
        elif( x.Key = Windows.Input.Key.N) then
            win.OpMode <- gui.MainWindow.OperateMode.IR_Node
        elif( x.Key = Windows.Input.Key.L) then
            win.OpMode <- gui.MainWindow.OperateMode.IR_Link
        elif( x.Key = Windows.Input.Key.M) then
            win.OpMode<- gui.MainWindow.OperateMode.Modify
        else
            //System.Windows.MessageBox.Show(""
            printfn "xkey:%A" x.Key
        )
    System.Windows.Application().Run(win) |> ignore


