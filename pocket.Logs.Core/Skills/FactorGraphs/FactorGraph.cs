namespace pocket.Logs.Core.Skills.FactorGraphs
{
    public class FactorGraph<TSelf, TValue, TVariable>
        where TSelf : FactorGraph<TSelf, TValue, TVariable>
        where TVariable : Variable<TValue>
    {
        public VariableFactory<TValue> VariableFactory { get; protected set; }
    }
}