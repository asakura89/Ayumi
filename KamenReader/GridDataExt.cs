namespace KamenReader;

public static class GridDataExt {
    public static IList<GridData> CleanupRawData(this FileReaderResult rawData) {
        IList<GridData> cleanupColumn = rawData.Data;
        Boolean titlesAreNotNull = rawData.Titles != null;
        Boolean titlesHaveValues = rawData.Titles.Any();
        Boolean allTitlesAreNotEmpty = !rawData.Titles.All(ttl => String.IsNullOrEmpty(ttl));
        if (titlesAreNotNull && titlesHaveValues && allTitlesAreNotEmpty) {
            cleanupColumn = rawData
                .Titles
                .Select((ttl, idx) => new {Ordinal = idx+1, Item = ttl})
                .Where(ttl => !String.IsNullOrEmpty(ttl.Item))
                .Join(rawData.Data, o => o.Ordinal, i => i.Column, (o, i) => i)
                .ToList();
        }

        return cleanupColumn
            .GroupBy(prm => prm.Row)
            .Select(grp => grp.Select(item => item))
            .Where(grd => !grd.Aggregate(true, (p, n) => p && String.IsNullOrEmpty(n.CellValue)))
            .SelectMany(grd => grd)
            .ToList();
    }
}