module ext

open System.Windows.Media
open System.Windows.Shapes
open System.Windows
open System.Windows.Controls

//public class TextShape : Shape
//{
//        public float X { get; set; }
//        public float Y { get; set; }
//
//        override DefiningGeometry
//            get { return Geometry.Empty; }
//
//        override OnRender(drawingContext:DrawingContext)=
//           
//            FormattedText ft = new FormattedText
//               (Text, new CultureInfo("ru-ru"),
//               FlowDirection.LeftToRight, 
//               new Typeface(
//                  new FontFamily("Arial"),
//                  FontStyles.Normal, 
//                  FontWeights.Bold, 
//                  new FontStretch()),
//               TextHeight,
//               new SolidColorBrush(TextColor));
//            drawingContext.DrawText
//               (ft, new Point(X, Y));
//            //base.OnRender(drawingContext);
//        }
//}
type LinkShape()=
    inherit Shape()    
    let _pts =ref null // (new PointCollection() )
    let mutable calcPts =null
    let mutable geoData =null
    member val X=0.0 with  get,set
    member val Y=0.0 with get,set

    static member CalcCurve(pts:Point[], tenstion)= //, out Point p1, out Point p2)
        //double deltaX, deltaY;
        let deltaX = pts.[2].X - pts.[0].X;
        let deltaY = pts.[2].Y - pts.[0].Y;
        let p1 = new Point((pts.[1].X - tenstion * deltaX), (pts.[1].Y - tenstion * deltaY));
        let p2 = new Point((pts.[1].X + tenstion * deltaX), (pts.[1].Y + tenstion * deltaY));
        (p1,p2)
    static member CalcCurveEnd( end1:Point,  adj:Point , tension )= //, out Point p1)   =         
        new Point(((tension * (adj.X - end1.X) + end1.X)), ((tension * (adj.Y - end1.Y) + end1.Y)));
    static member calcPoints(pts :PointCollection,closed,t) =
        let mutable i=0
        let mutable nrRetPts=0
        let mutable p1= new Point()
        let mutable p2= new Point()
        let tension = t * (1.0 / 3.0) //we are calculating contolpoints.

        if (closed) then
            nrRetPts <- (pts.Count + 1) * 3 - 2
        else
            nrRetPts <- pts.Count * 3 - 2

        let retPnt = Array.create nrRetPts (new Point() )
        if (not closed) then
            let p2 = LinkShape.CalcCurveEnd(pts.[0], pts.[1], tension)
            retPnt.[0] <- pts.[0]
            retPnt.[1] <- p1;

        for  i=  0  to ( pts.Count - ( if closed then 1 else 2) )  do
            let (mp1,mp2) = LinkShape.CalcCurve( [| pts.[i] ; pts.[i + 1] ; pts.[(i + 2) % pts.Count] |], tension) //, out  p1, out p2);
            p1 <- mp1
            p2 <- mp2
            retPnt.[3 * i + 2] <- p1
            retPnt.[3 * i + 3] <- pts.[i + 1]
            retPnt.[3 * i + 4] <- p2
        if (closed) then
            let (mp1,mp2) = LinkShape.CalcCurve( [| pts.[pts.Count - 1]; pts.[0]; pts.[1] |], tension ) //, out p1, out p2);
            p1 <- mp1
            p2 <- mp2
            retPnt.[nrRetPts - 2] <- p1
            retPnt.[0] <- pts.[0]
            retPnt.[1] <- p2
            retPnt.[nrRetPts - 1] <- retPnt.[0];
        else
            p1 <- LinkShape.CalcCurveEnd(pts.[pts.Count - 1], pts.[pts.Count - 2], tension)
            retPnt.[nrRetPts - 2] <- p1
            retPnt.[nrRetPts - 1] <- pts.[pts.Count - 1]
        new PointCollection(retPnt)

    member this.setPoints(pts:PointCollection) =
        _pts := pts
        calcPts <- LinkShape.calcPoints(!_pts,false,0.5)
        geoData <- new PathGeometry()
        let pf= new PathFigure()
        let ps= new PolyBezierSegment()
        ps.Points  <- !_pts
        pf.Segments.Add(ps)
        geoData.Figures.Add(pf)


    override this.DefiningGeometry with get ()= Geometry.Empty

    override this.OnRender(drawingContext:DrawingContext)=  
        if(!_pts <> null) then
            drawingContext.DrawGeometry( Brushes.Black,new Pen(),geoData)
            //PointCollection pnts = cardinalSpline(Points, 0.5, Closed);

            //    sgc.BeginFigure(pnts[0], true, false);
            //    for  i in 1..3..  pnts.Count
            //        sgc.BezierTo(pnts[i], pnts[i + 1], pnts[i + 2], true, false);

            //drawingContext.DrawGeometry()

type TestShape() =
    inherit Shape()
    //let mutable x1=0.0
    //let mutable x2=0.0
    //let mutable y1=0.0
    //let mutable y1=0.0
    static member val X1Property =    
        DependencyProperty.Register(  "X1", typeof<float>, typeof<TestShape>, 
            new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsMeasure))
    static member val X2Property =  
        DependencyProperty.Register(  "X2", typeof<float>, typeof<TestShape>, 
            new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsMeasure))
    static member val Y1Property =  
        DependencyProperty.Register(  "Y1", typeof<float>, typeof<TestShape>, 
            new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsMeasure))
    static member val Y2Property =  
        DependencyProperty.Register(  "Y2", typeof<float>, typeof<TestShape>, 
            new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsMeasure))
            

    member this.X1 
        with get() =  this.GetValue(TestShape.X1Property) |> unbox
        and set(a:float) =  this.SetValue(TestShape.X1Property,a)
    member this.X2 
        with get() =  this.GetValue(TestShape.X2Property) |> unbox
        and set(a:float) =  this.SetValue(TestShape.X2Property,a)
    member this.Y1
        with get() =  this.GetValue(TestShape.Y1Property) |> unbox
        and set(a:float) =  this.SetValue(TestShape.Y1Property,a)
    member this.Y2 
        with get() =  this.GetValue(TestShape.Y2Property) |> unbox
        and set(a:float) =  this.SetValue(TestShape.Y2Property,a)

    //member  this.X2 =0.0 with get,set
    //member val Y1 =0.0 with get,set
    //member val Y2 =0.0 with get,set
 
    override this.DefiningGeometry
        with get()=
            let geometry = new StreamGeometry()
            geometry.FillRule <- FillRule.EvenOdd
            use context = geometry.Open()
            context.BeginFigure(new Point(this.X1,this.Y1) ,true,false)
            context.LineTo(new Point(this.X2,this.Y2),true,true)
                //InternalDrawArrowGeometry(context)
            //
            // Freeze the geometry for performance benefits
            geometry.Freeze()
            geometry :> Geometry

