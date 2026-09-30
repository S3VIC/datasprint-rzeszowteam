namespace norm.cli;

public static class Program {
	public static Int32 Main(String[] args) {
		try {
			CliOptions options;
			try {
				options = CliArgumentsParser.Parse(args);
			}
			catch (Exception exception) when (exception is ArgumentException or FileNotFoundException) {
				Console.Error.WriteLine(exception.Message);
				LogUsageInfo();
				return 1;
			}
			String sql = QueryBuilder.BuildSqlCommand(options.ParquetPath, options.CityFilter, options.Limit);

			Console.WriteLine($"SQL query: {sql}");
			Console.WriteLine("Executing query and exporting results...");
			QueryExporter.ExecuteAndExport(sql, options.OutputCsvPath);

			Console.WriteLine($"Exported query result to: {options.OutputCsvPath}");
			return 0;
		}
		catch (Exception exception) {
			Console.Error.WriteLine(exception.Message);
			return 1;
		}
	}

	private static void LogUsageInfo() {
		Console.Error.WriteLine($"INFO: Usage: {CliArgumentsParser.Usage}");
	}
}