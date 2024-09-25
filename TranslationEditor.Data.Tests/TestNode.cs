namespace J113D.TranslationEditor.Data.Tests
{
    public class TestNode : Node
    {
        public TestNode(string name, string description)
            : base(name, description, NodeState.None)
        {
        }

        public void SetState(NodeState state)
        {
            State = state;
        }

    }
}
