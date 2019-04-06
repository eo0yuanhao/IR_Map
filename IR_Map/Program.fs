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
open System.Windows

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
    let mutable curCreateShape:Shape =null
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
    let mutable startPos= Point()
    let mutable curPos=Point()
    let mutable  endPos = Point()
    let do_node()= //startPos,endPos: Point)=
        let e =new Ellipse()
        e.Width <- 40.0
        e.Height <- 20.0
        e.StrokeThickness <- 1.0
        e.Fill <- Brushes.LightBlue
        e |> setpos (startPos.X-20.0) (startPos.Y-10.0) 
        canvas.Children.Add(e) |> ignore
    let do_link()=
        ()
    let do_modify()=
        ()

    let mutable nodeCreating= false
    let mutable linkCreating= false

    let do_nodeCreate_start(e:MouseEventArgs)=
        startPos<- e.GetPosition(canvas)
        let e =new Rectangle()
        e.Width <- 40.0
        e.Height <- 20.0
        e.StrokeThickness <- 1.0
        e.Fill <- Brushes.LightBlue
        e |> setpos  startPos.X startPos.Y //   (p.X-20.0) (p.Y-10.0) 
        curCreateShape <- e
        canvas.Children.Add(e) |> ignore 
        nodeCreating <- true

    let do_nodeCreate_move(e:MouseEventArgs)=
        if(nodeCreating) then
            curPos <- e.GetPosition(canvas)
            if(curPos.X >startPos.X) then
                if( curPos.Y > startPos.Y) then
                    let w= max (curPos.X - startPos.X) 40.0
                    let h= max (curPos.Y - startPos.Y) 20.0
                    curCreateShape |> setpos startPos.X startPos.Y
                    curCreateShape.SetValue(Canvas.WidthProperty,w)
                    curCreateShape.SetValue(Canvas.HeightProperty,h)
                else
                    let w= max (curPos.X - startPos.X) 40.0
                    let h= max(startPos.Y - curPos.Y) 20.0
                    curCreateShape |> setpos (startPos.X) (startPos.Y - h)
                    curCreateShape.SetValue(Canvas.WidthProperty,w)
                    curCreateShape.SetValue(Canvas.HeightProperty,h)
            else
                if( curPos.Y > startPos.Y) then
                    let w= max (startPos.X - curPos.X) 40.0
                    let h= max (curPos.Y - startPos.Y) 20.0
                    curCreateShape |> setpos (startPos.X-w ) (startPos.Y)
                    curCreateShape.SetValue(Canvas.WidthProperty,w)
                    curCreateShape.SetValue(Canvas.HeightProperty,h)
                else
                    let w= max (startPos.X - curPos.X) 40.0
                    let h= max(startPos.Y - curPos.Y) 20.0
                    curCreateShape |> setpos (startPos.X-w) (startPos.Y-h)
                    curCreateShape.SetValue(Canvas.WidthProperty,w)
                    curCreateShape.SetValue(Canvas.HeightProperty,h)

    let do_nodeCreate_end()=
        nodeCreating <- false

    let do_linkCreate_start(evt:MouseEventArgs)=
        startPos<- evt.GetPosition(canvas)
        let e = new ext.TestShape()
        e.X1 <- startPos.X
        e.Y1 <- startPos.Y
        e.StrokeThickness <- 3.0
        e.Fill <- Brushes.LightBlue
        e.Stroke <- Brushes.DarkBlue
        //e |> setpos  startPos.X startPos.Y //   (p.X-20.0) (p.Y-10.0) 
        curCreateShape <- e
        canvas.Children.Add(e) |> ignore 
        linkCreating <- true

    let do_linkCreate_move(evt:MouseEventArgs)=
        if(linkCreating) then
            curPos <- evt.GetPosition(canvas)
            if(curPos.X >startPos.X) then
                if( curPos.Y > startPos.Y) then
                    let w= max (curPos.X - startPos.X) 40.0
                    let h= max (curPos.Y - startPos.Y) 20.0
                    let e= curCreateShape :?> ext.TestShape
                    e.X2 <- curPos.X
                    e.Y2 <- curPos.Y
                else
                    let w= max (curPos.X - startPos.X) 40.0
                    let h= max(startPos.Y - curPos.Y) 20.0
                    let e= curCreateShape :?> ext.TestShape
                    e.X2 <- curPos.X
                    e.Y2 <- curPos.Y
            else
                if( curPos.Y > startPos.Y) then
                    let w= max (startPos.X - curPos.X) 40.0
                    let h= max (curPos.Y - startPos.Y) 20.0
                    let e= curCreateShape :?> ext.TestShape
                    e.X2 <- curPos.X
                    e.Y2 <- curPos.Y
                else
                    let w= max (startPos.X - curPos.X) 40.0
                    let h= max (startPos.Y - curPos.Y) 20.0
                    let e= curCreateShape :?> ext.TestShape
                    e.X2 <- curPos.X
                    e.Y2 <- curPos.Y
    let do_linkCreate_end()=
        linkCreating <- false


    // a Canvas Background must be specificate, or else MouseDown event not works
    let brush=  Windows.Media.Brushes.Transparent
    canvas.Background <- brush

    canvas.MouseDown.Add(fun e-> 
        match win.OpMode with 
        | OperateMode.Select  ->  do_select(e)
        | OperateMode.IR_Node ->  do_nodeCreate_start(e) //do_nodeDragCreate()//do_node(pos)
        | OperateMode.IR_Link ->  do_linkCreate_start(e) //do_linkDragCreate()//do_link()
        | OperateMode.Modify  ->  do_modify()
        | _ -> ()

        )
    canvas.MouseMove.Add(fun e->
        match win.OpMode with 
        | OperateMode.Select  ->  ()//do_select(e)
        | OperateMode.IR_Node ->  do_nodeCreate_move(e) //do_nodeDragCreate()//do_node(pos)
        | OperateMode.IR_Link ->  do_linkCreate_move(e) //do_linkDragCreate()//do_link()
        | OperateMode.Modify  ->  do_modify()
        | _ -> ()
        
        )
    canvas.MouseUp.Add(fun e ->
        endPos <- e.GetPosition(canvas)
        match win.OpMode with 
        | OperateMode.Select  ->  ()//do_select(x)
        | OperateMode.IR_Node -> do_nodeCreate_end() //startPos,endPos)
        | OperateMode.IR_Link -> do_linkCreate_end()
        | OperateMode.Modify  -> do_modify()
        | _ -> () 
        ) 

    System.Windows.Application().Run(win) |> ignore