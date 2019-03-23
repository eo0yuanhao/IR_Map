open System

// Learn more about F# at http://fsharp.org
// See the 'F# Tutorial' project for more help.

//[<EntryPoint>]
//let main argv = 
//    printfn "%A" argv
//    0 // return an integer exit code
type OperateMode= gui.MainWindow.OperateMode
type Keys = Windows.Input.Key

open System.Windows.Controls
open System.Windows.Media
open System.Windows.Shapes
open System.Windows.Input

[<STAThread>]
do
    let win=Gui_adapter.mainWindow
    let canvas = Gui_adapter.canvas
    win.KeyDown.Add(fun x-> 
        match x.Key with
        | Keys.V -> win.OpMode <- OperateMode.Select 
        | Keys.N -> win.OpMode <- OperateMode.IR_Node
        | Keys.L -> win.OpMode <- OperateMode.IR_Link
        | Keys.M -> win.OpMode <- OperateMode.Modify
        | Keys.G -> printfn "can:%f,%f" canvas.ActualHeight canvas.ActualWidth
        | _ -> printfn "xkey:%A" x.Key
        )
        //if( x.Key = Keys.V) then
        //    win.OpMode <- OperateMode.Select
        //elif( x.Key = Keys.N) then
        //    win.OpMode <- OperateMode.IR_Node
        //elif( x.Key = Keys.L) then
        //    win.OpMode <- OperateMode.IR_Link
        //elif( x.Key = Keys.M) then
        //    win.OpMode<- OperateMode.Modify
        //else
        //    //System.Windows.MessageBox.Show(""
        //    printfn "xkey:%A" x.Key
        //)
     
    let mutable curSelShape:Shape = null
    //let mutable current_OpMode = OperateMode.Select
    let mutable previous_OpMode = OperateMode.Select
    win.OperateMode_Changed.Add( fun e->
        let curMode=win.OpMode
        if(curMode = previous_OpMode) then
            ()
        else 
            if(previous_OpMode = OperateMode.Select) then
                if(curSelShape <> null) then
                    curSelShape.Fill <- Brushes.LightBlue
                    curSelShape <-  null
            previous_OpMode <- curMode
        )
    let setpos x y (v:Visual)=
        v.SetValue(Canvas.LeftProperty,x)
        v.SetValue(Canvas.TopProperty,y)
    let do_select(e:MouseButtonEventArgs)=
        // if not select shape
        if( e.OriginalSource = (canvas :> obj)) then
            if(curSelShape <> null) then
                curSelShape.Fill <- Brushes.LightBlue
                curSelShape <-  null
        else
            let sh= e.OriginalSource :?> Shape
            if( curSelShape <> null ) then
                curSelShape.Fill <- Brushes.LightBlue
            curSelShape <- sh
            sh.Fill <- Brushes.Blue
    let do_node(p:Windows.Point)=
        let e =new Ellipse()
        e.Width <- 40.0
        e.Height <- 20.0
        e.StrokeThickness <- 1.0
        e.Fill <- Brushes.LightBlue
        e |> setpos (p.X-20.0) (p.Y-10.0) 
        canvas.Children.Add(e) |> ignore
    let do_link()=
        ()
    let do_modify()=
        ()
    // a Canvas Background must be specificate, or else MouseDown event not works
    let brush=  Windows.Media.Brushes.Transparent
    canvas.Background <- brush
    canvas.MouseDown.Add(fun x-> 
        let pos= x.GetPosition(canvas)
        match win.OpMode with 
        | OperateMode.Select  ->  do_select(x)
        | OperateMode.IR_Node -> do_node(pos)
        | OperateMode.IR_Link -> do_link()
        | OperateMode.Modify  -> do_modify()
        | _ -> ()
        )

    System.Windows.Application().Run(win) |> ignore