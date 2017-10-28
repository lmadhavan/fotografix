#pragma once


// CInputDialog dialog

class CInputDialog : public CDialog
{
	DECLARE_DYNAMIC(CInputDialog)

public:
	CInputDialog(CWnd* pParent = NULL);   // standard constructor
	virtual ~CInputDialog();

// Dialog Data
	enum { IDD = IDD_INPUT };

protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support

	DECLARE_MESSAGE_MAP()

public:
	bool number;
	CString title;
	CString prompt;
	CString value;

public:
	virtual BOOL OnInitDialog();
};
