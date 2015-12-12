﻿#pragma checksum "..\..\MainGame.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "374497BC5B87839D8EA9C0178F94D3B7"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

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


namespace Scrabble {
    
    
    /// <summary>
    /// MainGameWindow
    /// </summary>
    public partial class MainGameWindow : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 7 "..\..\MainGame.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label GameQTrack;
        
        #line default
        #line hidden
        
        
        #line 14 "..\..\MainGame.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.StackPanel PlayerTray;
        
        #line default
        #line hidden
        
        
        #line 24 "..\..\MainGame.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.WrapPanel GameBoard;
        
        #line default
        #line hidden
        
        
        #line 25 "..\..\MainGame.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ListBox PlayedWords;
        
        #line default
        #line hidden
        
        
        #line 26 "..\..\MainGame.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ListBox AIPlayedWords;
        
        #line default
        #line hidden
        
        
        #line 27 "..\..\MainGame.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label ScoreLabel;
        
        #line default
        #line hidden
        
        
        #line 28 "..\..\MainGame.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label SelectedSequence;
        
        #line default
        #line hidden
        
        
        #line 33 "..\..\MainGame.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock AIStatusReadout;
        
        #line default
        #line hidden
        
        
        #line 36 "..\..\MainGame.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label AIScore1;
        
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
            System.Uri resourceLocater = new System.Uri("/Scrabble (WPF);component/maingame.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\MainGame.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
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
            this.GameQTrack = ((System.Windows.Controls.Label)(target));
            return;
            case 2:
            
            #line 13 "..\..\MainGame.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.clickSubmitWord);
            
            #line default
            #line hidden
            return;
            case 3:
            this.PlayerTray = ((System.Windows.Controls.StackPanel)(target));
            
            #line 14 "..\..\MainGame.xaml"
            this.PlayerTray.MouseDown += new System.Windows.Input.MouseButtonEventHandler(this.returnCharactersToTray);
            
            #line default
            #line hidden
            return;
            case 4:
            
            #line 23 "..\..\MainGame.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.clickResetTray);
            
            #line default
            #line hidden
            return;
            case 5:
            this.GameBoard = ((System.Windows.Controls.WrapPanel)(target));
            return;
            case 6:
            this.PlayedWords = ((System.Windows.Controls.ListBox)(target));
            return;
            case 7:
            this.AIPlayedWords = ((System.Windows.Controls.ListBox)(target));
            return;
            case 8:
            this.ScoreLabel = ((System.Windows.Controls.Label)(target));
            return;
            case 9:
            this.SelectedSequence = ((System.Windows.Controls.Label)(target));
            return;
            case 10:
            this.AIStatusReadout = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 11:
            
            #line 34 "..\..\MainGame.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.clickRequestNewTray);
            
            #line default
            #line hidden
            return;
            case 12:
            this.AIScore1 = ((System.Windows.Controls.Label)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}
