﻿using DnetIndexedDb;

namespace Ididit.Database.Entities;

internal class CategoryEntity
{
    [IndexDbKey(AutoIncrement = true)]
    public long Id { get; set; }

    [IndexDbIndex]
    public long? CategoryId { get; set; }

    [IndexDbIndex]
    public string Name { get; set; } = string.Empty;
}
