#region namesapces
using CoreNodeModels;
using Dynamo.Graph.Nodes;
using JsonData;
using ProtoCore.AST.AssociativeAST;
using System;
using System.Collections.Generic;
using System.Linq;
#endregion

namespace JsonDataUI
{

    [NodeName("JsonOptions")]
    [NodeCategory("JsonData.Elements.JsonObject")]
    [NodeDescription("Options for updating JsonObjects")]
    [OutPortTypes("JsonOption")]
    [OutPortDescriptions("Option to update a JsonObject when keys are already present in the object")]
    [IsDesignScriptCompatible]
    public class JsonOptions : DSDropDownBase
    {
        public JsonOptions() : base("JsonOption") { }

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

}
