namespace A180.CoreLib.Collections;

public class PredicateSet : HashSet<Func<bool>>
{
    public bool Yes => this.Any(item => item.Invoke());
}
