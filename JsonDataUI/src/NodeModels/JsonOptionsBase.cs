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
using Dynamo.Graph;
using System.Xml;
using JsonData.Elements;

namespace JsonDataUI.Nodes
{
    public abstract class JsonOptionsBase : NodeModel
    {
        #region Internal Properties
        internal bool needsOptions = true;
        internal bool needsNesting = true;
        internal bool nesting;
        internal string option;
        internal string[] options = new string[3] { "None", "Update", "Combine" };

        internal bool NeedsOptions
        {
            get { return needsOptions; }
            set { needsOptions = value; }
        }

        internal bool NeedsNesting
        {
            get { return needsNesting; }
            set { needsNesting = value; }
        }
        #endregion

        #region Properties

        /// <summary>
        /// Property controlling Nesting behaviour
        /// </summary>
        public bool Nesting
        {
            get { return nesting; }
            set
            {
                nesting = value;
                RaisePropertyChanged("Nested");
                OnNodeModified();
            }
        }

        public string Option
        {
            get { return option; }
            set
            {
                option = value;
                RaisePropertyChanged("Option");
                OnNodeModified();
            }
        }

        #endregion

        #region Constructors

        protected JsonOptionsBase()
        {
            RegisterAllPorts();
            ArgumentLacing = LacingStrategy.Auto;
            PopulateView();
        }

        [JsonConstructor]
        protected JsonOptionsBase(
            IEnumerable<PortModel> inPorts,
            IEnumerable<PortModel> outPorts) : base(inPorts, outPorts)
        {
           
        }

        #endregion

        #region Serialize/Deserialize 
        protected override void SerializeCore(XmlElement nodeElement, SaveContext context)
        {
            base.SerializeCore(nodeElement, context);

            if (nodeElement.OwnerDocument == null) { return; }

            var optionNode = nodeElement.OwnerDocument.CreateElement("jsonOptionNode");
            var nestingNode = nodeElement.OwnerDocument.CreateElement("jsonNestingNode");

            optionNode.InnerText = this.Option;
            nestingNode.InnerText = (this.Nesting) ? "true" : "false";

            nodeElement.AppendChild(optionNode);
            nodeElement.AppendChild(nestingNode);
        }

        protected override void DeserializeCore(XmlElement nodeElement, SaveContext context)
        {
            base.DeserializeCore(nodeElement, context);

            var optionNode = nodeElement.ChildNodes.Cast<XmlNode>().FirstOrDefault(n => n.Name == "jsonOptionNode");
            var nestingNode = nodeElement.ChildNodes.Cast<XmlNode>().FirstOrDefault(n => n.Name == "jsonNestingNode");

            if (optionNode != null && this.options.Contains(optionNode.InnerText))
            {
                this.Option = optionNode.InnerText;
            }

            if (nestingNode != null)
            {
                this.Nesting = nestingNode.InnerText == "true";
            }
        } 
        #endregion

        #region Methods

        public void PopulateView()
        {
            this.option = "None";
            this.nesting = true;
        }

        /// <summary>
        /// AssociativeNode for JsonOption property
        /// </summary>
        /// <returns>Function call returning selected JsonOption</returns>
        public static AssociativeNode JsonOptionASTNode(string option)
        {
            return AstFactory.BuildFunctionCall(
            new Func<string, JsonOption>(JsonOptions.ReturnOptionByName),
            new List<AssociativeNode> { AstFactory.BuildStringNode(option) }
            );

        }

        /// <summary>
        /// AssociativeNode for Nested property
        /// </summary>
        public static AssociativeNode NestedASTNode(bool nesting)
        {
            return AstFactory.BuildBooleanNode(nesting);
        }

        public List<AssociativeNode> ReturnInputNodes(List<AssociativeNode> inputAstNodes)
        {
            var inputs = new List<AssociativeNode>(inputAstNodes);
            if (NeedsNesting) { inputs.Add(NestedASTNode(this.nesting)); }
            if (NeedsOptions) { inputs.Add(JsonOptionASTNode(this.option)); }
            return inputs;
        }


        #endregion
    }
}
