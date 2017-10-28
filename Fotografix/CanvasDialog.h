#pragma once


// CCanvasDialog dialog

class CCanvasDialog : public CDialog
{
	DECLARE_DYNAMIC(CCanvasDialog)

public:
	CCanvasDialog(CWnd* pParent = NULL);   // standard constructor
	virtual ~CCanvasDialog();

// Dialog Data
	enum { IDD = IDD_CANVASSIZE };

protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support

	DECLARE_MESSAGE_MAP()

public:
	int ow;
	int oh;

	CString w1;
	CString h1;
	CString unitName;
	CComboBox unitBox;

public:
	virtual BOOL OnInitDialog();
	afx_msg void OnCbnSelchangeUnits();
	afx_msg void OnBnClickedOk();

public:
	float w2;
	float h2;
	int unit;
	int anchor;
	BOOL relative;
};
