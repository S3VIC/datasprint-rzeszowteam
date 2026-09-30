namespace norm.cli;

public static class PolishTextNormalizer {
	public static String NormalizeToAscii(String value) {
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
}
