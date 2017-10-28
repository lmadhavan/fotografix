#pragma once


// CSizeDialog dialog

class CSizeDialog : public CDialog
{
	DECLARE_DYNAMIC(CSizeDialog)

public:
	CSizeDialog(CWnd* pParent = NULL);   // standard constructor
	virtual ~CSizeDialog();

// Dialog Data
	enum { IDD = IDD_IMAGESIZE };

protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support

	DECLARE_MESSAGE_MAP()

public:
	int ow;
	int oh;

public:
	float w;
	float h;
	int res;
	int unit;
	BOOL resample;
	BOOL constrain;

	CWnd wW, wH, wReslabel, wRes, wResunit, wConstrain;

public:
	afx_msg void OnCbnSelchangeUnits();
	afx_msg void OnBnClickedOk();
	afx_msg void OnBnClickedResample();
	afx_msg void OnBnClickedConstrain();
	afx_msg void OnEnKillfocusW();
	afx_msg void OnEnKillfocusH();
	afx_msg void OnEnKillfocusRes();
	virtual BOOL OnInitDialog();
	void UpdateUnits();
};
