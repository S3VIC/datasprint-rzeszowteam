namespace norm.cli;

public static class OutputCsvPathValidator {
	public static String Validate(String outputCsvPath) {
		if (String.IsNullOrWhiteSpace(outputCsvPath)) {
			throw new ArgumentException("Output CSV path cannot be empty.", nameof(outputCsvPath));
		}

		String fullPath = BuildAbsolutePath(outputCsvPath, nameof(outputCsvPath), "Output CSV path is invalid.");
		if (Directory.Exists(fullPath)) {
			throw new ArgumentException("Output CSV path points to a directory. Provide a file path.", nameof(outputCsvPath));
		}

		if (!String.Equals(Path.GetExtension(fullPath), ".csv", StringComparison.OrdinalIgnoreCase)) {
			throw new ArgumentException("Output path must point to a .csv file.", nameof(outputCsvPath));
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
