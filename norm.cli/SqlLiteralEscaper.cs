namespace norm.cli;

public static class SqlLiteralEscaper {
	public static String Escape(String value) {
		return value.Replace("'", "''");
	}
}
