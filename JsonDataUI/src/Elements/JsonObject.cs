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
using JsonData;
using ProtoCore.AST.AssociativeAST;
using VMDataBridge;
using Newtonsoft.Json;
#endregion

namespace JsonDataUI.JsonObject
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
    [OutPortTypes("JsonObject")]
    [IsDesignScriptCompatible]
    public class JsonByKeysAndValues : Controls.JsonOptionsBase
    {
        #region Constructor
        public JsonByKeysAndValues()
        {
            RegisterAllPorts();
            this.PortDisconnected += JsonByKeysAndValues_PortDisconnected;

            ArgumentLacing = LacingStrategy.Auto;

            //ByKeysAndValuesCommand = new DelegateCommand(CreateJson, CanCreateJson);
        }

        [JsonConstructor]
        public JsonByKeysAndValues(
            IEnumerable<PortModel> inPorts,
            IEnumerable<PortModel> outPorts) : base(inPorts, outPorts)
        {

        }

        private void JsonByKeysAndValues_PortDisconnected(PortModel obj)
        {
            MessageBox.Show(obj.Name);
        }
        #endregion

        public override IEnumerable<AssociativeNode> BuildOutputAst(List<AssociativeNode> inputAstNodes)
        {
            if (!InPorts[0].Connectors.Any())
            {
                return new[]
                {
                    AstFactory.BuildAssignment(GetAstIdentifierForOutputIndex(0), AstFactory.BuildNullNode())
                };
            }

            var nestedNode = AstFactory.BuildBooleanNode(nested);
            var jsonOptionNode = AstFactory.BuildFunctionCall(
                new Func<string, JsonOption>(JsonData.JsonOptions.ReturnOptionByName),
                new List<AssociativeNode> { AstFactory.BuildStringNode(this.jsonOption.ToString())}
                );

            AssociativeNode byKeysAndValuesFuncNode =
                AstFactory.BuildFunctionCall(
                    new Func<List<string>, List<object>, bool, JsonData.JsonOption, JsonData.Elements.JsonObject>(JsonData.Elements.JsonObject.ByKeysAndValues),
                    new List<AssociativeNode> { inputAstNodes[0], inputAstNodes[1], nestedNode, jsonOptionNode }
                    );
            return new[]
            {
                AstFactory.BuildAssignment(GetAstIdentifierForOutputIndex(0), byKeysAndValuesFuncNode)
            };
        }
    }

}