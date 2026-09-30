namespace norm.cli;

using DuckDB.NET.Data;

public static class Program {
	private const String PolishUpperDiacritics = "ĄĆĘŁŃÓŚŻŹ";
	private const String AsciiUpperReplacements = "ACELNOSZZ";

	public static Int32 Main(String[] args) {
		try {
			if (args.Length < 2) {
				Console.Error.WriteLine("Usage: norm.cli <parquet-path> <city-name> [output-csv-path]");
				return 1;
			}

			String parquetPath = args[0];
			String cityName = args[1];
			String outputCsvPath = args.Length >= 3 ? args[2] : BuildDefaultCsvPath(parquetPath, cityName);
			String sql = BuildSqlCommand(parquetPath, cityName);

			ExecuteAndExport(sql, outputCsvPath);

			Console.WriteLine(sql);
			Console.WriteLine($"Exported query result to: {outputCsvPath}");
			return 0;
		}
		catch (Exception exception) {
			Console.Error.WriteLine(exception.Message);
			return 1;
		}
	}

	private static String BuildSqlCommand(String parquetPath, String cityName) {
		if (String.IsNullOrWhiteSpace(parquetPath)) {
			throw new ArgumentException("Parquet path cannot be empty.", nameof(parquetPath));
		}

		if (string.IsNullOrWhiteSpace(cityName)) {
			throw new ArgumentException("City name cannot be empty.", nameof(cityName));
		}

		String escapedParquetPath = EscapeSqlLiteral(parquetPath);
		String cityLikePattern = EscapeSqlLiteral(BuildCityLikePattern(cityName));
		const String normalizedColumnExpression = $"translate(upper(\"mrch_city_nm_raw\"), '{PolishUpperDiacritics}', '{AsciiUpperReplacements}')";
		String columns = String.Join(',', QueryConstants.SelectQueryFields);
		return
			$"select {columns} from read_parquet('{escapedParquetPath}') where {normalizedColumnExpression} like '{cityLikePattern}' and cp_flag = 1 and transaction_type = 'POS' group by all;";
	}

	private static String BuildCityLikePattern(string cityName) {
		String normalizedCity = NormalizePolishToAscii(cityName).Trim().ToUpperInvariant();
		return String.Concat(
			normalizedCity.Select(ch => ch switch {
				>= 'A' and <= 'Z' => ch,
				>= '0' and <= '9' => ch,
				' ' or '-' or '_' => '_',
				_ => '_'
			})
		);
	}

	private static String NormalizePolishToAscii(String value) {
		return String.Concat(value.Select(character => character switch {
			'ą' => 'a',
			'ć' => 'c',
			'ę' => 'e',
			'ł' => 'l',
			'ń' => 'n',
			'ó' => 'o',
			'ś' => 's',
			'ż' => 'z',
			'ź' => 'z',
			'Ą' => 'A',
			'Ć' => 'C',
			'Ę' => 'E',
			'Ł' => 'L',
			'Ń' => 'N',
			'Ó' => 'O',
			'Ś' => 'S',
			'Ż' => 'Z',
			'Ź' => 'Z',
			_ => character
		}));
	}

	private static string EscapeSqlLiteral(String value) {
		return value.Replace("'", "''");
	}

	private static String BuildDefaultCsvPath(String parquetPath, String cityName) {
		String parquetName = Path.GetFileNameWithoutExtension(parquetPath);
		String cityToken = string.Concat(
			NormalizePolishToAscii(cityName)
				.Trim()
				.ToLowerInvariant()
				.Select(character => char.IsLetterOrDigit(character) ? character : '-')
		).Trim('-');

		if (String.IsNullOrWhiteSpace(cityToken)) {
			cityToken = "city";
		}

		return Path.Combine(Environment.CurrentDirectory, $"{parquetName}-{cityToken}.csv");
	}

	private static void ExecuteAndExport(String selectQuery, String outputCsvPath) {
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
		String escapedCsvPath = EscapeSqlLiteral(outputCsvPath);
		String normalizedSelect = selectQuery.Trim().TrimEnd(';');
		return $"copy ({normalizedSelect}) to '{escapedCsvPath}' (header, delimiter ',');";
	}
}