#pragma once


// CNewDialog dialog

class CNewDialog : public CDialog
{
	DECLARE_DYNAMIC(CNewDialog)

public:
	CNewDialog(CWnd* pParent = NULL);   // standard constructor

// Dialog Data
	enum { IDD = IDD_NEWIMAGE };

protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support

	DECLARE_MESSAGE_MAP()

public:
	float w;
	float h;
	int res;
	int fill;
	int unit;

	CWnd wReslabel, wRes, wResunit;

public:
	afx_msg void OnCbnSelchangeUnits();
	afx_msg void OnBnClickedOk();
	afx_msg void OnBnClickedPreset();
	virtual BOOL OnInitDialog();
	void UpdateUnits();
};
