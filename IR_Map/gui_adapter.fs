module Gui_adapter


//gui.MainWindow
let mainWindow= gui.MainWindow()
mainWindow.OpMode <- gui.MainWindow.OperateMode.Select
let canvas= mainWindow.FindName("canvas") :?> System.Windows.Controls.Canvas