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
using ProtoCore.AST.AssociativeAST;
using Newtonsoft.Json;
using JsonDataUI.JsonObject;

namespace JsonDataUI.Controls
{
    public abstract class JsonOptionsBase : NodeModel
    {
        #region Internal Properties

        internal bool nested;
        internal JsonData.JsonOption jsonOption;
        internal string optionName;
        #endregion

        #region Properties

        public bool Nested
        {
            get { return nested; }
            set
            {
                nested = value;
                RaisePropertyChanged("Nested");
                OnNodeModified();
            }
        }

        public JsonData.JsonOption JsonOption
        {
            get { return jsonOption; }
            set
            {
                jsonOption = value;
                //jsonOption = (JsonData.JsonOption)Enum.Parse(typeof(JsonData.JsonOption), optionName);
                RaisePropertyChanged("JsonOption");
                OnNodeModified();
            }
        }

        #endregion

        #region Constructors

        protected JsonOptionsBase():base()
        {
            //ShouldDisplayPreviewCore = false;
        }

        [JsonConstructor]
        protected JsonOptionsBase(
            IEnumerable<PortModel> inPorts,
            IEnumerable<PortModel> outPorts) : base(inPorts, outPorts)
        {
            //ShouldDisplayPreviewCore = false;
            Nested = true;
            JsonOption = JsonData.JsonOption.None;
        }

        #endregion
     
    }

    public class JsonOptionsNodeViewCustomizations : INodeViewCustomization<JsonObject.JsonByKeysAndValues>
    {
        public void CustomizeView(JsonByKeysAndValues model, NodeView nodeView)
        {
            var JsonOptionsControl = new JsonOptionsControl();
            nodeView.inputGrid.Children.Add(JsonOptionsControl);

            JsonOptionsControl.DataContext = model;
        }

        public void Dispose() { }
    }
}
