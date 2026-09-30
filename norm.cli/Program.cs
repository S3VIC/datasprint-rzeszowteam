namespace norm.cli;

public static class Program {
	public static Int32 Main(String[] args) {
		try {
			if (args.Length < 2) {
				Console.Error.WriteLine($"Usage: {CliArgumentsParser.Usage}");
				return 1;
			}

			CliOptions options = CliArgumentsParser.Parse(args);
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
}