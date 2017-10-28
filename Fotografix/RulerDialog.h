#pragma once


// CRulerDialog dialog

class CRulerDialog : public CDialog
{
	DECLARE_DYNAMIC(CRulerDialog)

public:
	CRulerDialog(CWnd* pParent = NULL);   // standard constructor
	virtual ~CRulerDialog();

// Dialog Data
	enum { IDD = IDD_RULER };

protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support

	DECLARE_MESSAGE_MAP()

public:
	CString name;
	CString point1;
	CString point2;
	CString distance;
	CString angle;

public:
	afx_msg void OnBnClickedOk();
	afx_msg void OnBnClickedCancel();

protected:
	virtual void PostNcDestroy();

private:
	static int count;
	virtual BOOL OnInitDialog();
};
