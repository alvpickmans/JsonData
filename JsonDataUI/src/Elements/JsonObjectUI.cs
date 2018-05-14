#region namesapces
using System;
using System.Collections.Generic;
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Linq;
using CoreNodeModels;
using Dynamo.Graph.Nodes;
using Dynamo.UI.Commands;
using Autodesk.DesignScript.Runtime;
using JsonData;
using ProtoCore.AST.AssociativeAST;
using VMDataBridge;
using Newtonsoft.Json;
using JsonData.Elements;
#endregion

namespace JsonDataUI.Nodes
{

    [NodeName("JsonOptions")]
    [NodeCategory("JsonData.JsonObject")]
    [NodeDescription("Options for updating JsonObjects")]
    [OutPortTypes("JsonOption")]
    [OutPortDescriptions("Option to update a JsonObject when keys are already present in the object")]
    [IsDesignScriptCompatible]
    public class JsonOptions : DSDropDownBase
    {
        public JsonOptions() : base("JsonOption") { }

        [JsonConstructor]
        public JsonOptions(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts) : base("JsonOption", inPorts, outPorts) { }

        protected override SelectionState PopulateItemsCore( string currentSelection)
        {
            PopulateDropDownItems(currentSelection);
            return SelectionState.Done;
        }

        public void PopulateDropDownItems(string selection)
        {
            if(selection == "" || Items.Count == 0)
            {
                Items.Clear();

                var newItems = new List<DynamoDropDownItem>()
                {
                    new DynamoDropDownItem("None", JsonOption.None),
                    new DynamoDropDownItem("Update", JsonOption.Update),
                    new DynamoDropDownItem("Combine", JsonOption.Combine)
                };

                    newItems.ForEach(item => Items.Add(item));
                SelectedIndex = 0;
            }
            else
            {
                int index = Items.ToList().FindIndex(item => item.Name == selection);
                SelectedIndex = index;
            }
            
        }

        public override IEnumerable<AssociativeNode> BuildOutputAst(List<AssociativeNode> inputAstNodes)
        {
            if (Items.Count == 0 || SelectedIndex == -1)
            {
                return new[] { AstFactory.BuildAssignment(GetAstIdentifierForOutputIndex(0), AstFactory.BuildNullNode()) };
            }

            var args = new List<AssociativeNode>
            {
                AstFactory.BuildStringNode(Items[SelectedIndex].Name)
            };
            
            var functionCall = AstFactory.BuildFunctionCall<string, JsonOption>(
                JsonData.JsonOptions.ReturnOptionByName,
                args
                );
            var assign = AstFactory.BuildAssignment(GetAstIdentifierForOutputIndex(0), functionCall);

            return new List<AssociativeNode> { assign };
        }

    }

    [NodeName("JsonObject.ByKeysAndValuesUI")]
    [NodeCategory("JsonData.JsonObjectUI")]
    [NodeDescription("Create JsonObject by keys and values")]
    [InPortNames("keys", "values")]
    [InPortTypes("List<string>", "List<object>")]
    [OutPortNames("jsonObject")]
    [OutPortTypes("JsonData.JsonObject")]
    [IsDesignScriptCompatible]
    public class ByKeysAndValues : JsonOptionsBase
    {
        #region Constructor
        public ByKeysAndValues() : base()
        {
            //RegisterAllPorts();
            //bthis.PortDisconnected += JsonByKeysAndValues_PortDisconnected;
            //ArgumentLacing = LacingStrategy.Auto;
        }

        [JsonConstructor]
        public ByKeysAndValues(
            IEnumerable<PortModel> inPorts,
            IEnumerable<PortModel> outPorts) : base(inPorts,outPorts)
        {
            
        }

        #endregion

        [IsVisibleInDynamoLibrary(false)]
        public override IEnumerable<AssociativeNode> BuildOutputAst(List<AssociativeNode> inputAstNodes)
        {
            if (IsPartiallyApplied)
            {
                return new[]
                {
                    AstFactory.BuildAssignment(GetAstIdentifierForOutputIndex(0), AstFactory.BuildNullNode())
                };
            }

            var inputs = new List<AssociativeNode>()
            {
                inputAstNodes[0],
                inputAstNodes[1],
                NestedASTNode(this.nesting),
                JsonOptionASTNode(this.option)

            };
            AssociativeNode funcNode =
                AstFactory.BuildFunctionCall(
                    new Func<List<string>, List<object>, bool, JsonData.JsonOption, JsonObject>(JsonObject.ByKeysAndValues),
                    inputs
                    );
            

            return new[]
            {
                AstFactory.BuildAssignment(GetAstIdentifierForOutputIndex(0),funcNode)
            };
        }
    }

}