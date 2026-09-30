namespace norm.cli;

public static class CsvOutputPathBuilder {
	public static String BuildDefaultCsvPath(String parquetPath, String cityFilter) {
		String parquetName = Path.GetFileNameWithoutExtension(parquetPath);
		String cityToken = string.Concat(
			PolishTextNormalizer.NormalizeToAscii(cityFilter)
				.Trim()
				.ToLowerInvariant()
				.Select(character => char.IsLetterOrDigit(character) ? character : '-')
		).Trim('-');

		if (String.IsNullOrWhiteSpace(cityToken)) {
			cityToken = "city";
		}

		return Path.Combine(Environment.CurrentDirectory, $"{parquetName}-{cityToken}.csv");
	}
}
