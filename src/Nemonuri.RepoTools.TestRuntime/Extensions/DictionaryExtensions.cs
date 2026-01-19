namespace Nemonuri.RepoTools.TestRuntime.Extensions;

public static class DictionaryExtensions
{
    extension<TKey, TValue>(IDictionary<TKey, TValue> dict) where TKey : notnull
    {
        public void AddOrUpdate(TKey key, TValue value)
        {
            if (dict.ContainsKey(key))
            {
                dict[key] = value;
            }
            else
            {
                dict.Add(key, value);
            }
        }
    }
}
