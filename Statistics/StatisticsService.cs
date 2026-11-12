namespace NyxoMenu.Statistics;

internal sealed class StatisticsService
{
    private readonly Storage.RoundStore _store;
    public StatisticsService(Storage.RoundStore store) => _store = store;
    public int RoundCount => _store.Count;
}
