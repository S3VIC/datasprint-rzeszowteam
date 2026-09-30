namespace norm.cli;

public static class QueryConstants {
	public static readonly String[] SelectQueryFields = [
		"pymt_crd_acct_num_raw",
		"prod_id_pltfrm_cd_vcis",
		"transaction_type",
		"transaction_pos_entry_mode",
		"mrch_nm_raw",
		"mrch_catg_cd",
		"mrch_catg_nm",
		"mrch_city_nm_raw",
		"SUM(cs_tran_amt) AS cs_tran_amt",
		"COUNT(cs_tran_amt) AS tran_cnt",
		"issr_jurn",
		"issr_ctry_cd",
		"issr_ctry_nm",
		"crd_typ_cd",
		"crd_typ_nm",
		"channel_flg",
		"cp_flag",
		"prch_mnth_id",
		"prch_dt",
		"myweek",
		"report_ctry",
		"COALESCE(NULLIF(TRIM(lau_enr), ''), 'POZOSTALI') AS lau_enr",
		"COALESCE(NULLIF(TRIM(fua_enr), ''), 'POZOSTALI') AS fua_enr"
	];
}