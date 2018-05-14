using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
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
        }

        public void Dispose() { }
    }
}
