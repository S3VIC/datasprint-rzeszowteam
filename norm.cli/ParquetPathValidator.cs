namespace norm.cli;

public static class ParquetPathValidator {
	public static String Validate(String parquetPath) {
		if (String.IsNullOrWhiteSpace(parquetPath)) {
			throw new ArgumentException("Parquet path cannot be empty.", nameof(parquetPath));
		}

		String fullPath = BuildAbsolutePath(parquetPath, nameof(parquetPath), "Parquet path is invalid.");
		if (Directory.Exists(fullPath)) {
			throw new ArgumentException("Parquet path points to a directory. Provide a .parquet file path.", nameof(parquetPath));
		}

		if (!File.Exists(fullPath)) {
			throw new FileNotFoundException($"Parquet file was not found: {fullPath}", fullPath);
		}

		if (!String.Equals(Path.GetExtension(fullPath), ".parquet", StringComparison.OrdinalIgnoreCase)) {
			throw new ArgumentException("Parquet path must point to a file with .parquet extension.", nameof(parquetPath));
		}

		return fullPath;
	}

	private static String BuildAbsolutePath(String path, String argumentName, String errorMessage) {
		try {
			return Path.GetFullPath(path);
		}
		catch (Exception exception) when (exception is ArgumentException or NotSupportedException or PathTooLongException) {
			throw new ArgumentException(errorMessage, argumentName, exception);
		}
	}
}
