﻿#pragma checksum "..\..\..\..\Views\Pages\Routes.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "DD52D3AB336F704E8F08AFCCE0625C22A1AADF85"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using JustClimbTrial.ViewModels;
using JustClimbTrial.Views.Pages;
using JustClimbTrial.Views.UserControls;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
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
using System.Windows.Shell;


namespace JustClimbTrial.Views.Pages {
    
    
    /// <summary>
    /// Routes
    /// </summary>
    public partial class Routes : System.Windows.Controls.Page, System.Windows.Markup.IComponentConnector {
        
        
        #line 20 "..\..\..\..\Views\Pages\Routes.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid gridContainer;
        
        #line default
        #line hidden
        
        
        #line 47 "..\..\..\..\Views\Pages\Routes.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal JustClimbTrial.Views.UserControls.HeaderRowNavigation navHead;
        
        #line default
        #line hidden
        
        
        #line 59 "..\..\..\..\Views\Pages\Routes.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox ddlAge;
        
        #line default
        #line hidden
        
        
        #line 79 "..\..\..\..\Views\Pages\Routes.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox ddlDifficulty;
        
        #line default
        #line hidden
        
        
        #line 96 "..\..\..\..\Views\Pages\Routes.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.DataGrid dgridRoutes;
        
        #line default
        #line hidden
        
        
        #line 109 "..\..\..\..\Views\Pages\Routes.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.DataGridTextColumn routeNoColumn;
        
        #line default
        #line hidden
        
        
        #line 110 "..\..\..\..\Views\Pages\Routes.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.DataGridTextColumn difficultyColumn;
        
        #line default
        #line hidden
        
        
        #line 111 "..\..\..\..\Views\Pages\Routes.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.DataGridTextColumn ageGroupColumn;
        
        #line default
        #line hidden
        
        
        #line 117 "..\..\..\..\Views\Pages\Routes.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnGameStart;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/JustClimbTrial;component/views/pages/routes.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\Views\Pages\Routes.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal System.Delegate _CreateDelegate(System.Type delegateType, string handler) {
            return System.Delegate.CreateDelegate(delegateType, this, handler);
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            
            #line 13 "..\..\..\..\Views\Pages\Routes.xaml"
            ((JustClimbTrial.Views.Pages.Routes)(target)).Loaded += new System.Windows.RoutedEventHandler(this.Page_Loaded);
            
            #line default
            #line hidden
            return;
            case 2:
            this.gridContainer = ((System.Windows.Controls.Grid)(target));
            return;
            case 3:
            this.navHead = ((JustClimbTrial.Views.UserControls.HeaderRowNavigation)(target));
            return;
            case 4:
            this.ddlAge = ((System.Windows.Controls.ComboBox)(target));
            return;
            case 5:
            this.ddlDifficulty = ((System.Windows.Controls.ComboBox)(target));
            return;
            case 6:
            this.dgridRoutes = ((System.Windows.Controls.DataGrid)(target));
            return;
            case 7:
            this.routeNoColumn = ((System.Windows.Controls.DataGridTextColumn)(target));
            return;
            case 8:
            this.difficultyColumn = ((System.Windows.Controls.DataGridTextColumn)(target));
            return;
            case 9:
            this.ageGroupColumn = ((System.Windows.Controls.DataGridTextColumn)(target));
            return;
            case 10:
            this.btnGameStart = ((System.Windows.Controls.Button)(target));
            
            #line 119 "..\..\..\..\Views\Pages\Routes.xaml"
            this.btnGameStart.Click += new System.Windows.RoutedEventHandler(this.btnGameStart_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

