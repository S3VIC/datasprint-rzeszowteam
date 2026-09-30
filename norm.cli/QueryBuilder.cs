namespace norm.cli;

public static class QueryBuilder {
	private const Int32 WildcardFallbackThreshold = 2;
	private const String UpperCityColumnExpression = "upper(\"mrch_city_nm_raw\")";

	public static String BuildSqlCommand(String parquetPath, String cityFilter, Int32? limit = null) {
		if (String.IsNullOrWhiteSpace(parquetPath)) {
			throw new ArgumentException("Parquet path cannot be empty.", nameof(parquetPath));
		}
		if (limit is <= 0) {
			throw new ArgumentException("Limit must be greater than zero.", nameof(limit));
		}

		String[] cityFilters = ParseCityFilters(cityFilter);
		String escapedParquetPath = SqlLiteralEscaper.Escape(parquetPath);
		String cityPredicate = String.Join(
			" or ",
			cityFilters.Select(BuildCityPredicate)
		);
		String columns = String.Join(',', QueryConstants.SelectQueryFields);
		String limitClause = limit.HasValue ? $" limit {limit.Value}" : String.Empty;
		return
			$"select {columns} from read_parquet('{escapedParquetPath}') where ({cityPredicate}) and cp_flag = 1 and transaction_type = 'POS' group by all{limitClause};";
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

	private static String BuildCityPredicate(String cityName) {
		String wildcardPattern = BuildWildcardCityLikePattern(cityName);
		String wildcardCondition = $"{UpperCityColumnExpression} like '{SqlLiteralEscaper.Escape(wildcardPattern)}'";

		Int32 replacedDiacriticsCount = CountPolishDiacritics(cityName);
		if (replacedDiacriticsCount < WildcardFallbackThreshold) {
			return wildcardCondition;
		}

		String asciiPattern = BuildAsciiCityLikePattern(cityName);
		if (String.Equals(wildcardPattern, asciiPattern, StringComparison.Ordinal)) {
			return wildcardCondition;
		}

		String asciiCondition = $"{UpperCityColumnExpression} like '{SqlLiteralEscaper.Escape(asciiPattern)}'";
		return $"({wildcardCondition} or {asciiCondition})";
	}

	private static String BuildWildcardCityLikePattern(String cityName) {
		String upperCity = cityName.Trim().ToUpperInvariant();
		return String.Concat(
			upperCity.Select(character => character switch {
				_ when IsPolishDiacritic(character) => '_',
				>= 'A' and <= 'Z' => character,
				>= '0' and <= '9' => character,
				' ' or '-' or '_' => '_',
				_ => '_'
			})
		);
	}

	private static String BuildAsciiCityLikePattern(String cityName) {
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

	private static Int32 CountPolishDiacritics(String cityName) {
		return cityName.Count(IsPolishDiacritic);
	}

	private static Boolean IsPolishDiacritic(Char character) {
		return character is 'ą' or 'ć' or 'ę' or 'ł' or 'ń' or 'ó' or 'ś' or 'ż' or 'ź'
			or 'Ą' or 'Ć' or 'Ę' or 'Ł' or 'Ń' or 'Ó' or 'Ś' or 'Ż' or 'Ź';
	}
}
