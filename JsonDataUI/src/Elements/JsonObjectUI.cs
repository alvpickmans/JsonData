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
    [NodeName("JsonObject.ByKeysAndValues")]
    [NodeCategory("JsonData.Create")]
    [NodeDescription("JsonObject constructor by a given key-value pair. It accepts nested structures by providing keys divided by points as a single string.")]
    [InPortTypes("string[]", "var[]")]
    [OutPortTypes("JsonData.Elements.JsonObject")]
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
                            new Func<List<string>, List<object>, bool, JsonOption, JsonObject>(JsonObject.ByKeysAndValues),
                            ReturnInputNodes(inputAstNodes)
                        )
                    )
            };
        }
    }

    [NodeName("JsonObject.Add")]
    [NodeCategory("JsonData")]
    [NodeDescription(@"Adds new attribute to the JsonObject. If given key already on the object and update set to 
True, value associated with the key will be updated. An error will be thrown otherwise.")]
    [NodeSearchTags("json", "bykeysandvalues", "create", "new")]
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
                            new Func<JsonObject, List<string>, List<object>, bool, JsonOption, JsonObject>(JsonObject.Add),
                            ReturnInputNodes(inputAstNodes)
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
            this.NeedsOptions = false;
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
                            ReturnInputNodes(inputAstNodes)
                        )
                    )
            };
        }
    }

    [NodeName("JsonObject.GetValueByKey")]
    [NodeCategory("JsonData")]
    [NodeDescription(@"Returns the value associated with the given key from the dict. Returns null if key is not found.")]
    [NodeSearchTags("json", "getbykey", "getvaluebykey")]
    [IsDesignScriptCompatible]
    public class GetValueByKey : JsonOptionsBase
    {
        #region Constructor
        public GetValueByKey() : base()
        {
            InPorts.Add(new PortModel(PortType.Input, this, new PortData("jsonObject", "JsonObject")));
            InPorts.Add(new PortModel(PortType.Input, this, new PortData("keys", "Key(s) to query.")));
            OutPorts.Add(new PortModel(PortType.Output, this, new PortData("value", "Value associated to input key.")));
            this.NeedsOptions = false;
        }

        [JsonConstructor]
        public GetValueByKey(
            IEnumerable<PortModel> inPorts,
            IEnumerable<PortModel> outPorts) : base(inPorts, outPorts)
        {
            this.NeedsOptions = false;
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
                            new Func<JsonObject, string, bool, object>(JsonObject.GetValueByKey),
                            ReturnInputNodes(inputAstNodes)
                        )
                    )
            };
        }
    }

    [NodeName("JsonObject.Merge")]
    [NodeCategory("JsonData")]
    [NodeDescription(@"Merge one JsonObject with one or multiple other JsonObjects.")]
    [NodeSearchTags("json", "merge", "jsonobject")]
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
            this.NeedsNesting = false;
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
                            ReturnInputNodes(inputAstNodes)
                        )
                    )
            };
        }
    }

}