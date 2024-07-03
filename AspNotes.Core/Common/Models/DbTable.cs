namespace AspNotes.Core.Common.Models;

public abstract class DbTable
{
    public virtual string TableName { get; } = null!;

    public string TableAlias { get; protected set; } = null!;

    public string CreatedAt { get; private set; } = nameof(CreatedAt);

    public string UpdatedAt { get; private set; } = nameof(UpdatedAt);

    protected DbTable(string? alias = null)
    {
        CreatedAt = alias + "." + nameof(CreatedAt);

        UpdatedAt = alias + "." + nameof(UpdatedAt);
    }

    public string GetFormattedTableName()
    {
        if (!string.IsNullOrEmpty(TableAlias))
        {
            return $"{TableName} as {TableAlias}";
        }

        return TableName;
    }
}