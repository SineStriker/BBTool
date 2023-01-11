namespace A180.CoreLib.Collections;

public class PredicateSet : HashSet<Func<bool>>
{
    /// <summary>
    /// 存在一项返回值为true
    /// </summary>
    public bool Yes => this.Any(item => item.Invoke());

    /// <summary>
    /// 存在一项返回为false
    /// </summary>
    public bool No => this.Any(item => !item.Invoke());
}