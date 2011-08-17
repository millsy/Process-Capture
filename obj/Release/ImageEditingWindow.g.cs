﻿#pragma checksum "..\..\ImageEditingWindow.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "62CAD571AD839192ED9866425AA289FD"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.3053
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using DiagramDesigner;
using DiagramDesigner.Controls;
using ProcessCapture;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms.Integration;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace ProcessCapture {
    
    
    /// <summary>
    /// ImageEditingWindow
    /// </summary>
    public partial class ImageEditingWindow : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 8 "..\..\ImageEditingWindow.xaml"
        internal System.Windows.Input.CommandBinding hideImage;
        
        #line default
        #line hidden
        
        
        #line 9 "..\..\ImageEditingWindow.xaml"
        internal System.Windows.Input.CommandBinding saveImage;
        
        #line default
        #line hidden
        
        
        #line 10 "..\..\ImageEditingWindow.xaml"
        internal System.Windows.Input.CommandBinding undoImage;
        
        #line default
        #line hidden
        
        
        #line 40 "..\..\ImageEditingWindow.xaml"
        internal System.Windows.Controls.Button btnMinimize;
        
        #line default
        #line hidden
        
        
        #line 43 "..\..\ImageEditingWindow.xaml"
        internal System.Windows.Controls.Button btnMaximize;
        
        #line default
        #line hidden
        
        
        #line 46 "..\..\ImageEditingWindow.xaml"
        internal System.Windows.Controls.Button btnClose;
        
        #line default
        #line hidden
        
        
        #line 61 "..\..\ImageEditingWindow.xaml"
        internal System.Windows.Controls.ToolBar tbToolBar;
        
        #line default
        #line hidden
        
        
        #line 71 "..\..\ImageEditingWindow.xaml"
        internal System.Windows.Controls.ComboBox fillColour;
        
        #line default
        #line hidden
        
        
        #line 86 "..\..\ImageEditingWindow.xaml"
        internal System.Windows.Controls.ComboBox lineColour;
        
        #line default
        #line hidden
        
        
        #line 100 "..\..\ImageEditingWindow.xaml"
        internal System.Windows.Controls.ComboBox lineWidth;
        
        #line default
        #line hidden
        
        
        #line 127 "..\..\ImageEditingWindow.xaml"
        internal System.Windows.Controls.Canvas drawingCanvas;
        
        #line default
        #line hidden
        
        
        #line 129 "..\..\ImageEditingWindow.xaml"
        internal System.Windows.Media.ImageBrush fullSizeImage;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/ProcessCapture;component/imageeditingwindow.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\ImageEditingWindow.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.hideImage = ((System.Windows.Input.CommandBinding)(target));
            
            #line 8 "..\..\ImageEditingWindow.xaml"
            this.hideImage.CanExecute += new System.Windows.Input.CanExecuteRoutedEventHandler(this.hideImage_CanExecute);
            
            #line default
            #line hidden
            
            #line 8 "..\..\ImageEditingWindow.xaml"
            this.hideImage.Executed += new System.Windows.Input.ExecutedRoutedEventHandler(this.hideImage_Executed);
            
            #line default
            #line hidden
            return;
            case 2:
            this.saveImage = ((System.Windows.Input.CommandBinding)(target));
            
            #line 9 "..\..\ImageEditingWindow.xaml"
            this.saveImage.CanExecute += new System.Windows.Input.CanExecuteRoutedEventHandler(this.saveImage_CanExecute);
            
            #line default
            #line hidden
            
            #line 9 "..\..\ImageEditingWindow.xaml"
            this.saveImage.Executed += new System.Windows.Input.ExecutedRoutedEventHandler(this.saveImage_Executed);
            
            #line default
            #line hidden
            return;
            case 3:
            this.undoImage = ((System.Windows.Input.CommandBinding)(target));
            
            #line 10 "..\..\ImageEditingWindow.xaml"
            this.undoImage.CanExecute += new System.Windows.Input.CanExecuteRoutedEventHandler(this.undoImage_CanExecute);
            
            #line default
            #line hidden
            
            #line 10 "..\..\ImageEditingWindow.xaml"
            this.undoImage.Executed += new System.Windows.Input.ExecutedRoutedEventHandler(this.undoImage_Executed);
            
            #line default
            #line hidden
            return;
            case 4:
            
            #line 31 "..\..\ImageEditingWindow.xaml"
            ((System.Windows.Controls.Grid)(target)).MouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(this.MoveWindow);
            
            #line default
            #line hidden
            return;
            case 5:
            this.btnMinimize = ((System.Windows.Controls.Button)(target));
            return;
            case 6:
            this.btnMaximize = ((System.Windows.Controls.Button)(target));
            return;
            case 7:
            this.btnClose = ((System.Windows.Controls.Button)(target));
            return;
            case 8:
            this.tbToolBar = ((System.Windows.Controls.ToolBar)(target));
            return;
            case 9:
            this.fillColour = ((System.Windows.Controls.ComboBox)(target));
            return;
            case 10:
            this.lineColour = ((System.Windows.Controls.ComboBox)(target));
            return;
            case 11:
            this.lineWidth = ((System.Windows.Controls.ComboBox)(target));
            return;
            case 12:
            this.drawingCanvas = ((System.Windows.Controls.Canvas)(target));
            
            #line 127 "..\..\ImageEditingWindow.xaml"
            this.drawingCanvas.MouseMove += new System.Windows.Input.MouseEventHandler(this.drawingCanvas_MouseMove);
            
            #line default
            #line hidden
            
            #line 127 "..\..\ImageEditingWindow.xaml"
            this.drawingCanvas.MouseLeftButtonUp += new System.Windows.Input.MouseButtonEventHandler(this.drawingCanvas_MouseUp);
            
            #line default
            #line hidden
            
            #line 127 "..\..\ImageEditingWindow.xaml"
            this.drawingCanvas.MouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(this.drawingCanvas_MouseDown);
            
            #line default
            #line hidden
            return;
            case 13:
            this.fullSizeImage = ((System.Windows.Media.ImageBrush)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}