namespace norm.cli;

public sealed record CliOptions(String ParquetPath, String CityFilter, String OutputCsvPath, Int32? Limit);

public static class CliArgumentsParser {
	public const String Usage = "norm.cli <parquet-path> <city-name[,city-name...]> [output-csv-path] [-<number>]";

	public static CliOptions Parse(String[] args) {
		if (args.Length is < 2 or > 4) {
			throw new ArgumentException($"Invalid number of arguments. Usage: {Usage}");
		}

		Boolean hasTrailingLimit = args.Length >= 3 && LooksLikeLimitArgument(args[^1]);
		Int32? limit = hasTrailingLimit ? ParseLimitArgument(args[^1]) : null;

		if (args.Length == 4 && !hasTrailingLimit) {
			throw new ArgumentException($"When four arguments are provided, the last one must be a limit in format -<number>. Usage: {Usage}");
		}

		String parquetPath = ParquetPathValidator.Validate(args[0]);
		String cityFilter = args[1];
		String outputCsvPath = args.Length switch {
			2 => OutputCsvPathValidator.Validate(CsvOutputPathBuilder.BuildDefaultCsvPath(parquetPath, cityFilter)),
			3 when hasTrailingLimit => OutputCsvPathValidator.Validate(CsvOutputPathBuilder.BuildDefaultCsvPath(parquetPath, cityFilter)),
			3 => OutputCsvPathValidator.Validate(args[2]),
			4 => OutputCsvPathValidator.Validate(args[2]),
			_ => throw new ArgumentException($"Invalid number of arguments. Usage: {Usage}")
		};

		return new CliOptions(parquetPath, cityFilter, outputCsvPath, limit);
	}

	private static Boolean LooksLikeLimitArgument(String argument) {
		return argument.StartsWith("-", StringComparison.Ordinal);
	}

	private static Int32 ParseLimitArgument(String argument) {
		if (argument.Length <= 1 || !Int32.TryParse(argument[1..], out Int32 limit) || limit <= 0) {
			throw new ArgumentException("Limit argument must be in format -<positive-number>.");
		}

		return limit;
	}
}
