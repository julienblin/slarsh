namespace Slarsh.NHibernate
{
    internal interface INHQuery
    {
        object InternalExecute(NHContextProvider nhContextProvider);
    }
}
