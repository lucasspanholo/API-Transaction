namespace PdiAgile.Api.Models;

using System.Collections.Concurrent;

public class Transaction
{
    public decimal Value { get; set; }
    public DateTimeOffset DateTime { get; set; }
}

public static class TransactionStore
{
    public static ConcurrentBag<Transaction> Store { get; } = new();
}
