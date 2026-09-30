namespace norm.cli;

using DuckDB.NET.Data;

public static class QueryExporter {
	public static void ExecuteAndExport(String selectQuery, String outputCsvPath) {
		String? outputDirectory = Path.GetDirectoryName(outputCsvPath);
		if (!String.IsNullOrWhiteSpace(outputDirectory)) {
			Directory.CreateDirectory(outputDirectory);
		}

		using DuckDBConnection connection = new("Data Source=:memory:");
		connection.Open();

		using DuckDBCommand command = connection.CreateCommand();
		command.CommandText = BuildExportSql(selectQuery, outputCsvPath);
		command.ExecuteNonQuery();
	}

	private static String BuildExportSql(String selectQuery, String outputCsvPath) {
		String escapedCsvPath = SqlLiteralEscaper.Escape(outputCsvPath);
		String normalizedSelect = selectQuery.Trim().TrimEnd(';');
		return $"copy ({normalizedSelect}) to '{escapedCsvPath}' (header, delimiter ',');";
	}
}
