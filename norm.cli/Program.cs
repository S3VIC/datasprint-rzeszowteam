namespace norm.cli;

public static class Program {
	public static Int32 Main(String[] args) {
		try {
			if (args.Length < 2) {
				Console.Error.WriteLine("Usage: norm.cli <parquet-path> <city-name[,city-name...]> [output-csv-path]");
				return 1;
			}

			String parquetPath = args[0];
			String cityFilter = args[1];
			String outputCsvPath = args.Length >= 3
				? args[2]
				: CsvOutputPathBuilder.BuildDefaultCsvPath(parquetPath, cityFilter);
			String sql = QueryBuilder.BuildSqlCommand(parquetPath, cityFilter);

			Console.WriteLine($"SQL query: {sql}");
			Console.WriteLine("Executing query and exporting results...");
			QueryExporter.ExecuteAndExport(sql, outputCsvPath);

			Console.WriteLine($"Exported query result to: {outputCsvPath}");
			return 0;
		}
		catch (Exception exception) {
			Console.Error.WriteLine(exception.Message);
			return 1;
		}
	}
}