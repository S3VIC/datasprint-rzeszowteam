namespace norm.cli;

public static class QueryBuilder {
	private const String PolishUpperDiacritics = "ĄĆĘŁŃÓŚŻŹ";
	private const String AsciiUpperReplacements = "ACELNOSZZ";
	private const String NormalizedCityColumnExpression =
		$"translate(upper(\"mrch_city_nm_raw\"), '{PolishUpperDiacritics}', '{AsciiUpperReplacements}')";

	public static String BuildSqlCommand(String parquetPath, String cityFilter) {
		if (String.IsNullOrWhiteSpace(parquetPath)) {
			throw new ArgumentException("Parquet path cannot be empty.", nameof(parquetPath));
		}

		String[] cityFilters = ParseCityFilters(cityFilter);
		String escapedParquetPath = SqlLiteralEscaper.Escape(parquetPath);
		String cityPredicate = String.Join(
			" or ",
			cityFilters.Select(filter => $"{NormalizedCityColumnExpression} like '{SqlLiteralEscaper.Escape(BuildCityLikePattern(filter))}'")
		);
		String columns = String.Join(',', QueryConstants.SelectQueryFields);
		return
			$"select {columns} from read_parquet('{escapedParquetPath}') where ({cityPredicate}) and cp_flag = 1 and transaction_type = 'POS' group by all;";
	}

	private static String[] ParseCityFilters(String cityFilter) {
		if (String.IsNullOrWhiteSpace(cityFilter)) {
			throw new ArgumentException("City name cannot be empty.", nameof(cityFilter));
		}

		String[] cityFilters = cityFilter
			.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
			.Where(filter => !String.IsNullOrWhiteSpace(filter))
			.ToArray();

		if (cityFilters.Length == 0) {
			throw new ArgumentException("At least one city name must be provided.", nameof(cityFilter));
		}

		return cityFilters;
	}

	private static String BuildCityLikePattern(String cityName) {
		String normalizedCity = PolishTextNormalizer.NormalizeToAscii(cityName).Trim().ToUpperInvariant();
		return String.Concat(
			normalizedCity.Select(character => character switch {
				>= 'A' and <= 'Z' => character,
				>= '0' and <= '9' => character,
				' ' or '-' or '_' => '_',
				_ => '_'
			})
		);
	}
}
