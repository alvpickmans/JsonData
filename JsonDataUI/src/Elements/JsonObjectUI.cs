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

    [NodeName("JsonObject.ByKeysAndValues")]
    [NodeCategory("JsonData.Create")]
    [NodeDescription("JsonObject constructor by a given key-value pair. It accepts nested structures by providing keys divided by points as a single string.")]
    [NodeSearchTags("json", "bykeysandvalues", "create")]
    [IsDesignScriptCompatible]
    public class ByKeysAndValues : JsonOptionsBase
    {
        #region Constructor
        public ByKeysAndValues() : base()
        {
            InPorts.Add(new PortModel(PortType.Input, this, new PortData("keys", Properties.Resources.JsonObject_Keys_Set)));
            InPorts.Add(new PortModel(PortType.Input, this, new PortData("values", Properties.Resources.JsonObject_Values_Set)));
            OutPorts.Add(new PortModel(PortType.Output, this, new PortData("jsonObject", Properties.Resources.JsonObject_Json_Get)));
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

            // Level and Replication Guide
            // https://github.com/DynamoDS/Dynamo/blob/c4a3305559c04f04e6757a5db8a55bc98eae6c15/src/DynamoCore/Graph/Nodes/NodeModel.cs#L1328
            UseLevelAndReplicationGuide(inputAstNodes);
                       
            return new[]
            {
                AstFactory.BuildAssignment(GetAstIdentifierForOutputIndex(0),
                    AstFactory.BuildFunctionCall(
                            new Func<List<string>, List<object>, bool, JsonData.JsonOption, JsonObject>(JsonObject.ByKeysAndValues),
                            InputNodes(inputAstNodes)
                        )
                    )
            };
        }
    }

    [NodeName("JsonObject.Add")]
    [NodeCategory("JsonData")]
    [NodeDescription(@"Adds new attribute to the JsonObject. If given key already on the object and update set to 
True, value associated with the key will be updated. An error will be thrown otherwise.")]
    [NodeSearchTags("json", "bykeysandvalues", "create")]
    [IsDesignScriptCompatible]
    public class Add : JsonOptionsBase
    {
        #region Constructor
        public Add() : base()
        {
            InPorts.Add(new PortModel(PortType.Input, this, new PortData("jsonObject", "JsonObject to which add properties.")));
            InPorts.Add(new PortModel(PortType.Input, this, new PortData("keys", Properties.Resources.JsonObject_Keys_Set)));
            InPorts.Add(new PortModel(PortType.Input, this, new PortData("values", Properties.Resources.JsonObject_Values_Set)));
            OutPorts.Add(new PortModel(PortType.Output, this, new PortData("jsonObject", Properties.Resources.JsonObject_Json_Get)));
        }

        [JsonConstructor]
        public Add(
            IEnumerable<PortModel> inPorts,
            IEnumerable<PortModel> outPorts) : base(inPorts, outPorts)
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
            UseLevelAndReplicationGuide(inputAstNodes);

            return new[]
            {
                AstFactory.BuildAssignment(GetAstIdentifierForOutputIndex(0),
                    AstFactory.BuildFunctionCall(
                            new Func<JsonObject, List<string>, List<object>, bool, JsonData.JsonOption, JsonObject>(JsonObject.Add),
                            InputNodes(inputAstNodes)
                        )
                    )
            };
        }
    }

    [NodeName("JsonObject.Remove")]
    [NodeCategory("JsonData")]
    [NodeDescription(@"Remove keys from the given JsonObject if they exist.")]
    [NodeSearchTags("json", "remove", "keys")]
    [IsDesignScriptCompatible]
    public class Remove : JsonOptionsBase
    {
        #region Constructor
        public Remove() : base()
        {
            InPorts.Add(new PortModel(PortType.Input, this, new PortData("jsonObject", "JsonObject")));
            InPorts.Add(new PortModel(PortType.Input, this, new PortData("keys", "Key(s) to remove from JsonObject")));
            OutPorts.Add(new PortModel(PortType.Output, this, new PortData("jsonObject", Properties.Resources.JsonObject_Json_Get)));
            this.NeedsOptions = false;
        }

        [JsonConstructor]
        public Remove(
            IEnumerable<PortModel> inPorts,
            IEnumerable<PortModel> outPorts) : base(inPorts, outPorts)
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
            UseLevelAndReplicationGuide(inputAstNodes);

            return new[]
            {
                AstFactory.BuildAssignment(GetAstIdentifierForOutputIndex(0),
                    AstFactory.BuildFunctionCall(
                            new Func<JsonObject, List<string>, bool, JsonObject>(JsonObject.Remove),
                            InputNodes(inputAstNodes)
                        )
                    )
            };
        }
    }

    [NodeName("JsonObject.Merge")]
    [NodeCategory("JsonData")]
    [NodeDescription(@"Merge one JsonObject with one or multiple other JsonObjects.")]
    [NodeSearchTags("json", "remove", "keys")]
    [IsDesignScriptCompatible]
    public class Merge : JsonOptionsBase
    {
        #region Constructor
        public Merge() : base()
        {
            InPorts.Add(new PortModel(PortType.Input, this, new PortData("jsonObject", "Main JsonObject")));
            InPorts.Add(new PortModel(PortType.Input, this, new PortData("others", "JsonObject(s) to merge to main.")));
            OutPorts.Add(new PortModel(PortType.Output, this, new PortData("jsonObject", Properties.Resources.JsonObject_Json_Get)));
            this.NeedsNesting = false;
        }

        [JsonConstructor]
        public Merge(
            IEnumerable<PortModel> inPorts,
            IEnumerable<PortModel> outPorts) : base(inPorts, outPorts)
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
            UseLevelAndReplicationGuide(inputAstNodes);

            return new[]
            {
                AstFactory.BuildAssignment(GetAstIdentifierForOutputIndex(0),
                    AstFactory.BuildFunctionCall(
                            new Func<JsonObject, List<JsonObject>, JsonOption, JsonObject>(JsonObject.Merge),
                            InputNodes(inputAstNodes)
                        )
                    )
            };
        }
    }

}