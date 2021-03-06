﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Threading.Tasks;
using Autodesk.DesignScript.Runtime;
using Dynamo.Controls;
using Dynamo.Graph.Nodes;
using Dynamo.UI.Commands;
using Dynamo.Wpf;
using JsonDataUI.Nodes;
using ProtoCore.AST.AssociativeAST;
using Newtonsoft.Json;
using JsonDataUI;
using Dynamo.Graph;
using System.Xml;

namespace JsonDataUI.Views
{
    /// <summary>
    /// 
    /// </summary>
    public class JsonOptionsNodeViewCustomizations : INodeViewCustomization<JsonOptionsBase>
    {
        private JsonOptionsBase model;

        public void CustomizeView(JsonOptionsBase model, NodeView nodeView)
        {
            this.model = model;
            var JsonOptionsControl = new JsonOptionsControl();
            nodeView.inputGrid.Children.Add(JsonOptionsControl);

            JsonOptionsControl.DataContext = model;

            JsonOptionsControl.cBox_JsonOptions.ToolTip = @"Handling options where duplicate keys are found. Use JsonOptions 
                dropdown node to select appropiate behaviour";
            JsonOptionsControl.check_Nesting.ToolTip = @"Apply nesting behaviour if key input is a single string concatenated by
                dots, representing the desired nested structure";

            // Hiding and disabling Dropdown for options if JsonOption not needed for method.
            if (!model.NeedsOptions)
            {
                JsonOptionsControl.cBox_JsonOptions.Visibility = System.Windows.Visibility.Collapsed;
                JsonOptionsControl.cBox_JsonOptions.IsEnabled = false;
            }

            if (!model.NeedsNesting)
            {
                JsonOptionsControl.check_Nesting.IsEnabled = false;
                JsonOptionsControl.check_Nesting.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        public void Dispose() { }
    }
}
